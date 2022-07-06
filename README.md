# Scavenger Hunt API
![GitHubAction workflow](https://github.com/JerishBovas/ScavengerHuntAPI/actions/workflows/master_scavengerhuntapis.yml/badge.svg)

The Scavenger Hunt API is a web application built using .NET Web API with built-in authentication and login capabilities, to be used with the Scavenger Hunt Game for iOS.

## Available Endpoints

These are the available API endpoints at the time of writing. More coming in the future.

### Auth Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| https://scavengerhuntapis.azurewebsites.net/api/auth/register [POST] | User Registration |
| https://scavengerhuntapis.azurewebsites.net/api/auth/login [POST] | User Authentication |
| https://scavengerhuntapis.azurewebsites.net/api/auth/refreshtoken [POST] | Refreshes JWT Token |
| https://scavengerhuntapis.azurewebsites.net/api/auth/revoketoken [POST] | Revokes JWT Token |
| https://scavengerhuntapis.azurewebsites.net/api/auth/resetpassword [POST] | Reset user password |
| https://scavengerhuntapis.azurewebsites.net/api/auth/changename [PUT] | Change user's name |
| https://scavengerhuntapis.azurewebsites.net/api/auth/addimage [PUT] | Adds user profile image |

### Home Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| https://scavengerhuntapis.azurewebsites.net/api/home/ [GET] | Gets user info |
| https://scavengerhuntapis.azurewebsites.net/api/home/scores/ [GET] | Gets the score of user |
| https://scavengerhuntapis.azurewebsites.net/api/home/uploadimage/ [PUT] | Uploads given image to server |

### Game Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| https://scavengerhuntapis.azurewebsites.net/api/game/ [GET] | Get the list of Games |
| https://scavengerhuntapis.azurewebsites.net/api/game/{id} [GET] | Get a Game by ID |
| https://scavengerhuntapis.azurewebsites.net/api/game/ [POST] | Create Game |
| https://scavengerhuntapis.azurewebsites.net/api/game/{id} [PUT] | Update Game |
| https://scavengerhuntapis.azurewebsites.net/api/game/{id} [DELETE] | Delete Game |
| https://scavengerhuntapis.azurewebsites.net/api/game/{id} [POST] | Create Item |
| https://scavengerhuntapis.azurewebsites.net/api/game/{id}/{itemId} [PUT] | Update Item |
| https://scavengerhuntapis.azurewebsites.net/api/game/{id}/{itemId} [DELETE] | Delete Item |

### Team Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| https://scavengerhuntapis.azurewebsites.net/api/team/ [GET] | Get the list of Teams |
| https://scavengerhuntapis.azurewebsites.net/api/team/{id} [GET] | Get a Team by ID |
| https://scavengerhuntapis.azurewebsites.net/api/team/ [POST] | Create Team |
| https://scavengerhuntapis.azurewebsites.net/api/team/{id} [PUT] | Update Team |
| https://scavengerhuntapis.azurewebsites.net/api/team/{id} [DELETE] | Delete Team |

## Local Development Setup

Before running the application locally, make sure to add `appsettings.Development.json` file. Add the contents below to the file.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    // local azure storage emulator (Azurite)
    "ScavengerHunt_Storage": "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;",
    // local cosmos database emulator
    "ScavengerHunt_Database": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
  },
  // local database ID
  "ScavengerHunt_DATABASE_ID": "ScavengerHuntAPI",
  // local jwt token configuration
  "Jwt": {
    "Key": "kGaDeAvlmP3ugz0Yvr9wwGS3rkIVUndwFmIRZn81",
    "Issuer": "https://localhost:7190/",
    "Audience": "https://localhost:7190/"
  }
}
```

## Usage

Sample Data fetch from API using Swift

```swift
func refreshToken(accessToken: String, refreshToken: String) async throws -> TokenObject{
        let body = TokenObject(accessToken: accessToken, refreshToken: refreshToken)
        
        var request = URLRequest(url: URL(string: "https://scavengerhuntapis.azurewebsites.net/api/auth/refreshtoken")!)
        request.httpMethod = "POST"
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        request.httpBody = try? JSONEncoder().encode(body)
        
        let (data, response) = try await URLSession.shared.data(for: request)
        
        print(String(data: data, encoding: .utf8) ?? "")
        print(response)
        guard let response = response as? HTTPURLResponse, response.statusCode >= 200, response.statusCode < 300  else  {
            let error = try JSONDecoder().decode(ErrorObject.self, from: data)
            print(error)
            throw NetworkError.custom(error: error.title)
        }
        
        let tokenObj = try JSONDecoder().decode(T.self, from: data)
        
        return tokenObj
    }
```
