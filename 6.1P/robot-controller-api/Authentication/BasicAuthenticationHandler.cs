using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserDataAccess _userDataAccess;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserDataAccess userDataAccess)
        : base(options, logger, encoder, clock)
    {
        _userDataAccess = userDataAccess;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Response.Headers["WWW-Authenticate"] = @"Basic realm=""Access to the robot controller.""";

        var endpoint = Context.GetEndpoint();

        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]!);

            if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme."));
            }

            if (string.IsNullOrWhiteSpace(authHeader.Parameter))
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail("Missing credentials."));
            }

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

            if (credentials.Length != 2)
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials format."));
            }

            var email = credentials[0];
            var password = credentials[1];

            var user = _userDataAccess.GetUserByEmail(email);

            if (user == null)
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
            }

            var hasher = new PasswordHasher<User>();

            var verificationResult = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }
        catch
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
        }
    }
}