namespace VaporWebAPI.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;

using VaporWebAPI.Models;
using VaporWebAPI.DTOs;
using VaporWebAPI.Utils;
using VaporWebAPI.Data;
using VaporWebAPI.Services.Interfaces;

public class AuthService
{
    private readonly VaporDbContext context;
    private readonly IConfiguration configurator;
    private readonly PasswordHasher<string> passwordHasher;
    private readonly IEmailSender emailSender;

    public AuthService(VaporDbContext context, IConfiguration configuration, IEmailSender emailSender)
    {
        this.context = context;
        configurator = configuration;
        passwordHasher = new PasswordHasher<string>();
        this.emailSender = emailSender;
    }

    /// <summary>
    /// Registers a new user after validating all input fields and sends a verification email.
    /// </summary>
    public async Task<string> RegisterUserAsync(RegisterRequest request)
    {
        request.Username = request.Username.Trim();
        request.DisplayName = request.DisplayName.Trim();
        request.Email = request.Email.Trim();

        if (await context.Users.AnyAsync(u => u.Username.ToLower() == request.Username.ToLower()))
        {
            return "Username already exists";
        }

        string usernameFeedback = InputValidation.ValidateUsername(request.Username);
        if (usernameFeedback != "Valid")
        {
            return usernameFeedback;
        }

        string displayNameFeedback = InputValidation.ValidateDisplayName(request.DisplayName);
        if (displayNameFeedback != "Valid")
        {
            return displayNameFeedback;
        }

        if (!InputValidation.IsValidEmail(request.Email))
        {
            return "Invalid Email Address!";
        }

        if (await context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return "Email is already in use";
        }

        string passwordFeedback = InputValidation.ValidatePassword(request.Password);
        if (passwordFeedback != "Valid")
        {
            return passwordFeedback;
        }

        var hashedPassword = passwordHasher.HashPassword(null, request.Password);

        var newUser = new User
        {
            Username = request.Username,
            DisplayName = request.DisplayName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            RoleId = 1,
            ProfilePicture = "/assets/defaultVaporProfilePic.jpg"
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
        await GenerateAndSendEmailVerificationAsync(newUser.UserId, newUser.Email);

        return "User registered successfully!";
    }

    /// <summary>
    /// Generates a verification token, stores it, and sends a verification email to the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="userEmail">The user's email address.</param>
    public async Task<string> GenerateAndSendEmailVerificationAsync(int userId, string userEmail)
    {
        var token = Guid.NewGuid().ToString();

        var verification = new EmailVerification
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        context.EmailVerifications.Add(verification);
        await context.SaveChangesAsync();

        var verificationUrl = $"https://localhost:3000/verify-email?token={token}";
        var deleteUrl = $"https://localhost:3000/delete-account?token={token}";

        var user = await context.Users.FindAsync(userId);

        var model = new EmailVerificationModel
        {
            Username = user.Username,
            VerificationUrl = verificationUrl,
            DeleteUrl = deleteUrl
        };

        var emailHtml = await EmailTemplateRenderer.RenderAsync("VerifyEmail", "VerifyEmail.cshtml", model);

        await emailSender.SendAsync(userEmail, "Verify your email", emailHtml);

        return token;
    }

    /// <summary>
    /// Verifies a user's email using a token. Removes all related verification tokens upon success.
    /// </summary>
    public async Task<string> VerifyEmailAsync(string token)
    {
        var verificationEntry = await context.EmailVerifications
            .AsNoTracking()
            .Where(v => v.Token == token)
            .Select(v => new { v.UserId, v.ExpiresAt })
            .FirstOrDefaultAsync();

        if (verificationEntry == null)
            return "Invalid verification token.";

        if (verificationEntry.ExpiresAt < DateTime.UtcNow)
            return "Token expired.";

        var user = await context.Users.FindAsync(verificationEntry.UserId);
        if (user == null)
            return "User not found.";

        user.IsEmailVerified = true;

        var tokens = await context.EmailVerifications
            .Where(v => v.UserId == user.UserId)
            .ToListAsync();

        context.EmailVerifications.RemoveRange(tokens);

        await context.SaveChangesAsync();

        var model = new EmailVerificationModel()
        {
            Username = user.Username
        };

        var emailHtml = await EmailTemplateRenderer.RenderAsync("VerifySuccessEmail", "VerifySuccessEmail.cshtml", model);

        await emailSender.SendAsync(user.Email, "Email Verified!", emailHtml);

        return "✅ Email verified successfully!";
    }

    /// <summary>
    /// Resends an email verification link if the current one has expired (via email).
    /// </summary>
    public async Task<string> ResendEmailVerificationByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return "❌ User not found.";

        var activeToken = await context.EmailVerifications
            .Where(v => v.UserId == user.UserId && v.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync();

        if (activeToken != null)
            return "Please wait until your current verification link expires before requesting a new one.";

        var oldTokens = await context.EmailVerifications
            .Where(v => v.UserId == user.UserId)
            .ToListAsync();

        context.EmailVerifications.RemoveRange(oldTokens);
        await context.SaveChangesAsync();

        await GenerateAndSendEmailVerificationAsync(user.UserId, user.Email);
        return "✅ Verification email sent.";
    }

    /// <summary>
    /// Resends an email verification link if the current one has expired (via old token).
    /// </summary>
    public async Task<string> ResendEmailVerificationByTokenAsync(string token)
    {
        var expiredToken = await context.EmailVerifications
            .FirstOrDefaultAsync(v => v.Token == token);

        if (expiredToken == null)
            return "❌ Invalid or expired token.";

        var user = await context.Users.FindAsync(expiredToken.UserId);
        if (user == null)
            return "❌ User not found.";

        var activeToken = await context.EmailVerifications
            .Where(v => v.UserId == user.UserId && v.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (activeToken != null)
            return "Please wait until your current verification link expires.";

        context.EmailVerifications.RemoveRange(
            context.EmailVerifications.Where(v => v.UserId == user.UserId));

        await context.SaveChangesAsync();

        await GenerateAndSendEmailVerificationAsync(user.UserId, user.Email);
        return "✅ Verification email sent.";
    }

    //TODO: Restrict Access on N unsuccessful logins

    /// <summary>
    /// Authenticates a user and returns access and refresh tokens if successful.
    /// </summary>
    public async Task<(string accessToken, string refreshToken, string? error)?> LoginAsync(LoginRequest request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(
            u => u.Username.ToLower() == request.Username.ToLower());

        if (user == null)
        {
            return (string.Empty, string.Empty, "Invalid username or password");
        }

        if (!user.IsEmailVerified)
        {
            var encodedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));
            return (string.Empty, string.Empty, $"unverified:{encodedEmail}");
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);

        if (verificationResult != PasswordVerificationResult.Success)
        {
            return null;
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken().Trim();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1); // TEST INPUT: 2 Minutes
        await context.SaveChangesAsync();

        return (accessToken, refreshToken, null);
    }

    /// <summary>
    /// Generates a new access token and optionally a new refresh token based on expiration time.
    /// </summary>
    public async Task<(string? newAccessToken, string? newRefreshToken, string? error)> RefreshTokensAsync(string refreshToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return (null, null, "Invalid or expired refresh token");
        }

        var newAccessToken = GenerateJwtToken(user);
        string refreshTokenToReturn = refreshToken; 

        var timeUntilExpiry = user.RefreshTokenExpiryTime - DateTime.UtcNow;

        if (timeUntilExpiry?.TotalMinutes < 10) // TEST INPUT: 1 Minute 
        {
            refreshTokenToReturn = GenerateRefreshToken();
            user.RefreshToken = refreshTokenToReturn;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1); // TEST INPUT: 2 Minutes
            await context.SaveChangesAsync();
        }

        return (newAccessToken, refreshTokenToReturn, null);
    }

    /// <summary>
    /// Regenerates a new pair of access and refresh tokens for the specified user.
    /// </summary>
    public async Task<(string accessToken, string refreshToken)> RegenerateTokensAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1);

        await context.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    /// <summary>
    /// Logs out the user by invalidating their refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    public async Task<bool> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return false;

        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null) return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Generates a password reset token and sends a reset email to the user if they exist.
    /// </summary>
    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return "If this email exists, a reset link will be sent."; 

        var oldTokens = await context.PasswordResetTokens
            .Where(t => t.UserId == user.UserId)
            .ToListAsync();

        if (oldTokens.Count > 0)
        {
            return "Password reset token is still active!";
        }

        var token = Guid.NewGuid().ToString("N");

        var resetToken = new PasswordResetToken
        {
            UserId = user.UserId,
            Token = token,
            ExpirationTime = DateTime.UtcNow.AddMinutes(10)
        };

        context.PasswordResetTokens.Add(resetToken);
        await context.SaveChangesAsync();

        var resetLink = $"https://localhost:3000/reset-password/{token}";

        var model = new ResetPasswordEmailModel
        {
            Username = user.Username,
            ResetLink = resetLink
        };

        var emailHtml = await EmailTemplateRenderer.RenderAsync("ResetPassword", "ResetPassword.cshtml", model);

        await emailSender.SendAsync(user.Email, "Reset your password", emailHtml);

        return "Success";
    }

    /// <summary>
    /// Resets the user's password using a valid token and sends a confirmation email.
    /// </summary>
    public async Task<string> ResetPasswordAsync(string token, string newPassword)
    {
        var tokenEntry = await context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        if (tokenEntry == null)
            return "The password reset link is invalid. Please request a new one.";

        if (tokenEntry.ExpirationTime <= DateTime.UtcNow)
        {
            context.PasswordResetTokens.Remove(tokenEntry);
            await context.SaveChangesAsync();
            return "This password reset link has expired. Please request a new one.";
        }

        var user = tokenEntry.User;

        string passwordFeedback = InputValidation.ValidatePassword(newPassword);
        if (passwordFeedback != "Valid")
            return passwordFeedback;

        var hasher = new PasswordHasher<string>();
        user.PasswordHash = hasher.HashPassword(null, newPassword);

        context.PasswordResetTokens.Remove(tokenEntry);
        await context.SaveChangesAsync();

        var model = new EmailVerificationModel()
        {
            Username = user.Username
        };

        var emailHtml = await EmailTemplateRenderer.RenderAsync("PasswordChanged", "PasswordChanged.cshtml", model);

        await emailSender.SendAsync(user.Email, "Password has been changed!", emailHtml);

        return "Success";
    }

    /// <summary>
    /// Deletes a user's account using a valid verification token.
    /// </summary>
    public async Task<string> DeleteAccountViaVerificationTokenAsync(string token)
    {
        var userId = await context.EmailVerifications
            .Where(v => v.Token == token)
            .Select(v => v.UserId)
            .FirstOrDefaultAsync();

        if (userId == 0)
            return "Invalid or already used token.";

        var user = await context.Users
            .Include(u => u.EmailVerifications)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return "No user associated with this token.";

        try
        {
            string email = user.Email;
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            var model = new EmailVerificationModel
            {
                Username = user.Username
            };

            var emailHtml = await EmailTemplateRenderer.RenderAsync("DeleteEmail", "DeleteEmail.cshtml", model);

            await emailSender.SendAsync(email, "Account Deleted", emailHtml);

            return "✅ Account successfully deleted.";
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return $"Concurrency issue during deletion: {ex.Message}";
        }
    }

    /// <summary>
    /// Unlinks a user's Google account after checking password availability.
    /// </summary>
    public async Task<string> UnlinkGoogleAccountAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return "User not found.";

        if (user.PasswordHash == "GOOGLE_AUTH" || string.IsNullOrEmpty(user.PasswordHash))
            return "You must set a password before unlinking your Google account.";

        user.GoogleId = null;
        user.IsGoogleAuthenticated = false;

        await context.SaveChangesAsync();
        return "Success";
    }

    /// <summary>
    /// Generates a JWT access token for the specified user with claims.
    /// </summary>
    public string GenerateJwtToken(User user)
    {
        var secretKey = Encoding.UTF8.GetBytes(configurator["JwtSettings:Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleId == 2 ? "Admin" : "User"),
                new Claim("displayName", user.DisplayName ?? ""),
                new Claim("wallet", user.Wallet.ToString()),
                new Claim("profilePicture", user.ProfilePicture ?? ""),
                new Claim("points", user.Points.ToString()),
                new Claim("isGoogleAuthenticated", user.IsGoogleAuthenticated.ToString().ToLower())
            };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(configurator["JwtSettings:ExpiryMinutes"])),
            Issuer = configurator["JwtSettings:Issuer"],
            Audience = configurator["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
