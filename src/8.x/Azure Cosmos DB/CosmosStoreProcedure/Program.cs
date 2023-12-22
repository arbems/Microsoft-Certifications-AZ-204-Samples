using CosmosStoreProcedure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Win32;
using System.Configuration;

string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"]!;
string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"]!;

CosmosClient cosmosClient;

Database database = null!;
Container container = null!;

string databaseId = "TodoDatabase";
string containerId = "TodoItemContainer";

await GetStartedDemoAsync();

async Task GetStartedDemoAsync()
{
    // Create a new instance of the Cosmos Client
    cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

    string storedProcedureId = "spCreateToDoItem";
    await RegisterAStoredProcedure(storedProcedureId);
    await SpCreateToDoItem(storedProcedureId);

    storedProcedureId = "spCreateToDoItems";
    await RegisterAStoredProcedure(storedProcedureId);
    await SpCreateToDoItems(storedProcedureId);

    storedProcedureId = "spDeleteToDoItems";
    await RegisterAStoredProcedure(storedProcedureId);
    await SpDeleteToDoItems(storedProcedureId, "SELECT * FROM c where CONTAINS(c.id, '10') OR CONTAINS(c.id, '11')");
}

async Task RegisterAStoredProcedure(string storedProcedureId)
{
    container = cosmosClient.GetContainer(databaseId, containerId);

    StoredProcedureResponse spResponse =
        await container.Scripts.CreateStoredProcedureAsync(
            new StoredProcedureProperties
            {
                Id = storedProcedureId,
                Body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $@"js\{storedProcedureId}.js"))
            });

    Console.WriteLine("Created Stored Procedure in database with id: {0} Operation consumed {1} RUs.\n", spResponse.Resource.Id, spResponse.RequestCharge);
}

async Task SpCreateToDoItem(string storedProcedureId)
{
    var item = new
    {
        id = "6",
        ListId = "123",
        Title = "Item 6",
        Note = "Note item 6",
        Priority = "High",
        Reminder = DateTime.Parse("2023-01-10T10:00:00"),
        Done = false,
        List = new TodoList
        {
            Id = "123",
            Title = "List 1",
            Colour = "#6666FF"
        }
    };

    container =  cosmosClient.GetContainer(databaseId, containerId);

    StoredProcedureExecuteResponse<dynamic> spExecuteResponse = 
        await container.Scripts.ExecuteStoredProcedureAsync<dynamic>(
            storedProcedureId, 
            new PartitionKey(item.ListId), 
            new[] { item });

    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", spExecuteResponse.Resource.Id, spExecuteResponse.RequestCharge);
}

async Task SpCreateToDoItems(string storedProcedureId)
{
    var items = new[] {
        new {
            id = "10",
            ListId = "123",
            Title = "Item 10",
            Note = "Note item 10",
            Priority = "High",
            Reminder = DateTime.Parse("2023-01-10T10:00:00"),
            Done = false,
            List = new TodoList
            {
                Id = "123",
                Title = "List 1",
                Colour = "#6666FF"
            }
        },
        new {
            id = "11",
            ListId = "123",
            Title = "Item 11",
            Note = "Note item 11",
            Priority = "High",
            Reminder = DateTime.Parse("2023-01-10T10:00:00"),
            Done = false,
            List = new TodoList
            {
                Id = "123",
                Title = "List 1",
                Colour = "#6666FF"
            }
        }
    };

    container = cosmosClient.GetContainer(databaseId, containerId);

    StoredProcedureExecuteResponse<dynamic> spExecuteResponse =
        await container.Scripts.ExecuteStoredProcedureAsync<dynamic>(
            storedProcedureId,
            new PartitionKey(items[0].ListId),
            new[] { items });

    Console.WriteLine("Created items in database. Operation consumed {0} RUs.\n", spExecuteResponse.RequestCharge);
}

async Task SpDeleteToDoItems(string storedProcedureId, string query)
{
    container = cosmosClient.GetContainer(databaseId, containerId);

    StoredProcedureExecuteResponse<dynamic> spExecuteResponse =
        await container.Scripts.ExecuteStoredProcedureAsync<dynamic>(
            storedProcedureId,
            new PartitionKey("123"),
            new[] { query });

    Console.WriteLine("Deleted items in database. Operation consumed {1} RUs.\n", spExecuteResponse.Resource.Id, spExecuteResponse.RequestCharge);
}