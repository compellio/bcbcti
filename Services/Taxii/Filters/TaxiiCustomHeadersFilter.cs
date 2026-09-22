using Compellio.Bcbcti.Services.Serialization.Formats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Compellio.Bcbcti.Services.Taxii.Filters;

public class TaxiiCustomHeadersFilter : IAsyncResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: ITaxiiDateAddedBounds boundedObject })
        {
            if (boundedObject.DateAddedFirst.HasValue)
            {
                context.HttpContext.Response.Headers.TryAdd("X-TAXII-Date-Added-First", boundedObject.DateAddedFirst.Value.ToTaxii());
            }
            
            if (boundedObject.DateAddedLast.HasValue)
            {
                context.HttpContext.Response.Headers.TryAdd("X-TAXII-Date-Added-Last", boundedObject.DateAddedLast.Value.ToTaxii());
            }
        }

        return next();
    }
}