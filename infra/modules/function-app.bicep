param createOrderQueueName string
param databaseName string
param databaseServerFullyQualifiedDomainName string
param deploymentContainerUri string
param environmentName string
param functionAppSettings object
param location string

@secure()
param queueStorageConnectionString string

param resourceSuffix string
param storageAccountId string
param storageAccountName string

@secure()
param storageConnectionString string

param updateOrderStatusQueueName string

@minLength(3)
param workloadName string

var uniqueSuffix = take(resourceSuffix, 5)
var functionAppName = take('func-${workloadName}-${environmentName}-${uniqueSuffix}', 60)
var blobDataContributorRoleId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
)

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

resource deploymentIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'id-${workloadName}-functions-${environmentName}'
  location: location
}

var databaseConnectionString = 'Server=tcp:${databaseServerFullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Authentication=Active Directory Managed Identity;User ID=${deploymentIdentity.properties.clientId};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource deploymentStorageAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccountId, deploymentIdentity.id, 'function-deployment-storage')
  scope: storageAccount
  properties: {
    principalId: deploymentIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: blobDataContributorRoleId
  }
}

resource plan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: 'asp-${workloadName}-functions-${environmentName}'
  location: location
  kind: 'functionapp'
  sku: {
    name: 'FC1'
    tier: 'FlexConsumption'
  }
  properties: {
    reserved: true
  }
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${deploymentIdentity.id}': {}
    }
  }
  properties: {
    functionAppConfig: {
      deployment: {
        storage: {
          authentication: {
            type: 'UserAssignedIdentity'
            userAssignedIdentityResourceId: deploymentIdentity.id
          }
          type: 'blobContainer'
          value: deploymentContainerUri
        }
      }
      runtime: {
        name: 'dotnet-isolated'
        version: '10.0'
      }
      scaleAndConcurrency: {
        instanceMemoryMB: functionAppSettings.instanceMemoryMB
        maximumInstanceCount: functionAppSettings.maximumInstanceCount
      }
    }
    httpsOnly: true
    serverFarmId: plan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: storageConnectionString
        }
        {
          name: 'Database__ConnectionString'
          value: databaseConnectionString
        }
        {
          name: 'Queues__ConnectionString'
          value: queueStorageConnectionString
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
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
    }
  }
  dependsOn: [
    deploymentStorageAccess
  ]
}

output functionAppName string = functionApp.name
output functionIdentityClientId string = deploymentIdentity.properties.clientId
output functionIdentityName string = deploymentIdentity.name
