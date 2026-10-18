param apiImage string
param containerAppSettings object
param createOrderQueueName string
param databaseName string
param databaseServerFullyQualifiedDomainName string
param environmentName string
param location string
param productImageContainerName string
param productReviewTableName string

@secure()
param storageConnectionString string

param resourceSuffix string
param orderUpdateQueueName string

@minLength(3)
param workloadName string

var uniqueSuffix = take(resourceSuffix, 5)
var registryName = take(toLower('cr${workloadName}${environmentName}${uniqueSuffix}'), 50)
var containerAppName = 'ca-${workloadName}-${environmentName}'

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
  name: 'id-${workloadName}-api-${environmentName}'
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

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'cae-${workloadName}-${environmentName}'
  location: location
  properties: {
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
          name: 'storage-connection'
          value: storageConnectionString
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
              secretRef: 'storage-connection'
            }
            {
              name: 'Queues__CreateOrderQueueName'
              value: createOrderQueueName
            }
            {
              name: 'Queues__OrderUpdateQueueName'
              value: orderUpdateQueueName
            }
            {
              name: 'ProductImages__ConnectionString'
              secretRef: 'storage-connection'
            }
            {
              name: 'ProductImages__ContainerName'
              value: productImageContainerName
            }
            {
              name: 'ProductReviews__ConnectionString'
              secretRef: 'storage-connection'
            }
            {
              name: 'ProductReviews__TableName'
              value: productReviewTableName
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
