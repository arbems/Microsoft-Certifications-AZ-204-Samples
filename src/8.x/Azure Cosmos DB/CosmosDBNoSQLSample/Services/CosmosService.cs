using CosmosDBNoSQLSample.Common;
using CosmosDBNoSQLSample.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDBNoSQLSample.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _client;

        public CosmosService()
        {
            _client = new CosmosClient(
                connectionString: "AccountEndpoint=https://cosmosdbdemo00001.documents.azure.com:443/;AccountKey=kMsClf75smovoDN0rp17fMQpcK71QiP3ngBmL6JtwiFJm0LPv267Z1ots2Iln0W5UWDoCCNeRVllACDb40tUjA==;"
            );
        }
        private Database database => _client.GetDatabase("TodoDb");

        public async Task<IEnumerable<TodoItem>> GetTodoItemsLinqAsync()
        {
            var container = database.GetContainer("TodoItem");

            var queryable = container.GetItemLinqQueryable<TodoItem>();

            using FeedIterator<TodoItem> feed = queryable
                .Where(p => p.Priority == PriorityLevel.High)
                .OrderByDescending(p => p.Title)
                .ToFeedIterator();

            List<TodoItem> results = [];

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<IEnumerable<TodoItem>> GetTodoItemsQueryAsync()
        {
            string sql = """
                        SELECT
                            t.listId,
                            t.id,
                            t.title,
                            t.note,
                            t.priority,
                            t.reminder
                        FROM TodoList p
                        JOIN t IN p.items
                        WHERE t.priority = @priorityFilter
                        """;

            var container = database.GetContainer("TodoList");

            var query = new QueryDefinition(query: sql)
                .WithParameter("@priorityFilter", PriorityLevel.High);

            using FeedIterator<TodoItem> feed = 
                container.GetItemQueryIterator<TodoItem>(queryDefinition: query);

            List<TodoItem> results = [];

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                results.AddRange(response); 
            }

            return results;
        }



    }
}