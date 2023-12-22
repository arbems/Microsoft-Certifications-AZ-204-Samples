Database: TodoDb

Container: TodoItem
PartitionKey: ListId
Data: 
```
{
    "id": "1",
    "listId": 123,
    "title": "Circle",
    "note": "Note circle",
    "priority": 3,
    "reminder": "2023-01-10T10:00:00",
    "done": false,
    "list": {
        "id": "123",
        "title": "Shape",
        "colour": "#6666FF"
    }
}
{
    "id": "2",
    "listId": 123,
    "title": "Square",
    "note": "Note square",
    "priority": 3,
    "reminder": "2023-01-10T10:00:00",
    "done": false,
    "list": {
        "id": "123",
        "title": "Shape",
        "colour": "#6666FF"
    }
}
{
    "id": "3",
    "listId": 123,
    "title": "Rectangle",
    "note": "Note rectangle",
    "priority": 1,
    "reminder": "2023-01-10T10:00:00",
    "done": false,
    "list": {
        "id": "123",
        "title": "Shape",
        "colour": "#6666FF"
    }
}
```

Container: TodoList
PartitionKey: Id
Data: 

```
{
    "id": "123",
    "title": "Shape",
    "colour": "#6666FF",
    "items": [
        {
            "id": "1",
            "listId": "123",
            "title": "Circle",
            "note": "Note circle",
            "priority": 3,
            "reminder": "2023-01-10T10:00:00",
            "done": false
        },
        {
            "id": "3",
            "listId": "123",
            "title": "Square",
            "note": "Note square",
            "priority": 3,
            "reminder": "2023-01-10T10:00:00",
            "done": false
        },
        {
            "id": "2",
            "listId": "123",
            "title": "Rectangle",
            "note": "Note rectangle",
            "priority": 1,
            "reminder": "2023-01-10T10:00:00",
            "done": false
        }
    ]
}
```