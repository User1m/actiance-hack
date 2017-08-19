# Actiance Hack

The purpose of this hack is to enable Actiance to leverage and understand Microsoft Graph Teams related APIs.

## Getting Started

```
Open Actiance.sln in Visual Studio
Run Clarence project
Connect to Bot via BotFramework Emulator or Side-loaded Teams bot
```

## Code Walk-through

*Helpers/Storage.cs* - in memory data store

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

```cs
public static async Task Monitor()
{
    System.Timers.Timer t = new System.Timers.Timer();
    t.AutoReset = false;
    t.Interval = 10 * 1000;

    try
    {
        if (members == null)
        {
            members = await MessagesController.GetConverationMembers();
        }
```


* For each member, make a delta query for their messages, store them, and ingest those messages
* Store delta queries for future calls (done in *APIService.GetInitialMessageDeltasForUser()*)

```cs
        Console.WriteLine("-------------\n STARTED: INGESTING USER MESSAGES \n-------------");

        foreach (var member in members)
        {
            List<Message> userMessages = await APIService.GetInitialMessageDeltasForUser(member.ObjectId);
            Storage.userMessages.Add(member.ObjectId, userMessages);
            await IngestMessagesForUser(member.ObjectId);
        }

        Console.WriteLine("-------------\n DONE: INGESTING USER MESSAGES \n-------------");
```

* Start timer (every 10secs) to call delta query for new messages posted to user (done in *PollForUserMessages()*). *Timer is running in the background as well*

```cs
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
    }
    catch (Exception e)
    {
        Console.WriteLine($"ERROR IN BACKGROUND THREAD-0: {e.Message}");
        Console.WriteLine($"ERROR IN BACKGROUND THREAD-0: {e.StackTrace}");
    }
}
```

* For each message of a member, use SimpleDLP Helper to check if message content contains DLP content
* Send Compliance Message if DLP content is found

```cs
public async static Task<bool> IngestMessagesForUser(string userId)
{
  var tasks = new List<Task>();

  foreach (Message entry in Storage.userMessages[userId])
  {
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
                  //Send Compliance Message if DLP content found
                  tasks.Add(SendComplianceMsgAsync(member, entry.BodyPreview, entry.Sender.EmailAddress.Address, recipientsEmails));
                  break;
              }
          }
      }
 ...
```

* Call stored delta query for new messages (done in *APIService.GetUpdatedMessageFromDeltaLink()*)
* Ingest those messages and check for DLP content (done in *IngestMessagesForUser()*)

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


