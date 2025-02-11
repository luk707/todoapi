namespace TodoApi.Models;

public class Filter<T>
{
    public Dictionary<string, Dictionary<string, object>> Conditions { get; set; } = new();
}
