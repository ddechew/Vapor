import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const CartItemsTable = () => {
  const [cartItems, setCartItems] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchCartItems = async () => {
    try {
      const data = await adminService.getCartItems();
      setCartItems(data);
    } catch (err) {
      console.error("Failed to fetch cart items:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCartItems();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(cartItems, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "cart_items_export.json";
    link.click();
  };

  if (loading) return <p>Loading cart items...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>🛒 Cart Items</h2>
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
            {cartItems.map((item) => (
              <tr key={item.cartItemId}>
                <td>{item.cartItemId}</td>
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

export default CartItemsTable;
