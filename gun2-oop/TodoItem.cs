using System.Text.Json.Serialization;

public class TodoItem
{
    public int Id { get; }
    public string Title { get; private set; }

    [JsonInclude]
    public bool IsDone { get; private set; }

    public DateTime CreatedAt { get; }

    public TodoItem(int id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Başlık boş olamaz.", nameof(title));

        Id = id;
        Title = title.Trim();
        CreatedAt = DateTime.Now;
    }

    public void Complete() => IsDone = true;
}