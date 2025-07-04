import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const WishlistTable = () => {
  const [wishlist, setWishlist] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchWishlist = async () => {
    try {
      const data = await adminService.getWishlistItems();
      setWishlist(data);
    } catch (err) {
      console.error("Failed to fetch wishlist items:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWishlist();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(wishlist, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "wishlist_export.json";
    link.click();
  };

  if (loading) return <p>Loading wishlist...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>❤️ Wishlist Items</h2>
        <div className="admin-table-actions">
          <button className="admin-export-btn" onClick={exportToJSON}>
            Export JSON
          </button>
        </div>
      </div>

      <div className="admin-table-scroll">
        <table className="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>User</th>
              <th>App</th>
              <th>Added At</th>
            </tr>
          </thead>
          <tbody>
            {wishlist.map((item) => (
              <tr key={item.wishlistId}>
                <td>{item.wishlistId}</td>
                <td>{item.userDisplayName || "Deleted User"}</td>
                <td>{item.appName}</td>
                <td>{new Date(item.addedAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default WishlistTable;
