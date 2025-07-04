import fetchWithAuth from "../utils/fetchWithAuth";

const ADMIN_API_BASE = "https://localhost:7003/api/admin";

const adminService = {
  //===================================================================================================================================
  // == USER ==
  //===================================================================================================================================

  async getUsers() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/users`);
    if (!res.ok) throw new Error("Failed to fetch users");
    return await res.json();
  },

  async getApps() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/apps`);
    if (!res.ok) throw new Error("Failed to fetch apps");
    return await res.json();
  },

  async downloadUsersExcel() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/users/export`, {
      method: "GET",
    });
    return res.blob();
  },

  async updateUser(userId, updatedUser) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/users/${userId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(updatedUser)
    });
    if (!res.ok) {
      const errorData = await res.json();
      throw new Error(errorData.message || "Failed to update user");
    };
    return await res.json();
  },

  async createUser(newUser) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/users`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(newUser)
    });
    if (!res.ok) {
      const errorData = await res.json();
      throw new Error(errorData.message || "Failed to create user");
    };
    return await res.json();
  },

  async deleteUser(userId) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/users/${userId}`, {
      method: "DELETE"
    });
    if (!res.ok) throw new Error("Failed to delete user");
    return await res.json();
  },

  //===================================================================================================================================
  // == ROLES ==
  //=================================================================================================================================== 

  async getRoles() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/roles`);
    if (!res.ok) throw new Error("Failed to fetch roles");
    return res.json();
  },

  async createRole(roleData) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/roles`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(roleData),
    });
    if (!res.ok) {
      const error = await res.json();
      throw new Error(error.message || "Failed to create role");
    }
    return res.json();
  },

  async updateRole(roleId, roleData) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/roles/${roleId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(roleData),
    });
    if (!res.ok) {
      const errorData = await res.json();
      throw new Error(errorData.message || "Failed to update role");
    }
    return res.json();
  },

  async deleteRole(roleId) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/roles/${roleId}`, {
      method: "DELETE",
    });
    if (!res.ok) {
      const errorData = await res.json();
      throw new Error(errorData.message || "Failed to delete role");
    }
    return true;
  },

  async downloadRolesExcel() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/roles/export`, {
      method: "GET"
    });
    return res.blob();
  },

  //===================================================================================================================================
  // == DEVELOPERS ==
  //=================================================================================================================================== 

  async getDevelopers() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/developers`);
    if (!res.ok) throw new Error("Failed to fetch developers");
    return res.json();
  },

  async createDeveloper(dev) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/developers`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dev)
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.message || "Failed to create developer");
    }
    return res.json();
  },

  async updateDeveloper(id, dev) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/developers/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dev)
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.message || "Failed to update developer");
    }
    return res.json();
  },

  async deleteDeveloper(id) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/developers/${id}`, {
      method: "DELETE"
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.message || "Failed to delete developer");
    }
    return true;
  },

  async downloadDevelopersExcel() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/developers/export`, {
      method: "GET"
    });
    return res.blob();
  },

  //===================================================================================================================================
  // == APP TYPES ==
  //=================================================================================================================================== 

  async getAppTypes() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/apptypes`);
    return await res.json();
  },

  async createAppType(data) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/apptypes`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return await res.json();
  },

  async updateAppType(id, data) {
    await fetchWithAuth(`${ADMIN_API_BASE}/apptypes/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
  },

  async deleteAppType(id) {
    await fetchWithAuth(`${ADMIN_API_BASE}/apptypes/${id}`, {
      method: "DELETE",
    });
  },

  //===================================================================================================================================
  // == APPS ==
  //=================================================================================================================================== 

  async createApp(appData) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/apps`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(appData),
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.message || "Failed to create app");
    }

    return res.json();
  },

  async updateApp(appId, appData) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/apps/${appId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(appData),
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.message || "Failed to update app");
    }

    return res.json();
  },

  async deleteApp(appId) {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/apps/${appId}`, {
      method: "DELETE",
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.message || "Failed to delete app");
    }

    return true;
  },

  //===================================================================================================================================
  // == GENRES ==
  //=================================================================================================================================== 

  async getGenres() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/genres`);
    if (!res.ok) {
      throw new Error("Failed to fetch genres");
    }
    return await res.json();
  },

  //===================================================================================================================================
  // == PUBLISHERS ==
  //=================================================================================================================================== 

  async getPublishers() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/publishers`);
    if (!res.ok) throw new Error("Failed to fetch publishers");
    return await res.json();
  },

  //===================================================================================================================================
  // == POSTS ==
  //=================================================================================================================================== 

  async getPosts() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/posts`);
    if (!res.ok) throw new Error("Failed to fetch posts");
    return await res.json();
  },

  //===================================================================================================================================
  // == POST COMMENTS ==
  //=================================================================================================================================== 

  async getComments() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/comments`);
    if (!res.ok) throw new Error("Failed to fetch comments");
    return await res.json();
  },

  //===================================================================================================================================
  // == POST COMMENTS ==
  //=================================================================================================================================== 

  async getPostLikes() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/postlikes`);
    if (!res.ok) throw new Error("Failed to fetch post likes");
    return await res.json();
  },

  //===================================================================================================================================
  // == POST COMMENTS ==
  //=================================================================================================================================== 

  async getAppReviews() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/appreviews`);
    if (!res.ok) throw new Error("Failed to fetch app reviews");
    return await res.json();
  },

  //===================================================================================================================================
  // == APP IMAGES ==
  //=================================================================================================================================== 

  async getAppImages() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/appimages`);
    if (!res.ok) throw new Error("Failed to fetch app images");
    return await res.json();
  },

  //===================================================================================================================================
  // == APP VIDEOS ==
  //=================================================================================================================================== 

  async getAppVideos() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/appvideos`);
    if (!res.ok) throw new Error("Failed to fetch app videos");
    return await res.json();
  },

  //===================================================================================================================================
  // == CART ITEMS ==
  //=================================================================================================================================== 

  async getCartItems() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/cartitems`);
    if (!res.ok) throw new Error("Failed to fetch cart items.");
    return await res.json();
  },

  //===================================================================================================================================
  // == WISHLIST ITEMS ==
  //=================================================================================================================================== 

  async getWishlistItems() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/wishlist`);
    if (!res.ok) {
      throw new Error("Failed to fetch wishlist items");
    }
    return await res.json();
  },

  //===================================================================================================================================
  // == PURCHASE HISTORY ==
  //=================================================================================================================================== 

  async getPurchaseHistory() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/purchase-history`);
    if (!res.ok) throw new Error("Failed to fetch purchase history");
    return await res.json();
  },

  //===================================================================================================================================
  // == NOTIFICATIONS ==
  //=================================================================================================================================== 

  async getNotifications() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/notifications`);
    if (!res.ok) throw new Error("Failed to fetch notifications");
    return await res.json();
  },

  //===================================================================================================================================
  // == USER LIBRARIES ==
  //=================================================================================================================================== 

  async getUserLibraries() {
    const res = await fetchWithAuth(`${ADMIN_API_BASE}/libraries`);
    if (!res.ok) throw new Error("Failed to fetch user libraries");
    return await res.json();
  }
};

export default adminService;
