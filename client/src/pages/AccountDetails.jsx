import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";

import AuthContext from "../context/AuthContext";
import authService from "../services/authService";
import userService from "../services/userService";

import "../styles/AccountDetails.css";

const AccountDetails = () => {
  const { user, updateUserInfo } = useContext(AuthContext);
  const [unlinking, setUnlinking] = useState(false);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const obfuscateEmail = (email) => {
    const [name, domain] = email.split("@");
    if (!name || !domain) return email;

    const visibleChars = 3;
    const hiddenPart = "*".repeat(Math.max(name.length - visibleChars, 2));
    return `${name.slice(0, visibleChars)}${hiddenPart}@${domain}`;
  };

  const handleUnlinkGoogle = async () => {

    setUnlinking(true);
    setError("");

    try {
      await userService.unlinkGoogle();
      await authService.regenerateTokens(); 
      updateUserInfo({ isGoogleAuthenticated: false });
      alert("✅ Google account unlinked.");
    } catch (err) {
      setError(err.message || "Failed to unlink.");
    } finally {
      setUnlinking(false);
    }
  };

  return (
    <div className="account-container">
      <h2 className="account-title">
        Account Details: <span>{user.username}</span>
      </h2>

      <div className="account-section">
        <h4>Wallet Balance</h4>
        <p className="wallet-amount">€{user.wallet?.toFixed(2)}</p>
        <div className="funds-button">
          <a href="/wallet">+ Add funds to your wallet</a>
        </div>
      </div>

      <div className="account-section contact-info">
        <h4>Contact Info</h4>
        <p><span>Email:</span> {obfuscateEmail(user.email)}</p>
      </div>

      <div className="account-section settings-buttons">
        <h4>Settings</h4>
        <button onClick={() => navigate("/purchase-history")}>
          View Purchase History
        </button>
        <button onClick={() => navigate("/change-username")}>Change Username</button>
        <button onClick={() => navigate("/change-password")}>Change Password</button>
        {!user.isGoogleAuthenticated && (
          <button onClick={() => navigate("/change-email")}>Change Email</button>
        )}
        {user.email?.endsWith("@gmail.com") && user.isGoogleAuthenticated && (
          <button
            onClick={handleUnlinkGoogle}
            className="btn btn-secondary"
            disabled={unlinking}
          >
            {unlinking && <span className="spinner-in-button" />}
            {unlinking ? "Unlinking..." : "Unlink Google Account"}
          </button>
        )}
        {error && <p className="error-msg" style={{ marginTop: "10px" }}>❌ {error}</p>}

      </div>

    </div>
  );
};

export default AccountDetails;
