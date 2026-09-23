namespace Compellio.Bcbcti.Services.Storage.Models;

public class Page<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required bool HasMore { get; init; }
}