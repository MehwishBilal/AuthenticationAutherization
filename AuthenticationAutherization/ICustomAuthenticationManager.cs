namespace AuthenticationAutherization
{
    public interface ICustomAuthenticationManager
    {
        string Authenticate(string username, string password);
        IDictionary<string, Tuple<string,string>> Tokens { get; }
    }
}
