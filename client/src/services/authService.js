import fetchWithAuth from "../utils/fetchWithAuth";

const AUTH_API_BASE = "https://localhost:7003/api/auth";

const authService = {
    async login(credentials) {
        const response = await fetch(`${AUTH_API_BASE}/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(credentials),
        });

        const data = await response.json();

        if (!response.ok) {
            if (data.message?.startsWith("unverified:")) {
                const email = data.message.split(":")[1];
                window.location.href = `/verify-email?unverified=${encodeURIComponent(email)}`;
                return;
            }

            throw new Error(data.error || data.message || "Login failed");
        }

        return {
            token: data.accessToken,
            refreshToken: data.refreshToken
        };
    },

    async regenerateTokens() {
        const res = await fetchWithAuth(`${AUTH_API_BASE}/regenerate-tokens`, {
            method: "POST",
        });

        if (!res.ok) throw new Error("Failed to regenerate tokens");

        const data = await res.json();

        localStorage.setItem("token", data.token);
        localStorage.setItem("refreshToken", data.refreshToken);

        return data; 
    },

    async logout() {
        const storedRefreshToken = localStorage.getItem("refreshToken");

        if (storedRefreshToken) {
            try {
                await fetch(`${AUTH_API_BASE}/logout`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({ refreshToken: storedRefreshToken })
                });
            } catch (err) {
                console.warn("Logout request failed:", err);
            }
        }

        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("cart");
    },

    async register(userData) {
        const response = await fetch(`${AUTH_API_BASE}/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(userData),
        });

        const contentType = response.headers.get("content-type");
        let data;

        if (contentType && contentType.includes("application/json")) {
            data = await response.json();
        } else {
            data = { message: await response.text() };
        }

        if (!response.ok) throw new Error(data.message || "Registration failed");
        return data;
    },

    async googleLogin() {
        const urlParams = new URLSearchParams(window.location.search);
        const token = urlParams.get("token");
        const refreshToken = urlParams.get("refreshToken");

        if (!token || !refreshToken) {
            throw new Error("Missing tokens in Google login redirect.");
        }

        return { token, refreshToken };
    },

    async verifyEmail(token) {
        const response = await fetch(`${AUTH_API_BASE}/verify-email?token=${token}`);
        const text = await response.text();

        return text;
    },

    async resendVerification(email) {
        const response = await fetch(`${AUTH_API_BASE}/resend-verification`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(email)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Failed to resend verification email");
        }

        return true;
    },

    async resendVerificationByToken(token) {
        const response = await fetch(`${AUTH_API_BASE}/resend-verification-by-token?token=${token}`, {
            method: "POST"
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Failed to resend verification email");
        }

        return true;
    },

    async deleteAccountByToken(token) {
        const response = await fetch(`${AUTH_API_BASE}/delete-account?token=${token}`, {
            method: "DELETE"
        });

        const contentType = response.headers.get("content-type");

        let data;
        if (contentType && contentType.includes("application/json")) {
            data = await response.json();
        } else {
            data = { message: await response.text() }; // wrap text in a fake object
        }

        if (!response.ok) throw new Error(data.message || "Failed to delete account");

        return data.message;
    },

    async requestPasswordReset(email) {
        const res = await fetch(`${AUTH_API_BASE}/request-password-reset`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email }),
        });

        if (!res.ok) throw new Error("Reset request failed");
        return await res.json();
    },

    async resetPassword(token, newPassword) {
        const res = await fetch(`${AUTH_API_BASE}/reset-password`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ token, newPassword }),
        });

        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || "Reset failed");
        }

        return await res.json();
    }
}

export default authService;