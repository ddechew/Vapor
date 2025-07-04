import { Link } from "react-router-dom";
import "../../styles/PostCard.css"

const PostCard = ({ post, user, isLiked, onLike, onOpenLikers, onOpenComments, onDelete }) => {
  const isPostOwner = user?.username === post.username;
  return (
    <div className="post-card">
      <div className="post-header">
        <Link to={`/appid/${post.appId}`}>
          <img src={post.headerImage} alt="Game" className="post-game-image" />
        </Link>
        <div className="post-info">
          <Link to={`/appid/${post.appId}`}>
            <h4>{post.appName}</h4>
          </Link>
          <div className="post-user-details">
            {post.username ? (
              <>
                <Link to={`/profile/${post.username}`}>
                  <img
                    src={post.avatar || "/assets/default-profile.jpg"}
                    alt="User Avatar"
                    className="post-user-avatar"
                  />
                </Link>
                <Link to={`/profile/${post.username}`} className="post-user-name">
                  <strong>{post.userDisplayName}</strong>
                </Link>
              </>
            ) : (
              <>
                <img
                  src={post.avatar || "/assets/defaultVaporProfilePic.jpg"}
                  alt="User Avatar"
                  className="post-user-avatar"
                />
                <span className="post-user-name">
                  <strong>{post.userDisplayName}</strong>
                </span>
              </>
            )}
          </div>
          <span className="post-time">{new Date(post.createdAt).toLocaleString()}</span>
        </div>
      </div>

      <div className="post-body">
        <p>{post.content}</p>
        {post.imageUrl && (
          <img src={post.imageUrl} alt="Post" className="post-image" />
        )}
      </div>

      <div className="post-footer">
        <div
          className="like-section"
          title="View who liked this"
          onClick={() => {
            if (!user) return (window.location.href = "/login");
            onOpenLikers(post.postId);
          }}
        >
          <button
            className={`like-btn ${isLiked ? "liked" : ""}`}
            onClick={(e) => {
              e.stopPropagation();
              if (!user) return (window.location.href = "/login");
              onLike(post.postId);
            }}
          >
            {isLiked ? "❤️" : "🤍"}
          </button>
          {post.likesCount > 0 && <span className="like-count">{post.likesCount}</span>}
        </div>

        <div
          className="comment-section"
          title="View comments"
          onClick={() => {
            if (!user) return (window.location.href = "/login");
            onOpenComments(post.postId);
          }}
        >
          <button
            className="comment-btn"
            onClick={(e) => {
              e.stopPropagation();
              if (!user) return (window.location.href = "/login");
              onOpenComments(post.postId);
            }}
          >
            💬
          </button>
          {post.commentsCount > 0 && <span className="comment-count">{post.commentsCount}</span>}
        </div>

        {isPostOwner && (
          <button
            className="delete-post-btn"
            onClick={() => {
              if (window.confirm("Are you sure you want to delete this post?")) {
                onDelete(post.postId);
              }
            }}
          >
            🗑️
          </button>
        )}
      </div>
    </div>
  );
};

export default PostCard;