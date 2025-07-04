import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";

import AuthContext from "../context/AuthContext";
import userService from "../services/userService";

import "../styles/AccountDetails.css";
import "../styles/Auth.css";

const ChangeEmail = () => {
  const [newEmail, setNewEmail] = useState("");
  const [confirmEmail, setConfirmEmail] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const { updateUserInfo } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (newEmail !== confirmEmail) {
      setError("Emails do not match.");
      return;
    }

    try {
      setLoading(true);
      await userService.initiateEmailChange(newEmail);
      setShowModal(true);
    } catch (err) {
      setError(err.message || "Failed to initiate email change.");
      setLoading(false);
    } 
  };

  const handleModalClose = () => {
    setShowModal(false);
    navigate("/account");
  };

  return (
    <div className="account-container">
      <h2 className="account-title">Change Email</h2>
      <div className="account-section">
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label>New Email:</label>
            <input
              className="form-control"
              type="email"
              value={newEmail}
              onChange={(e) => setNewEmail(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label>Confirm New Email:</label>
            <input
              className="form-control"
              type="email"
              value={confirmEmail}
              onChange={(e) => setConfirmEmail(e.target.value)}
              required
            />
          </div>

          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading && <span className="spinner-in-button" />}
            {loading ? "Sending..." : "Update Email"}
          </button>

          {error && <p className="error-msg">❌ {error}</p>}
        </form>
      </div>

      {showModal && (
        <div className="modal-backdrop">
          <div className="modal-box">
            <h4>Confirmation Email Sent</h4>
            <p>Please check your new email inbox to confirm the address change.</p>
            <button onClick={handleModalClose} className="btn btn-primary">
              OK
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChangeEmail;
