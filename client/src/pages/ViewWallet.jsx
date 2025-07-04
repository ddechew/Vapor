import { useContext, useEffect, useRef } from "react";
import { useNavigate, useLocation } from "react-router-dom";

import AuthContext from "../context/AuthContext";
import authService from "../services/authService";
import userService from "../services/userService";
import stripeService from "../services/stripeService";

import "../styles/ViewWallet.css";

const walletOptions = [5, 10, 25, 50, 100];

const ViewWallet = () => {
  const navigate = useNavigate();
  const { user, updateUserInfo } = useContext(AuthContext);
  const hasConfirmed = useRef(false);

  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const success = query.get("success") === "true";
  const sessionId = query.get("session_id");

  useEffect(() => {
    if (!user || !success || !sessionId || hasConfirmed.current) return;

    hasConfirmed.current = true;

    stripeService.confirmWalletPayment(sessionId)
      .then(async () => {
        const userToUpdate = await userService.getUserById(user.userId);
        updateUserInfo(userToUpdate);
        await authService.regenerateTokens();
        alert("Wallet successfully updated!");
        navigate("/wallet", { replace: true });
      })
      .catch((err) => {
        console.error("Wallet update failed:", err);
      });
  }, [user, success, sessionId]);

  useEffect(() => {
    const canceled = query.get("canceled") === "true";
    const sessionId = query.get("session_id");

    if (canceled && sessionId && user) {
      stripeService.cancelStripeSession(sessionId)
        .then(() => console.log("🧹 Stripe session cleanup complete (wallet)."))
        .catch((err) => console.error("❌ Failed to clean up canceled wallet session:", err));
    }
  }, [user, location]);


  const handleAddFunds = async (amount) => {
    try {
      const { sessionUrl } = await stripeService.createStripeSession(amount, user.userId);
      window.location.href = sessionUrl; 
    } catch (err) {
      console.error("Stripe session creation failed:", err.message);
      alert("Failed to start payment session.");
    }
  };

  return (
    <div className="wallet-page">
      <div className="wallet-main">
        <h1>Add funds to your Wallet</h1>
        <p className="wallet-description">
          Funds in your Vapor Wallet may be used for purchasing games or other digital content. You will have a chance to review your order before it's placed.
        </p>

        <div className="wallet-options">
          {walletOptions.map((amount) => (
            <div key={amount} className="wallet-card">
              <div className="wallet-card-info">
                <h2>Add {amount},--€</h2>
                <span className="wallet-note">
                  {amount === 5 ? "Minimum fund level" : ""}
                </span>
              </div>
              <div className="wallet-card-actions">
                <span className="wallet-price">{amount},--€</span>
                <button onClick={() => handleAddFunds(amount)}>Add funds</button>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="wallet-sidebar">
        <h3>YOUR VAPOR ACCOUNT</h3>
        <div className="wallet-balance">
          <span>Current Wallet balance</span>
          <span className="balance-amount">€{user?.wallet?.toFixed(2) || "0.00"}</span> {/* TODO: Dynamic */}
        </div>
        <div className="wallet-points">
          <span>Points:</span>
          <span className="points-amount" title="Earn 1 point for every 1€ spent. Redeem for discounts!">
            {user?.points || 0}
          </span>
        </div>
        <button onClick={() => navigate("/account")}>See my account details</button>
      </div>
    </div>
  );

};

export default ViewWallet;