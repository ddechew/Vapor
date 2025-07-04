namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Stripe;
using Stripe.Checkout;

using System.Threading.Tasks;

using VaporWebAPI.Data;
using VaporWebAPI.Models;
using VaporWebAPI.Utils;

public class StripeService
{
    private readonly StripeSettings settings;
    private readonly VaporDbContext context;

    public StripeService(IOptions<StripeSettings> stripeSettings, VaporDbContext context)
    {
        settings = stripeSettings.Value;
        StripeConfiguration.ApiKey = settings.SecretKey;
        this.context = context;
    }

    /// <summary>
    /// Confirms a wallet top-up by validating a Stripe session ID and adds the amount to the user's wallet.
    /// Also creates a purchase history record.
    /// </summary>
    /// <param name="sessionId">The Stripe checkout session ID.</param>
    /// <param name="userId">The ID of the user completing the payment.</param>
    /// <returns>A status message indicating success or failure.</returns>
    public async Task<string> ConfirmWalletPaymentBySessionIdAsync(string sessionId, int userId)
    {
        var record = await context.PaymentTokens
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == userId);


        if (record == null)
            return "Invalid session.";

        if (record.ExpiresAt < DateTime.UtcNow)
        {
            context.PaymentTokens.Remove(record);
            await context.SaveChangesAsync();
            return "Expired session. Please try again.";
        }

        record.User.Wallet += record.Amount;

        context.PaymentTokens.Remove(record);

        context.PurchaseHistories.Add(new PurchaseHistory
        {
            UserId = userId,
            AppId = null,
            PurchaseDate = DateTime.Now,
            PriceAtPurchase = 0,
            PaymentMethod = "Stripe Wallet Top-Up",
            WalletChange = record.Amount,
            WalletBalanceAfter = record.User.Wallet,
        });

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return "Success";
    }

    /// <summary>
    /// Deletes a stored payment token by its Stripe session ID.
    /// </summary>
    /// <param name="sessionId">The Stripe checkout session ID.</param>
    /// <returns>True if the token was deleted, false if not found.</returns>
    public async Task<bool> DeletePaymentTokenBySessionIdAsync(string sessionId)
    {
        var token = await context.PaymentTokens
            .FirstOrDefaultAsync(p => p.SessionId == sessionId);

        if (token != null)
        {
            context.PaymentTokens.Remove(token);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates a Stripe Checkout session for a wallet top-up, optionally triggering auto-purchase logic after success.
    /// </summary>
    /// <param name="amount">The amount to add to the user's wallet.</param>
    /// <param name="userId">The user initiating the transaction.</param>
    /// <param name="autoPurchase">If true, proceeds with purchase after top-up.</param>
    /// <param name="pointsToUse">Optional: the number of discount points used in the auto-purchase flow.</param>
    /// <returns>The created Stripe checkout session.</returns>
    public async Task<Session> CreateCheckoutSession(decimal amount, int userId, bool autoPurchase = false, int pointsToUse = 0)
    {
        string productName = autoPurchase
            ? $"Add {amount:F2}€ to Wallet and complete transaction"
            : $"Add {amount:F2}€ to Vapor Wallet";

        var sessionOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100),
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productName
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = autoPurchase
                ? "https://localhost:3000/cart?success=true&session_id={CHECKOUT_SESSION_ID}&autoPurchase=true&pointsToUse=" + pointsToUse
                : "https://localhost:3000/wallet?success=true&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = autoPurchase
                ? "https://localhost:3000/cart?canceled=true&session_id={CHECKOUT_SESSION_ID}"
                : "https://localhost:3000/wallet?canceled=true&session_id={CHECKOUT_SESSION_ID}",
            ClientReferenceId = userId.ToString()
        };

        var service = new SessionService();
        var session = await service.CreateAsync(sessionOptions);

        var oldTokens = context.PaymentTokens.Where(t => t.UserId == userId);
        context.PaymentTokens.RemoveRange(oldTokens);

        var token = new PaymentToken
        {
            UserId = userId,
            Amount = amount,
            Token = Guid.NewGuid().ToString("N"),
            SessionId = session.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        context.PaymentTokens.Add(token);
        await context.SaveChangesAsync();

        return session;
    }
}
