import { useState } from "react";
import { useNavigate } from "react-router-dom";

import authService from "../services/authService";

import "../styles/Auth.css";

const Register = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    username: "",
    displayName: "",
    email: "",
    password: "",
    repeatPassword: ""
  });
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (formData.password !== formData.repeatPassword) {
      setError("Passwords do not match!");
      return;
    }

    setLoading(true);

    try {
      await authService.register(formData);
      setSuccess("✅ Registration successful! Please check your email and verify your account before logging in.");
      setTimeout(() => navigate("/login"), 5000);
    } catch (err) {
      setError(err.message);
      setLoading(false);
    } 
  };

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h2>Register</h2>
        {error && <p className="error">{error}</p>}
        {success && <p className="success">{success}</p>}
        <form onSubmit={handleSubmit}>
          <input type="text" name="username" placeholder="Username" onChange={handleChange} required />
          <input type="text" name="displayName" placeholder="Display Name" onChange={handleChange} required />
          <input type="email" name="email" placeholder="Email" onChange={handleChange} required />
          <input type="password" name="password" placeholder="Password" onChange={handleChange} required />
          <input type="password" name="repeatPassword" placeholder="Repeat Password" onChange={handleChange} required />
          <button type="submit" disabled={loading}>
            {loading ? <span className="spinner-in-button" /> : "Register"}
          </button>
        </form>
        <div style={{ marginTop: "20px" }}>
          <p>
            Already have an account? <a href="/login">Login</a>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
