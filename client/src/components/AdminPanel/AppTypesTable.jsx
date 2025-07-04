import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const AppTypesTable = () => {
  const [appTypes, setAppTypes] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchAppTypes = async () => {
    try {
      const data = await adminService.getAppTypes();
      setAppTypes(data);
    } catch (err) {
      console.error("Failed to fetch app types:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAppTypes();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(appTypes, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "app_types_export.json";
    link.click();
  };

  if (loading) return <p>Loading app types...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>📦 Application Types</h2>
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
              <th>Type Name</th>
            </tr>
          </thead>
          <tbody>
            {appTypes.map((t) => (
              <tr key={t.appTypeId}>
                <td>{t.appTypeId}</td>
                <td>{t.typeName}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AppTypesTable;
