import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const CommentsTable = () => {
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchComments = async () => {
    try {
      const data = await adminService.getComments();
      setComments(data);
    } catch (err) {
      console.error("Failed to fetch comments:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchComments();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(comments, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "comments_export.json";
    link.click();
  };

  if (loading) return <p>Loading comments...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>💬 Comments</h2>
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
              <th>Content</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            {comments.map((c) => (
              <tr key={c.commentId}>
                <td>{c.commentId}</td>
                <td>{c.postContent.slice(0, 50)}...</td>
                <td>{c.userDisplayName ?? "Deleted User"}</td>
                <td>{c.content}</td>
                <td>{new Date(c.createdAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default CommentsTable;
