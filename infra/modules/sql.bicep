param entraAdministratorClientId string
param entraAdministratorName string
param entraAdministratorTenantId string
param environmentName string
param location string
param resourceSuffix string
param settings object

@minLength(3)
param workloadName string

var uniqueSuffix = take(resourceSuffix, 5)
var serverName = take(toLower('sql-${workloadName}-${environmentName}-${uniqueSuffix}'), 63)
var databaseName = 'sqldb-${workloadName}-${environmentName}'

resource server 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: serverName
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: entraAdministratorName
      principalType: 'Application'
      sid: entraAdministratorClientId
      tenantId: entraAdministratorTenantId
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

resource entraOnlyAuthentication 'Microsoft.Sql/servers/azureADOnlyAuthentications@2023-08-01-preview' = {
  parent: server
  name: 'Default'
  properties: {
    azureADOnlyAuthentication: true
  }
}

resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: server
  name: 'AllowAzureServices'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: server
  name: databaseName
  location: location
  sku: {
    capacity: settings.maxVcores
    family: settings.family
    name: settings.skuName
    tier: settings.tier
  }
  properties: {
    autoPauseDelay: settings.autoPauseDelayMinutes
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    licenseType: 'LicenseIncluded'
    maxSizeBytes: settings.maxSizeBytes
    minCapacity: json(settings.minVcores)
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: settings.backupStorageRedundancy
    zoneRedundant: false
  }
}

output databaseName string = database.name
output serverFullyQualifiedDomainName string = server.properties.fullyQualifiedDomainName
output serverName string = server.name
