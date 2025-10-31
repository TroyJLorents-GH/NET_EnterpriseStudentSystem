# Azure Services Setup Guide

This guide walks you through setting up all Azure resources needed for the Enterprise Student System.

## Prerequisites

- Azure subscription (Free tier works for development)
- Azure CLI installed (optional but recommended)
- Visual Studio 2022 or VS Code with Azure extensions

## Step-by-Step Setup

### 1. Create Resource Group

### 2. Create Azure SQL Database

### 3. Create Application Insights

### 4. Create Azure SignalR Service

### 5. Create App Service for API

### 6. Configure App Service Setting

### 7. Migrate Database Schema


### 8. Deploy Application


### 9. Verify Deployment

### 10. Configure CORS (if needed)



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

Or in Azure Portal:
- Navigate to App Service
- Go to **Monitoring** > **Log stream**


### Auto-scaling

Set up based on CPU or memory metrics in Azure Portal:
1. Navigate to App Service Plan
2. Go to **Scale out (App Service plan)**
3. Enable autoscale
4. Configure rules

### Database Scaling



### App Service Backups

1. Navigate to App Service
2. Go to **Backups**
3. Configure automatic backups to Azure Storage



## Additional Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Application Insights Documentation](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Azure SignalR Documentation](https://docs.microsoft.com/en-us/azure/azure-signalr/)
- [Azure SQL Documentation](https://docs.microsoft.com/en-us/azure/azure-sql/)

---

**Support**: For Azure-specific issues, consult the Azure Portal help or Azure support team.
