using CosmosDBNoSQLSample.Models;
using CosmosDBNoSQLSample.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDBNoSQLSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController(ICosmosService cosmosService) : ControllerBase
    {
        private readonly ICosmosService _cosmosService = cosmosService;

        [HttpGet("GetTodoItemsLinq", Name = "GetTodoItemsLinq")]
        public async Task<IEnumerable<TodoItem>> GetTodoItemsLinq()
        {
            return await _cosmosService.GetTodoItemsLinqAsync();
        }

        [HttpGet("GetTodoItemsQuery", Name = "GetTodoItemsQuery")]
        public async Task<IEnumerable<TodoItem>> GetTodoItemsQuery()
        {
            return await _cosmosService.GetTodoItemsQueryAsync();
        }

    }
}
