// TaskPilot Azure Bicep Template
// Deploys TaskPilotApi (Web API), TaskPilotUI (Blazor WASM), TaskPilotArchival (Azure Function), and Azure SQL Database

@description('Environment name (dev, staging, prod)')
param environment string = 'dev'

@description('Location for all resources')
param location string = 'Canada Central'

@description('Base name for all resources')
param baseName string = 'taskpilot'

@description('SQL Server administrator login')
param sqlAdminLogin string = 'taskpilotadmin'

@description('SQL Server administrator password')
@secure()
param sqlAdminPassword string

// Variables
var resourcePrefix = '${baseName}-${environment}'
var appServicePlanName = '${resourcePrefix}-plan'
var webApiName = '${resourcePrefix}-api'
var blazorAppName = '${resourcePrefix}-ui'
var functionAppName = '${resourcePrefix}-archival'
var applicationInsightsName = '${resourcePrefix}-insights'
var logAnalyticsWorkspaceName = '${resourcePrefix}-logs'
var sqlServerName = '${resourcePrefix}-sql-server'
var sqlDatabaseName = '${resourcePrefix}-db'

// External service connection strings - values come from parameters file
@secure()
param serviceBusConnectionString string

@secure()
param cosmosDbConnectionString string

@secure()
param azureBlobConnectionString string

param azureOpenAIEndpoint string

@secure()
param azureOpenAIApiKey string

param azureSearchEndpoint string

@secure()
param azureSearchApiKey string

// Azure AD Configuration
param azureAdTenantId string
param azureAdClientId string

// Log Analytics Workspace
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Application Insights
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648 // 2GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
  }
}

// SQL Server Firewall Rule - Allow Azure Services
resource sqlFirewallRuleAzure 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// App Service Plan (Basic B1 tier for Web API and Blazor App)
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  properties: {
    reserved: false
  }
}

// Consumption Plan for Function App (Y1 - Dynamic/Consumption)
resource functionAppServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${resourcePrefix}-func-plan'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {
    reserved: false
  }
  kind: 'functionapp'
}

// Web API App Service
resource webApiApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webApiName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'dotnet'
        }
      ]
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'CosmosDb__DatabaseName'
          value: 'taskpilot-cosmicdb'
        }
        {
          name: 'ServiceBus__ArchivalQueueName'
          value: 'board-archival-queue'
        }
        {
          name: 'AzureAd__Instance'
          value: 'https://login.microsoftonline.com/'
        }
        {
          name: 'AzureAd__TenantId'
          value: azureAdTenantId
        }
        {
          name: 'AzureAd__ClientId'
          value: azureAdClientId
        }
        {
          name: 'AzureAd__Audience'
          value: azureAdClientId
        }
        {
          name: 'AzureBlob__ContainerName'
          value: 'files'
        }
        {
          name: 'Avatar__Size'
          value: '256'
        }
        {
          name: 'Cors__AllowedOrigins__0'
          value: 'https://${blazorAppName}.azurewebsites.net'
        }
        {
          name: 'AzureOpenAI__Endpoint'
          value: azureOpenAIEndpoint
        }
        {
          name: 'AzureOpenAI__DeploymentName'
          value: 'gpt-35-turbo'
        }
        {
          name: 'AzureAISearch__Endpoint'
          value: azureSearchEndpoint
        }
        {
          name: 'KernelMemory__EmbeddingGenerator__Type'
          value: 'AzureOpenAI'
        }
        {
          name: 'KernelMemory__EmbeddingGenerator__Endpoint'
          value: azureOpenAIEndpoint
        }
        {
          name: 'KernelMemory__EmbeddingGenerator__DeploymentName'
          value: 'text-embedding-3-small'
        }
        {
          name: 'AzureOpenAI__ApiKey'
          value: azureOpenAIApiKey
        }
        {
          name: 'AzureAISearch__ApiKey'
          value: azureSearchApiKey
        }
        {
          name: 'KernelMemory__EmbeddingGenerator__ApiKey'
          value: azureOpenAIApiKey
        }
        {
          name: 'AzureBlob__ConnectionString'
          value: azureBlobConnectionString
        }
      ]
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
          type: 'SQLAzure'
        }
        {
          name: 'ServiceBus'
          connectionString: serviceBusConnectionString
          type: 'Custom'
        }
        {
          name: 'CosmosDb'
          connectionString: cosmosDbConnectionString
          type: 'Custom'
        }
      ]
    }
  }
  dependsOn: [
    sqlDatabase
  ]
}

// Blazor WASM App Service
resource blazorApp 'Microsoft.Web/sites@2022-09-01' = {
  name: blazorAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'dotnet'
        }
      ]
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'App__BaseUrl'
          value: 'https://${blazorAppName}.azurewebsites.net'
        }
        {
          name: 'AzureAd__ClientId'
          value: azureAdClientId
        }
        {
          name: 'AzureAd__TenantId'
          value: azureAdTenantId
        }
        {
          name: 'AzureAd__RedirectUri'
          value: 'https://${blazorAppName}.azurewebsites.net/login'
        }
        {
          name: 'AzureAd__Scope'
          value: 'api://${azureAdClientId}/TaskPilot_API.all'
        }
        {
          name: 'Api__BaseUrl'
          value: 'https://${webApiName}.azurewebsites.net'
        }
        {
          name: 'ProSettings__NavTheme'
          value: 'light'
        }
        {
          name: 'ProSettings__Layout'
          value: 'side'
        }
        {
          name: 'ProSettings__ContentWidth'
          value: 'Fluid'
        }
        {
          name: 'ProSettings__FixedHeader'
          value: 'false'
        }
        {
          name: 'ProSettings__FixSiderbar'
          value: 'true'
        }
        {
          name: 'ProSettings__Title'
          value: 'TaskPilot'
        }
        {
          name: 'ProSettings__PrimaryColor'
          value: '#1890ff'
        }
        {
          name: 'ProSettings__ColorWeak'
          value: 'false'
        }
        {
          name: 'ProSettings__SplitMenus'
          value: 'false'
        }
        {
          name: 'ProSettings__HeaderRender'
          value: 'false'
        }
        {
          name: 'ProSettings__FooterRender'
          value: 'true'
        }
        {
          name: 'ProSettings__MenuRender'
          value: 'true'
        }
        {
          name: 'ProSettings__MenuHeaderRender'
          value: 'true'
        }
        {
          name: 'ProSettings__HeaderHeight'
          value: '48'
        }
        {
          name: 'ProSettings__CollapsedButtonRender'
          value: 'true'
        }
        {
          name: 'ProSettings__SettingDrawer'
          value: 'false'
        }
        {
          name: 'ProSettings__Menu__Locale'
          value: 'true'
        }
      ]
    }
  }
}

// Function App (Consumption Plan - Free)
resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: azureBlobConnectionString
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: azureBlobConnectionString
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'ServiceBusConnection'
          value: serviceBusConnectionString
        }
        {
          name: 'AzureBlob__ConnectionString'
          value: azureBlobConnectionString
        }
        {
          name: 'AzureBlob__ContainerName'
          value: 'archivals'
        }
        {
          name: 'CosmosDbConnection'
          value: cosmosDbConnectionString
        }
        {
          name: 'CosmosDbDatabaseId'
          value: 'taskpilot-cosmicdb'
        }
        {
          name: 'CosmosDbContainerId'
          value: 'ArchivalJobs'
        }
        {
          name: 'ConnectionStrings__SqlServer'
          value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
      ]
    }
  }
  dependsOn: [
    sqlDatabase
  ]
}

// Outputs
output webApiUrl string = 'https://${webApiApp.properties.defaultHostName}'
output blazorAppUrl string = 'https://${blazorApp.properties.defaultHostName}'
output functionAppUrl string = 'https://${functionApp.properties.defaultHostName}'
output applicationInsightsInstrumentationKey string = applicationInsights.properties.InstrumentationKey
output resourceGroupName string = resourceGroup().name
output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output sqlConnectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
