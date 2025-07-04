import { useEffect, useState, useContext, useRef } from "react";
import { Link, useSearchParams, useNavigate } from "react-router-dom";

import { PostCard, CommentsModal, LikersModal } from "../components/Community";
import LoadingSpinner from "../components/LoadingSpinner";

import AuthContext from "../context/AuthContext";
import postService from "../services/postService";

import "../styles/Community.css";

const Community = () => {
    const { user } = useContext(AuthContext);
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showLikersModal, setShowLikersModal] = useState(false);
    const [likers, setLikers] = useState([]);
    const [modalPostId, setModalPostId] = useState(null);
    const [likedPosts, setLikedPosts] = useState([]);

    const [showCommentsModal, setShowCommentsModal] = useState(false);
    const [selectedPostId, setSelectedPostId] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    const [editingCommentId, setEditingCommentId] = useState(null);
    const [editedCommentText, setEditedCommentText] = useState("");

    const postRefs = useRef({});
    const [searchParams, setSearchParams] = useSearchParams();
    const navigate = useNavigate();

    const fetchPosts = () => {
        postService.getAllPosts()
            .then(setPosts)
            .catch((err) => console.error("Failed to fetch posts:", err))
            .finally(() => setLoading(false));
    };

    useEffect(() => {
        const postIdParam = searchParams.get("postId");
        if (!postIdParam) return;

        const el = postRefs.current[postIdParam];
        if (el) {
            el.scrollIntoView({ behavior: "smooth", block: "start" });
            el.style.outline = "2px solid #66aaff";
            setTimeout(() => {
                el.style.outline = "none";
            }, 2000);

            setTimeout(() => {
                searchParams.delete("postId");
                setSearchParams(searchParams, { replace: true });
            }, 500);
        }
    }, [searchParams, posts]);


    useEffect(() => {
        const loadCommunityData = async () => {
            try {
                setLoading(true);

                const postsData = await postService.getAllPosts();
                setPosts(postsData);

                if (user) {
                    const likedPostsData = await postService.getUserLikedPosts();
                    setLikedPosts(likedPostsData);
                } else {
                    setLikedPosts([]); 
                }

            } catch (err) {
                console.error("Failed to load community data", err);
            } finally {
                setLoading(false);
            }
        };

        loadCommunityData();
    }, [user]);


    const openLikersModal = async (postId) => {
        try {
            const data = await postService.getPostLikers(postId);
            setLikers(data);
            setModalPostId(postId);
            setShowLikersModal(true);
        } catch (err) {
            console.error("Failed to fetch likers:", err);
        }
    };

    const openCommentsModal = async (postId) => {
        setSelectedPostId(postId);
        try {
            const data = await postService.getPostComments(postId);
            setComments(data);
            setShowCommentsModal(true);
        } catch (err) {
            console.error("Failed to fetch comments:", err);
        }
    };

    const handleEditComment = async (commentId, newText) => {
        if (!newText.trim()) return;

        try {
            await postService.editComment(commentId, newText);
            const updated = await postService.getPostComments(selectedPostId);
            setComments(updated);
            setEditingCommentId(null);
            setEditedCommentText("");
            fetchPosts(); 
        } catch (err) {
            console.error("Failed to edit comment:", err.message);
        }
    };

    const handleLike = async (postId) => {
        try {
            await postService.togglePostLike(postId);

            if (likedPosts.includes(postId)) {
                setLikedPosts(prev => prev.filter(id => id !== postId));
            } else {
                setLikedPosts(prev => [...prev, postId]);
            }

            fetchPosts();
        } catch (err) {
            console.error("Failed to toggle like:", err.message);
        }
    };

    const handlePostComment = async () => {
        if (!newComment.trim()) return;
        try {
            await postService.addPostComment({ postId: selectedPostId, commentText: newComment });
            const updated = await postService.getPostComments(selectedPostId);
            setComments(updated);
            setNewComment("");
            fetchPosts();
        } catch (err) {
            console.error("Failed to post comment:", err);
        }
    };

    const handleDeleteComment = async (commentId, isOwnComment) => {
        try {
            if (isOwnComment) {
                await postService.deleteComment(commentId);
            } else {
                await postService.deleteCommentAsPostOwner(commentId);
            }
            const updated = await postService.getPostComments(selectedPostId);
            setComments(updated);
            fetchPosts(); 
        } catch (err) {
            console.error("Failed to delete comment:", err.message);
        }
    };

    const handlePostDelete = async (postId) => {
        try {
            await postService.deletePost(postId);
            fetchPosts();
        } catch (err) {
            alert("Failed to delete post.");
            console.error(err.message);
        }
    }


    return (
        <div className="community-container">
            <div className="community-header">
                <span className="community-icon">💬</span>
                <h2 className="community-title">Vapor Community Posts</h2>
            </div>

            {user && (
                <div className="create-post-button-wrapper">
                    <Link to="/create-post" className="btn btn-primary create-post-btn">
                        <span className="plus-icon">+</span> Create Post
                    </Link>
                </div>
            )}

            {loading && <LoadingSpinner />}
            {!loading && posts.length === 0 && <p>No posts yet. Be the first to share something!</p>}

            {posts.map((post) => (
                <div key={post.postId} ref={el => postRefs.current[post.postId] = el}>
                    <PostCard
                        post={post}
                        user={user}
                        isLiked={likedPosts.includes(post.postId)}
                        onLike={handleLike}
                        onOpenLikers={openLikersModal}
                        onOpenComments={openCommentsModal}
                        onDelete={handlePostDelete}
                    />
                </div>
            ))}


            {showLikersModal && (
                <LikersModal likers={likers} onClose={() => setShowLikersModal(false)} />
            )}

            {showCommentsModal && (
                <CommentsModal
                    comments={comments}
                    user={user}
                    newComment={newComment}
                    setNewComment={setNewComment}
                    onClose={() => setShowCommentsModal(false)}
                    onCommentSubmit={handlePostComment}
                    onCommentDelete={handleDeleteComment}
                    selectedPostId={selectedPostId}
                    postOwnerUsername={posts.find(p => p.postId == selectedPostId)?.username}
                    editingCommentId={editingCommentId}
                    setEditingCommentId={setEditingCommentId}
                    editedCommentText={editedCommentText}
                    setEditedCommentText={setEditedCommentText}
                    onCommentEditSubmit={handleEditComment}
                />
            )}
        </div>
    );
};

export default Community;