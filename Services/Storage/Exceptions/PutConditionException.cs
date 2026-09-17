namespace Bcbcti.Services.Storage.Exceptions;

public class PutConditionException(string message, Exception innerException)
    : ProviderOperationException(message, innerException);