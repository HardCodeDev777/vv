namespace vv.Core;

internal sealed class UserException : Exception
{
    public UserException(string message) : base(message) { }
}
