namespace Compellio.Bcbcti.Models.Taxii;

public abstract class Envelope<T>
{
    // TODO TAXII requires omitting empty arrays
    public required T[] Objects { get; set; }
}