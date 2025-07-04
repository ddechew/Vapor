import { useEffect, useState, useRef } from "react";
import authService from "../services/authService";
import "../styles/Auth.css";

const VerifyEmail = () => {
  const [message, setMessage] = useState("");
  const [expired, setExpired] = useState(false);
  const [resendDisabled, setResendDisabled] = useState(false);
  const [loading, setLoading] = useState(true);
  const [email, setEmail] = useState("");
  const hasRunRef = useRef(false);

  useEffect(() => {
    if (hasRunRef.current) return;
    hasRunRef.current = true;

    const params = new URLSearchParams(window.location.search);
    const token = params.get("token");
    const unverified = params.get("unverified");

    if (token) {
      authService.verifyEmail(token)
        .then((text) => {
          if (text.startsWith("✅")) {
            setMessage("✅ Your email has been verified! You can now log in.");
          } else if (text.includes("expired")) {
            setMessage(`❌ ${text}`);
            setExpired(true);
          } else {
            setMessage(`❌ ${text}`);
          }
        })
        .catch(() => {
          setMessage("❌ Something went wrong during verification.");
        })
        .finally(() => setLoading(false));
    } else if (unverified) {
      try {
        const base64 = decodeURIComponent(unverified);
        const decoded = atob(base64);
        setEmail(decoded);
        setExpired(true);
        setMessage("❌ Your email is not verified. You can resend the verification email.");
      } catch (err) {
        console.error("Invalid base64 email in URL:", err);
        setMessage("❌ Invalid verification link.");
      } finally {
        setLoading(false);
      }
    } else {
      setMessage("❌ Invalid verification link.");
      setLoading(false);
    }
  }, []);

  const resendVerification = async () => {
    try {
      setResendDisabled(true);
      const params = new URLSearchParams(window.location.search);
      const token = params.get("token");
      const emailParam = params.get("unverified");
  
      if (token) {
        await authService.resendVerificationByToken(token);
      } else if (emailParam) {
        const base64 = decodeURIComponent(emailParam);
        const decoded = atob(base64);
        await authService.resendVerification(decoded);
      } else {
        setMessage("❌ Missing verification context.");
        return;
      }
  
      setMessage("✅ A new verification email has been sent.");
      setExpired(false);
    } catch (err) {
      const text = err.message;
      setMessage(text.includes("Please wait") ? `⏳ ${text}` : "❌ Failed to resend verification email.");
    } finally {
      setTimeout(() => setResendDisabled(false), 5000);
    }
  };  

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h2>Email Verification</h2>

        {loading ? (
          <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "60px" }}>
            <span className="spinner-in-button" />
          </div>
        ) : (
          <>
            <p>{message}</p>
            {expired && (
              <button onClick={resendVerification} disabled={resendDisabled}>
                {resendDisabled ? (
                  <span className="spinner-in-button" style={{ marginRight: "8px" }} />
                ) : null}
                {resendDisabled ? "Sending..." : "Resend Verification Email"}
              </button>
            )}
            <a href="/login">Go to Login</a>
          </>
        )}
      </div>
    </div>
  );
};

export default VerifyEmail;
