namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using System.Security.Claims;

using VaporWebAPI.Services;
using VaporWebAPI.DTOs;

/// <summary>
/// Handles Stripe-based payment sessions, confirmations, and cancellations for wallet top-ups and purchases.
/// </summary>
[ApiController]
[Route("api/stripe")]
public class StripeController : ControllerBase
{
    private readonly StripeService stripeService;

    public StripeController(StripeService stripeService)
    {
        this.stripeService = stripeService;
    }

    /// <summary>
    /// Creates a Stripe Checkout Session for the specified user and amount.
    /// Supports optional auto-purchase flow and point discount application.
    /// </summary>
    /// <param name="request">Session creation parameters (amount, userId, flags).</param>
    /// <returns>A Stripe session URL to redirect the user for payment.</returns>
    [HttpPost("create-session")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateSessionRequest request)
    {
        var session = await stripeService.CreateCheckoutSession(
            request.Amount,
            request.UserId,
            request.AutoPurchase,
            request.PointsToUse
            );

        if (session?.Url == null)
        {
            return BadRequest("Failed to create Stripe session");
        }
        return Ok(new { sessionUrl = session.Url });
    }

    /// <summary>
    /// Cancels a pending Stripe session and deletes the associated payment token.
    /// </summary>
    /// <param name="session_id">The Stripe session ID to cancel.</param>
    /// <returns>Status message indicating success or failure.</returns>
    [HttpPost("cancel-session")]
    [AllowAnonymous]
    public async Task<IActionResult> CancelSession([FromQuery] string session_id)
    {
        if (string.IsNullOrWhiteSpace(session_id))
            return BadRequest("Missing session ID");

        var result = await stripeService.DeletePaymentTokenBySessionIdAsync(session_id);
        return Ok(new { message = result ? "Payment session canceled." : "Nothing to cancel." });
    }

    /// <summary>
    /// Confirms a Stripe payment session by ID and credits the user’s wallet.
    /// </summary>
    /// <param name="session_id">The session ID returned from Stripe.</param>
    /// <returns>Status message indicating if the wallet was successfully updated.</returns>
    [HttpPost("confirm-session")]
    [Authorize]
    public async Task<IActionResult> ConfirmBySessionId([FromQuery] string session_id)
    {
        if (string.IsNullOrWhiteSpace(session_id))
            return BadRequest("Missing session ID");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await stripeService.ConfirmWalletPaymentBySessionIdAsync(session_id, userId);

        return result == "Success"
            ? Ok(new { message = "Wallet updated." })
            : BadRequest(new { message = result });
    }
}
