# PushServiceClient bindings for Azure Functions

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) bindings for Azure Functions supports input binding.

## Packages

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) bindings for Azure Functions are provided in the [Lib.Azure.WebJobs.Extensions.WebPush](https://www.nuget.org/packages/Lib.Azure.WebJobs.Extensions.WebPush) NuGet package.

```
PM>  Install-Package Lib.Azure.WebJobs.Extensions.WebPush
```

## Input

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) input binding uses `HttpClientFactory` to retrieve [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) instance and passes it to the input parameter of the function.

### Input - language-specific examples

#### Input - C# examples
In [C# class libraries](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), use the [`PushService`](../api/Lib.Azure.WebJobs.Extensions.WebPush.Bindings.PushServiceAttribute.html) attribute.

The attribute's constructor takes the application server public key and application server private key. For information about those settings and other properties that you can configure, see [Input - configuration](#input---configuration).

This section contains the following examples:
- [Azure Cosmos DB trigger, subscriptions from Azure Cosmos DB](#azure-cosmos-db-trigger-subscriptions-from-azure-cosmos-db)

##### Azure Cosmos DB trigger, subscriptions from Azure Cosmos DB
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that broadcasts a notification to all known subscriptions. The function is triggered by a change in Azure Cosmos DB collection and retrieves subscriptions also from Azure Cosmos DB.

```cs
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using Lib.Net.Http.WebPush;
using Lib.Azure.WebJobs.Extensions.WebPush.Bindings;

namespace Demo.Azure.Funtions.PushNotifications
{
    public static class SendNotificationFunction
    {
        private static readonly Uri _subscriptionsCollectionUri = UriFactory.CreateDocumentCollectionUri("PushNotifications", "SubscriptionsCollection");

        [FunctionName("SendNotification")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            collectionName: "NotificationsCollection",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "NotificationsLeaseCollection",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<PushMessage> notifications,
            [CosmosDB(
            databaseName: "PushNotifications",
            collectionName: "SubscriptionsCollection",
            ConnectionStringSetting = "CosmosDBConnection")]DocumentClient cosmosDbClient,
            [PushService(
            PublicKeySetting = "ApplicationServerPublicKey",
            PrivateKeySetting = "ApplicationServerPrivateKey",
            SubjectSetting = "ApplicationServerSubject")]PushServiceClient pushServiceClient)
        {
            if (notifications != null)
            {
                IDocumentQuery<PushSubscription> subscriptionQuery = cosmosDbClient.CreateDocumentQuery<PushSubscription>(_subscriptionsCollectionUri, new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = -1
                }).AsDocumentQuery();

                while (subscriptionQuery.HasMoreResults)
                {
                    foreach (PushSubscription subscription in await subscriptionQuery.ExecuteNextAsync())
                    {
                        foreach (PushMessage notification in notifications)
                        {
                            // Fire-and-forget
                            pushServiceClient.RequestPushMessageDeliveryAsync(subscription, notification);
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
