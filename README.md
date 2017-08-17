# Actiance Hack

The purpose of this hack is to enable Actiance to leverage Microsoft Graph APIs through the use of a generalized chat bot.

## Getting Started

```
Open Actiance.sln in Visual Studio
```

## Web Requests to Execute

Get an access token

```bash
curl -X POST \
  https://login.microsoftonline.com/{{tenant_id}}/oauth2/v2.0/token \
  -H 'content-type: application/x-www-form-urlencoded' \
  -d 'client_id={{client_id}}&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default&client_secret={{client_secret}}&grant_type=client_credentials'
```

Create a subscription
```bash
curl -X POST \
  https://graph.microsoft.com/v1.0/subscriptions \
  -H 'authorization: Bearer {{access_token}}' \
  -H 'content-type: application/json' \
  -d '{
  "changeType": "created,updated",
  "notificationUrl": "{{ngrok_hostname}}/api/graph/subscribe",
  "resource": "/users/migolfi@actiancehack.onmicrosoft.com/messages",
  "expirationDateTime": "2017-08-16T11:00:00.0000000Z",
  "clientState": "SecretClientState"
}'
```

## Built With

* [BotFramework](https://dev.botframework.com/) - The bot framework used
* C#
* .Net Core


## Authors

* **Claudius Mbemba** - [User1m](https://github.com/User1m)
* **Michael Golfi** - [Michael-Golfi](https://github.com/Michael-Golfi)


