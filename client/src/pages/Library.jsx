import { useEffect, useState, useContext } from "react";
import { Link } from "react-router-dom";

import LoadingSpinner from "../components/LoadingSpinner";

import AuthContext from "../context/AuthContext";
import userService from "../services/userService";

import "../styles/Library.css";

const Library = () => {
  const [library, setLibrary] = useState([]);
  const [loading, setLoading] = useState(true);
  const { user } = useContext(AuthContext);

  useEffect(() => {
    const fetchLibrary = async () => {
      try {
        setLoading(true);
        const data = await userService.getUserLibrary();
        setLibrary(data);
      } catch (err) {
        console.error("Failed to load library:", err.message);
      } finally {
        setLoading(false);
      }
    };
    fetchLibrary();
  }, []);

  return loading ? (
    <LoadingSpinner />
  ) : (
    <div className="library-container">
      <h1>Your Game Library</h1>
      {library.length === 0 ? (
        <div>
          <p>You haven't added any games yet.</p>
          <Link to={'/'}>Go to Store</Link>
        </div>
      ) : (
        library.map((game) => (
          <div key={game.appId} className="library-game">
            <Link to={`/appid/${game.appId}`} className="library-item">
              <img
                src={game.headerImage || "/default-header.jpg"} 
                alt={game.appName}
                className="library-thumb"
              />
              <span className="library-title">{game.appName}</span>
            </Link>

            {game.relatedApps.length > 0 && (
              <div className="library-related">
                {game.relatedApps.map((ra) => (
                  <div
                    key={ra.appId}
                    className={`related-app ${ra.isOwned ? "owned" : "not-owned"}`}
                  >
                    <Link to={`/appid/${ra.appId}`} className="related-link">
                      <img
                        src={ra.headerImage || "/default-header.jpg"}
                        alt={ra.appName}
                        className="related-thumb"
                      />
                      <div className="related-info">
                        <span className="related-name">{ra.appName}</span>
                        <span className="related-price">
                          {ra.price === 0 ? "Free" : `${ra.price}€`}
                        </span>
                        {!ra.isOwned && <span className="not-owned-text">❌ Not Owned</span>}
                      </div>
                    </Link>
                  </div>
                ))}
              </div>
            )}
          </div>
        ))
      )}
    </div>
  );
};

export default Library;
