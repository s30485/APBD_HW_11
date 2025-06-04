# APBD_HW_11 Secure REST API

## Configuration

Before running the application, create an `appsettings.json` file in the project root with the following structure:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "Jwt": {
    "Issuer": "http://localhost:5300/",
    "Audience": "http://localhost:5300/",
    "SecretKey": "your-secret-key",
    "ExpiryMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "your-conn-str"
  }
}
```
