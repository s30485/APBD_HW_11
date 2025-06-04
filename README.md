to run the app correctly, in appsettings you should include a connection string to a database, whether its a local db like in docker, or a remote one - it's up to you.

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "YOUR-CONNECTION-STRING"
  }
}

The reasons that I did not split my solution into many projects are:
1. The project is not complicated enough, so there is no need to split it
2. I don't reuse the DTOs anywhere else, so no need to split
3. The project is easier to read and understand, so you have less to check 😏👍