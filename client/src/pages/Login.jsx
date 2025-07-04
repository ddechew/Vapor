import { useState, useContext } from "react";
import { useNavigate, Link } from "react-router-dom";

import { GoogleOAuthProvider } from "@react-oauth/google";

import AuthContext from "../context/AuthContext";
import authService from "../services/authService";

import "../styles/Auth.css";

const Login = () => {
  const { login } = useContext(AuthContext);
  const navigate = useNavigate();
  const [formData, setFormData] = useState({ username: "", password: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const result = await authService.login(formData);

      if (!result) return;

      const { token, refreshToken } = result;
      login({ token, refreshToken });
      navigate("/");
    } catch (err) {
      const message = err.message || "Login failed.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <GoogleOAuthProvider clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID}>
      <div className="auth-container">
        <div className="auth-box">
          <h2>Login</h2>
          {error && <p className="error">{error}</p>}
          <form onSubmit={handleSubmit}>
            <input type="text" name="username" placeholder="Username" onChange={handleChange} required />
            <input type="password" name="password" placeholder="Password" onChange={handleChange} required />
            <button type="submit" disabled={loading}>
              {loading ? <span className="spinner-in-button" style={{ marginRight: "8px" }} /> : null}
              {loading ? "Logging in..." : "Login"}
            </button>
          </form>

          <div className="google-login-container">
            <button onClick={() => window.location.href = "https://localhost:7003/api/auth/google-login"}>
              Sign in with Google
            </button>

          </div>

          <Link to="/forgot-password" className="forgot-password-login-link">
            Forgot your password?
          </Link>
          
          <p>Don't have an account? <a href="/register">Register</a></p>
        </div>
      </div>
    </GoogleOAuthProvider>
  );
};

export default Login;
