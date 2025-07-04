namespace VaporWebAPI.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

using VaporWebAPI.Data;
using VaporWebAPI.DTOs;
using VaporWebAPI.Models;
using VaporWebAPI.Services.Interfaces;
using VaporWebAPI.Utils;

public class GoogleAuthService
{
    private readonly VaporDbContext context;
    private readonly AuthService authService;
    private readonly IEmailSender emailSender;

    public GoogleAuthService(VaporDbContext context, AuthService authService, IEmailSender emailSender)
    {
        this.context = context;
        this.authService = authService;
        this.emailSender = emailSender;
    }

    /// <summary>
    /// Initializes Google authentication properties with a specified redirect URL.
    /// </summary>
    /// <param name="redirectUrl">The URL to redirect to after Google login.</param>
    /// <returns>Authentication properties with the redirect URL set.</returns>
    public AuthenticationProperties GetGoogleLoginProperties(string redirectUrl)
    {
        return new AuthenticationProperties { RedirectUri = redirectUrl };
    }

    /// <summary>
    /// Handles the Google OAuth callback, logs in or registers the user,
    /// and returns access and refresh tokens.
    /// </summary>
    /// <param name="httpContext">The HTTP context from the OAuth middleware.</param>
    /// <returns>
    /// A tuple containing the access token and refresh token if successful; otherwise, null.
    /// </returns>
    public async Task<(string accessToken, string refreshToken)?> HandleGoogleCallbackAsync(HttpContext httpContext)
    {
        var result = await httpContext.AuthenticateAsync("External");

        if (!result.Succeeded || result.Principal == null)
            return null;

        var claims = result.Principal.Claims.ToList();
        var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleId))
            return null;

        var user = await context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

        if (user == null)
        {
            user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                user.GoogleId = googleId;
                user.IsGoogleAuthenticated = true;
                await context.SaveChangesAsync();

                var notifyModel = new EmailVerificationModel
                {
                    Username = user.Username
                };

                var notifyHtml = await EmailTemplateRenderer.RenderAsync("GoogleLinked", "GoogleLinked.cshtml", notifyModel);
                await emailSender.SendAsync(user.Email, "Your account is now linked with Google", notifyHtml);
            }
            else
            {
                var baseUsername = email.Split('@')[0];
                string finalUsername = baseUsername;
                int suffix = 1;

                while (await context.Users.AnyAsync(u => u.Username == finalUsername))
                {
                    finalUsername = $"{baseUsername}{suffix++}";
                }

                user = new User
                {
                    Username = finalUsername,
                    DisplayName = name ?? baseUsername,
                    Email = email,
                    PasswordHash = "GOOGLE_AUTH",
                    RoleId = 1,
                    ProfilePicture = "/assets/defaultVaporProfilePic.jpg",
                    IsEmailVerified = true,
                    IsGoogleAuthenticated = true,
                    GoogleId = googleId,
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                var model = new EmailVerificationModel
                {
                    Username = user.Username,
                    FromGoogle = true
                };

                var emailHtml = await EmailTemplateRenderer.RenderAsync("VerifySuccessEmail", "VerifySuccessEmail.cshtml", model);
                await emailSender.SendAsync(user.Email, "Email Verified!", emailHtml);
            }
        }

        var accessToken = authService.GenerateJwtToken(user);
        var refreshToken = authService.GenerateRefreshToken().Trim();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1);
        await context.SaveChangesAsync();

        return (accessToken, refreshToken);
    }
}
