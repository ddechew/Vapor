import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const AppImagesTable = () => {
  const [images, setImages] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchImages = async () => {
    try {
      const data = await adminService.getAppImages();
      setImages(data);
    } catch (err) {
      console.error("Failed to fetch app images:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchImages();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(images, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "app_images_export.json";
    link.click();
  };

  if (loading) return <p>Loading app images...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>🖼️ App Images</h2>
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
              <th>Image URL</th>
            </tr>
          </thead>
          <tbody>
            {images.map((img) => (
              <tr key={img.imageId}>
                <td>{img.imageId}</td>
                <td>{img.appName}</td>
                <td style={{ maxWidth: "400px", wordBreak: "break-word" }}>{img.imageUrl}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AppImagesTable;
