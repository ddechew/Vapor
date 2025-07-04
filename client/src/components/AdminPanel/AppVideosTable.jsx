import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const AppVideosTable = () => {
  const [videos, setVideos] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchVideos = async () => {
    try {
      const data = await adminService.getAppVideos();
      setVideos(data);
    } catch (err) {
      console.error("Failed to fetch app videos:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchVideos();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(videos, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "app_videos_export.json";
    link.click();
  };

  if (loading) return <p>Loading app videos...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>🎥 App Videos</h2>
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
              <th>Video URL</th>
            </tr>
          </thead>
          <tbody>
            {videos.map((v) => (
              <tr key={v.videoId}>
                <td>{v.videoId}</td>
                <td>{v.appName}</td>
                <td style={{ maxWidth: "400px", wordBreak: "break-word" }}>{v.videoUrl}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AppVideosTable;
