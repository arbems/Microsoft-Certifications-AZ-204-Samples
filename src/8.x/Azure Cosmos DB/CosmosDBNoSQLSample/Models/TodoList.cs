using System.Drawing;
using CosmosDBNoSQLSample.Common;
using Newtonsoft.Json;

namespace CosmosDBNoSQLSample.Models
{
    public class TodoList
    {
        public string? Title { get; set; }
        public Colour Colour { get; set; } = Colour.White;

        public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
