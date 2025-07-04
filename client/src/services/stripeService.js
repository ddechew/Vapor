import fetchWithAuth from "../utils/fetchWithAuth";

const STRIPE_API_BASE = "https://localhost:7003/api/stripe";

const stripeService = {
    async createStripeSession(amount, userId, autoPurchase = false, pointsToUse = 0) {
        const response = await fetchWithAuth(`${STRIPE_API_BASE}/create-session`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ amount, userId, autoPurchase, pointsToUse }),
        });

        if (!response.ok) throw new Error("Failed to create Stripe session");
        return await response.json();
    },

    async confirmWalletPayment(sessionId) {
        const response = await fetchWithAuth(`${STRIPE_API_BASE}/confirm-session?session_id=${encodeURIComponent(sessionId)}`, {
            method: "POST"
        });

        if (!response.ok) throw new Error("Failed to confirm wallet payment");
        return await response.json();
    },

    async cancelStripeSession(sessionId) {
        const response = await fetch(
            `${STRIPE_API_BASE}/cancel-session?session_id=${encodeURIComponent(sessionId)}`,
            { method: "POST" }
        );

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || "Failed to cancel Stripe session");
        }

        return await response.json(); 
    },
};

export default stripeService;
