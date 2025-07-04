namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

using RegisterRequest = DTOs.RegisterRequest;
using LoginRequest = DTOs.LoginRequest;
using System.Security.Claims;

using VaporWebAPI.DTOs;
using VaporWebAPI.Services;
using VaporWebAPI.Services.Interfaces;

/// <summary>
/// Handles authentication-related operations including registration, login, logout, email verification,
/// password reset, token refresh, and Google OAuth callbacks.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService authService;
    private readonly GoogleAuthService googleAuthService;
    private readonly IEmailSender emailSender;

    public AuthController(AuthService authService, GoogleAuthService googleAuthService, IEmailSender emailSender)
    {
        this.authService = authService;
        this.googleAuthService = googleAuthService;
        this.emailSender = emailSender;
    }

    /// <summary>
    /// Registers a new user with a username, password, email, and display name.
    /// </summary>
    /// <param name="request">The registration details.</param>
    /// <returns>A success or error message.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        string result = await authService.RegisterUserAsync(request);

        if (result == "User registered successfully!")
        {
            return Ok(new { message = result });
        }

        return BadRequest(new { message = result });
    }

    /// <summary>
    /// Authenticates a user using username and password, returning JWT tokens.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>Access and refresh tokens if successful.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var tokens = await authService.LoginAsync(request);

        if (tokens == null)
            return Unauthorized(new { message = "Invalid login" });

        if (!string.IsNullOrEmpty(tokens?.error))
        {
            if (tokens.Value.error.StartsWith("unverified:"))
            {
                return BadRequest(new { message = tokens.Value.error });
            }

            return Unauthorized(new { message = tokens.Value.error });
        }

        return Ok(new
        {
            tokens.Value.accessToken,
            tokens.Value.refreshToken
        });
    }

    /// <summary>
    /// Verifies a user's email address using a token sent via email.
    /// </summary>
    /// <param name="token">The verification token.</param>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var result = await authService.VerifyEmailAsync(token);
        if (result.StartsWith("✅"))
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Resends a verification email based on the user's email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationByEmail([FromBody] string email)
    {
        var result = await authService.ResendEmailVerificationByEmailAsync(email);

        if (result.StartsWith("❌") || result.StartsWith("Please wait"))
            return BadRequest(result);

        return Ok("✅ Verification email sent.");
    }

    /// <summary>
    /// Resends a verification email based on an expired token.
    /// </summary>
    /// <param name="token">The expired verification token.</param>
    [AllowAnonymous]
    [HttpPost("resend-verification-by-token")]
    public async Task<IActionResult> ResendVerificationByToken([FromQuery] string token)
    {
        var result = await authService.ResendEmailVerificationByTokenAsync(token);

        if (result.StartsWith("❌") || result.StartsWith("Please wait"))
            return BadRequest(result);

        return Ok("✅ Verification email sent.");
    }

    /// <summary>
    /// Sends a password reset link to the provided email address if the user exists.
    /// </summary>
    /// <param name="request">The email to send the reset link to.</param>
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid email.");

        var result = await authService.GeneratePasswordResetTokenAsync(request.Email);

        return Ok(new { message = "If this email exists, a reset link has been sent." });
    }

    /// <summary>
    /// Resets the user's password using a valid token and new password.
    /// </summary>
    /// <param name="input">Reset token and new password input.</param>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordInput input)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request.");

        var result = await authService.ResetPasswordAsync(input.Token, input.NewPassword);

        return result == "Success"
            ? Ok(new { message = "Password has been reset." })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Deletes the user's account using a verification token sent to their email.
    /// </summary>
    /// <param name="token">The token provided in the email.</param>
    [AllowAnonymous]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccount([FromQuery] string token)
    {
        var result = await authService.DeleteAccountViaVerificationTokenAsync(token);
        return result.StartsWith("✅") ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Refreshes expired access tokens using a valid refresh token.
    /// </summary>
    /// <param name="model">The refresh token model.</param>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest model)
    {
        var (accessToken, refreshToken, error) = await authService.RefreshTokensAsync(model.RefreshToken);

        if (error != null)
            return Unauthorized(new { message = error });

        return Ok(new { token = accessToken, refreshToken });
    }

    /// <summary>
    /// Regenerates new access and refresh tokens for the currently authenticated user.
    /// </summary>
    [Authorize]
    [HttpPost("regenerate-tokens")]
    public async Task<IActionResult> RegenerateTokens()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var (accessToken, refreshToken) = await authService.RegenerateTokensAsync(userId);

        return Ok(new
        {
            token = accessToken,
            refreshToken = refreshToken
        });
    }

    /// <summary>
    /// Logs the user out and invalidates the refresh token.
    /// </summary>
    /// <param name="request">The logout request containing the refresh token.</param>
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var success = await authService.LogoutAsync(request.RefreshToken);

        if (!success)
            return Unauthorized(new { message = "Logout failed. Invalid refresh token." });

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Initiates the Google OAuth login flow.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleCallback", "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            AllowRefresh = true,
            Parameters = { { "prompt", "select_account" } }
        };

        return Challenge(properties, "Google"); 
    }

    /// <summary>
    /// Handles the Google login callback, linking or creating a user account,
    /// then redirects to the frontend with tokens.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("signin-google")]
    public async Task<IActionResult> GoogleCallback()
    {
        var tokens = await googleAuthService.HandleGoogleCallbackAsync(HttpContext);
        if (tokens == null)
        {
            return Unauthorized(new { message = "Google login failed." });
        }

        var frontendRedirectUrl = $"https://localhost:3000/google-auth?token={tokens.Value.accessToken}&refreshToken={tokens.Value.refreshToken}";
        return Redirect(frontendRedirectUrl);
    }
}   
