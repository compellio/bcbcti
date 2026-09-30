namespace Compellio.Bcbcti.Services.Storage.Exceptions;

public class ProviderOperationException(string? message, Exception? innerException = null)
    : StorageException(message, innerException);