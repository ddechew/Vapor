import { useEffect, useState } from "react";
import adminService from "../../services/adminService";
import "../../styles/AdminTable.css";

const PostsTable = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchPosts = async () => {
    try {
      const data = await adminService.getPosts();
      setPosts(data);
    } catch (err) {
      console.error("Failed to fetch posts:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPosts();
  }, []);

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(posts, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "posts_export.json";
    link.click();
  };

  if (loading) return <p>Loading posts...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>📝 Posts</h2>
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
              <th>Content</th>
              <th>Likes</th>
              <th>Comments</th>
            </tr>
          </thead>
          <tbody>
            {posts.map((post) => (
              <tr key={post.postId}>
                <td>{post.postId}</td>
                <td>{post.userDisplayName ?? "Deleted user"}</td>
                <td>{post.appName}</td>
                <td style={{ maxWidth: "300px" }}>{post.content}</td>
                <td>{post.likesCount}</td>
                <td>{post.commentsCount}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PostsTable;
