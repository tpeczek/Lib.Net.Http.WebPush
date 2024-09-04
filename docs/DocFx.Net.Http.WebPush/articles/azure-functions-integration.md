# PushServiceClient bindings for Azure Functions

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) extensions for Azure Functions supports input binding.

## Packages

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) extensions for Azure Functions are provided in the [Lib.Azure.WebJobs.Extensions.WebPush](https://www.nuget.org/packages/Lib.Azure.WebJobs.Extensions.WebPush) (in-process model) and [Lib.Azure.Functions.Worker.Extensions.WebPush](https://www.nuget.org/packages/Lib.Azure.Functions.Worker.Extensions.WebPush) (isolated worker model) NuGet packages.

```
PM>  Install-Package Lib.Azure.WebJobs.Extensions.WebPush
```

```
PM>  Install-Package Lib.Azure.Functions.Worker.Extensions.WebPush
```

## Input

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) input binding uses `HttpClientFactory` to retrieve [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) instance and passes it to the input parameter of the function.

### Input - language-specific examples

#### Input - C# examples

##### Isolated Worker Model
In [the isolated worker model functions](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows), use the [`PushServiceInput`](../api/Lib.Azure.Functions.Worker.Extensions.WebPush.PushServiceInputAttribute.html) attribute.

The attribute's constructor takes the application server public key and application server private key. For information about those settings and other properties that you can configure, see [Input - configuration](#input---configuration).

###### Azure Cosmos DB trigger, subscriptions from Azure Cosmos DB
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that broadcasts a notification to all known subscriptions. The function is triggered by a change in Azure Cosmos DB collection and retrieves subscriptions also from Azure Cosmos DB.

```cs
...

namespace Demo.Azure.Functions.Worker.PushNotifications
{
    public class SendNotificationFunction
    {
        private readonly ILogger _logger;

        public SendNotificationFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendNotificationFunction>();
        }

        [Function("SendNotificationFunction")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            containerName: "Notifications",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "NotificationsLeaseCollection",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Notification> notifications,
            [CosmosDBInput(
            databaseName: "PushNotifications",
            containerName: "Subscriptions",
            Connection = "CosmosDBConnection")] CosmosClient cosmosClient,
            [PushServiceInput(
            PublicKeySetting = "ApplicationServerPublicKey",
            PrivateKeySetting = "ApplicationServerPrivateKey",
            SubjectSetting = "ApplicationServerSubject")] PushServiceClient pushServiceClient)
        {
            if (notifications != null)
            {
                Container subscriptionsContainer = cosmosClient.GetDatabase("PushNotifications").GetContainer("Subscriptions");
                using (FeedIterator<PushSubscription> subscriptionsIterator = subscriptionsContainer.GetItemQueryIterator<PushSubscription>())
                {
                    while (subscriptionsIterator.HasMoreResults)
                    {
                        foreach (PushSubscription subscription in await subscriptionsIterator.ReadNextAsync())
                        {
                            foreach (Notification notification in notifications)
                            {
                                // Fire-and-forget
                                pushServiceClient.RequestPushMessageDeliveryAsync(subscription, new PushMessage(notification.Content)
                                {
                                    Topic = notification.Topic,
                                    TimeToLive = notification.TimeToLive,
                                    Urgency = notification.Urgency
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    public class Notification
    {
        public string? Topic { get; set; }

        public string Content { get; set; } = String.Empty;

        public int? TimeToLive { get; set; }

        public PushMessageUrgency Urgency { get; set; }
    }
}
```

##### In-process Model
In [C# class libraries](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), use the [`PushService`](../api/Lib.Azure.WebJobs.Extensions.WebPush.Bindings.PushServiceAttribute.html) attribute.

The attribute's constructor takes the application server public key and application server private key. For information about those settings and other properties that you can configure, see [Input - configuration](#input---configuration).

###### Azure Cosmos DB trigger, subscriptions from Azure Cosmos DB
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that broadcasts a notification to all known subscriptions. The function is triggered by a change in Azure Cosmos DB collection and retrieves subscriptions also from Azure Cosmos DB.

```cs
...

namespace Demo.Azure.Funtions.PushNotifications
{
    public class Notification
    {
        public string Topic { get; set; }

        public string Content {  get; set; }

        public int? TimeToLive { get; set; }

        public PushMessageUrgency Urgency { get; set; }
    }

    public static class SendNotificationFunction
    {
        [FunctionName("SendNotification")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            containerName: "Notifications",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "NotificationsLeaseCollection",
            CreateLeaseContainerIfNotExists = true)]IReadOnlyList<Notification> notifications,
            [CosmosDB(
            databaseName: "PushNotifications",
            containerName: "Subscriptions",
            Connection = "CosmosDBConnection")]CosmosClient cosmosClient,
            [PushService(
            PublicKeySetting = "ApplicationServerPublicKey",
            PrivateKeySetting = "ApplicationServerPrivateKey",
            SubjectSetting = "ApplicationServerSubject")]PushServiceClient pushServiceClient)
        {
            if (notifications != null)
            {
                Container subscriptionsContainer = cosmosClient.GetDatabase("PushNotifications").GetContainer("Subscriptions");
                using (FeedIterator<PushSubscription> subscriptionsIterator = subscriptionsContainer.GetItemQueryIterator<PushSubscription>())
                {
                    while (subscriptionsIterator.HasMoreResults)
                    {
                        foreach (PushSubscription subscription in await subscriptionsIterator.ReadNextAsync())
                        {
                            foreach (Notification notification in notifications)
                            {
                                // Fire-and-forget
                                pushServiceClient.RequestPushMessageDeliveryAsync(subscription, new PushMessage(notification.Content)
                                {
                                    Topic = notification.Topic,
                                    TimeToLive = notification.TimeToLive,
                                    Urgency = notification.Urgency
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
```

### Input - configuration

The following table explains the binding configuration properties that you set in the *function.json* file and the [`PushService`](../api/Lib.Azure.WebJobs.Extensions.WebPush.Bindings.PushServiceAttribute.html) attribute.

|function.json property | Attribute property |Description|
|---------|---------|----------------------|
|**type** || Must be set to `pushService`. |
|**direction** || Must be set to `in`. |
|**publicKeySetting**|**PublicKeySetting** | The name of an app setting that contains the application server public key. |
|**privateKeySetting**|**PrivateKeySetting** | The name of an app setting that contains the application server private key. |
|**subjectSetting**|**SubjectSetting** | The name of an app setting that contains the contact information for the application server. |
