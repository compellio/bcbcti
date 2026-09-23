namespace Compellio.Bcbcti.Services.Taxii.Filters;

// TODO-REVIEW placement
public interface ITaxiiDateAddedBounds
{
    public DateTime? DateAddedFirst { get; }
    public DateTime? DateAddedLast { get; }
}