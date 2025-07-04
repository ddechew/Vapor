import fetchWithAuth from "../utils/fetchWithAuth.js";

const APP_API_BASE = "https://localhost:7003/api/apps";

const appService = {
    async getTop5Apps() {
        try {
            const response = await fetch(`${APP_API_BASE}/top5`);
            if (!response.ok) throw new Error("Failed to fetch top 5 apps.");
            return await response.json();
        } catch (error) {
            console.error("Error fetching top 5 apps:", error);
            return [];
        }
    },

    async getAppDetails(appId) {
        try {
            const response = await fetch(`${APP_API_BASE}/${appId}`);
            if (!response.ok) throw new Error("Failed to fetch app details.");
            return await response.json();
        } catch (error) {
            console.error("Error fetching app details:", error);
            return null;
        }
    },

    async getPaginatedApps(page = 1, limit = 12) {
        try {
            const response = await fetch(`${APP_API_BASE}?page=${page}&limit=${limit}`);
            if (!response.ok) throw new Error("Failed to fetch paginated apps.");
            return await response.json();
        } catch (error) {
            console.error("Error fetching paginated apps:", error);
            return [];
        }
    },

    async getRelatedApps(baseAppId) {
        try {
            const response = await fetch(`${APP_API_BASE}/related/${baseAppId}`);
            if (!response.ok) throw new Error("Failed to fetch related apps.");
            return await response.json();
        } catch (error) {
            console.error(`Error fetching related apps for baseAppId=${baseAppId}:`, error);
            return [];
        }
    },

    async searchApps(query = "", isFree = false, genres = [], priceSort = "") {
        const params = new URLSearchParams();

        if (query) params.append("query", query);
        if (isFree) params.append("isFree", true);
        if (priceSort) params.append("priceSort", priceSort);
        genres.forEach(g => params.append("genres", g)); 

        const response = await fetch(`${APP_API_BASE}/search?${params.toString()}`);
        if (!response.ok) throw new Error("Search failed");
        return await response.json();
    },

    async getGenres() {
        const response = await fetch(`${APP_API_BASE}/genres`);
        if (!response.ok) throw new Error("Genre fetch failed");
        return await response.json();
    },

    async getReviews(appId) {
        try {
            const response = await fetch(`${APP_API_BASE}/${appId}/reviews`);
            if (!response.ok) throw new Error("Failed to fetch reviews.");
            return await response.json();
        } catch (error) {
            console.error(`Error fetching reviews for appId=${appId}:`, error);
            return [];
        }
    },

    async submitReview(reviewData) {
        try {
            const response = await fetchWithAuth(`${APP_API_BASE}/review`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(reviewData),
            });

            if (!response.ok) {
                const { error } = await response.json();
                throw new Error(error || "Failed to post review.");
            }

            return "success";
        } catch (error) {
            console.error("Error posting review:", error);
            throw error;
        }
    },

    async checkOwnership(appId) {
        try {
            const response = await fetchWithAuth(`${APP_API_BASE}/owns/${appId}`);
            if (!response.ok) throw new Error("Failed to check ownership.");
            return await response.json();
        } catch (error) {
            console.error(`Error checking ownership for appId=${appId}:`, error);
            return false;
        }
    },

    async hasReviewed(appId) {
        try {
            const res = await fetchWithAuth(`${APP_API_BASE}/has-reviewed/${appId}`);
            if (!res.ok) throw new Error("Failed to check review status.");
            return await res.json(); 
        } catch (err) {
            console.error("Error checking review status:", err);
            return false;
        }
    },

    async updateReview(reviewId, reviewData) {
        try {
            const response = await fetchWithAuth(`https://localhost:7003/api/apps/review/${reviewId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(reviewData),
            });

            if (!response.ok) {
                const { error } = await response.json();
                throw new Error(error || "Failed to update review.");
            }

            return "success";
        } catch (error) {
            console.error("Error updating review:", error);
            throw error;
        }
    },


}

export default appService;