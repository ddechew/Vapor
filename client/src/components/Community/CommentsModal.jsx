import { useState } from "react";
import { Link } from "react-router-dom";
import "../../styles/CommentsModal.css"


const CommentsModal = ({
    comments,
    user,
    newComment,
    setNewComment,
    onCommentSubmit,
    onCommentDelete,
    onClose,
    postOwnerUsername,
    editingCommentId,
    setEditingCommentId,
    editedCommentText,
    setEditedCommentText,
    onCommentEditSubmit
}) => {

    const [isSaving, setIsSaving] = useState(false);

    return (
        <div className="comments-modal-backdrop" onClick={onClose}>
            <div className="comments-modal" onClick={(e) => e.stopPropagation()}>
                <h4>💬 Comments</h4>
                <ul className="comments-list">
                    {comments.map((comment) => (
                        <div key={comment.commentId} className="comment-entry">
                            {comment.displayName === "[deleted]" ? (
                                <>
                                    <img
                                        src="/assets/defaultVaporProfilePic.jpg"
                                        alt="Hidden Avatar"
                                        className="comment-avatar"
                                    />
                                    <div className="comment-content">
                                        <div className="comment-header">
                                            <span className="comment-username">[deleted]</span>
                                            <span className="comment-time">
                                                {new Date(comment.createdAt).toLocaleString()}
                                            </span>
                                        </div>
                                        <p>{comment.commentText}</p>
                                    </div>
                                </>
                            ) : (
                                <>
                                    <Link to={`/profile/${comment.username}`}>
                                        <img
                                            src={comment.profilePicture || "/assets/defaultVaporProfilePic.jpg"}
                                            alt="Avatar"
                                            className="comment-avatar"
                                        />
                                    </Link>

                                    <div className="comment-content">
                                        <div className="comment-header">
                                            <Link to={`/profile/${comment.username}`} className="comment-username">
                                                {comment.displayName}
                                            </Link>
                                            <span className="comment-time">
                                                {new Date(comment.createdAt).toLocaleString()}
                                            </span>
                                        </div>

                                        {editingCommentId === comment.commentId ? (
                                            <>
                                                <textarea
                                                    rows="2"
                                                    value={editedCommentText}
                                                    onChange={(e) => setEditedCommentText(e.target.value)}
                                                    className="comment-edit-textarea"
                                                />
                                                <div className="edit-actions">
                                                    <button
                                                        className="btn btn-sm btn-success"
                                                        onClick={async () => {
                                                            if (isSaving || !editedCommentText.trim()) return;
                                                            setIsSaving(true);
                                                            await onCommentEditSubmit(comment.commentId, editedCommentText);
                                                            setIsSaving(false);
                                                        }}
                                                        disabled={
                                                            isSaving ||
                                                            !editedCommentText.trim() ||
                                                            editedCommentText.trim() === comment.commentText.trim()
                                                        }
                                                    >
                                                        💾 {isSaving ? "Saving..." : "Save"}
                                                    </button>
                                                    <button className="btn btn-sm btn-secondary" onClick={() => {
                                                        setEditingCommentId(null);
                                                        setEditedCommentText("");
                                                    }}>❌ Cancel</button>
                                                </div>
                                            </>
                                        ) : (
                                            <p className="comment-text">
                                                {comment.commentText}
                                                {comment.isEdited && <span className="edited-label"> (edited)</span>}
                                            </p>
                                        )}
                                    </div>

                                    {(user?.username === comment.username || user?.username === postOwnerUsername) && (
                                        <div className="comment-actions">
                                            {user?.username === comment.username && (
                                                <button className="edit-comment-btn" onClick={() => {
                                                    setEditingCommentId(comment.commentId);
                                                    setEditedCommentText(comment.commentText);
                                                }}>✏️</button>
                                            )}
                                            <button className="delete-comment-btn" onClick={() => onCommentDelete(comment.commentId, user.username === comment.username)}>🗑️</button>
                                        </div>
                                    )}

                                </>

                            )}
                        </div>
                    ))}
                </ul>

                {user ? (
                    <div className="comment-form">
                        <textarea
                            rows="3"
                            placeholder="Write a comment..."
                            value={newComment}
                            onChange={(e) => setNewComment(e.target.value)}
                        />
                        <button className="btn btn-primary" onClick={onCommentSubmit}>
                            Post Comment
                        </button>
                    </div>
                ) : (
                    <p style={{ marginTop: "1rem" }}>
                        <Link to="/login">Log in</Link> to leave a comment.
                    </p>
                )}
            </div>
        </div>
    )
};

export default CommentsModal;
