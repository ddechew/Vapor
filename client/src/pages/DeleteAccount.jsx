import { useEffect, useState, useRef } from "react";
import authService from "../services/authService";

const DeleteAccount = () => {
  const [message, setMessage] = useState("Processing account deletion...");
  const hasRun = useRef(false);

  useEffect(() => {
    if (hasRun.current) return;
    hasRun.current = true;

    const token = new URLSearchParams(window.location.search).get("token");
    
    if (!token) {
      setMessage("❌ Invalid or missing token.");
      return;
    }

    authService
      .deleteAccountByToken(token)
      .then((result) => {
        setMessage(result);
      })
      .catch((err) => {
        setMessage(`❌ ${err.message}`);
      });
  }, []);

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h2>Account Deletion</h2>
        <p>{message}</p>
        <a href="/register">Register a new account</a>
      </div>
    </div>
  );
};

export default DeleteAccount;
