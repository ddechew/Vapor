import { useState, useEffect, useContext } from "react";
import { useParams, Link } from "react-router-dom";

import YouTube from "react-youtube";
import ThumbnailCarousel from "../components/ThumbnailCarousel.jsx";
import RelatedApps from "../components/RelatedApps.jsx";
import AppReviews from "../components/AppReviews.jsx";
import LoadingSpinner from "../components/LoadingSpinner.jsx";

import AuthContext from "../context/AuthContext.jsx";
import { useCart } from "../context/CartContext.jsx";
import { useWishlist } from "../context/WishlistContext";

import userService from "../services/userService.js";
import appService from "../services/appService.js";
import wishlistService from "../services/wishlistService";


import "../styles/AppDetails.css";

const AppDetails = () => {
  const { id } = useParams();

  const [app, setApp] = useState(null);
  const [selectedMedia, setSelectedMedia] = useState("");
  const [mediaItems, setMediaItems] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [relatedApps, setRelatedApps] = useState([]);
  const [baseApp, setBaseApp] = useState(null);
  const { cart, addToCart } = useCart();
  const { user } = useContext(AuthContext);
  const { wishlist, addToWishlist } = useWishlist();
  const [isInWishlist, setIsInWishlist] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [isOwned, setIsOwned] = useState(false);
  const [ownsBaseApp, setOwnsBaseApp] = useState(true);
  const [reviewSummary, setReviewSummary] = useState(null);
  const [loading, setLoading] = useState(true);


  useEffect(() => {
    const fetchDetails = async () => {
      try {
        setLoading(true);
        const data = await appService.getAppDetails(id);
        if (data.baseAppId) {
          const base = await appService.getAppDetails(data.baseAppId);
          setBaseApp(base);

          const related = await appService.getRelatedApps(data.baseAppId);
          setRelatedApps(related || []);

          if (user) {
            const library = await userService.getUserLibrary();
            const ownedAppIds = library.map(a => a.appId);
            setIsOwned(ownedAppIds.includes(parseInt(id)));
            const baseOwned = ownedAppIds.includes(data.baseAppId);
            const baseInCart = cart.some(item => item.appId === data.baseAppId);
            setOwnsBaseApp(baseOwned || baseInCart);


            if (!ownedAppIds.includes(data.baseAppId)) {
              const isBaseAppInCart = cart.some(c => c.appId === data.baseAppId);
              setOwnsBaseApp(isBaseAppInCart);
            }
          }
        } else {
          const related = await appService.getRelatedApps(data.appId);
          setRelatedApps(related || []);
          setBaseApp(null);
        }

        setApp(data);

        if (user && data?.appId) {
          const inWishlist = wishlist.some(item => item.appId === data.appId);
          setIsInWishlist(inWishlist);
        }

        const combinedMedia = [
          ...(Object.entries(data.videos || {}).map(([url, thumbnail]) => ({ url, thumbnail, type: "video" }))),
          ...(Object.entries(data.screenshots || {}).map(([url, thumbnail]) => ({ url, thumbnail, type: "image" }))),
        ];

        if (user) {
          const library = await userService.getUserLibrary();
          const ownedAppIds = library.map(a => a.appId);
          setIsOwned(ownedAppIds.includes(parseInt(id)));
        }

        setMediaItems(combinedMedia);
        setSelectedMedia(combinedMedia[0]?.url || "");
        setCurrentIndex(0);

        if (data.baseAppId) {
          const base = await appService.getAppDetails(data.baseAppId);
          setBaseApp(base);

          const related = await appService.getRelatedApps(data.baseAppId);
          setRelatedApps(related || []);

          if (user) {
            const library = await userService.getUserLibrary();
            const ownedAppIds = library.map(a => a.appId);
            const baseOwned = ownedAppIds.includes(data.baseAppId);

            const baseInCart = cart.some(item => item.appId === data.baseAppId);

            setIsOwned(ownedAppIds.includes(parseInt(id)));
            setOwnsBaseApp(baseOwned || baseInCart);
          }
        } else {
          const related = await appService.getRelatedApps(data.appId);
          setRelatedApps(related || []);
          setBaseApp(null);
        }
      } catch (error) {
        console.error("Failed to load app details:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchDetails();
  }, [id, user, cart, wishlist]);

  useEffect(() => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }, [id]);

  useEffect(() => {
    let timeout;
    if (selectedMedia && !selectedMedia.includes(".mp4") && !isYouTube) {
      timeout = setTimeout(() => {
        goToNextMedia();
      }, 4000);
    }
    return () => clearTimeout(timeout);
  }, [selectedMedia]);

  const goToNextMedia = () => {
    const nextIndex = (currentIndex + 1) % mediaItems.length;
    setCurrentIndex(nextIndex);
    setSelectedMedia(mediaItems[nextIndex].url);
  };

  const handleThumbnailClick = (mediaUrl) => {
    setSelectedMedia(mediaUrl);
    const index = mediaItems.findIndex(item => item.url === mediaUrl);
    if (index !== -1) setCurrentIndex(index);
  };

  const handleAddToWishlist = async () => {
    try {
      await wishlistService.addToWishlist(app.appId);
      addToWishlist(app);
      setIsInWishlist(true);
      alert("💖 Added to wishlist!");
    } catch (err) {
      alert(err.message);
    }
  };


  const handleAddToCart = async () => {
    try {
      if (app.baseAppId) {
        const userLibrary = await userService.getUserLibrary();
        const ownedAppIds = userLibrary.map(a => a.appId);
        const ownsBase = ownedAppIds.includes(app.baseAppId);
        const baseInCart = cart.some(item => item.appId === app.baseAppId);

        if (!ownsBase && !baseInCart) {
          alert(`You must own or have added the base game "${baseApp?.name}" before purchasing this content.`);
          return;
        }
      }
      await addToCart(app, user);
      setShowModal(true);
    } catch (err) {
      console.error("Error adding to cart:", err);
      alert("Failed to add to cart.");
    }
  };

  const handleAddFreeApp = async () => {
    if (app.baseAppId) {
      const userLibrary = await userService.getUserLibrary();
      const ownedAppIds = userLibrary.map(a => a.appId);
      if (!ownedAppIds.includes(app.baseAppId)) {
        alert(`You must own the base game "${baseApp?.name}" before adding this content.`);
        return;
      }
    }

    try {
      await userService.addFreeAppToLibrary(app.appId);
      setIsOwned(true);
    } catch (err) {
      console.error("Failed to add free app:", err);
      alert("Failed to add this app to your library.");
    }
  };

  const isYouTube = selectedMedia.includes("youtube.com") || selectedMedia.includes("youtu.be");


  if (loading) return <LoadingSpinner />;
  if (!app) return <div>App not found.</div>;
  return (
    <div className="app-details-container">
      <h1 className="game-title">{app.name}</h1>

      <div className="game-content">
        <div className="media-section">
          {isYouTube ? (
            <YouTube
              videoId={new URL(selectedMedia).searchParams.get("v") || selectedMedia.split("/").pop()}
              opts={{
                width: "100%",
                height: "450px",
                playerVars: {
                  autoplay: 1,
                  mute: 1,
                  modestbranding: 1,
                  rel: 0,
                },
              }}
              className="game-video"
              onEnd={goToNextMedia}
            />
          ) : selectedMedia.includes(".mp4") ? (
            <video
              key={selectedMedia}
              className="game-video"
              controls
              autoPlay
              muted
              onEnded={goToNextMedia}
              onError={(e) => {
                const error = e?.target?.error;
                if (error?.code !== error?.MEDIA_ERR_ABORTED) {
                  console.error("Video error:", error);
                }
              }}
            >
              <source src={selectedMedia} type="video/mp4" />
              Your browser does not support the video tag.
            </video>
          ) : (
            <img src={selectedMedia} alt="Screenshot" className="game-image" />
          )}

          {mediaItems.length > 1 && (
            <ThumbnailCarousel
              mediaItems={[
                ...(Object.entries(app.videos || {}).map(([url, thumbnail]) => ({ url, thumbnail }))),
                ...(Object.entries(app.screenshots || {}).map(([url, thumb]) => ({ url, thumbnail: thumb })))
              ]}
              onMediaSelect={handleThumbnailClick}
              selectedMedia={selectedMedia}
              currentIndex={currentIndex}
            />
          )}
        </div>

        <div className="game-info">
          <img src={app.headerImage} alt="Game Cover" className="cover-image" />
          {reviewSummary && (
            <div className="review-summary-line">
              <strong>All Reviews:</strong>
              <span
                className={`review-tag ${reviewSummary.summary.replace(" ", "-").toLowerCase()}`}
                title={`Based on ${reviewSummary.total} review${reviewSummary.total === 1 ? "" : "s"}`}
              >
                {reviewSummary.summary}
              </span>
              <span className="review-count">({reviewSummary.total})</span>

            </div>
          )}

          <p><strong>Release Date:</strong> {app.releaseDate}</p>
          <p><strong>Developer:</strong> {app.developer?.join(", ")}</p>
          <p><strong>Publisher:</strong> {app.publisher?.join(", ")}</p>
          {app.genres && app.genres.length > 0 && (
            <p><strong>Genres:</strong> {app.genres.join(", ")}</p>
          )}

          {app.price == '0.00€' ? (
            <p className="price"><strong>Free</strong></p>
          ) : (
            <p className="price"><strong>Price:</strong> {app.price}</p>
          )}

          {isOwned ? (
            <Link to="/library" className="buy-button owned">In Library</Link>
          ) : app.price === "0.00€" ? (
            <button
              onClick={handleAddFreeApp}
              className="buy-button"
              disabled={app.baseAppId && !ownsBaseApp}
            >
              Add to Library
            </button>

          ) : cart.some(item => item.appId === app.appId) ? (
            <Link to="/cart" className="buy-button disabled" disabled>In Cart</Link>
          ) : (
            <button
              onClick={handleAddToCart}
              className="buy-button"
              disabled={app.baseAppId && !ownsBaseApp}
            >
              Add to Cart
            </button>
          )}

          {!isOwned && (
            isInWishlist ? (
              <Link to="/wishlist" className="wishlist-button disabled">In Wishlist</Link>
            ) : (
              <button onClick={handleAddToWishlist} className="wishlist-button">
                💖 Add to Wishlist
              </button>
            )
          )}


        </div>
      </div>

      {baseApp && (
        <div className="base-app-banner">
          <p>This content requires the base game <strong>{baseApp.name}</strong>.</p>
          <Link to={`/appid/${baseApp.appId}`} className="back-to-base-button">
            ← View Main Application
          </Link>
        </div>
      )}

      <hr className="section-divider" />
      <h3 className="description-heading">About..</h3>
      <p className="description-below">{app.description}</p>

      <RelatedApps relatedApps={relatedApps} />
      {app && <AppReviews appId={app.appId} onSummaryChange={setReviewSummary} />}

      {showModal && (
        <div className="cart-modal-overlay">
          <div className="cart-modal">
            <h3>Item added to cart!</h3>
            <div className="modal-actions">
              <Link to="/cart" className="modal-btn checkout-btn">Go to Cart</Link>
              <button onClick={() => setShowModal(false)} className="modal-btn">Continue Shopping</button>
            </div>
          </div>
        </div>
      )}

    </div>
  );
};

export default AppDetails;
