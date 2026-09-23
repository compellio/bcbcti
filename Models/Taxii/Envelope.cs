namespace Compellio.Bcbcti.Models.Taxii;

public abstract class Envelope<T>
{
    public required T[] Objects { get; set; }
}