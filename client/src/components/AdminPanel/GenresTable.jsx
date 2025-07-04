import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const GenresTable = () => {
  const [genres, setGenres] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchGenres = async () => {
    try {
      const data = await adminService.getGenres();
      setGenres(data);
    } catch (err) {
      console.error("Failed to fetch genres:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchGenres();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(genres, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "genres_export.json";
    link.click();
  };

  if (loading) return <p>Loading genres...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>🎼 Genres</h2>
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
              <th>Genre Name</th>
            </tr>
          </thead>
          <tbody>
            {genres.map((g) => (
              <tr key={g.genreId}>
                <td>{g.genreId}</td>
                <td>{g.name}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default GenresTable;
