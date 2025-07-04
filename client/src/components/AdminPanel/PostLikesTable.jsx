import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const PostLikesTable = () => {
  const [likes, setLikes] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchPostLikes = async () => {
    try {
      const data = await adminService.getPostLikes();
      setLikes(data);
    } catch (err) {
      console.error("Failed to fetch post likes:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPostLikes();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(likes, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "post_likes_export.json";
    link.click();
  };

  if (loading) return <p>Loading post likes...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>👍 Post Likes</h2>
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
              <th>Post</th>
              <th>User</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            {likes.map((l) => (
              <tr key={l.likeId}>
                <td>{l.likeId}</td>
                <td>{l.postContent.slice(0, 50)}...</td>
                <td>{l.userDisplayName ?? "Deleted User"}</td>
                <td>{new Date(l.createdAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PostLikesTable;
