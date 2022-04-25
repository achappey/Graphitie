using Microsoft.Graph;

namespace Graphitie.Extensions;

public static class GraphExtensions
{
    public static async Task<IEnumerable<T>> PagedRequest<T>(this GraphServiceClient client, ICollectionPage<T> page, int pauseAfter = 200, int delay = 1500)
    {
        List<T> result = new List<T>();

        int count = 0;

        var pageIterator = PageIterator<T>
            .CreatePageIterator(
                client,
                page,
                (m) =>
                {
                    count++;
                    result.Add(m);

                    return count < pauseAfter;
                }
            );

        await pageIterator.IterateAsync();

        while (pageIterator.State != PagingState.Complete)
        {
            await Task.Delay(delay);
            count = 0;
            await pageIterator.ResumeAsync();
        }

        return result;

    }
}