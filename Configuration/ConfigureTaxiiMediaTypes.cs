using Bcbcti.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Bcbcti.Configuration;

public class ConfigureTaxiiMediaTypes(IOptions<TaxiiOptions> taxiiOptions) : IPostConfigureOptions<MvcOptions>
{
    private const string TaxiiMediaType = "application/taxii+json";
    private const string PinnedTaxiiMediaType = "application/taxii+json;version=2.1";

    public void PostConfigure(string? name, MvcOptions options)
    {
        foreach (var formatter in options.OutputFormatters.OfType<SystemTextJsonOutputFormatter>())
        {
            formatter.SupportedMediaTypes.Add(TaxiiMediaType);
            formatter.SupportedMediaTypes.Add(PinnedTaxiiMediaType);
        }

        foreach (var formatter in options.InputFormatters.OfType<SystemTextJsonInputFormatter>())
        {
            formatter.SupportedMediaTypes.Add(TaxiiMediaType);
            formatter.SupportedMediaTypes.Add(PinnedTaxiiMediaType);
        }
    }
}