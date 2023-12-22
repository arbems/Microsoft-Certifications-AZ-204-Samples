using CosmosDBNoSQLSample.Common;
using Newtonsoft.Json;

namespace CosmosDBNoSQLSample.Models
{
    public class TodoItem
    {
        public int ListId { get; set; }

        public string? Title { get; set; }

        public string? Note { get; set; }

        [JsonProperty("priority")]
        public PriorityLevel Priority { get; set; }

        public DateTime? Reminder { get; set; }

        public bool Done { get; set; }

        public TodoList List { get; set; } = null!;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
