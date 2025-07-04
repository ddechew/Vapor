import { useState, useRef, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom"

import authService from "../services/authService";

import "../styles/Auth.css";

const ForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const submitted = useRef(false);
  const navigate = useNavigate();
  const isLoggedIn = !!localStorage.getItem("token");

  useEffect(() => {
    if (message) {
      const timeout = setTimeout(() => {
        navigate(isLoggedIn ? "/change-password" : "/login");
      }, 5000);
      return () => clearTimeout(timeout);
    }
  }, [message, navigate, isLoggedIn]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (submitted.current) return; 
    submitted.current = true;

    setError("");
    setMessage("");

    try {
      setLoading(true);
      await authService.requestPasswordReset(email);
      setMessage("✅ If this email exists, a reset link has been sent.");
    } catch (err) {
      setError("Something went wrong. Please try again.");
      setLoading(false);
    } finally {
      submitted.current = false;
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h2>Forgot Password</h2>

        {message && <p className="success">{message}</p>}
        {error && <p className="error">{error}</p>}

        <form onSubmit={handleSubmit}>
          <input
            type="email"
            placeholder="Email address"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
          <button className="btn btn-primary" type="submit" disabled={loading || !!message}>
            {loading ? (
              <>
                <span className="spinner-in-button" style={{ marginRight: "8px" }} />
                Sending...
              </>
            ) : (
              "Send Reset Link"
            )}
          </button>
        </form>

        <p className="back-to-login-text">
          {isLoggedIn ? "Want to return to password settings?" : "Remembered your password?"}{" "}
          <Link
            to={isLoggedIn ? "/change-password" : "/login"}
            className="back-to-login-link"
          >
            {isLoggedIn ? "Back to Change Password" : "Go back to Login"}
          </Link>
        </p>
      </div>
    </div>

  );
};

export default ForgotPassword;
