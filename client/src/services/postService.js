import fetchWithAuth from "../utils/fetchWithAuth";

const POST_API_BASE = "https://localhost:7003/api/post";

const postService = {
    async getAllPosts() {
        const response = await fetch(POST_API_BASE);
        if (!response.ok) throw new Error("Failed to fetch posts");
        return await response.json();
    },

    async togglePostLike(postId) {
        const res = await fetchWithAuth(`${POST_API_BASE}/${postId}/like`, {
            method: "POST"
        });
        if (!res.ok) throw new Error("Failed to toggle like");
        return await res.text();
    },

    async getPostLikers(postId) {
        const res = await fetchWithAuth(`${POST_API_BASE}/${postId}/likers`);
        if (!res.ok) throw new Error("Failed to fetch likers.");
        return await res.json();
    },

    async getUserLikedPosts() {
        const res = await fetchWithAuth(`${POST_API_BASE}/liked`);
        if (!res.ok) throw new Error("Failed to fetch liked posts.");
        return await res.json();
    },

    async createPost(postData) {
        const res = await fetchWithAuth(POST_API_BASE, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(postData),
        });
        if (!res.ok) throw new Error("Failed to create post");
        return await res.text();
    },

    async editComment(commentId, newText) {
        const res = await fetchWithAuth(`${POST_API_BASE}/comment/${commentId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newText)
        });

        if (!res.ok) throw new Error("Failed to edit comment.");
        return await res.text();
    },

    async uploadPostImage(file) {
        try {
            const formData = new FormData();
            formData.append("file", file);

            const res = await fetchWithAuth(`${POST_API_BASE}/upload-image`, {
                method: "POST",
                body: formData,
            });

            if (!res.ok) {
                const text = await res.text();
                console.error("[uploadPostImage] Server error:", text);
                throw new Error(text || "Failed to upload post image");
            }

            const result = await res.json();
            console.log("[uploadPostImage] Parsed result:", result);
            return result;

        } catch (err) {
            console.error("[uploadPostImage] Unexpected error:", err);
            throw err; // rethrow so CreatePost can show it
        }
    },


    async getPostComments(postId) {
        const res = await fetchWithAuth(`${POST_API_BASE}/${postId}/comments`);
        if (!res.ok) throw new Error("Failed to fetch comments.");
        return await res.json();
    },

    async addPostComment(commentData) {
        const res = await fetchWithAuth(`${POST_API_BASE}/comment`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(commentData),
        });
        if (!res.ok) throw new Error("Failed to post comment.");
        return await res.text();
    },

    async deleteComment(commentId) {
        const res = await fetchWithAuth(`${POST_API_BASE}/comment/${commentId}`, {
            method: "DELETE",
        });

        if (!res.ok) throw new Error("Failed to delete comment.");
        return await res.text();
    },

    async deletePost(postId) {
        const res = await fetchWithAuth(`${POST_API_BASE}/${postId}`, {
            method: "DELETE",
        });

        if (!res.ok) throw new Error("Failed to delete post.");
        return await res.text();
    },

    async deleteCommentAsPostOwner(commentId) {
        const res = await fetchWithAuth(`${POST_API_BASE}/comment/by-owner/${commentId}`, {
            method: "DELETE",
        });

        if (!res.ok) throw new Error("Failed to delete comment as post owner.");
        return await res.text();
    }
};

export default postService;