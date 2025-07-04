import { useEffect, useState } from "react";
import { useLocation, Link } from "react-router-dom";

import SearchBar from "../components/SearchBar.jsx";

import appService from "../services/appService";

import "../styles/Store.css"; 

const SearchResults = () => {
  const location = useLocation();
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(true);

  const searchParams = new URLSearchParams(location.search);
  const query = searchParams.get("query") || "";
  const isFree = searchParams.get("isFree") === "true";
  const genres = searchParams.getAll("genres"); 
  const priceSort = searchParams.get("priceSort") || "";
  

  useEffect(() => {
    const fetchResults = async () => {
      try {
        setLoading(true);
        const data = await appService.searchApps(query, isFree, genres, priceSort);
        setResults(data);
      } catch (err) {
        console.error("Search failed", err);
      } finally {
        setLoading(false);
      }
    };

    fetchResults();
  },  [location.search]);

  return (
    <div className="container mt-4">
        <SearchBar/>
        {(query || isFree || genres.length > 0 || priceSort) && (
  <div className="active-filters mb-3">
    <strong>Filtering by:</strong>
    <ul className="filter-list">
      {query && <li>🔍 Query: <em>{query}</em></li>}
      {isFree && <li>🆓 Free Only</li>}
      {genres.length > 0 && <li>🎮 Genres: {genres.join(", ")}</li>}
      {priceSort && (
        <li>💰 Price: {priceSort === "asc" ? "Low to High" : "High to Low"}</li>
      )}
    </ul>
  </div>
)}
      <h2 className="mb-3">Search Results for: <em>"{query}"</em> {isFree && "(Free only)"}</h2>

      {loading ? (
        <p>Loading results...</p>
      ) : results.length === 0 ? (
        <p>No results found.</p>
      ) : (
        <div className="row">
          {results.map((game) => (
            <div key={game.appId} className="col-lg-4 col-md-6 col-sm-12 mb-4">
              <Link to={`/appid/${game.appId}`} className="card-link">
                <div className="card game-card">
                  <img src={game.headerImage} alt={game.name} className="card-img-top" />
                  <div className="card-body">
                    <h5 className="card-title">{game.name}</h5>
                    <p className="card-text">{game.price === "0" ? "Free" : game.price}</p>
                  </div>
                </div>
              </Link>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default SearchResults;
