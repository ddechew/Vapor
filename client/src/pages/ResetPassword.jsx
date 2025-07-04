import { useState, useContext } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";

import AuthContext from "../context/AuthContext";
import authService from "../services/authService";

import "../styles/Auth.css";

const ResetPassword = () => {
  const { token } = useParams();
  const [newPassword, setNewPassword] = useState("");
  const [repeatPassword, setRepeatPassword] = useState("");
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { logout, isAuthenticated } = useContext(AuthContext);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setMessage("");

    if (newPassword !== repeatPassword) {
      setError("Passwords do not match.");
      return;
    }

    try {
      setLoading(true);
      await authService.resetPassword(token, newPassword);
      setMessage("✅ Password reset successfully. Redirecting to login...");
      setTimeout(() => navigate("/login"), 3000);
      await logout();
    } catch (err) {
      setError(err.message || "Reset failed. Try again.");
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h2>Reset Password</h2>
        <form onSubmit={handleSubmit}>
          <input
            type="password"
            placeholder="New Password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Repeat Password"
            value={repeatPassword}
            onChange={(e) => setRepeatPassword(e.target.value)}
            required
          />
          <button type="submit" disabled={loading}>
            {loading ? <span className="spinner-in-button" /> : "Reset Password"}
          </button>
          {message && <p className="success">{message}</p>}
          {error && <p className="error">{error}</p>}
        </form>
        <div className="back-to-login-text">
          {isAuthenticated ? "Return to password settings?" : "Remembered your password?"}{" "}
          <Link className="back-to-login-link" to={isAuthenticated ? "/change-password" : "/login"}>
            {isAuthenticated ? "Back to Change Password" : "Go back to Login"}
          </Link>
        </div>
      </div>
    </div>
  );
};

export default ResetPassword;
