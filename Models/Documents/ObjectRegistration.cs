namespace Bcbcti.Models.Documents;

public enum RegistrationState
{
    Unregistered, // Creating failed
    Creating, // awaiting callback for 1st registration
    Updating, // awaiting callback for update registration
    Registered, // ok
    Deleted
    
    // TODO case where update attempted but failed?
}

public class ObjectRegistration // this is more of an ObjectState rather than RegistrationState -> the receipt is the registration state
{
    public required string ObjectId { get; set; }
    public required string CollectionId { get; set; }
    
    public required RegistrationState State { get; set; }
    public string? TarId { get; set; }

    public required DateTime CurrentObjectVersion { get; set; } // TODO calculate from History?
    public required int CurrentTarVersion { get; set; } // TODO calculate from History?
    
    public required DateTime CreatedAt { get; set; } // first registration - TODO calculate from History?
    public DateTime? UpdatedAt { get; set; } // last update registration - TODO calculate from History?
    
    public required ObjectRegistrationVersion[] History { get; set; }
}