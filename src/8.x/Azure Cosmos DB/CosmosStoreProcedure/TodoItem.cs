using Newtonsoft.Json;

namespace CosmosStoreProcedure;

public class TodoItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    public string ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public string Priority { get; set; } = string.Empty;

    public DateTime? Reminder { get; set; }

    public bool Done { get; set; }

    public TodoList List { get; set; } = null!;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class TodoList
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string Colour { get; set; } = "#FFFFFF";

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}