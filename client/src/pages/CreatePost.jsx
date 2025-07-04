import { useEffect, useState, useContext } from "react";
import { useNavigate } from "react-router-dom";

import LoadingSpinner from "../components/LoadingSpinner"

import AuthContext from "../context/AuthContext";
import userService from "../services/userService";
import postService from "../services/postService";

import "../styles/CreatePost.css";

const CreatePost = () => {
  const { user } = useContext(AuthContext);
  const [ownedGames, setOwnedGames] = useState([]);
  const [selectedAppId, setSelectedAppId] = useState("");
  const [content, setContent] = useState("");
  const [imageFile, setImageFile] = useState(null);
  const [uploadPreview, setUploadPreview] = useState(null);
  const [error, setError] = useState("");
  const [posting, setPosting] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchLibrary = async () => {
      try {
        const library = await userService.getUserLibrary();
        setOwnedGames(library);
      } catch (err) {
        console.error("Failed to fetch user library:", err);
      }
    };

    if (user) {
      fetchLibrary();
    }
  }, [user]);

  const handleImageChange = (e) => {
    if (e) e.preventDefault();
    const file = e.target.files[0];
    if (file) {
      setImageFile(file);
      setUploadPreview(URL.createObjectURL(file));
    }
  };

  const handleSubmit = async () => {
    setError("");
    setPosting(true);

    try {
      if (!selectedAppId || isNaN(parseInt(selectedAppId))) {
        throw new Error("Please select a valid game.");
      }

      let imageUrl = null;
      if (imageFile) {
        try {
          const resp = await postService.uploadPostImage(imageFile);
          console.log("Upload response", resp);
          if (!resp.imageUrl) throw new Error("No URL returned.");
          imageUrl = resp.imageUrl;
        } catch (err) {
          throw new Error("Image upload failed: " + err.message);
        }
      }

      console.log("Creating post…");
      await postService.createPost({
        appId: parseInt(selectedAppId, 10),
        content,
        imageUrl,
      });
      console.log("Post created, navigating");
      navigate("/community");
    } catch (err) {
      console.error("Error in submit:", err);
      setError(err.message || "Failed to create post.");
    } finally {
      setPosting(false);
    }
  };

  if (!user) {
    return (
      <div className="create-post-page-container">
        <h2>📝 Create a Post</h2>
        <LoadingSpinner />
      </div>
    );
  }

  return (
    <div className="create-post-page-container">
      <h2>📝 Create a Post</h2>
      <div className="create-post-form">
        <label>Game:</label>
        <select
          value={selectedAppId}
          onChange={e => setSelectedAppId(e.target.value)}
          required
        >
          <option value="">-- Select owned game --</option>
          {ownedGames.map(g => (
            <option key={g.appId} value={g.appId}>{g.appName}</option>
          ))}
        </select>

        <label>What's on your mind?</label>
        <textarea
          rows="3"
          value={content}
          onChange={e => setContent(e.target.value)}
          required
        />

        <label>Optional image:</label>
        <input type="file" accept="image/*" onChange={handleImageChange} />

        {uploadPreview && (
          <div className="post-preview-image">
            <img src={uploadPreview} alt="Preview" />
          </div>
        )}

        <button
          type="button"
          className="btn btn-primary"
          disabled={posting}
          onClick={handleSubmit}
        >
          {posting ? "Posting..." : "Post"}
        </button>

        {error && <p className="error-msg">❌ {error}</p>}
      </div>
    </div>
  );
};

export default CreatePost;
