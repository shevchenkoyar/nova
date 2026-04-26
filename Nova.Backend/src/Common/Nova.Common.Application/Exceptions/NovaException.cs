using Nova.Common.Domain;

namespace Nova.Common.Application.Exceptions;

public sealed class NovaException(
    string requestName, 
    Error? error = null, 
    Exception? innerException = null) : Exception("Application exception", innerException)
{
    public string RequestName { get; } = requestName;

    public Error? Error { get; } = error;
}
