# Azure Services Setup Guide

This guide walks you through setting up all Azure resources needed for the Enterprise Student System.

## Prerequisites

- Azure subscription (Free tier works for development)
- Azure CLI installed (optional but recommended)
- Visual Studio 2022 or VS Code with Azure extensions

## Step-by-Step Setup

### 1. Create Resource Group

```bash
az group create --name StudentSystem-RG --location eastus
```

Or via Azure Portal:
1. Navigate to **Resource groups**
2. Click **Create**
3. Name: `StudentSystem-RG`
4. Region: East US
5. Click **Review + Create**

### 2. Create Azure SQL Database

```bash
az sql server create \
  --name studentsystem-sql \
  --resource-group StudentSystem-RG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YourSecurePassword123!

az sql db create \
  --name MyDatabase \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --service-objective S0
```

Or via Azure Portal:
1. Navigate to **SQL databases**
2. Click **Create**
3. Select your subscription and resource group
4. Database name: `MyDatabase`
5. Create new server:
   - Server name: `studentsystem-sql`
   - Admin login: `sqladmin`
   - Password: Choose a secure password
   - Location: East US
6. Compute + storage: Standard S0 (10 DTUs)
7. Click **Review + Create**

**Important**: Configure firewall rules to allow your IP and Azure services

### 3. Create Application Insights

```bash
az monitor app-insights component create \
  --app StudentsystemAppInsights \
  --location eastus \
  --resource-group StudentSystem-RG \
  --application-type web
```

Or via Azure Portal:
1. Navigate to **Application Insights**
2. Click **Create**
3. Name: `StudentsystemAppInsights`
4. Resource group: `StudentSystem-RG`
5. Region: East US
6. Click **Review + Create**

**Save the Connection String** from Overview page:
```
InstrumentationKey=xxx;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/
```

### 4. Create Azure SignalR Service

```bash
az signalr create \
  --name studentsystem-signalr \
  --resource-group StudentSystem-RG \
  --sku Free_F1 \
  --unit-count 1 \
  --service-mode Default
```

Or via Azure Portal:
1. Navigate to **Azure SignalR Service**
2. Click **Create**
3. Name: `studentsystem-signalr`
4. Resource group: `StudentSystem-RG`
5. Location: East US
6. Pricing tier: Free (1 unit)
7. Service mode: Default
8. Click **Review + Create**

**Save the Connection String** from Keys section

### 5. Create App Service for API

```bash
az appservice plan create \
  --name StudentSystem-Plan \
  --resource-group StudentSystem-RG \
  --sku B1 \
  --is-linux

az webapp create \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --plan StudentSystem-Plan \
  --runtime "DOTNETCORE:8.0"
```

Or via Azure Portal:
1. Navigate to **App Services**
2. Click **Create**
3. Name: `studentsystem-api`
4. Publish: Code
5. Runtime stack: .NET 8 (STS)
6. Operating System: Windows
7. Region: East US
8. Pricing: Basic B1
9. Click **Review + Create**

### 6. Configure App Service Settings

Add these application settings in the App Service:

```bash
az webapp config appsettings set \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --settings \
    "ConnectionStrings__DefaultConnection=Server=tcp:studentsystem-sql.database.windows.net,1433;Database=MyDatabase;User ID=sqladmin;Password=YourPassword;Encrypt=True;TrustServerCertificate=False;" \
    "ApplicationInsights__ConnectionString=<your-app-insights-connection-string>" \
    "Azure__SignalR__ConnectionString=<your-signalr-connection-string>"
```

Or via Azure Portal:
1. Navigate to your App Service
2. Go to **Configuration** > **Application settings**
3. Add new settings:
   - Name: `ConnectionStrings__DefaultConnection`
     Value: Your Azure SQL connection string
   - Name: `ApplicationInsights__ConnectionString`
     Value: Your App Insights connection string
   - Name: `Azure__SignalR__ConnectionString`
     Value: Your SignalR connection string

### 7. Migrate Database Schema

Option A: From Visual Studio:
1. Open Package Manager Console
2. Set Default project to `StudentSystem.Infrastructure`
3. Run: `Update-Database`

Option B: From Command Line:
```bash
cd StudentSystem.API
dotnet ef database update --project ../StudentSystem.Infrastructure
```

Option C: Use SQL scripts to create tables manually in Azure SQL

### 8. Deploy Application

#### Via Visual Studio:
1. Right-click `StudentSystem.API` project
2. Select **Publish**
3. Target: Azure
4. Specific target: Azure App Service (Windows)
5. Select your `studentsystem-api` app service
6. Click **Publish**

#### Via Azure DevOps:
1. Create a new pipeline
2. Use the included `azure-pipelines.yml`
3. Configure variables in pipeline
4. Run the pipeline

#### Via Azure CLI:
```bash
cd StudentSystem.API
dotnet publish -c Release -o ./publish
cd publish
zip -r deploy.zip .
az webapp deploy \
  --resource-group StudentSystem-RG \
  --name studentsystem-api \
  --src-path deploy.zip \
  --type zip
```

### 9. Verify Deployment

1. Navigate to: `https://studentsystem-api.azurewebsites.net`
2. You should see Swagger UI
3. Test endpoints:
   - Health check: `https://studentsystem-api.azurewebsites.net/health`
   - API: `https://studentsystem-api.azurewebsites.net/api/students`

### 10. Configure CORS (if needed)

If using React or Angular frontend from different domain:

```bash
az webapp cors add \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --allowed-origins https://yourdomain.com
```

## Cost Optimization for Free Tier

Use these tiers to stay within free limits:

- **App Service**: Free F1 or Basic B1
- **Azure SQL**: Basic (5 DTU) - $4.90/month
- **Application Insights**: First 5GB free per month
- **SignalR**: Free tier - 20 concurrent connections

**Estimated Monthly Cost**: $5-10 with these configurations

## Monitoring Setup

### Application Insights

1. Navigate to your App Insights resource
2. Configure:
   - **Alerts**: Set up for failures, response time
   - **Availability tests**: Create ping tests for your API
   - **Live Metrics**: View real-time performance

### Log Stream

View live logs from App Service:
```bash
az webapp log tail --name studentsystem-api --resource-group StudentSystem-RG
```

Or in Azure Portal:
- Navigate to App Service
- Go to **Monitoring** > **Log stream**

## Security Best Practices

### 1. Use Azure Key Vault

Store secrets securely:

```bash
az keyvault create \
  --name studentsystem-kv \
  --resource-group StudentSystem-RG \
  --location eastus

az keyvault secret set \
  --vault-name studentsystem-kv \
  --name "SqlConnectionString" \
  --value "Your-Connection-String"
```

### 2. Enable Managed Identity

```bash
az webapp identity assign \
  --name studentsystem-api \
  --resource-group StudentSystem-RG
```

### 3. Configure Network Security

- Enable HTTPS only
- Restrict SQL firewall to App Service IP
- Use VNet integration for production

## Troubleshooting

### Issue: App Service shows "503 Service Unavailable"
- Check Application logs in App Service
- Verify connection strings are correct
- Ensure SQL firewall allows Azure services

### Issue: Database connection fails
- Verify connection string format
- Check SQL Server firewall rules
- Test connection from Azure Portal query editor

### Issue: Application Insights not receiving data
- Verify instrumentation key is correct
- Check that App Insights SDK is installed
- Wait 1-2 minutes for data to appear

### Issue: SignalR connections failing
- Ensure Azure SignalR is configured in code
- Check connection string
- Verify CORS settings

## Scaling for Production

### Horizontal Scaling

```bash
az appservice plan update \
  --name StudentSystem-Plan \
  --resource-group StudentSystem-RG \
  --number-of-workers 3
```

### Auto-scaling

Set up based on CPU or memory metrics in Azure Portal:
1. Navigate to App Service Plan
2. Go to **Scale out (App Service plan)**
3. Enable autoscale
4. Configure rules

### Database Scaling

Upgrade to higher tier:
```bash
az sql db update \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --name MyDatabase \
  --service-objective S2
```

## Backup and Disaster Recovery

### Database Backups

Azure SQL automatically creates backups. Configure retention:

```bash
az sql db ltr-policy set \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --database MyDatabase \
  --weekly-retention P4W \
  --monthly-retention P12M \
  --yearly-retention P5Y
```

### App Service Backups

1. Navigate to App Service
2. Go to **Backups**
3. Configure automatic backups to Azure Storage

## Clean Up Resources

To delete all resources:

```bash
az group delete --name StudentSystem-RG --yes --no-wait
```

**Warning**: This permanently deletes ALL resources in the resource group!

## Additional Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Application Insights Documentation](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Azure SignalR Documentation](https://docs.microsoft.com/en-us/azure/azure-signalr/)
- [Azure SQL Documentation](https://docs.microsoft.com/en-us/azure/azure-sql/)

---

**Support**: For Azure-specific issues, consult the Azure Portal help or Azure support team.
