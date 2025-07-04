import { useContext } from "react";
import { Link, useLocation } from "react-router-dom";

import UserDropdown from "./UserDropdown.jsx";
import NotificationDropdown from "./NotificationDropdown";

import AuthContext from "../context/AuthContext.jsx";

import "../styles/Navbar.css";
import vaporLogo from "../assets/vapor_nav.png";

const Navbar = () => {
  const { user } = useContext(AuthContext);
  const location = useLocation();

  return (
    <nav className="navbar navbar-expand-lg">
      <div className="container">

        <Link className="navbar-brand" to="/">
          <img
            src={vaporLogo}
            alt="Vapor Logo"
            className="navbar-logo"
          />
        </Link>
        <div className="collapse navbar-collapse">
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <Link className={`nav-link ${location.pathname === "/" ? "active" : ""}`} to="/">Store</Link>
            </li>
            {user && (
              <li className="nav-item">
                <Link className={`nav-link ${location.pathname === "/library" ? "active" : ""}`} to="/library">Library</Link>
              </li>
            )}
            <li className="nav-item">
              <Link className={`nav-link ${location.pathname === "/community" ? "active" : ""}`} to="/community">Community</Link>
            </li>
            <li className="nav-item">
              <Link className={`nav-link ${location.pathname === "/about" ? "active" : ""}`} to="/about">About</Link>
            </li>
          </ul>
          <ul className="navbar-nav">
            {user ? (
              <>
                <li className="nav-item me-2">
                  <NotificationDropdown />
                </li>
                <li className="nav-item">
                  <UserDropdown />
                </li>
              </>
            ) : (
              <li className="nav-item">
                <Link className="btn btn-primary" to="/login">Login</Link>
              </li>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
