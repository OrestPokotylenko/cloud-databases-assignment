param apiImage string
param containerAppSettings object
param createOrderQueueName string
param databaseName string
param databaseServerFullyQualifiedDomainName string
param environmentName string
param location string

@secure()
param queueStorageConnectionString string

param resourceNamePrefix string
param resourceSuffix string
param updateOrderStatusQueueName string

var registryName = take('acr00${replace('${resourceNamePrefix}${resourceSuffix}', '-', '')}', 50)
var containerAppName = '${resourceNamePrefix}-api'

resource registry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: registryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
    publicNetworkAccess: 'Enabled'
  }
}

resource pullIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${resourceNamePrefix}-api-identity'
  location: location
}

var databaseConnectionString = 'Server=tcp:${databaseServerFullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Authentication=Active Directory Managed Identity;User ID=${pullIdentity.properties.clientId};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource acrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(registry.id, pullIdentity.id, 'acr-pull')
  scope: registry
  properties: {
    principalId: pullIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '7f951dda-4ed3-4680-a7ca-43fe172d538d'
    )
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${resourceNamePrefix}-logs'
  location: location
  properties: {
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${resourceNamePrefix}-cae'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: listKeys(logAnalytics.id, '2022-10-01').primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${pullIdentity.id}': {}
    }
  }
  properties: {
    environmentId: environment.id
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        allowInsecure: false
        external: true
        targetPort: 8080
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
        transport: 'auto'
      }
      registries: [
        {
          identity: pullIdentity.id
          server: registry.properties.loginServer
        }
      ]
      secrets: [
        {
          name: 'queue-storage-connection'
          value: queueStorageConnectionString
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'api'
          image: apiImage
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: environmentName == 'prod' ? 'Production' : 'Development'
            }
            {
              name: 'Database__ConnectionString'
              value: databaseConnectionString
            }
            {
              name: 'Queues__ConnectionString'
              secretRef: 'queue-storage-connection'
            }
            {
              name: 'Queues__CreateOrderQueueName'
              value: createOrderQueueName
            }
            {
              name: 'Queues__UpdateOrderStatusQueueName'
              value: updateOrderStatusQueueName
            }
          ]
          resources: {
            cpu: json(containerAppSettings.cpu)
            memory: containerAppSettings.memory
          }
        }
      ]
      scale: {
        maxReplicas: containerAppSettings.maxReplicas
        minReplicas: containerAppSettings.minReplicas
      }
    }
  }
  dependsOn: [
    acrPull
  ]
}

output apiUrl string = 'https://${containerApp.properties.configuration.ingress.fqdn}'
output apiIdentityClientId string = pullIdentity.properties.clientId
output apiIdentityName string = pullIdentity.name
output containerAppName string = containerApp.name
output containerRegistryName string = registry.name
