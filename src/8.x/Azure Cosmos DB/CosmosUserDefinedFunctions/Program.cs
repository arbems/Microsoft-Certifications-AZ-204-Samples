using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System.Configuration;

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

    string udfId = "isOverdue";
    await RegisterAUserDefinedFunction(udfId);
    await GetItemQueryAsync();
}

async Task GetItemQueryAsync()
{
    container = cosmosClient.GetContainer(databaseId, containerId);

    var iterator = container.GetItemQueryIterator<dynamic>(
        "SELECT c.id, c.Title, udf.isOverdue(c) AS IsOverdue FROM c WHERE udf.isOverdue(c) = true");

    while (iterator.HasMoreResults)
    {
        var results = await iterator.ReadNextAsync();
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }
}

async Task RegisterAUserDefinedFunction(string udfId)
{
    container = cosmosClient.GetContainer(databaseId, containerId);

    UserDefinedFunctionResponse spResponse =
        await container.Scripts.CreateUserDefinedFunctionAsync(new UserDefinedFunctionProperties
        {
            Id = udfId,
            Body = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $@"js\{udfId}.js"))
        });

    Console.WriteLine("Created UDF in database with id: {0} Operation consumed {1} RUs.\n", spResponse.Resource.Id, spResponse.RequestCharge);
}