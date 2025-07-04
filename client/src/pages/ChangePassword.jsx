import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";

import userService from "../services/userService";

import "../styles/AccountDetails.css";
import "../styles/Auth.css"

const ChangePassword = () => {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [repeatPassword, setRepeatPassword] = useState("");
  const [error, setError] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");

    if (newPassword === currentPassword) {
      setError("New password must be different from the current password.");
      return;
    }

    if (newPassword !== repeatPassword) {
      setError("New passwords do not match.");
      return;
    }

    try {
      setLoading(true);
      await userService.changePassword(currentPassword, newPassword);
      setShowModal(true);
    } catch (err) {
      setError(err.message || "Failed to change password.");
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
      <h2 className="account-title">Change Password</h2>

      <div className="account-section">
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label>Current Password:</label>
            <input
              className="form-control"
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label>New Password:</label>
            <input
              className="form-control"
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label>Repeat New Password:</label>
            <input
              className="form-control"
              type="password"
              value={repeatPassword}
              onChange={(e) => setRepeatPassword(e.target.value)}
              required
            />
          </div>

          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading && <span className="spinner-in-button"/>}
            {loading ? "Changing..." : "Change Password"}
          </button>
        </form>

        <div className="forgot-password-container">
            <Link to="/forgot-password" className="forgot-password-link">
              Forgot your password?
            </Link>
          </div>


        {error && <p style={{ color: "#f55", marginTop: "10px" }}>{error}</p>}
      </div>

      {showModal && (
        <div className="modal-backdrop">
          <div className="modal-box">
            <h4>Password Changed</h4>
            <p>Your password has been updated successfully.</p>
            <button onClick={handleModalClose} className="btn btn-primary">
              OK
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChangePassword;
