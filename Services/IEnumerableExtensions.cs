namespace Compellio.Bcbcti.Services;

// see https://stackoverflow.com/a/75476487
// TODO REVIEW
internal static class IEnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : struct
    {
        return enumerable.Where(item => item.HasValue).Select(item => item!.Value);
    }
    
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
        return enumerable.Where(item => item is not null).Select(item => item!);
    }
}