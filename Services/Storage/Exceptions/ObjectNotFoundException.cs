namespace Compellio.Bcbcti.Services.Storage.Exceptions;

public class ObjectNotFoundException(string message, Exception? innerException = null)
    : ProviderOperationException(message, innerException);