import { useNavigate } from "react-router-dom";

import "../styles/Footer.css";
import vaporLogo from "../assets/vapor_nav.png";

const Footer = () => {
  const navigate = useNavigate();

  const handleNavigation = (path) => {
    navigate(path);
    setTimeout(() => {
      window.scrollTo({ top: 0, behavior: "smooth" });
    }, 0);
  };

  const handleLogoClick = () => {
    handleNavigation("/");
  };

  return (
    <footer className="footer">
      <div className="footer-content">
        <button className="footer-logo-link" onClick={handleLogoClick}>
          <img src={vaporLogo} alt="Vapor Logo" className="footer-logo" />
        </button>
        <p>© 2025 Vapor. All rights reserved.</p>
        <p>All trademarks are property of their respective owners.</p>
        <div className="footer-links">
          <button onClick={() => handleNavigation("/privacy-policy")} className="footer-link-btn">
            Privacy Policy
          </button>{" "}
          |{" "}
          <button onClick={() => handleNavigation("/terms-of-service")} className="footer-link-btn">
            Terms of Service
          </button>{" "}
          |{" "}
          <button onClick={() => handleNavigation("/refund-policy")} className="footer-link-btn">
            Refund Policy
          </button>{" "}
          |{" "}
          <button onClick={() => handleNavigation("/support")} className="footer-link-btn">
            Support
          </button>{" "}
          |{" "}
          <button onClick={() => handleNavigation("/about")} className="footer-link-btn">
            About Us
          </button>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
