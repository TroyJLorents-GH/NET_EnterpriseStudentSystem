# Azure Deployment Guide - Blazor Full Stack Application

This guide walks you through deploying the Enterprise Student System with both the API backend and Blazor Server frontend to Azure.

## Architecture Overview

Your application consists of:
- **StudentSystem.API** - REST API backend (.NET 8)
- **StudentSystem.Web** - Blazor Server frontend (.NET 8)
- **Azure SQL Database** - Data storage
- **Application Insights** - Monitoring
- **Azure SignalR** - Real-time communication for Blazor

## Prerequisites

- Azure subscription (Free tier works for testing)
- Azure CLI installed (optional but recommended)
- Visual Studio 2022 or VS Code
- SQL Server with MyDatabase already populated with data

## Step-by-Step Deployment

### 1. Create Resource Group

All resources will go in one resource group for easy management.

**Azure Portal:**
1. Navigate to **Resource groups**
2. Click **Create**
3. Name: `StudentSystem-RG`
4. Region: **East US** (or your preferred region)
5. Click **Review + Create**

**Azure CLI:**
```bash
az group create --name StudentSystem-RG --location eastus
```

---

### 2. Create Azure SQL Database

You'll migrate your existing MyDatabase schema and data to Azure SQL.

**Azure Portal:**
1. Navigate to **SQL databases**
2. Click **Create**
3. **Basics:**
   - Subscription: Your subscription
   - Resource group: `StudentSystem-RG`
   - Database name: `MyDatabase`
   - Server: Click **Create new**
     - Server name: `studentsystem-sql` (must be globally unique)
     - Location: East US
     - Authentication method: **Use SQL authentication**
     - Server admin login: `sqladmin`
     - Password: Create a strong password (save it!)
4. **Compute + storage:**
   - Service tier: **Basic** ($4.90/month) or **Standard S0** ($15/month)
5. **Networking:**
   - Connectivity method: **Public endpoint**
   - Allow Azure services: **Yes**
   - Add current client IP: **Yes**
6. Click **Review + Create**

**Important:** Save your connection string from the database overview page:
```
Server=tcp:studentsystem-sql.database.windows.net,1433;Initial Catalog=MyDatabase;Persist Security Info=False;User ID=sqladmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**Azure CLI:**
```bash
az sql server create \
  --name studentsystem-sql \
  --resource-group StudentSystem-RG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password "YourSecurePassword123!"

az sql db create \
  --name MyDatabase \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --service-objective Basic

# Allow Azure services
az sql server firewall-rule create \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow your IP
az sql server firewall-rule create \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --name AllowMyIP \
  --start-ip-address YOUR_IP \
  --end-ip-address YOUR_IP
```

---

### 3. Create Application Insights

Monitors both API and Web applications.

**Azure Portal:**
1. Navigate to **Application Insights**
2. Click **Create**
3. Name: `studentsystem-insights`
4. Resource group: `StudentSystem-RG`
5. Region: East US
6. Click **Review + Create**

**After creation:**
- Go to the resource
- Copy the **Connection String** from the Overview page
- Format: `InstrumentationKey=xxx;IngestionEndpoint=https://...`

**Azure CLI:**
```bash
az monitor app-insights component create \
  --app studentsystem-insights \
  --location eastus \
  --resource-group StudentSystem-RG \
  --application-type web
```

---

### 4. Create Azure SignalR Service

Required for Blazor Server's real-time WebSocket connections.

**Azure Portal:**
1. Navigate to **Azure SignalR Service**
2. Click **Create**
3. Name: `studentsystem-signalr`
4. Resource group: `StudentSystem-RG`
5. Location: East US
6. Pricing tier: **Free** (1 unit, up to 20 connections)
7. Service mode: **Default**
8. Click **Review + Create**

**After creation:**
- Navigate to **Settings** > **Keys**
- Copy the **Connection string** (primary)

**Azure CLI:**
```bash
az signalr create \
  --name studentsystem-signalr \
  --resource-group StudentSystem-RG \
  --sku Free_F1 \
  --unit-count 1 \
  --service-mode Default
```

---

### 5. Create App Service Plan

One plan can host both the API and Web applications.

**Azure Portal:**
1. Navigate to **App Service plans**
2. Click **Create**
3. Name: `StudentSystem-Plan`
4. Resource group: `StudentSystem-RG`
5. Operating System: **Windows**
6. Region: East US
7. Pricing tier: **Basic B1** ($13.14/month) or **Free F1** (for testing)
8. Click **Review + Create**

**Azure CLI:**
```bash
az appservice plan create \
  --name StudentSystem-Plan \
  --resource-group StudentSystem-RG \
  --sku B1
```

---

### 6. Create App Service for API

**Azure Portal:**
1. Navigate to **App Services**
2. Click **Create**
3. **Basics:**
   - Name: `studentsystem-api` (must be globally unique)
   - Publish: **Code**
   - Runtime stack: **.NET 8 (LTS)**
   - Operating System: **Windows**
   - Region: East US
4. **App Service Plan:**
   - Select existing: `StudentSystem-Plan`
5. Click **Review + Create**

**Azure CLI:**
```bash
az webapp create \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --plan StudentSystem-Plan \
  --runtime "DOTNET:8"
```

---

### 7. Create App Service for Blazor Web

**Azure Portal:**
1. Navigate to **App Services**
2. Click **Create**
3. **Basics:**
   - Name: `studentsystem-web` (must be globally unique)
   - Publish: **Code**
   - Runtime stack: **.NET 8 (LTS)**
   - Operating System: **Windows**
   - Region: East US
4. **App Service Plan:**
   - Select existing: `StudentSystem-Plan`
5. Click **Review + Create**

**Azure CLI:**
```bash
az webapp create \
  --name studentsystem-web \
  --resource-group StudentSystem-RG \
  --plan StudentSystem-Plan \
  --runtime "DOTNET:8"
```

---

### 8. Configure API App Service Settings

Add these application settings in the **studentsystem-api** App Service:

**Azure Portal:**
1. Navigate to `studentsystem-api` App Service
2. Go to **Configuration** > **Application settings**
3. Click **New connection string**:
   - Name: `DefaultConnection`
   - Value: `Server=tcp:studentsystem-sql.database.windows.net,1433;Initial Catalog=MyDatabase;User ID=sqladmin;Password={your_password};Encrypt=True;`
   - Type: **SQLAzure**
4. Click **New application setting** for each:

| Name | Value |
|------|-------|
| `ApplicationInsights__ConnectionString` | Your App Insights connection string |
| `Azure__SignalR__ConnectionString` | Your SignalR connection string |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

5. Click **Save**

**Azure CLI:**
```bash
az webapp config connection-string set \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:studentsystem-sql.database.windows.net,1433;Initial Catalog=MyDatabase;User ID=sqladmin;Password=YourPassword;Encrypt=True;"

az webapp config appsettings set \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --settings \
    ApplicationInsights__ConnectionString="<your-connection-string>" \
    Azure__SignalR__ConnectionString="<your-signalr-connection-string>" \
    ASPNETCORE_ENVIRONMENT="Production"
```

---

### 9. Configure Web App Service Settings

Add these settings in the **studentsystem-web** App Service:

**Azure Portal:**
1. Navigate to `studentsystem-web` App Service
2. Go to **Configuration** > **Application settings**
3. Click **New application setting** for each:

| Name | Value |
|------|-------|
| `ApiSettings__BaseUrl` | `https://studentsystem-api.azurewebsites.net` |
| `ApplicationInsights__ConnectionString` | Your App Insights connection string |
| `Azure__SignalR__ConnectionString` | Your SignalR connection string |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

4. Click **Save**

**Azure CLI:**
```bash
az webapp config appsettings set \
  --name studentsystem-web \
  --resource-group StudentSystem-RG \
  --settings \
    ApiSettings__BaseUrl="https://studentsystem-api.azurewebsites.net" \
    ApplicationInsights__ConnectionString="<your-connection-string>" \
    Azure__SignalR__ConnectionString="<your-signalr-connection-string>" \
    ASPNETCORE_ENVIRONMENT="Production"
```

---

### 10. Migrate Your Database to Azure SQL

You need to copy your existing tables and data from local SQL Server to Azure SQL.

**Option A: Using SQL Server Management Studio (SSMS)**

1. Open SSMS
2. Connect to your **local** SQL Server (`.\SQLEXPRESS`)
3. Right-click `MyDatabase`
4. Select **Tasks** > **Generate Scripts**
5. Choose your tables (StudentData, ClassSchedule2254, StudentClassAssignments, GraderApplications)
6. Advanced options:
   - Types of data to script: **Schema and data**
7. Save script
8. Connect to Azure SQL: `studentsystem-sql.database.windows.net`
9. Open and execute the saved script

**Option B: Using Entity Framework Migrations**

```bash
cd C:\Users\Troy\projects\EnterpriseStudentSystem\StudentSystem.API

# Update connection string temporarily in appsettings.json to point to Azure SQL
# Then run:
dotnet ef database update --project ../StudentSystem.Infrastructure
```

**Option C: Azure Data Migration Service**

1. Navigate to **Azure Database Migration Service**
2. Create a new migration project
3. Source: Your local SQL Server
4. Target: Azure SQL Database
5. Follow the wizard

---

### 11. Deploy API to Azure

**Option A: Visual Studio (Recommended for beginners)**

1. Open solution in Visual Studio
2. Right-click **StudentSystem.API** project
3. Select **Publish**
4. Target: **Azure**
5. Specific target: **Azure App Service (Windows)**
6. Select your subscription
7. Select `studentsystem-api` App Service
8. Click **Finish**
9. Click **Publish** button

**Option B: Command Line**

```bash
cd C:\Users\Troy\projects\EnterpriseStudentSystem\StudentSystem.API
dotnet publish -c Release -o ./publish

# Create zip file
cd publish
tar -a -c -f deploy.zip *

# Deploy
az webapp deploy \
  --resource-group StudentSystem-RG \
  --name studentsystem-api \
  --src-path deploy.zip \
  --type zip
```

---

### 12. Deploy Blazor Web to Azure

**Option A: Visual Studio**

1. Right-click **StudentSystem.Web** project
2. Select **Publish**
3. Target: **Azure**
4. Specific target: **Azure App Service (Windows)**
5. Select `studentsystem-web` App Service
6. Click **Finish**
7. Click **Publish** button

**Option B: Command Line**

```bash
cd C:\Users\Troy\projects\EnterpriseStudentSystem\StudentSystem.Web
dotnet publish -c Release -o ./publish

# Create zip file
cd publish
tar -a -c -f deploy.zip *

# Deploy
az webapp deploy \
  --resource-group StudentSystem-RG \
  --name studentsystem-web \
  --src-path deploy.zip \
  --type zip
```

---

### 13. Verify Deployment

**Test API:**
1. Open browser: `https://studentsystem-api.azurewebsites.net`
2. You should see the Swagger UI
3. Test endpoints:
   - `https://studentsystem-api.azurewebsites.net/api/students`
   - `https://studentsystem-api.azurewebsites.net/api/assignments`

**Test Blazor Web:**
1. Open browser: `https://studentsystem-web.azurewebsites.net`
2. You should see your Blazor application
3. Navigate through:
   - Students page
   - Classes page
   - Assignments page
4. Test creating/editing assignments

---

### 14. Configure Custom Domain (Optional)

If you have a custom domain:

**API:**
```bash
az webapp config hostname add \
  --webapp-name studentsystem-api \
  --resource-group StudentSystem-RG \
  --hostname api.yourdomain.com
```

**Web:**
```bash
az webapp config hostname add \
  --webapp-name studentsystem-web \
  --resource-group StudentSystem-RG \
  --hostname app.yourdomain.com
```

Then update DNS records to point to Azure.

---

## Cost Breakdown

### Minimal Setup (Free/Low Cost)
- **App Service Plan**: Free F1 ($0) or Basic B1 ($13.14/month)
- **Azure SQL Database**: Basic tier ($4.90/month)
- **Application Insights**: First 5GB free/month
- **Azure SignalR**: Free tier ($0)

**Total Estimated Cost**: $5-20/month

### Production Setup
- **App Service Plan**: Standard S1 ($69.35/month)
- **Azure SQL Database**: Standard S2 ($74/month)
- **Application Insights**: ~$2.30/GB after 5GB
- **Azure SignalR**: Standard ($49/month for 1000 units)

**Total Estimated Cost**: $150-200/month

---

## Monitoring and Troubleshooting

### View Live Logs

**Azure Portal:**
1. Navigate to App Service (`studentsystem-api` or `studentsystem-web`)
2. Go to **Monitoring** > **Log stream**
3. Watch real-time logs

**Azure CLI:**
```bash
# API logs
az webapp log tail \
  --name studentsystem-api \
  --resource-group StudentSystem-RG

# Web logs
az webapp log tail \
  --name studentsystem-web \
  --resource-group StudentSystem-RG
```

### Application Insights

1. Navigate to `studentsystem-insights`
2. View:
   - **Live Metrics**: Real-time performance
   - **Failures**: Error tracking
   - **Performance**: Response times
   - **Availability**: Uptime monitoring

### Common Issues

**Issue: 502 Bad Gateway or 503 Service Unavailable**
- Check App Service logs
- Verify connection strings are correct
- Ensure database firewall allows Azure services
- Restart the App Service

**Issue: Database connection fails**
- Verify Azure SQL firewall rules include App Service IPs
- Check connection string format
- Test connection from Azure Portal SQL query editor

**Issue: Blazor app can't connect to API**
- Verify `ApiSettings__BaseUrl` is set correctly in Web App Service
- Check API is responding: visit `https://studentsystem-api.azurewebsites.net`
- Check CORS settings if needed

**Issue: SignalR disconnects**
- Ensure Azure SignalR connection string is correct
- Verify Free tier limits (20 connections max)
- Check WebSockets are enabled in App Service (Configuration > General settings)

---

## Security Best Practices

### 1. Enable HTTPS Only

**Azure Portal:**
1. Navigate to App Service
2. Go to **Settings** > **Configuration** > **General settings**
3. HTTPS Only: **On**

```bash
az webapp update \
  --name studentsystem-api \
  --resource-group StudentSystem-RG \
  --https-only true

az webapp update \
  --name studentsystem-web \
  --resource-group StudentSystem-RG \
  --https-only true
```

### 2. Use Azure Key Vault (Recommended for Production)

Store secrets securely:

```bash
# Create Key Vault
az keyvault create \
  --name studentsystem-kv \
  --resource-group StudentSystem-RG \
  --location eastus

# Store secrets
az keyvault secret set \
  --vault-name studentsystem-kv \
  --name "SqlConnectionString" \
  --value "Server=tcp:studentsystem-sql.database.windows.net..."

# Enable Managed Identity for App Services
az webapp identity assign \
  --name studentsystem-api \
  --resource-group StudentSystem-RG

# Grant access to Key Vault
az keyvault set-policy \
  --name studentsystem-kv \
  --object-id <managed-identity-id> \
  --secret-permissions get list
```

Then reference in App Service configuration:
```
@Microsoft.KeyVault(SecretUri=https://studentsystem-kv.vault.azure.net/secrets/SqlConnectionString/)
```

### 3. Restrict SQL Firewall

Only allow specific IPs:
1. Navigate to Azure SQL Server
2. Go to **Security** > **Networking**
3. Remove "Allow Azure services"
4. Add specific App Service outbound IPs

---

## Scaling for Growth

### Horizontal Scaling (Multiple Instances)

```bash
az appservice plan update \
  --name StudentSystem-Plan \
  --resource-group StudentSystem-RG \
  --number-of-workers 3
```

### Auto-scaling

1. Navigate to App Service Plan
2. Go to **Scale out (App Service plan)**
3. Enable autoscale
4. Configure rules:
   - Scale out when CPU > 70%
   - Scale in when CPU < 30%
   - Min instances: 1
   - Max instances: 5

### Database Scaling

```bash
# Upgrade to higher tier
az sql db update \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --name MyDatabase \
  --service-objective S2
```

---

## Backup and Disaster Recovery

### Database Backups

Azure SQL automatically creates backups:
- Point-in-time restore: Last 7-35 days
- Long-term retention: Configure as needed

**Configure long-term retention:**
```bash
az sql db ltr-policy set \
  --resource-group StudentSystem-RG \
  --server studentsystem-sql \
  --database MyDatabase \
  --weekly-retention P4W \
  --monthly-retention P12M
```

### App Service Backups

1. Navigate to App Service
2. Go to **Backups**
3. Configure:
   - Storage account
   - Backup frequency
   - Retention period

---

## Continuous Deployment (CI/CD)

### Using GitHub Actions

Create `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy-api:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      - name: Build API
        run: dotnet publish StudentSystem.API/StudentSystem.API.csproj -c Release -o api-publish
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: studentsystem-api
          publish-profile: ${{ secrets.AZURE_API_PUBLISH_PROFILE }}
          package: api-publish

  deploy-web:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      - name: Build Web
        run: dotnet publish StudentSystem.Web/StudentSystem.Web.csproj -c Release -o web-publish
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: studentsystem-web
          publish-profile: ${{ secrets.AZURE_WEB_PUBLISH_PROFILE }}
          package: web-publish
```

Download publish profiles from each App Service and add as GitHub secrets.

---

## Clean Up Resources

To delete everything and stop charges:

```bash
az group delete --name StudentSystem-RG --yes --no-wait
```

**Warning**: This permanently deletes ALL resources!

---

## Your Deployment URLs

After deployment, your application will be available at:

- **API (Swagger)**: `https://studentsystem-api.azurewebsites.net`
- **Web Application**: `https://studentsystem-web.azurewebsites.net`
- **Application Insights**: Available in Azure Portal

---

## Next Steps

1. Set up alerts in Application Insights
2. Configure availability tests
3. Set up Azure Key Vault for secrets
4. Configure custom domain names
5. Set up staging slots for zero-downtime deployments
6. Implement CI/CD pipeline

---

## Support Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Blazor on Azure](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server)
- [Azure SignalR with Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server#azure-signalr-service)
- [Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)

---

**Need Help?** Check the troubleshooting section or Azure Portal help resources.
