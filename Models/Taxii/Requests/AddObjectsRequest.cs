using System.ComponentModel.DataAnnotations;
using Compellio.Bcbcti.Options;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Models.Taxii.Requests;

public class AddObjectsRequest : Envelope<StixObjectResource>, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        // TODO-FIXME using services outside Services.Taxii scope which defines TaxiiOptions
        var max = context.GetRequiredService<IOptions<TaxiiOptions>>().Value.MaxUploadCount;
        
        if (Objects.Length < 1)
        {
            yield return new ValidationResult("You must provide at least one object.", [nameof(Objects)]);
        }
        else if (Objects.Length > max)
        {
            yield return new ValidationResult($"At most {max} objects are allowed.", [nameof(Objects)]);
        }
    }
}