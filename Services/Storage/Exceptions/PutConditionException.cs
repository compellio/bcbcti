namespace Compellio.Bcbcti.Services.Storage.Exceptions;

public class PutConditionException(string message, Exception? innerException = null)
    : ProviderOperationException(message, innerException);