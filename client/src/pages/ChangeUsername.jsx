import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";

import AuthContext from "../context/AuthContext";
import userService from "../services/userService";
import authService from "../services/authService";

import "../styles/AccountDetails.css";
import "../styles/Auth.css";

const ChangeUsername = () => {
  const { updateUserInfo } = useContext(AuthContext);
  const [newUsername, setNewUsername] = useState("");
  const [error, setError] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      setLoading(true);
      await userService.changeUsername(newUsername);
      await authService.regenerateTokens();
      updateUserInfo({ username: newUsername });
      setShowModal(true);
    } catch (err) {
      setError(err.message || "Failed to change username.");
    } finally {
      setLoading(false);
    }
  };

  const handleModalClose = () => {
    setShowModal(false);
    navigate("/account");
  };

  return (
    <div className="account-container">
      <h2 className="account-title">Change Username</h2>

      <div className="account-section">
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label>New Username:</label>
            <input
              className="form-control"
              type="text"
              value={newUsername}
              onChange={(e) => setNewUsername(e.target.value)}
              required
            />
          </div>

          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading && <span className="spinner-in-button" />}
            {loading ? "Changing..." : "Change Username"}
          </button>

          {error && <p className="error-msg">❌ {error}</p>}
        </form>
      </div>

      {showModal && (
        <div className="modal-backdrop">
          <div className="modal-box">
            <h4>Username Changed</h4>
            <p>Your username has been updated successfully.</p>
            <button onClick={handleModalClose} className="btn btn-primary">
              OK
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChangeUsername;
