using CosmosDBNoSQLSample.Models;

namespace CosmosDBNoSQLSample.Services
{
    public interface ICosmosService
    {
        Task<IEnumerable<TodoItem>> GetTodoItemsLinqAsync();
        Task<IEnumerable<TodoItem>> GetTodoItemsQueryAsync();
    }
}
