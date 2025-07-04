import { useContext, useEffect, useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import LoadingSpinner from "../components/LoadingSpinner";

import AuthContext from "../context/AuthContext";
import { useCart } from "../context/CartContext";

import userService from "../services/userService";
import wishlistService from "../services/wishlistService";
import cartService from "../services/cartService";
import stripeService from "../services/stripeService";
import authService from "../services/authService";

import '../styles/Cart.css'

const Cart = () => {
  const { cart, setCart, removeFromCart, clearCart } = useCart();
  const { user, updateUserInfo } = useContext(AuthContext);
  const navigate = useNavigate();
  const location = useLocation();
  const [walletBalance, setWalletBalance] = useState(0);
  const [showStripeConfirm, setShowStripeConfirm] = useState(false);
  const [stripeHandled, setStripeHandled] = useState(false);
  const [selectedDiscount, setSelectedDiscount] = useState(null);
  const [loading, setLoading] = useState(false);



  const DISCOUNT_TIERS = [
    { points: 100, percent: 10 },
    { points: 250, percent: 25 },
    { points: 500, percent: 50 }
  ];

  const safePointsToUse = DISCOUNT_TIERS.find(t => t.points === selectedDiscount?.points && user?.points >= t.points)?.points || 0;

  useEffect(() => {
    const query = new URLSearchParams(location.search);
    const success = query.get("success") === "true";
    const sessionId = query.get("session_id");
    const autoPurchase = query.get("autoPurchase") === "true";
    const pointsUsed = parseInt(query.get("pointsToUse") || "0");

    if (success && autoPurchase && sessionId && user && !stripeHandled) {
      const processStripeReturn = async () => {
        setLoading(true);
        try {
          const { message } = await stripeService.confirmWalletPayment(sessionId);
          if (message === "Expired session. Please try again.") {
            alert("⏳ Your Stripe payment session expired. Please try again.");
            setStripeHandled(true);
            return;
          }

          if (message !== "Wallet updated.") {
            alert("❌ Payment failed: " + message);
            setStripeHandled(true);
            return;
          }
          await userService.purchaseWithWallet(pointsUsed);
          const updatedUser = await userService.getUserById(user.userId);
          updateUserInfo(updatedUser);
          clearCart();
          setStripeHandled(true);
          alert("✅ Wallet topped up and apps purchased!");

          try {
            wishlistService.normalize();
          } catch (normalizeErr) {
            console.warn("⚠ Wishlist normalization failed:", normalizeErr);
          }

          try {
            await authService.regenerateTokens();
          } catch {
            console.warn("⚠ Token regeneration failed after Stripe.");
          }

        } catch (err) {
          console.error("❌ Stripe return failed:", err);
          alert("Something went wrong after payment.");
        } finally {
          setLoading(false);
        }
      };


      processStripeReturn();
    }
  }, [user, location, stripeHandled]);

  useEffect(() => {
    const query = new URLSearchParams(location.search);
    const canceled = query.get("canceled") === "true";
    const sessionId = query.get("session_id");

    if (canceled && sessionId && user) {
      stripeService.cancelStripeSession(sessionId)
        .then(() => console.log("🧹 Stripe session cleanup complete."))
        .catch((err) => console.error("❌ Failed to clean up canceled session:", err));
    }
  }, [location, user]);

  useEffect(() => {
    const loadUserCart = async () => {
      if (user) {
        try {
          const dbCart = await cartService.getCart(user.userId);
          setCart(dbCart);
          const userInfo = await userService.getUserById(user.userId);
          setWalletBalance(parseFloat(userInfo.wallet || 0));
        } catch (err) {
          console.error("Failed to load user cart:", err);
        }
      }
    };

    loadUserCart();
  }, [user, setCart]);

  const handleRemove = async (appId) => {
    const app = cart.find(a => a.appId === appId);
    if (!app) return;

    if (user) await cartService.removeFromCart(user.userId, appId);
    removeFromCart(appId);

    const relatedApps = cart.filter(a => a.baseAppId === appId);
    for (const dlc of relatedApps) {
      if (user) await cartService.removeFromCart(user.userId, dlc.appId);
      removeFromCart(dlc.appId);
    }
  };

  const handleClear = async () => {
    if (user) {
      await cartService.clearCart(user.userId);
    }
    clearCart();
  };

  const originalTotal = cart.reduce((sum, app) => sum + parseFloat(app.price || 0), 0);
  const totalPrice = selectedDiscount && user?.points >= selectedDiscount.points
    ? originalTotal * (1 - selectedDiscount.percent / 100)
    : originalTotal;
  const roundedWallet = parseFloat(walletBalance.toFixed(2));
  const roundedTotal = parseFloat(totalPrice.toFixed(2));
  const roundedRemaining = Math.max(roundedTotal - roundedWallet, 0);
  const displayRemainingAmount = roundedRemaining.toFixed(2);

  const handleWalletPayment = async () => {
    setLoading(true);
    try {
      await userService.purchaseWithWallet(safePointsToUse);
      const updatedUser = await userService.getUserById(user.userId);
      updateUserInfo(updatedUser);
      await wishlistService.normalize();
      try {
        await authService.regenerateTokens();
      } catch (error) {
        console.warn("⚠ Token regeneration failed after purchase. Will use fetched user data instead.");
      }

      clearCart();
      alert("Purchase successful using wallet!");
    } catch (err) {
      console.error("Payment error:", err);
      alert("Failed to process payment.");
    } finally {
      setLoading(false);
    }
  };

  const proceedToStripe = async () => {
    try {
      const session = await stripeService.createStripeSession(
        roundedRemaining,
        user.userId,
        true,
        selectedDiscount?.points || 0
      );
      window.location.href = session.sessionUrl;
    } catch (err) {
      console.error("Stripe error:", err);
      alert("Stripe payment failed.");
    }
  };

  return (
    <div className="container mt-4">
      <h2>Your Cart</h2>
      {loading && <LoadingSpinner />}
      {cart.length === 0 ? (
        <p>Your cart is empty.</p>
      ) : (
        <>
          <div className="cart-list">
            {cart.map(app => (
              <div className="cart-item" key={app.appId}>
                <img src={app.headerImage} alt={app.name} className="cart-thumb" />
                <div className="cart-details">
                  <Link to={`/appid/${app.appId}`} className="cart-name">{app.name}</Link>
                  {selectedDiscount && user?.points >= selectedDiscount.points ? (
                    <p className="cart-price">
                      <del>{parseFloat(app.price).toFixed(2)}€</del>{" "}
                      <span className="discounted-price">
                        {(parseFloat(app.price) * (1 - selectedDiscount.percent / 100)).toFixed(2)}€
                      </span>
                    </p>
                  ) : (
                    <p className="cart-price">
                      {app.price === "0" ? "Free" : `${parseFloat(app.price).toFixed(2)}€`}
                    </p>
                  )}

                  <button onClick={() => handleRemove(app.appId)} className="cart-remove">Remove</button>
                </div>
              </div>
            ))}
          </div>

          <button onClick={handleClear} className="clear-cart-btn">Clear Cart</button>

          <div style={{ marginTop: "30px" }}>
            <div className="cart-summary">
              <h4>
                Total:{" "}
                {selectedDiscount && user?.points >= selectedDiscount.points ? (
                  <>
                    <del>{originalTotal.toFixed(2)} €</del>{" "}
                    <span className="text-success">{totalPrice.toFixed(2)} €</span>
                  </>
                ) : (
                  `${totalPrice.toFixed(2)} €`
                )}
              </h4>

              <h5>You currently have <strong>{user?.points || 0}</strong> points</h5>

              {cart.length > 0 && user?.points >= 100 && (
                <div className="form-check mt-3">
                  <p><strong>Use points for discount:</strong></p>
                  {DISCOUNT_TIERS.map(tier => (
                    <div key={tier.points} className="form-check">
                      <input
                        className="form-check-input"
                        type="radio"
                        name="discountTier"
                        id={`discount-${tier.points}`}
                        value={tier.points}
                        disabled={user.points < tier.points}
                        checked={selectedDiscount?.points === tier.points}
                        onChange={() => setSelectedDiscount(tier)}
                      />
                      <label className="form-check-label" htmlFor={`discount-${tier.points}`}>
                        Use {tier.points} points for {tier.percent}% discount
                      </label>
                    </div>
                  ))}
                  <div className="mt-2">
                    {selectedDiscount && (
                      <button
                        className="btn btn-sm btn-outline-secondary"
                        onClick={() => setSelectedDiscount(null)}
                      >
                        Clear Discount Selection
                      </button>
                    )}
                  </div>
                </div>
              )}

              {roundedWallet >= roundedTotal ? (
                <button className="btn btn-success mt-3" onClick={handleWalletPayment}>
                  Pay with Wallet
                </button>
              ) : (
                <>
                  <p className="text-danger">
                    Insufficient funds. You need <strong>{displayRemainingAmount} €</strong> more.
                  </p>
                  <button className="btn btn-warning" onClick={() => setShowStripeConfirm(true)}>
                    Add Funds
                  </button>
                  <button
                    className="btn btn-outline-info"
                    onClick={() => navigate("/wallet")}
                  >
                    View Wallet
                  </button>
                </>
              )}
            </div>

          </div>
        </>
      )}

      {showStripeConfirm && (
        <div className="cart-modal-overlay">
          <div className="cart-modal">
            <h3>Complete Purchase via Stripe</h3>
            <p>
              You don’t have enough wallet balance. You’ll be redirected to Stripe to add <strong>€{displayRemainingAmount}</strong> and complete your transaction.
            </p>
            <div className="modal-actions">
              <button className="modal-btn checkout-btn" onClick={proceedToStripe}>
                Proceed to Stripe
              </button>
              <button className="modal-btn" onClick={() => setShowStripeConfirm(false)}>
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
    ;
};

export default Cart;
