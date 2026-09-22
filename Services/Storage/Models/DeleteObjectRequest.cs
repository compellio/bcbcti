namespace Compellio.Bcbcti.Services.Storage.Models;

public class DeleteObjectRequest
{
    public required string ObjectKey { get; set; }
    public Condition Condition { get; set; } = Condition.None;
}