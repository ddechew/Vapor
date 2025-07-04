namespace VaporWebAPI.Utils;

using System.Net.Mail;
using System.Text.RegularExpressions;

/// <summary>
/// Utility class that provides input validation methods for common fields such as email, password, username, and display name.
/// </summary>
public static class InputValidation
{
    /// <summary>
    /// Validates the structure of an email address.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email is valid; otherwise, false.</returns>    
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the strength and format of a password.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>A string message indicating the validation result or "Valid" if successful.</returns>
    public static string ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        { 
            return "Password cannot be empty.";
        }

        if (password.Length < 8)
        { 
            return "Password must be at least 8 characters long.";
        }

        if (!password.Any(char.IsUpper))
        {
            return "Password must contain at least one uppercase letter.";
        }

        if (!password.Any(char.IsLower))
        {
            return "Password must contain at least one lowercase letter.";
        }

        if (!password.Any(char.IsDigit))
        {
            return "Password must contain at least one number.";
        }

        if (!Regex.IsMatch(password, @"[\W_]"))
        {
            return "Password must contain at least one special character (!@#$%^&* etc.).";
        }

        return "Valid";
    }

    /// <summary>
    /// Validates a username according to specific naming rules.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <returns>A string message indicating the validation result or "Valid" if successful.</returns>
    public static string ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return "Username cannot be empty.";
        }

        if (username.Length < 4 || username.Length > 20)
        {
            return "Username must be between 4 and 20 characters long.";
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z]{4,}"))
        {
            return "Username must start with at least 4 letters.";
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_.]+$"))
        {
            return "Username can only contain letters, numbers, underscores (_), and dots (.).";
        }

        if (username.Contains("..") || username.StartsWith(".") || username.EndsWith("."))
        {
            return "Username cannot have consecutive dots or start/end with a dot.";
        }

        return "Valid";
    }

    /// <summary>
    /// Validates a display name according to specific format and content rules.
    /// </summary>
    /// <param name="displayName">The display name to validate.</param>
    /// <returns>A string message indicating the validation result or "Valid" if successful.</returns>
    public static string ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return "Display name cannot be empty.";
        }

        if (displayName.Length < 3 || displayName.Length > 30)
        {
            return "Display name must be between 3 and 30 characters long.";
        }

        if (!Regex.IsMatch(displayName, @"^[a-zA-Z0-9\s._-]+$"))
        {
            return "Display name can only contain letters, numbers, spaces, dots, dashes, and underscores.";
        }

        if (!Regex.IsMatch(displayName, @"[a-zA-Z]"))
        {
            return "Display name must contain at least one letter.";
        }

        return "Valid";
    }
}
