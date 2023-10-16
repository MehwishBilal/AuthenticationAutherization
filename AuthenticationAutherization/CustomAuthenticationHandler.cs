using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;

namespace AuthenticationAutherization
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {


    }
    public class CustomAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly ICustomAuthenticationManager CustomAuthenticationManager;
        public CustomAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ICustomAuthenticationManager customAuthenticationManager) : base(options, logger, encoder, clock)
        {
            this.CustomAuthenticationManager = customAuthenticationManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string authorizationHeader = Request.Headers["Authorization"];
            if(string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            if(!authorizationHeader.StartsWith("bearer",StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            string token = authorizationHeader.Substring("bearer".Length).Trim();
            if(string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            try
            {
                return ValidateToken(token);

            }
            catch (Exception ex)
            {

                return AuthenticateResult.Fail("Unauthorized");
            }
        }

        private AuthenticateResult ValidateToken(string Token)
        {
            var ValidatedToken = CustomAuthenticationManager.Tokens.FirstOrDefault(t => t.Key == Token);

            if (ValidatedToken.Key == null)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            // model identity information.
            // Those claims could come from any number of sources, such as  database, or even local storage
            //ClaimsIdentity is like a passport
            var claims = new List<Claim>                                            
            {
                   new Claim(ClaimTypes.Name, ValidatedToken.Value.Item1),
                   new Claim(ClaimTypes.Role, ValidatedToken.Value.Item2)
            };
            //Identity - authentication as service
            //API that provides set of standard functionalities for authentication and authorization features
            //we can create new user account and provide login mechanism with different user roles and user profile.
            var identity = new ClaimsIdentity(claims, Scheme.Name);

            //a user can have multiple ways identifying themselves
            var principal = new GenericPrincipal(identity, new[] { ValidatedToken.Value.Item2});    //roles
            //Generate Ticket 
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);


        }
    }
}
