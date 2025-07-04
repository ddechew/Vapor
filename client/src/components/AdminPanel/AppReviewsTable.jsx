import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const AppReviewsTable = () => {
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchReviews = async () => {
    try {
      const data = await adminService.getAppReviews();
      setReviews(data);
    } catch (err) {
      console.error("Failed to fetch app reviews:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReviews();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(reviews, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "app_reviews_export.json";
    link.click();
  };

  if (loading) return <p>Loading app reviews...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>📝 App Reviews</h2>
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
              <th>Recommended</th>
              <th>Review</th>
              <th>Created</th>
              <th>Edited</th>
            </tr>
          </thead>
          <tbody>
            {reviews.map((r) => (
              <tr key={r.reviewId}>
                <td>{r.reviewId}</td>
                <td>{r.appName}</td>
                <td>{r.userDisplayName ?? "Deleted User"}</td>
                <td>{r.isRecommended ? "👍 Yes" : "👎 No"}</td>
                <td>{r.reviewText}</td>
                <td>{new Date(r.createdAt).toLocaleString()}</td>
                <td>{r.isEdited ? "✅" : "—"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AppReviewsTable;
