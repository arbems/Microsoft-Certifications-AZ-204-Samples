using CosmosGettingStarted;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Configuration;
using System.Net;

// The Azure Cosmos DB endpoint for running this sample.
string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

// The primary key for the Azure Cosmos account.
string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

// The Cosmos client instance
CosmosClient cosmosClient;

// The database we will create
Database database = null!;

// The container we will create.
Container container = null!;

// The name of the database and container we will create
string databaseId = "TodoDatabase";
string containerId = "TodoItemContainer";

await GetStartedDemoAsync();

async Task GetStartedDemoAsync()
{
    // Create a new instance of the Cosmos Client
    cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
    await CreateDatabaseAsync();
    await CreateContainerAsync();
    await AddItemsToContainerAsync();
    await QueryItemsAsync();
    await LinqItemsAsync();
    await ReplaceTodoItemAsync();
    await TransactionalBatchAsync();
    await DeleteTodoItemAsync();
    //await DeleteDatabaseAndCleanupAsync();
}

async Task CreateDatabaseAsync()
{
    // Create a new database
    database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
    Console.WriteLine("Created Database: {0}\n", database.Id);
}

async Task CreateContainerAsync()
{
    // Create a new container
    container = await database.CreateContainerIfNotExistsAsync(containerId, "/ListId");
    Console.WriteLine("Created Container: {0}\n", container.Id);
}

async Task AddItemsToContainerAsync()
{
    // Create a item object for the Item1
    TodoItem item1 = new()
    {
        Id = "1",
        ListId = "123",
        Title = "Item 1",
        Note = "Note item 1",
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

    try
    {
        // Read the item to see if it exists.  
        ItemResponse<TodoItem> itemResponse = await container.ReadItemAsync<TodoItem>(item1.Id, new PartitionKey(item1.ListId));
        Console.WriteLine("Item in database with id: {0} already exists\n", itemResponse.Resource.Id);
    }
    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        // Create an item in the container. Note we provide the value of the partition key for this item, which is "Andersen"
        ItemResponse<TodoItem> itemResponse = await container.CreateItemAsync<TodoItem>(item1, new PartitionKey(item1.ListId));

        // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemResponse.Resource.Id, itemResponse.RequestCharge);
    }

    // Create a item object for the Item2
    TodoItem item2 = new()
    {
        Id = "2",
        ListId = "123",
        Title = "Item 2",
        Note = "Note item 2",
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

    try
    {
        ItemResponse<TodoItem> itemResponse = await container.ReadItemAsync<TodoItem>(item2.Id, new PartitionKey(item2.ListId));
        Console.WriteLine("Item in database with id: {0} already exists\n", itemResponse.Resource.Id);
    }
    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        ItemResponse<TodoItem> itemResponse = await container.CreateItemAsync<TodoItem>(item2, new PartitionKey(item2.ListId));

        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemResponse.Resource.Id, itemResponse.RequestCharge);
    }

    // Create a item object for the Item3
    TodoItem item3 = new()
    {
        Id = "3",
        ListId = "123",
        Title = "Item 3",
        Note = "Note item 3",
        Priority = "Low",
        Reminder = DateTime.Parse("2023-01-10T10:00:00"),
        Done = false,
        List = new TodoList
        {
            Id = "123",
            Title = "List 1",
            Colour = "#6666FF"
        }
    };

    try
    {
        ItemResponse<TodoItem> itemResponse = await container.ReadItemAsync<TodoItem>(item3.Id, new PartitionKey(item3.ListId));
        Console.WriteLine("Item in database with id: {0} already exists\n", itemResponse.Resource.Id);
    }
    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        ItemResponse<TodoItem> itemResponse = await container.CreateItemAsync<TodoItem>(item3, new PartitionKey(item3.ListId));

        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemResponse.Resource.Id, itemResponse.RequestCharge);
    }
}

async Task QueryItemsAsync()
{
    var sqlQueryText = "SELECT * FROM c WHERE c.ListId = '123'";

    Console.WriteLine("Running query: {0}\n", sqlQueryText);

    QueryDefinition queryDefinition = new(sqlQueryText);
    using FeedIterator<TodoItem> queryResultSetIterator = container.GetItemQueryIterator<TodoItem>(queryDefinition);

    List<TodoItem> items = [];

    while (queryResultSetIterator.HasMoreResults)
    {
        FeedResponse<TodoItem> currentResultSet = await queryResultSetIterator.ReadNextAsync();
        Console.WriteLine($"Request Charge: {currentResultSet.RequestCharge} RUs");

        foreach (TodoItem item in currentResultSet)
        {
            items.Add(item);
            Console.WriteLine("\tRead {0}\n", item);
        }
    }
}

async Task LinqItemsAsync()
{
    Console.WriteLine("Running query but using LINQ.\n");

    var queryable = container.GetItemLinqQueryable<TodoItem>();

    using FeedIterator<TodoItem> feed = queryable
        .Where(p => p.ListId == "123")
        .OrderByDescending(p => p.Title)
        .ToFeedIterator();

    List<TodoItem> items = [];

    while (feed.HasMoreResults)
    {
        var currentResultSet = await feed.ReadNextAsync();
        Console.WriteLine($"Request Charge: {currentResultSet.RequestCharge} RUs");

        foreach (TodoItem item in currentResultSet)
        {
            items.Add(item);
            Console.WriteLine("\tRead {0}\n", item);
        }
    }
}

async Task ReplaceTodoItemAsync()
{
    ItemResponse<TodoItem> itemResponse = await container.ReadItemAsync<TodoItem>("1", new PartitionKey("123"));
    var itemBody = itemResponse.Resource;

    // update done from false to true
    itemBody.Done = true;
    // update colour of list
    itemBody.List.Colour = "#111111";

    // replace the item with the updated content
    itemResponse = await container.ReplaceItemAsync<TodoItem>(itemBody, itemBody.Id, new PartitionKey(itemBody.ListId));
    Console.WriteLine("Updated item [{0},{1}].\n \tBody is now: {2}\n", itemBody.ListId, itemBody.Id, itemResponse.Resource);
}

async Task TransactionalBatchAsync()
{
    TodoItem item4 = new()
    {
        Id = "4",
        ListId = "123",
        Title = "Item 4",
        Note = "Note item 4",
        Priority = "Low",
        Reminder = DateTime.Parse("2023-01-10T10:00:00"),
        Done = false,
        List = new TodoList
        {
            Id = "123",
            Title = "List 1",
            Colour = "#6666FF"
        }
    };
    TodoItem item5 = new()
    {
        Id = "5",
        ListId = "123",
        Title = "Item 5",
        Note = "Note item 5",
        Priority = "Low",
        Reminder = DateTime.Parse("2023-01-10T10:00:00"),
        Done = false,
        List = new TodoList
        {
            Id = "123",
            Title = "List 1",
            Colour = "#6666FF"
        }
    };

    var batch = container.CreateTransactionalBatch(new PartitionKey("123"))
    .ReadItem("1")
    .CreateItem(item4)
    .CreateItem(item5);

    using var response = await batch.ExecuteAsync();

    Console.WriteLine($"[{response.StatusCode}]\t{response.RequestCharge} RUs");
}

async Task DeleteTodoItemAsync()
{
    var partitionKeyValue = "123";
    var itemId = "3";

    // Delete an item. Note we must provide the partition key value and id of the item to delete
    ItemResponse<TodoItem> ItemResponse = await container.DeleteItemAsync<TodoItem>(itemId, new PartitionKey(partitionKeyValue));
    Console.WriteLine("Deleted Item [{0},{1}]\n", partitionKeyValue, itemId);
}

async Task DeleteDatabaseAndCleanupAsync()
{
    DatabaseResponse databaseResourceResponse = await database.DeleteAsync();

    Console.WriteLine("Deleted Database: {0}\n", databaseId);

    //Dispose of CosmosClient
    cosmosClient.Dispose();
}