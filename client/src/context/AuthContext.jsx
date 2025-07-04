import { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

import { useCart } from "../context/CartContext";

import authService from "../services/authService";
import cartService from "../services/cartService";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);
  const { setCart } = useCart(); 

  const loadUserCart = async (userId) => {
    try {
      const cartFromDb = await cartService.getCart(userId);
      setCart(cartFromDb);
    } catch (err) {
      console.error("Failed to load cart after login", err);
    }
  };

  const updateUserInfo = (updated) => {
    setUser(prev => {
      if (!prev) return prev;
      return {
        ...prev,
        email: updated.email ?? prev.email,
        wallet: updated.wallet ?? prev.wallet,
        points: updated.points ?? prev.points,
        displayName: updated.displayName ?? prev.displayName,
        profilePicture: updated.profilePicture ?? prev.profilePicture,
        role: updated.role ?? prev.role,
        username: updated.username ?? prev.username,
        isGoogleAuthenticated: updated.isGoogleAuthenticated ?? prev.isGoogleAuthenticated
      };
    });
  };

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      try {
        const decoded = jwtDecode(token);
        const userId = decoded?.nameid;
        const username = decoded?.name;
        const email = decoded?.email;
        const displayName = decoded?.displayName;
        const role = decoded?.role;
        const wallet = parseFloat(decoded?.wallet || 0);
        const profilePicture = decoded?.profilePicture;
        const points = parseInt(decoded?.points || 0);
        const isGoogleAuthenticated = decoded?.isGoogleAuthenticated === "true";
        setUser({
          token,
          userId,
          username,
          email,
          displayName,
          wallet,
          profilePicture,
          points,
          role,
          isGoogleAuthenticated
        });
        setIsAuthenticated(true);
        loadUserCart(userId);
      } catch (err) {
        console.error("Invalid token on load");
        localStorage.removeItem("token");
        setUser(null);
        setIsAuthenticated(false);
      }
    }
    setLoading(false)
  }, []);


  const login = async ({ token, refreshToken }) => {
    try {
      const decoded = jwtDecode(token);
      const userId = decoded?.nameid;

      if (!userId) throw new Error("userId missing");

      localStorage.setItem("token", token);
      localStorage.setItem("refreshToken", refreshToken); 

      setUser({
        token,
        userId,
        username: decoded?.name,
        email: decoded?.email,
        displayName: decoded?.displayName,
        wallet: parseFloat(decoded?.wallet || 0),
        role: decoded.role,
        profilePicture: decoded?.profilePicture,
        points: parseInt(decoded?.points),
        isGoogleAuthenticated: decoded?.isGoogleAuthenticated === "true"
      });

      setIsAuthenticated(true);

      const guestCart = JSON.parse(localStorage.getItem("cart")) || [];
      const guestAppIds = guestCart.map((item) => item.appId);

      if (guestAppIds.length > 0) {
        await cartService.mergeGuestCart(userId, guestAppIds);
        localStorage.removeItem("cart");
      }

      await loadUserCart(userId);
    } catch (err) {
      console.error("Login error:", err);
    }
  };

  const logout = async () => {
    try {
      await authService.logout(); 
    } catch (err) {
      console.warn("Logout request failed:", err);
    }
    setUser(null);
    setIsAuthenticated(false);
    setCart([]);
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, login, logout, loading, updateUserInfo }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;
