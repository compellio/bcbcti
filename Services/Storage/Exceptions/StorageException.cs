namespace Compellio.Bcbcti.Services.Storage.Exceptions;

public class StorageException(string? message, Exception? innerException) : Exception(message, innerException);