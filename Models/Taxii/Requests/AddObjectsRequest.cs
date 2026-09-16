using System.ComponentModel.DataAnnotations;
using BCBCTI.Models.Taxii;
using Bcbcti.Options;
using Microsoft.Extensions.Options;

namespace Bcbcti.Models.Taxii.Requests;

// TODO REVIEW This should perhaps be a EnvelopeResource but things get complicated with EnvelopeResource because it defines objects
//    as abstract StixObject instead of StixObjectResource => JSON deserialization complexity 
public class AddObjectsRequest : IValidatableObject
{
    // TODO need custom JsonConverter to properly cast objects into the right Models.Stix resources
    public required StixObjectResource[] Objects { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
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