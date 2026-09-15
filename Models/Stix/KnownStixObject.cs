namespace Bcbcti.Models.Stix;

public class KnownStixObject : StixObject
{
    public KnownStixObject(string type, Guid id) : base($"{type}--{id}")
    {
        Type = type;
    }

    public override string Type { get; }
}