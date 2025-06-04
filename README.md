# APBD_HW_11 Secure REST API

## Configuration

Before running the application, create an `appsettings.json` file in the project root with the following structure:

```json
{
  "Jwt": {
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "SecretKey": "your-secret-key",
    "ExpiryMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "your-secure-db-connection"
  }
}
```

**IMPORTANT:** Do not publish real connection strings or secret keys to version control.
