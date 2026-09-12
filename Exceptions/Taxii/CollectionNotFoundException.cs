using System.Diagnostics.CodeAnalysis;

namespace Bcbcti.Exceptions.Taxii;

public class CollectionNotFoundException(string id) : TaxiiException($"Collection '{id}' not found")
{

    /// <remark>
    /// Replicates ArgumentNullException.ThrowIfNull method
    /// </remark>
    public static void ThrowIfNull([NotNull] object? argument, string id)
    {
        if (argument is null)
        {
            throw new CollectionNotFoundException(id);
        }
    }

};