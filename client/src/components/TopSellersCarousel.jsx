import { useState, useEffect } from "react";
import { Link } from "react-router-dom";

import Slider from "react-slick";
import SafeYouTubePlayer from "./SafeYouTubePlayer";

import appService from "../services/appService";

import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import "../styles/TopSellersCarousel.css";

const TopSellersCarousel = () => {
  const [topGames, setTopGames] = useState([]);
  const [selectedGame, setSelectedGame] = useState(null);
  const [sliderRef, setSliderRef] = useState(null);

  useEffect(() => {
    const fetchTopGames = async () => {
      const top5 = await appService.getTop5Apps();
      setTopGames(top5);
      if (top5.length > 0) {
        setSelectedGame(top5[0]);
      }
    };

    fetchTopGames();
  }, []);

  const sliderSettings = {
    dots: false,
    infinite: true,
    speed: 500,
    slidesToShow: 1,
    slidesToScroll: 1,
    autoplay: true,
    autoplaySpeed: 4000,
    beforeChange: (_, next) => setSelectedGame(topGames[next]),
  };

  return (
    <div className="store-container">
      <h2 className="top-sellers-title">🔥 Top Sellers 🔥</h2>

      <div className="carousel-container">
        <button className="carousel-nav left" onClick={() => sliderRef?.slickPrev()}>
          &#10094;
        </button>

        <div className="carousel-left">
          <Slider ref={setSliderRef} {...sliderSettings} className="top-games-carousel">
            {topGames.map((game) => (
              <div key={game.appId} className="carousel-item">
                <div className="video-container">
                  {game.trailer.includes("youtube.com") ? (
                    (() => {
                      const videoId = new URL(game.trailer).pathname.split("/").pop();
                      const opts = {
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
                        <SafeYouTubePlayer
                          videoId={videoId}
                          opts={opts}
                          className="carousel-iframe"
                        />

                      );
                    })()
                  ) : (
                    <video className="carousel-video" autoPlay muted loop disablePictureInPicture>
                      <source src={game.trailer} type="video/mp4" />
                      Your browser does not support the video tag.
                    </video>
                  )}
                </div>
              </div>
            ))}
          </Slider>
        </div>

        <button className="carousel-nav right" onClick={() => sliderRef?.slickNext()}>
          &#10095;
        </button>

        <div className="carousel-right">
          {selectedGame ? (
            <>
              <h3>{selectedGame.name}</h3>
              <p className="game-description">{selectedGame.shortDescription || "No description available."}</p>
              <p><strong>Genres:</strong> {selectedGame.genres?.slice(0, 3).join(", ") || "Unknown"}</p>
              <p><strong>Release Date:</strong> {selectedGame.releaseDate || "Unknown"}</p>
              <p>{selectedGame.price}</p>
              <Link to={`/appid/${selectedGame.appId}`} className="view-btn">
                View Game
              </Link>
            </>
          ) : (
            <p>Select a game to see details...</p>
          )}
        </div>
      </div>

      <div className="carousel-indicators">
        {topGames.map((game, index) => (
          <span
            key={game.appId}
            className={`indicator ${selectedGame?.appId === game.appId ? "active" : ""}`}
            onClick={() => sliderRef?.slickGoTo(index)}
          />
        ))}
      </div>
    </div>
  );
};

export default TopSellersCarousel;
