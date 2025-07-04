# 🛠️ Vapor – Local Development Setup Instructions

Dear User,

To run the application, you must configure environment-specific credentials that are intentionally excluded for security purposes.

Please follow the steps below to prepare the application for local development:

---

## 1. 📄 `appsettings.json` – Backend Configuration

Navigate to the `VaporWebAPI` folder and update the `appsettings.json` file with the following structure:

```json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server connection string"
},
"JwtSettings": {
  "Secret": "A long secret key (min. 32 characters)",
  "Issuer": "https://localhost:PORT",
  "Audience": "https://localhost:PORT",
  "ExpiryMinutes": 15
},
"GoogleAuth": {
  "ClientId": "Your Google OAuth Client ID",
  "ClientSecret": "Your Google OAuth Client Secret"
},
"Stripe": {
  "SecretKey": "Your Stripe Secret Key",
  "PublishableKey": "Your Stripe Publishable Key"
},
"Smtp": {
  "Host": "smtp.office365.com",
  "Port": 587,
  "EnableSSL": true,
  "Username": "Your email address (e.g., Office365 account)",
  "Password": "Your email password or app password"
},
"SteamAPIKey": {
  "API_KEY": "Your Steam API key" /* NOT MANDATORY */
},
"YouTubeAPIKey": {
  "API_KEY": "Your YouTube Data API key" /* NOT MANDATORY */
}
```

---

## 2. 📄 `.env` – Frontend Configuration

Modify the file named `.env` in the `client` folder (React frontend) with the following values:

```env
HTTPS=true
SSL_CRT_FILE=./ssl/server.crt
SSL_KEY_FILE=./ssl/server.key
REACT_APP_GOOGLE_CLIENT_ID=Your-Google-Client-ID
```

---

## 3. 🚀 `launchSettings.json` – Backend Launch Configuration

Located in:  
`VaporWebAPI/Properties/launchSettings.json`

### Launch Profiles Included:
- `http` – runs on `http://localhost:5093`  
- `https` – runs on `https://localhost:7003` and `http://localhost:5093`  
- `IIS Express` – optional, for Visual Studio

Swagger UI launches by default. The environment is set to `Development`.

```json
// Example structure (partial):
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:5093",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:7003;http://localhost:5093",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```



