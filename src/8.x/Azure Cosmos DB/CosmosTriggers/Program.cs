using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System.Configuration;
using System.Net;

string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"]!;
string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"]!;

CosmosClient cosmosClient;

Container container = null!;

string databaseId = "TodoDatabase";
string containerId = "TodoItemContainer";

await GetStartedDemoAsync();

async Task GetStartedDemoAsync()
{
    // Create a new instance of the Cosmos Client
    cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

    // Pre-triggers
    string triggerId = "validateToDoItemTimestamp";
    //await RegisterATrigger(triggerId);

    // Post-triggers
    triggerId = "updateMetadata";
    //await RegisterATrigger(triggerId);

    await AddItemsToContainerAsync();
}

async Task RegisterATrigger(string triggerId)
{
    container = cosmosClient.GetContainer(databaseId, containerId);

    TriggerResponse spResponse =
        await container.Scripts.CreateTriggerAsync(
            new TriggerProperties
            {
                Id = triggerId,
                Body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $@"js\{triggerId}.js")),
                TriggerType = TriggerType.Pre,
                TriggerOperation = TriggerOperation.Create
            });

    Console.WriteLine("Created Trigger in database with id: {0} Operation consumed {1} RUs.\n", spResponse.Resource.Id, spResponse.RequestCharge);
}

async Task AddItemsToContainerAsync()
{
    var item = new
    {
        id = "20",
        ListId = "123",
        Title = "Item 20",
        Note = "Note item 20",
        Priority = "High",
        Reminder = DateTime.Parse("2023-01-10T10:00:00"),
        Done = false,
        List = new
        {
            Id = "123",
            Title = "List 1",
            Colour = "#6666FF"
        }
    };

    try
    {
        container = cosmosClient.GetContainer(databaseId, containerId);

        ItemResponse<dynamic> itemResponse = await container.ReadItemAsync<dynamic>(item.id, new PartitionKey(item.ListId));
        Console.WriteLine("Item in database with id: {0} already exists\n", itemResponse.Resource.Id);
    }
    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        var requestOptions = new ItemRequestOptions
        {
            PreTriggers = new List<string> { "validateToDoItemTimestamp" }
        };

        ItemResponse<dynamic> itemResponse = await container.CreateItemAsync<dynamic>(item, new PartitionKey(item.ListId), requestOptions);
        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemResponse.Resource.Id, itemResponse.RequestCharge);
    }
}