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

var createOrderQueueName = 'order-create'
var orderUpdateQueueName = 'order-update'

module storage './modules/storage.bicep' = {
  name: 'storage'
  params: {
    environmentName: environmentName
    location: location
    queueNames: [
      createOrderQueueName
      orderUpdateQueueName
    ]
    resourceSuffix: resourceSuffix
    workloadName: workloadName
  }
}

module sql './modules/sql.bicep' = {
  name: 'sql'
  params: {
    entraAdministratorClientId: sqlEntraAdministratorClientId
    entraAdministratorName: sqlEntraAdministratorName
    entraAdministratorTenantId: sqlEntraAdministratorTenantId
    environmentName: environmentName
    location: location
    resourceSuffix: resourceSuffix
    settings: sqlSettings
    workloadName: workloadName
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
    resourceSuffix: resourceSuffix
    orderUpdateQueueName: orderUpdateQueueName
    workloadName: workloadName
  }
}

module functions './modules/function-app.bicep' = {
  name: 'function-app'
  params: {
    createOrderQueueName: createOrderQueueName
    databaseName: sql.outputs.databaseName
    databaseServerFullyQualifiedDomainName: sql.outputs.serverFullyQualifiedDomainName
    deploymentContainerUri: storage.outputs.deploymentContainerUri
    environmentName: environmentName
    functionAppSettings: functionAppSettings
    location: location
    queueStorageConnectionString: storageConnectionString
    resourceSuffix: resourceSuffix
    storageAccountId: storage.outputs.storageAccountId
    storageAccountName: storage.outputs.storageAccountName
    storageConnectionString: storageConnectionString
    orderUpdateQueueName: orderUpdateQueueName
    workloadName: workloadName
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
