namespace Compellio.Bcbcti.Models.Stix;

public class StixBundle(Guid id) : KnownStixObject("bundle", id)
{
    public required StixObject[] Objects { get; set; }
}