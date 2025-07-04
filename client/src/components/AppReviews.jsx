import { useEffect, useState, useContext } from 'react';
import { Link } from 'react-router-dom';

import AuthContext from '../context/AuthContext';
import appService from '../services/appService';

import "../styles/AppReviews.css"

const AppReviews = ({ appId, onSummaryChange }) => {
    const { user } = useContext(AuthContext);
    const [reviews, setReviews] = useState([]);
    const [newReview, setNewReview] = useState({ isRecommended: true, reviewText: '' });
    const [canReview, setCanReview] = useState(false);
    const [hasReviewed, setHasReviewed] = useState(false);
    const [editingReviewId, setEditingReviewId] = useState(null);
    const [editedReview, setEditedReview] = useState({ isRecommended: true, reviewText: '' });
    const [originalReview, setOriginalReview] = useState(null);
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        appService.getReviews(appId).then((data) => {
            setReviews(data);
            if (onSummaryChange) {
                const recommendedCount = data.filter(r => r.isRecommended).length;
                const notRecommendedCount = data.length - recommendedCount;

                const summary =
                    recommendedCount > notRecommendedCount
                        ? "Mostly Positive"
                        : notRecommendedCount > recommendedCount
                            ? "Mostly Negative"
                            : "Neutral";

                onSummaryChange({ summary, total: data.length });
            }
        })

        const checkReviewEligibility = async () => {
            const owns = await appService.checkOwnership(appId);
            const reviewed = await appService.hasReviewed(appId);
            setHasReviewed(reviewed);
            setCanReview(owns && !reviewed); 
        };

        if (user) {
            checkReviewEligibility();
        }
    }, [appId, user]);

    const handleUpdateReview = async (reviewId) => {
        try {
            await appService.updateReview(reviewId, {
                appId,
                reviewText: editedReview.reviewText,
                isRecommended: editedReview.isRecommended,
            });

            const updated = await appService.getReviews(appId);
            setReviews(updated);
            setEditingReviewId(null);

            if (onSummaryChange) {
                const recommendedCount = updated.filter(r => r.isRecommended).length;
                const notRecommendedCount = updated.length - recommendedCount;

                const summary =
                    recommendedCount > notRecommendedCount
                        ? "Mostly Positive"
                        : notRecommendedCount > recommendedCount
                            ? "Mostly Negative"
                            : "Neutral";

                onSummaryChange({ summary, total: updated.length });
            }
        } catch (err) {
            alert("Failed to update review.");
            console.error(err);
        }
    };


    const formatDate = (isoString) => {
        const date = new Date(isoString);
        return date.toLocaleDateString(undefined, {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
        });
    };

    const hasChanges = () => {
        return (
            originalReview &&
            (editedReview.reviewText !== originalReview.reviewText ||
                editedReview.isRecommended !== originalReview.isRecommended)
        );
    };

    const handleSubmit = async () => {
        await appService.submitReview({
            appId,
            isRecommended: newReview.isRecommended,
            reviewText: newReview.reviewText
        });

        const updated = await appService.getReviews(appId);
        setReviews(updated);
        setNewReview({ isRecommended: true, reviewText: '' });
        setCanReview(false);
        setHasReviewed(true);

        if (onSummaryChange) {
            const recommendedCount = updated.filter(r => r.isRecommended).length;
            const notRecommendedCount = updated.length - recommendedCount;

            const summary =
                recommendedCount > notRecommendedCount
                    ? "Mostly Positive"
                    : notRecommendedCount > recommendedCount
                        ? "Mostly Negative"
                        : "Neutral";

            onSummaryChange({ summary, total: updated.length });
        }
    };

    return (
        <div className="app-reviews">
            <h3>Reviews</h3>
            {reviews.map((r) => (
                <div key={r.reviewId} className="review-item">
                    <div className="review-header">
                        {r.username ? (
                            <Link to={`/profile/${r.username}`}>
                                <img
                                    src={r.profilePictureUrl || "/assets/default-profile.png"}
                                    alt="User"
                                    className="review-avatar"
                                />
                            </Link>
                        ) : (
                            <img
                                src={r.profilePictureUrl || "/assets/default-profile.png"}
                                alt="User"
                                className="review-avatar"
                            />
                        )}
                        <div className="review-user-info">
                            <div className="review-user-top">
                                {r.username ? (
                                    <Link to={`/profile/${r.username}`} className="review-username">
                                        {r.userDisplayName}
                                    </Link>

                                ) : (
                                    <span className="review-username deleted-user">{r.userDisplayName} (deleted)</span>
                                )}
                                <span className="review-date">
                                    {formatDate(r.createdAt)}{r.isEdited && <em style={{ color: "#aaa", marginLeft: "6px" }}>(edited)</em>}
                                </span>

                            </div>
                            <span className={`review-recommendation ${r.isRecommended ? "recommended-badge" : "not-recommended-badge"}`}>
                                {r.isRecommended ? "👍 Recommended" : "👎 Not Recommended"}
                            </span>

                        </div>

                    </div>
                    {editingReviewId === r.reviewId ? (
                        <div className="submit-review">
                            <textarea
                                value={editedReview.reviewText}
                                onChange={(e) => setEditedReview({ ...editedReview, reviewText: e.target.value })}
                            />
                            <label>
                                <input
                                    type="checkbox"
                                    checked={editedReview.isRecommended}
                                    onChange={(e) =>
                                        setEditedReview({ ...editedReview, isRecommended: e.target.checked })
                                    }
                                />
                                Recommended?
                            </label>
                            <div style={{ display: "flex", gap: "10px" }}>
                                <button
                                    onClick={() => {
                                        setIsSaving(true);
                                        handleUpdateReview(r.reviewId).finally(() => setIsSaving(false));
                                    }}
                                    disabled={!hasChanges() || isSaving}
                                >
                                    {isSaving ? "Saving..." : "💾 Save"}
                                </button>


                                <button onClick={() => setEditingReviewId(null)}>❌ Cancel</button>
                            </div>
                        </div>
                    ) : (
                        <>
                            <p className="review-text">{r.reviewText}</p>
                            {user?.username === r.username && (
                                <button
                                    className="edit-review-btn"
                                    onClick={() => {
                                        setEditingReviewId(r.reviewId);
                                        const original = { reviewText: r.reviewText, isRecommended: r.isRecommended };
                                        setEditedReview(original);
                                        setOriginalReview(original);
                                    }}
                                >
                                    ✏️ Edit
                                </button>
                            )}
                        </>
                    )}

                </div>
            ))}

            {reviews.length === 0 && (
                <p className="no-reviews-msg" style={{ color: "#888", fontStyle: "italic" }}>
                    No reviews yet. Be the first to leave one!
                </p>
            )}

            {!canReview && hasReviewed && (
                <p style={{ color: "#888", fontStyle: "italic" }}>
                    ✅ You have already submitted a review for this app.
                </p>
            )}

            {canReview && (
                <div className="submit-review">
                    <textarea
                        value={newReview.reviewText}
                        onChange={(e) => setNewReview({ ...newReview, reviewText: e.target.value })}
                    />
                    <label>
                        <input
                            type="checkbox"
                            checked={newReview.isRecommended}
                            onChange={(e) => setNewReview({ ...newReview, isRecommended: e.target.checked })}
                        />
                        Recommended?
                    </label>
                    <button onClick={handleSubmit}>Submit Review</button>
                </div>
            )}
        </div>
    );
};

export default AppReviews;
