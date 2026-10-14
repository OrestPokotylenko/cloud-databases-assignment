targetScope = 'resourceGroup'

param apiImage string
param containerAppSettings object
param environmentName string
param functionAppSettings object
param location string
param resourceSuffix string
param sqlEntraAdministratorClientId string
param sqlEntraAdministratorName string
param sqlEntraAdministratorTenantId string
param sqlSettings object
param workloadName string

var resourceNamePrefix = '${workloadName}-${environmentName}'
var createOrderQueueName = 'order-create'
var updateOrderStatusQueueName = 'order-status-update'

module storage './modules/storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    queueNames: [
      createOrderQueueName
      updateOrderStatusQueueName
    ]
    resourceNamePrefix: resourceNamePrefix
    resourceSuffix: resourceSuffix
  }
}

module sql './modules/sql.bicep' = {
  name: 'sql'
  params: {
    entraAdministratorClientId: sqlEntraAdministratorClientId
    entraAdministratorName: sqlEntraAdministratorName
    entraAdministratorTenantId: sqlEntraAdministratorTenantId
    location: location
    resourceNamePrefix: resourceNamePrefix
    resourceSuffix: resourceSuffix
    settings: sqlSettings
  }
}

var storageConnectionString = storage.outputs.connectionString

module containerPlatform './modules/container-platform.bicep' = {
  name: 'container-platform'
  params: {
    apiImage: apiImage
    containerAppSettings: containerAppSettings
    createOrderQueueName: createOrderQueueName
    databaseName: sql.outputs.databaseName
    databaseServerFullyQualifiedDomainName: sql.outputs.serverFullyQualifiedDomainName
    environmentName: environmentName
    location: location
    queueStorageConnectionString: storageConnectionString
    resourceNamePrefix: resourceNamePrefix
    resourceSuffix: resourceSuffix
    updateOrderStatusQueueName: updateOrderStatusQueueName
  }
}

module functions './modules/function-app.bicep' = {
  name: 'function-app'
  params: {
    createOrderQueueName: createOrderQueueName
    databaseName: sql.outputs.databaseName
    databaseServerFullyQualifiedDomainName: sql.outputs.serverFullyQualifiedDomainName
    deploymentContainerUri: storage.outputs.deploymentContainerUri
    functionAppSettings: functionAppSettings
    location: location
    queueStorageConnectionString: storageConnectionString
    resourceNamePrefix: resourceNamePrefix
    resourceSuffix: resourceSuffix
    storageAccountId: storage.outputs.storageAccountId
    storageAccountName: storage.outputs.storageAccountName
    storageConnectionString: storageConnectionString
    updateOrderStatusQueueName: updateOrderStatusQueueName
  }
}

output apiUrl string = containerPlatform.outputs.apiUrl
output apiIdentityClientId string = containerPlatform.outputs.apiIdentityClientId
output apiIdentityName string = containerPlatform.outputs.apiIdentityName
output containerAppName string = containerPlatform.outputs.containerAppName
output containerRegistryName string = containerPlatform.outputs.containerRegistryName
output functionAppName string = functions.outputs.functionAppName
output functionIdentityClientId string = functions.outputs.functionIdentityClientId
output functionIdentityName string = functions.outputs.functionIdentityName
output sqlDatabaseName string = sql.outputs.databaseName
output sqlServerName string = sql.outputs.serverName
output sqlServerFullyQualifiedDomainName string = sql.outputs.serverFullyQualifiedDomainName
