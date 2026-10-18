using '../main.bicep'

param environmentName = 'prod'
param location = 'francecentral'
param workloadName = 'onlinestore'

param containerAppSettings = {
  cpu: '0.5'
  memory: '1Gi'
  minReplicas: 0
  maxReplicas: 2
}

param functionAppSettings = {
  instanceMemoryMB: 2048
  maximumInstanceCount: 100
}

param sqlSettings = {
  skuName: 'GP_S_Gen5_2'
  tier: 'GeneralPurpose'
  family: 'Gen5'
  minVcores: '0.5'
  maxVcores: 2
  autoPauseDelayMinutes: 30
  maxSizeBytes: 5368709120
  backupStorageRedundancy: 'Local'
}

param sqlEntraAdministratorClientId = readEnvironmentVariable('AZURE_CLIENT_ID')
param sqlEntraAdministratorName = readEnvironmentVariable('AZURE_SQL_ADMINISTRATOR_NAME')
