namespace Compellio.Bcbcti.Services.Storage.Exceptions;

public class ProviderOperationException(string? message, Exception? innerException) : StorageException(message, innerException);