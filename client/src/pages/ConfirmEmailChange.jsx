import { useEffect, useState, useRef, useContext } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";

import { jwtDecode } from "jwt-decode";

import AuthContext from "../context/AuthContext";
import userService from "../services/userService";
import authService from "../services/authService";

const ConfirmEmailChange = () => {
  const [searchParams] = useSearchParams();
  const [message, setMessage] = useState("Confirming...");
  const [success, setSuccess] = useState(false);
  const navigate = useNavigate();
  const hasRun = useRef(false);
  const { updateUserInfo } = useContext(AuthContext);

  useEffect(() => {
    const token = searchParams.get("token");
    if (!token) {
      setMessage("Invalid or missing token.");
      return;
    }

    if (hasRun.current) return;
    hasRun.current = true;

    const confirm = async () => {
      try {
        await userService.confirmEmailChange(token);

        const { token: newToken, refreshToken } = await authService.regenerateTokens();
        const decoded = jwtDecode(newToken);  

        updateUserInfo({ email: decoded?.email }); 

        setMessage("✅ Your email has been updated successfully.");
        setSuccess(true);
        setTimeout(() => navigate("/account"), 3000);
      } catch (err) {
        setMessage(`❌ ${err.message || "Email confirmation failed."}`);
      }
    };

    confirm();
  }, [searchParams, navigate, updateUserInfo]);

  return (
    <div className="account-container">
      <h2 className="account-title">Email Confirmation</h2>
      <div className="account-section">
        <p style={{ color: success ? "#4caf50" : "#f55" }}>{message}</p>
        {!success && (
          <button className="btn btn-secondary" onClick={() => navigate("/")}>
            Go to Home
          </button>
        )}
      </div>
    </div>
  );
};

export default ConfirmEmailChange;
