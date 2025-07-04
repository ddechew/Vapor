import fetchWithAuth from "../utils/fetchWithAuth";

const WISHLIST_BASE_API = "https://localhost:7003/api/wishlist";

const wishlistService = {
  async getWishlist() {
    const res = await fetchWithAuth(`${WISHLIST_BASE_API}`);
    if (!res.ok) throw new Error("Failed to fetch wishlist");
    return await res.json();
  },

  async addToWishlist(appId, priority = 1) {
    const res = await fetchWithAuth(`${WISHLIST_BASE_API}/add`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ appId, priority }),
    });

    if (!res.ok) {
      const msg = await res.text();
      throw new Error(msg || "Failed to add to wishlist");
    }
  },

  async removeFromWishlist(appId) {
    const res = await fetchWithAuth(`${WISHLIST_BASE_API}/remove/${appId}`, {
      method: "DELETE",
    });
    if (!res.ok) throw new Error("Failed to remove from wishlist");
  },

  async updatePriority(appId, priority) {
    const res = await fetchWithAuth(`${WISHLIST_BASE_API}/priority`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ appId, priority }),
    });
    if (!res.ok) throw new Error("Failed to update wishlist priority");
  },

  async normalize() {
    const res = await fetchWithAuth(`${WISHLIST_BASE_API}/normalize`, {
      method: "PUT",
    });
    if (!res.ok) throw new Error("Failed to normalize wishlist priorities");
  },
};

export default wishlistService;
