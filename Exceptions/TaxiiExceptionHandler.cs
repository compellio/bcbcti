using Bcbcti.Exceptions.Taxii;
using Bcbcti.Models.Taxii;
using Microsoft.AspNetCore.Diagnostics;

namespace Bcbcti.Exceptions;

// TODO FIXME does this applies to system exceptions as well (e.g. 415, 406, etc.)?

public class TaxiiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        
        // TODO FIXME - mapping feels hacky
        var (status, title) = exception switch
        {
            CollectionNotFoundException => (StatusCodes.Status404NotFound, "Collection not found"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Error")
        };

        var error = new ErrorResource
        {
            Title = title,
            Description = exception is TaxiiException ? exception.Message : null,
            HttpStatus = status.ToString()
        };

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/taxii+json;version=2.1";
        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);

        return true;
    }
}