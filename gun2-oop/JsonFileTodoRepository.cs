using System.Text.Json;

public class JsonFileTodoRepository : ITodoRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonFileTodoRepository(string filePath)
    {
        _filePath = filePath;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        if (!File.Exists(_filePath))
        {
            SaveAll(new List<TodoItem>());
        }
    }

    public IReadOnlyList<TodoItem> GetAll()
    {
        if (!File.Exists(_filePath)) return new List<TodoItem>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions) ?? new List<TodoItem>();
    }

    public TodoItem? GetById(int id) => GetAll().FirstOrDefault(t => t.Id == id);

    public TodoItem Add(string title)
    {
        var items = GetAll().ToList();
        int sonId = items.Count == 0 ? 0 : items.Max(t => t.Id);

        var newItem = new TodoItem(sonId + 1, title);
        items.Add(newItem);

        SaveAll(items);
        return newItem;
    }

    public bool Complete(int id)
    {
        var items = GetAll().ToList();
        var item = items.FirstOrDefault(t => t.Id == id);
        if (item is null) return false;

        item.Complete();
        SaveAll(items);
        return true;
    }

    public bool Remove(int id)
    {
        var items = GetAll().ToList();
        var item = items.FirstOrDefault(t => t.Id == id);
        if (item is null) return false;

        items.Remove(item);
        SaveAll(items);
        return true;
    }

    private void SaveAll(List<TodoItem> items)
    {
        var json = JsonSerializer.Serialize(items, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}