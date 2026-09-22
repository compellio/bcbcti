namespace Compellio.Bcbcti.Extensions;

internal static class ListExtensions
{
    extension<TSource>(IReadOnlyList<TSource> items)
    {
        public Task<IReadOnlyList<TDest>> ParallelSelectAsync<TDest>(
            Func<TSource, CancellationToken, Task<TDest>> selector, CancellationToken ct = default) =>
            items.ParallelSelectAsync(new ParallelOptions { CancellationToken = ct }, selector);

        public Task<IReadOnlyList<TDest>> ParallelSelectAsync<TDest>(int maxDegreeOfParallelism,
            Func<TSource, CancellationToken, Task<TDest>> selector, CancellationToken ct = default) =>
            items.ParallelSelectAsync(
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism), CancellationToken = ct
                }, selector);

        private async Task<IReadOnlyList<TDest>> ParallelSelectAsync<TDest>(ParallelOptions options,
            Func<TSource, CancellationToken, Task<TDest>> selector)
        {
            var results = new TDest[items.Count];

            await Parallel.ForEachAsync(Enumerable.Range(0, items.Count), options,
                async (i, ctoken) => { results[i] = await selector(items[i], ctoken); });

            return results;
        }
    }
}