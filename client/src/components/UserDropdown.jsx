import { useState, useContext, useRef, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";

import AuthContext from "../context/AuthContext";

import "../styles/UserDropdown.css";

const UserDropdown = () => {
  const { user, logout } = useContext(AuthContext);
  const [open, setOpen] = useState(false);
  const dropdownRef = useRef(null);
  const navigate = useNavigate();

  
  const handleLogout = async () => {
    await logout();   
    navigate("/login"); 
  };

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  if (!user) return null;

  return (
    <div className="user-dropdown" ref={dropdownRef}>
      <div className="user-toggle" onClick={() => setOpen(!open)}>
        <img
          src={user.profilePicture}
          alt="Profile"
          className="avatar"
        />
        <span>{user.displayName || "User"}</span>
        <span className="wallet">€{user.wallet?.toFixed(2) || "0.00"}</span>
      </div>

      {open && (
        <div className="dropdown-menu">
          <Link to={`/profile/${user.username}`}>View my profile</Link>
          <Link to="/account">
            Account details: <span style={{ color: "#00c0ff" }}>{user.username}</span>
          </Link>
          <Link to="/wallet">View my wallet</Link>
          {user.role === "Admin" && (
            <Link to="/admin" className="admin-link">Admin Panel</Link>
          )}
          <hr />
          <button onClick={handleLogout}>Sign out</button>
        </div>
      )}
    </div>
  );
};

export default UserDropdown;
