import fetchWithAuth from "../utils/fetchWithAuth";

const USER_API_BASE = "https://localhost:7003/api/user";

const userService = {
    async getUserById(userId) {
        const response = await fetchWithAuth(`${USER_API_BASE}/${userId}`)
        if (!response.ok) throw new Error("Failed to fetch user details");
        return await response.json();
    },

    async getUserByUsername(username) {
        const res = await fetch(`${USER_API_BASE}/by-username/${username}`);
        if (!res.ok) throw new Error("Failed to fetch user profile.");
        return await res.json();
    },

    async updateProfile(profileData) {
        const response = await fetchWithAuth(`${USER_API_BASE}/update-profile`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(profileData)
        });
        if (!response.ok) throw new Error("Failed to update profile.");
        return await response.json();
    },

    async uploadProfilePicture(blob) {
        const formData = new FormData();
        formData.append("file", blob, "profile.jpg");

        const res = await fetchWithAuth(`${USER_API_BASE}/upload-profile-picture`, {
            method: "POST",
            body: formData,
        });

        if (!res.ok) {
            const error = await res.text();
            throw new Error(error || "Failed to upload image.");
        }

        return await res.json(); 
    },

    async initiateEmailChange(newEmail) {
        const response = await fetchWithAuth(`${USER_API_BASE}/initiate-email-change`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ newEmail })
        });

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            throw new Error(data.message || "Failed to initiate email change.");
        }

        return data;
    },

    async confirmEmailChange(token) {
        const res = await fetch(`${USER_API_BASE}/confirm-email-change?token=${encodeURIComponent(token)}`, {
            method: "POST"
        });

        const data = await res.json().catch(() => ({}));

        if (!res.ok) {
            throw new Error(data.message || "Email confirmation failed.");
        }

        return data;
    },

    async changePassword(currentPassword, newPassword) {
        const response = await fetchWithAuth(`${USER_API_BASE}/change-password`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ currentPassword, newPassword }),
        });

        const data = await response.json(); 

        if (!response.ok) {
            throw new Error(data.message || "Failed to change password.");
        }

        return data;
    },

    async changeUsername(newUsername) {
        const res = await fetchWithAuth(`${USER_API_BASE}/change-username`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newUsername), 
        });

        const data = await res.json().catch(() => ({}));

        if (!res.ok) {
            throw new Error(data.message || "Failed to change username.");
        }

        return data;
    },

    async getUserLibrary() {
        const response = await fetchWithAuth(`${USER_API_BASE}/library`);
        if (!response.ok) {
            throw new Error("Failed to fetch user library");
        }
        return await response.json();
    },

    async getUserLibraryByUsername(username) {
        const response = await fetch(`${USER_API_BASE}/library/${username}`);
        if (!response.ok) throw new Error("Failed to fetch user library by username");
        return await response.json();
    },

    async unlinkGoogle() {
        const res = await fetchWithAuth(`${USER_API_BASE}/unlink-google`, {
            method: "POST"
        });

        if (!res.ok) {
            const err = await res.json();
            throw new Error(err.message || "Failed to unlink Google account.");
        }

        return true;
    },

    async getPurchaseHistory() {
        const res = await fetchWithAuth(`${USER_API_BASE}/purchase-history`);
        if (!res.ok) throw new Error("Failed to fetch purchase history.");
        return await res.json();
    },


    async purchaseWithWallet(pointsToUse = 0) {
        const res = await fetchWithAuth(
            `${USER_API_BASE}/wallet/purchase?pointsToUse=${pointsToUse}`,
            { method: "POST" }
        );

        if (!res.ok) throw new Error("Wallet purchase failed");
        return await res.json();
    },

    async addFreeAppToLibrary(appId) {
        const res = await fetchWithAuth(`${USER_API_BASE}/add-free/${appId}`, {
            method: "POST"
        });
        if (!res.ok) throw new Error("Failed to add free app to library.");
        return await res.json();
    }
}

export default userService;