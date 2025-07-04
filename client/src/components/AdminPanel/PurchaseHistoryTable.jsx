import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const PurchaseHistoryTable = () => {
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchHistory = async () => {
    try {
      const data = await adminService.getPurchaseHistory();
      setHistory(data);
    } catch (err) {
      console.error("Failed to fetch purchase history:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchHistory();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(history, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "purchase_history_export.json";
    link.click();
  };

  if (loading) return <p>Loading purchase history...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>🧾 Purchase History</h2>
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
              <th>App</th>
              <th>User</th>
              <th>Purchase Date</th>
            </tr>
          </thead>
          <tbody>
            {history.map((entry) => (
              <tr key={entry.appLibraryId}>
                <td>{entry.appLibraryId}</td>
                <td>{entry.appName}</td>
                <td>{entry.userDisplayName || "🗑️ Deleted User"}</td>
                <td>{new Date(entry.purchaseDate).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PurchaseHistoryTable;
