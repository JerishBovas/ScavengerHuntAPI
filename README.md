# Scavenger Hunt API
![GitHubAction workflow](https://github.com/JerishBovas/ScavengerHuntAPI/actions/workflows/master_scavengerhuntapitest.yml/badge.svg)

The Scavenger Hunt API is a web application built using .NET Web API with built-in authentication and login capabilities, to be used with the Scavenger Hunt Game for iOS.

## Available Endpoints

These are the available API endpoints at the time of writing. More coming in the future.

### Auth Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| /api/v1/auth/register [POST] | User Registration |
| /api/v1/auth/login [POST] | User Authentication |
| /api/v1/auth/refreshtoken [POST] | Refreshes JWT Token |
| /api/v1/auth/revoketoken* [POST] | Revokes JWT Token |
| /api/v1/auth/resetpassword* [POST] | Reset user password |

### Accounts Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| /api/v1/accounts/all* [GET] | Get all users info |
| /api/v1/accounts/* [GET] | Gets user info |
| /api/v1/accounts/scores* [GET] | Gets the scores of user |
| /api/v1/accounts/profileimage/* [PUT] | Uploads given image to server |
| /api/v1/accounts/name* [PUT] | Change user's name |
| /api/v1/accounts/{id}* [DELETE] | Delete user account |

### Home Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| /api/v1/.well-known/apple-app-site-association [GET] | Apple site association file |
| /api/v1/Home/leaderboard/ [GET] | Gets Top playes of the game |
| /api/v1/Home/populargames/ [GET] | Gets popular games in the game |

### Game Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| /api/v1/game/* [GET] | Get the list of Games |
| /api/v1/game/{id}* [GET] | Get a Game by ID |
| /api/v1/game/* [POST] | Create Game |
| /api/v1/game/{id}* [PUT] | Update Game |
| /api/v1/game/{id}* [DELETE] | Delete Game |
| /api/v1/game/{id}* [POST] | Create Item |
| /api/v1/game/{id}/items/{itemId}* [PUT] | Update Item |
| /api/v1/game/{id}/items/{itemId}* [DELETE] | Delete Item |
| /api/v1/game/image* [PUT] | Upload Image |

### Team Controller

| Endpoint                           | Description                         |
| ---------------------------------- | ----------------------------------- |
| /api/v1/team/* [GET] | Get the list of Teams |
| /api/v1/team/{id}* [GET] | Get a Team by ID |
| /api/v1/team/* [POST] | Create Team |
| /api/v1/team/{id}* [PUT] | Update Team |
| /api/v1/team/{id}* [DELETE] | Delete Team |
| /api/v1/team/image* [PUT] | Upload Image |

*Needs Authorization

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
        
        var request = URLRequest(url: URL(string: "{domain}/api/v1/refreshtoken")!)
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
