import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const PublishersTable = () => {
  const [publishers, setPublishers] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchPublishers = async () => {
    try {
      const data = await adminService.getPublishers();
      setPublishers(data);
    } catch (err) {
      console.error("Failed to fetch publishers:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPublishers();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(publishers, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "publishers_export.json";
    link.click();
  };

  if (loading) return <p>Loading publishers...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>🏢 Publishers</h2>
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
              <th>Publisher Name</th>
            </tr>
          </thead>
          <tbody>
            {publishers.map((p) => (
              <tr key={p.publisherId}>
                <td>{p.publisherId}</td>
                <td>{p.publisherName}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PublishersTable;
