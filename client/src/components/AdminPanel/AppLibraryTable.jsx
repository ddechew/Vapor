import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const UserLibrariesTable = () => {
  const [libraries, setLibraries] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchLibraries = async () => {
    try {
      const data = await adminService.getUserLibraries();
      setLibraries(data);
    } catch (err) {
      console.error("Failed to fetch user libraries:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLibraries();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(libraries, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "user_libraries_export.json";
    link.click();
  };

  if (loading) return <p>Loading user libraries...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>📚 User Libraries</h2>
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
              <th>Purchase Date</th>
            </tr>
          </thead>
          <tbody>
            {libraries.map((lib) => (
              <tr key={lib.libraryId}>
                <td>{lib.libraryId}</td>
                <td>{lib.userDisplayName || "🗑️ Deleted User"}</td>
                <td>{lib.appName}</td>
                <td>{new Date(lib.purchaseDate).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default UserLibrariesTable;
