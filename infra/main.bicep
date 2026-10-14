targetScope = 'subscription'

@allowed([
  'dev'
  'prod'
])
@description('Deployment environment name.')
param environmentName string

@description('Azure region for all resources.')
param location string = 'francecentral'

@description('Short workload name used in resource names.')
param workloadName string = 'onlinestore'

@description('Container App compute and scaling configuration.')
param containerAppSettings object

@description('Azure Functions Flex Consumption configuration.')
param functionAppSettings object

@description('Azure SQL Database compute and storage configuration.')
param sqlSettings object

@description('Display name of the Microsoft Entra application used by the deployment pipeline.')
param sqlEntraAdministratorName string

@description('Application/client ID of the Microsoft Entra application used by the deployment pipeline.')
param sqlEntraAdministratorClientId string

@description('API image. The deployment pipeline replaces the bootstrap image after pushing to ACR.')
param apiImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

var resourceGroupName = 'rg-${workloadName}-${environmentName}'
var resourceSuffix = uniqueString(subscription().subscriptionId, resourceGroupName)

resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
  tags: {
    environment: environmentName
    workload: workloadName
    managedBy: 'bicep'
  }
}

module workload './workload.bicep' = {
  name: 'workload-${environmentName}'
  scope: resourceGroup
  params: {
    apiImage: apiImage
    containerAppSettings: containerAppSettings
    environmentName: environmentName
    functionAppSettings: functionAppSettings
    location: location
    resourceSuffix: resourceSuffix
    sqlEntraAdministratorClientId: sqlEntraAdministratorClientId
    sqlEntraAdministratorName: sqlEntraAdministratorName
    sqlEntraAdministratorTenantId: tenant().tenantId
    sqlSettings: sqlSettings
    workloadName: workloadName
  }
}

output apiUrl string = workload.outputs.apiUrl
output containerAppName string = workload.outputs.containerAppName
output containerRegistryName string = workload.outputs.containerRegistryName
output functionAppName string = workload.outputs.functionAppName
output resourceGroupName string = resourceGroup.name
output apiIdentityClientId string = workload.outputs.apiIdentityClientId
output apiIdentityName string = workload.outputs.apiIdentityName
output functionIdentityClientId string = workload.outputs.functionIdentityClientId
output functionIdentityName string = workload.outputs.functionIdentityName
output sqlDatabaseName string = workload.outputs.sqlDatabaseName
output sqlServerName string = workload.outputs.sqlServerName
output sqlServerFullyQualifiedDomainName string = workload.outputs.sqlServerFullyQualifiedDomainName
