# Actiance Hack

The purpose of this hack is to enable Actiance to leverage Microsoft Graph APIs through the use of a generalized chat bot.

## Getting Started

```
Open Actiance.sln in Visual Studio
```

## Code Walk-through


### Dialogs/MainDialog.cs
* Call MS Graph to get the profile of the user and their manager
* Start the monitoring service on a background thread to not throttle UI process

```cs
if (Storage.user == null)
{
    //call API services
    Storage.user = await APIService.GetUserProfile(contextName);
    Storage.manager = await APIService.GetUserManager(Storage.user.Id);

    ///start monitoring service on background thread
    //await MontiorService.IngestMessagesForUser(Storage.user.Id);
    Thread thread = new Thread(async () =>
    {
        Thread.CurrentThread.IsBackground = true;
        await MonitorService.Monitor();
    });
    thread.Start();
}
```

### Services/MonitorService.cs
* Get all members of the Teams conversation

*public static async Task Monitor()*
```cs
if (members == null)
{
    members = await MessagesController.GetConverationMembers();
}
```

* For each member, make a delta query for thier messages, store them, and ingest those messages
```cs
foreach (var member in members)
{
    List<Message> userMessages = await APIService.GetInitialMessageDeltasForUser(member.ObjectId);
    Storage.userMessages.Add(member.ObjectId, userMessages);
    await IngestMessagesForUser(member.ObjectId);
}
```

* For each message of a member, use SimpleDLP Helper to check if message content contains DLP content
* Send Compliance Message if DLP content found

```cs
public async static Task<bool> IngestMessagesForUser(string userId)
{
  var tasks = new List<Task>();

  foreach (Message entry in Storage.userMessages[userId])
  {
      //if (entry.BodyPreview.Contains(Resources.ResourceManager.GetString("DLPPhrase")))
      if (SimpleDLP.containsRestrictedPhrases(entry.BodyPreview))
      {
          if (members == null)
          {
              members = await MessagesController.GetConverationMembers();
          }
          foreach (var member in members)
          {
              if (member.ObjectId == userId && entry.Sender.EmailAddress.Name != "Clarence")
              {
                  Console.WriteLine("-------------\nFOUND ISSUE\n-------------");
                  string recipientsEmails = string.Empty;
                  foreach (var rep in entry.ToRecipients)
                  {
                      recipientsEmails += $"{rep.EmailAddress.Address},";
                  }
                  tasks.Add(SendComplianceMsgAsync(member, entry.BodyPreview, entry.Sender.EmailAddress.Address, recipientsEmails));
                  break;
              }
          }
      }
 ...
```

* Start timer to call delta query for new messages posted to user every 10secs. *Timer is running in the backgroun as well*

*public static async Task Monitor()*
```cs
System.Timers.Timer t = new System.Timers.Timer();
t.AutoReset = false;
t.Interval = 10 * 1000;
...
t.Elapsed += async delegate
{
    try
    {
        await PollForUserMessages();
    }
    catch (Exception e)
    {
        Console.WriteLine($"ERROR IN TIMER THREAD: {e.Message}");
        Console.WriteLine($"ERROR IN TIMER THREAD: {e.StackTrace}");
    }
    finally
    {
        t.Start();
    }
};
t.Start();
```

* Call stored Delta query for new messages
* Ingest those messages and check for DLP content

```cs
public static async Task<bool> PollForUserMessages()
{
    Console.WriteLine("-------------\nTIMER: Polling Again \n-------------");

    try
    {
        if (Storage.deltaStore.Count > 0)
        {
            await APIService.GetUpdatedMessageFromDeltaLink();
            foreach (var member in members)
            {
                await IngestMessagesForUser(member.ObjectId);
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"ERROR IN BACKGROUND THREAD-1: {e.Message}");
        Console.WriteLine($"ERROR IN BACKGROUND THREAD-1: {e.StackTrace}");
    }
    return true;
  }
}
```

## Sample MS Graph Web Requests to Execute:

Get an access token
```bash
curl -X POST \
  https://login.microsoftonline.com/{{tenant_id}}/oauth2/v2.0/token \
  -H 'content-type: application/x-www-form-urlencoded' \
  -d 'client_id={{client_id}}&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default&client_secret={{client_secret}}&grant_type=client_credentials'
```

Create a subscription (webhooks)
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
* [MS Graph .Net SDK](https://www.nuget.org/packages/Microsoft.Graph/)
* [MS Teams .Net SDK](https://www.nuget.org/packages/Microsoft.Bot.Connector.Teams)

## Authors

* **Claudius Mbemba** - [User1m](https://github.com/User1m)
* **Michael Golfi** - [Michael-Golfi](https://github.com/Michael-Golfi)


