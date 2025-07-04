import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";

import TopSellersCarousel from "../components/TopSellersCarousel";
import "../components/SearchBar.jsx";
import SearchBar from "../components/SearchBar.jsx";
import SafeYouTubePlayer from "../components/SafeYouTubePlayer.jsx";
import LoadingSpinner from "../components/LoadingSpinner.jsx";

import appService from "../services/appService";

import "../styles/Store.css";

const Store = () => {
  const [games, setGames] = useState([]);
  const [hoveredGame, setHoveredGame] = useState(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const videoRefs = useRef({});
  const hoverTimeout = useRef(null);

  useEffect(() => {
    const fetchGames = async () => {
      setLoading(true);
      const paginatedGames = await appService.getPaginatedApps();
      setGames(paginatedGames);
      setLoading(false);
    };

    fetchGames();
  }, []);

  const fetchGamesForPage = async (pageNum) => {
    setLoading(true);
    const newGames = await appService.getPaginatedApps(pageNum, 12);
    setGames(newGames);
    setLoading(false);
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    fetchGamesForPage(newPage);
    window.scrollTo({ top: 750, behavior: "smooth" });
  };

  useEffect(() => {
    fetchGamesForPage(page);
  }, [page]);

  const handleMouseEnter = async (gameId, trailerUrl) => {
    if (!trailerUrl) return;
    hoverTimeout.current = setTimeout(() => {
      setHoveredGame(gameId);
      const video = videoRefs.current[gameId];
      if (video) {
        video.play().catch(err => console.warn("Video play error:", err.message));
      }
    }, 250);
  };

  const handleMouseLeave = async (gameId) => {
    clearTimeout(hoverTimeout.current);
    setHoveredGame(null);

    const video = videoRefs.current[gameId];
    if (video) {
      try {
        await video.pause();
        video.currentTime = 0;
      } catch (err) {
        console.warn("Video pause was interrupted:", err.message);
      }
    }
  };

  return loading ? (
    <LoadingSpinner />
  ) : (
    <div className="container mt-4">
      <SearchBar
        onSearch={({ query, isFree, genre }) => {
          appService.searchApps(query, isFree, genre).then(setGames);
        }}
        onSuggestionClick={(app) => {
          window.location.href = `/appid/${app.appId}`;
        }}
      />

      <div className="carousel-section">
        <TopSellersCarousel />
      </div>
      <div className="row">
        {games.map((game) => (
          <div key={game.appId} className="col-lg-4 col-md-6 col-sm-12 mb-4">
            <Link to={`/appid/${game.appId}`} className="card-link">
              <div
                className="card game-card"
                onMouseEnter={() => handleMouseEnter(game.appId, game.trailer)}
                onMouseLeave={() => handleMouseLeave(game.appId)}
              >
                <div className="game-media">
                  {hoveredGame === game.appId && game.trailer ? (
                    game.trailer.includes("youtube.com") ? (
                      (() => {
                        const videoId = new URL(game.trailer).pathname.split("/").pop();

                        const opts = {
                          height: "200",
                          width: "100%",
                          playerVars: {
                            autoplay: 1,
                            mute: 1,
                            controls: 0,
                            modestbranding: 1,
                            rel: 0,
                            fs: 0,
                            loop: 1,
                            playlist: videoId,
                          },
                        };
                        return (
                          <div className="card-img-top youtube-wrapper">
                            {typeof window !== "undefined" && (
                              <SafeYouTubePlayer
                                videoId={videoId}
                                opts={opts}
                                className="youtube-player"
                              />
                            )}
                          </div>
                        );
                      })()
                    ) : (
                      <video
                        ref={(el) => (videoRefs.current[game.appId] = el)}
                        className="card-img-top"
                        muted
                        loop
                        disablePictureInPicture
                        autoPlay
                        preload="none"
                      >
                        <source src={game.trailer} type="video/mp4" />
                        Your browser does not support the video tag.
                      </video>
                    )
                  ) : (
                    <img src={game.headerImage} alt={game.name} className="card-img-top" />
                  )}
                </div>
                <div className="card-body">
                  <h5 className="card-title">{game.name}</h5>
                  <span className="card-text">{game.price == 0 ? "Free" : `${game.price}`}</span>
                </div>
              </div>
            </Link>
          </div>
        ))}
      </div>

      <div className="text-center d-flex justify-content-center gap-3">
        <button
          className="pagination-btn"
          onClick={() => handlePageChange(page - 1)}
          disabled={page === 1 || loading}
        >
          Previous
        </button>

        <span className="align-self-center" style={{ fontWeight: "bold", color: "#66aaff" }}>
          Page {page}
        </span>

        <button
          className="pagination-btn"
          onClick={() => handlePageChange(page + 1)}
          disabled={loading || games.length < 12}
        >
          Next
        </button>
      </div>

    </div>
  );
};

export default Store;
