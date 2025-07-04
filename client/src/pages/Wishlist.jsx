import { useEffect, useState, useContext } from "react";
import { Link } from "react-router-dom";

import AuthContext from "../context/AuthContext";
import { useWishlist } from "../context/WishlistContext";
import wishlistService from "../services/wishlistService";

import "../styles/Wishlist.css";

const Wishlist = () => {
    const { user } = useContext(AuthContext);
    const { wishlist, setWishlist } = useWishlist();
    const [loading, setLoading] = useState(true);

    const fetchWishlist = async () => {
        try {
            const data = await wishlistService.getWishlist();
            setWishlist(data);
        } catch (err) {
            console.error(err);
            alert("Failed to load wishlist.");
        } finally {
            setLoading(false);
        }
    };

    const handleRemove = async (appId) => {
        try {
            await wishlistService.removeFromWishlist(appId);
            setWishlist((prev) => prev.filter((a) => a.appId !== appId));
            await fetchWishlist();
        } catch (err) {
            alert("Failed to remove from wishlist.");
        }
    };

    const handlePriorityChange = async (appId, delta) => {
        const item = wishlist.find((w) => w.appId === appId);
        if (!item) return;

        const newPriority = item.priority + delta;
        const maxPriority = wishlist.length;

        if (newPriority < 1 || newPriority > maxPriority) return;

        try {
            await wishlistService.updatePriority(appId, newPriority);
            await fetchWishlist();
        } catch (err) {
            alert("Failed to update priority.");
        }
    };


    useEffect(() => {
        if (user) fetchWishlist();
    }, [user]);

    if (!user) return <p>Please log in to view your wishlist.</p>;

    return (
        <div className="container mt-4 wishlist-page">
            <h2>Your Wishlist</h2>
            {loading ? (
                <p>Loading...</p>
            ) : wishlist.length === 0 ? (
                <p>Your wishlist is empty.</p>
            ) : (
                <div className="wishlist-list">
                    {wishlist.map((app) => (
                        <div key={app.appId} className="wishlist-card">
                            <Link to={`/appid/${app.appId}`}>
                                <img
                                    src={app.headerImage}
                                    alt={app.name}
                                    className="wishlist-image"
                                />
                            </Link>

                            <div className="wishlist-info">
                                <Link to={`/appid/${app.appId}`} className="wishlist-title">
                                    {app.name}
                                </Link>
                                <p className="wishlist-price">{app.price}</p>
                                <div className="wishlist-controls">
                                    <button onClick={() => handlePriorityChange(app.appId, -1)}>-</button>
                                    <input
                                        type="text"
                                        value={app.priority}
                                        className="priority-input"
                                        onChange={(e) => {
                                            const value = e.target.value;
                                            if (/^\d*$/.test(value)) {
                                                setWishlist((prev) =>
                                                    prev.map((item) =>
                                                        item.appId === app.appId ? { ...item, priority: value } : item
                                                    )
                                                );
                                            }
                                        }}
                                        onBlur={async (e) => {
                                            const newPriority = parseInt(e.target.value);
                                            if (!isNaN(newPriority) && newPriority >= 1 && newPriority <= wishlist.length && newPriority !== app.priority) {
                                                try {
                                                    await wishlistService.updatePriority(app.appId, newPriority);
                                                    await fetchWishlist();
                                                } catch {
                                                    alert("Failed to update priority.");
                                                }
                                            } else {
                                                await fetchWishlist();
                                            }
                                        }}
                                    />
                                    <button onClick={() => handlePriorityChange(app.appId, 1)}>+</button>

                                    <button
                                        className="remove-button"
                                        onClick={() => handleRemove(app.appId)}
                                    >
                                        Remove
                                    </button>
                                </div>

                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default Wishlist;
