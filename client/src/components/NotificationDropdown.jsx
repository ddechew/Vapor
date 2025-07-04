import { useEffect, useRef, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";

import { useNotifications } from "../context/NotificationContext";

import "../styles/NotificationDropdown.css"; 
import notificationIcon from "../assets/vapor_notifications.png";

const NotificationDropdown = () => {
    const { notifications, unreadCount, fetchNotifications } = useNotifications();
    const [open, setOpen] = useState(false);
    const dropdownRef = useRef(null);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
                setOpen(false);
            }
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleMarkAsRead = async (id) => {
        try {
            await import("../services/notificationService").then(({ default: ns }) =>
                ns.markAsRead(id)
            );
            fetchNotifications();
        } catch (err) {
            console.error("Failed to mark as read:", err);
        }
    };

    const handleClearAll = async () => {
        try {
            await import("../services/notificationService").then(({ default: ns }) =>
                ns.clearAll()
            );
            fetchNotifications();
        } catch (err) {
            console.error("Failed to clear notifications:", err);
        }
    };

    const scrollToPost = (postId) => {
        const url = `/community?postId=${postId}`;
        if (location.pathname === "/community") {
            navigate("/", { replace: true });
            setTimeout(() => navigate(url), 100);
        } else {
            navigate(url);
        }
    };

    return (
        <div className="notification-wrapper" ref={dropdownRef}>
            <div className="cart-link" onClick={() => setOpen(!open)}>
                <img src={notificationIcon} alt="Notifications" className="cart-icon" />
                {unreadCount > 0 && <span className="cart-count">({unreadCount})</span>}
            </div>

            {open && (
                <div className="notification-dropdown">
                    {notifications.length === 0 ? (
                        <div className="no-notifications">No notifications yet.</div>
                    ) : (
                        <>
                            <ul className="notification-list">
                                {notifications.map((n) => (
                                    <li key={n.notificationId}>
                                        {n.postId ? (
                                            <span className="notification-link" 
                                                    onClick={() => {
                                                    scrollToPost(n.postId);
                                                    // handleMarkAsRead(n.notificationId); 
                                                }}
                                            >
                                            {n.message} 
                                            </span>
                                        ) : (
                                            <span>{n.message}</span>
                                        )}
                                        <button onClick={() => handleMarkAsRead(n.notificationId)}>✖</button>
                                    </li>
                                ))}

                            </ul>
                            <div className="clear-all-btn-wrapper">
                                <button className="clear-all-btn" onClick={handleClearAll}>
                                    Clear All
                                </button>
                            </div>
                        </>
                    )}
                </div>
            )}
        </div>
    );
};

export default NotificationDropdown;