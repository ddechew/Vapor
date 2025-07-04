import React, { createContext, useContext, useState, useEffect } from "react";
import wishlistService from "../services/wishlistService";
import AuthContext from "./AuthContext";

const WishlistContext = createContext();

export const WishlistProvider = ({ children }) => {
  const [wishlist, setWishlist] = useState([]);
  const { user } = useContext(AuthContext);

  useEffect(() => {
    const fetchWishlist = async () => {
      if (user) {
        try {
          const data = await wishlistService.getWishlist(user.userId);
          setWishlist(data);
        } catch (err) {
          console.error("Failed to load wishlist:", err);
        }
      } else {
        setWishlist([]);
      }
    };

    fetchWishlist();
  }, [user]);

  const addToWishlist = (app) => {
    setWishlist((prev) => [...prev, app]);
  };

  const removeFromWishlist = (appId) => {
    setWishlist((prev) => prev.filter((item) => item.appId !== appId));
  };

  const clearWishlist = () => setWishlist([]);

  return (
    <WishlistContext.Provider
      value={{ wishlist, setWishlist, addToWishlist, removeFromWishlist, clearWishlist }}
    >
      {children}
    </WishlistContext.Provider>
  );
};

export const useWishlist = () => useContext(WishlistContext);
