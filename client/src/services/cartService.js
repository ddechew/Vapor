const CART_API_BASE = "https://localhost:7003/api/cart";

const cartService = {
    async getCart(userId) {
        const response = await fetch(`${CART_API_BASE}/${userId}`);
        if (!response.ok) throw new Error("Failed to fetch cart.");
        return await response.json();
    },

    async addToCart(userId, appId) {
        const response = await fetch(`${CART_API_BASE}/add`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userId, appId }),
        });
        if (!response.ok) throw new Error("Failed to add to cart.");
    },

    async removeFromCart(userId, appId) {
        const response = await fetch(`${CART_API_BASE}/remove/${userId}/${appId}`, {
            method: "DELETE",
        });
        if (!response.ok) throw new Error("Failed to remove from cart.");
    },

    async clearCart(userId) {
        const response = await fetch(`${CART_API_BASE}/clear/${userId}`, {
            method: "DELETE",
        });
        if (!response.ok) throw new Error("Failed to clear cart.");
    },

    async mergeGuestCart(userId, guestAppIds) {
        const response = await fetch(`${CART_API_BASE}/merge`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userId, guestAppIds }),
        });
        if (!response.ok) throw new Error("Failed to merge guest cart.");
    }
}

export default cartService;