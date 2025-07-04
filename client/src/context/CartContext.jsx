import { createContext, useContext, useEffect, useState } from "react";
import cartService from "../services/cartService";

const CartContext = createContext();

export const CartProvider = ({ children }) => {
  const [cart, setCart] = useState(() => {
    const stored = localStorage.getItem("cart");
    return stored ? JSON.parse(stored) : [];
  });

  useEffect(() => {
    localStorage.setItem("cart", JSON.stringify(cart));
  }, [cart]);

  const addToCart = async (app, user) => {
    const alreadyExists = cart.some(item => item.appId === app.appId);
    if (!alreadyExists) {
      setCart((prev) => [...prev, app]);
  
      if (user) {
        try {
          await cartService.addToCart(user.userId, app.appId); 
        } catch (err) {
          console.error("Failed to sync to DB cart:", err);
        }
      }
    }
  };

  const removeFromCart = (appId) => {
    setCart((prev) => prev.filter(item => item.appId !== appId));
  };

  const clearCart = () => setCart([]);

  return (
    <CartContext.Provider value={{ cart, setCart, addToCart, removeFromCart, clearCart }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => useContext(CartContext);
