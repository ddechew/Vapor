import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";

import { useWishlist } from "../context/WishlistContext";
import { useCart } from "../context/CartContext"; 

import appService from "../services/appService";

import "../styles/SearchBar.css";
import cartIcon from "../assets/shopping_cart.png"; 
import wishlistIcon from "../assets/wishlistHeart.png";

const SearchBar = ({ onSuggestionClick }) => {
  const [query, setQuery] = useState("");
  const [isFree, setIsFree] = useState(false);
  const [genres, setGenres] = useState([]); 
  const [selectedGenres, setSelectedGenres] = useState([]);
  const [suggestions, setSuggestions] = useState([]);
  const [priceSort, setPriceSort] = useState("");
  const { cart } = useCart();
  const { wishlist } = useWishlist();

  const navigate = useNavigate();

  useEffect(() => {
    appService.getGenres().then(setGenres);
  }, []);

  useEffect(() => {
    const delay = setTimeout(async () => {
      if (query.trim().length > 0) {
        try {
          const data = await appService.searchApps(query, isFree, selectedGenres, priceSort);
          setSuggestions(data.slice(0, 6));
        } catch (err) {
          console.error("Suggestion fetch failed:", err);
        }
      } else {
        setSuggestions([]); 
      }
    }, 250);

    return () => clearTimeout(delay);
  }, [query, isFree, selectedGenres, priceSort]);


  const handleSearch = () => {
    const trimmedQuery = query.trim();
    const hasFilters = isFree || selectedGenres.length > 0 || priceSort;

    if (!trimmedQuery && !hasFilters) {
      navigate("/"); 
      return;
    }

    const params = new URLSearchParams();
    if (trimmedQuery) params.append("query", trimmedQuery);
    if (isFree) params.append("isFree", true);
    if (priceSort) params.append("priceSort", priceSort); 
    selectedGenres.forEach(g => params.append("genres", g));

    navigate(`/search?${params.toString()}`);
  };


  const handleSuggestionClick = (app) => {
    onSuggestionClick?.(app);
    setSuggestions([]);
    setQuery("");
  };

  return (
    <div className="search-bar-wrapper">
      {cart.length > 0 && (
        <div className="cart-container">
          <Link to="/cart" className="cart-link">
            <img src={cartIcon} alt="Cart" className="cart-icon" />
            <span className="cart-count">({cart.length})</span>
          </Link>
        </div>
      )}
      {wishlist.length > 0 && (
        <div className="cart-container">
          <Link to="/wishlist" className="cart-link">
            <img src={wishlistIcon} alt="Wishlist" className="cart-icon" />
            <span className="cart-count">({wishlist.length})</span>
          </Link>
        </div>
      )}

      <div className="search-bar">
        <input
          type="text"
          placeholder="Search for a game..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />

        <details className="filters-dropdown">
          <summary>Filters</summary>
          <div className="filters-menu">
            <label>
              <input
                type="checkbox"
                checked={isFree}
                onChange={() => setIsFree(!isFree)}
              />
              Free only
            </label>

            <label>
              Price:
              <select
                value={priceSort}
                onChange={(e) => setPriceSort(e.target.value)}
                className="sort-dropdown"
              >
                <option value="">No sorting</option>
                <option value="asc">Ascending</option>
                <option value="desc">Descending</option>
              </select>
            </label>

            <div className="genre-checkboxes">
              <label>Genres:</label>
              <div className="genre-options">
                {genres.map((g) => {
                  const genreName = g.genreName || g; 
                  return (
                    <label key={genreName} className="genre-option">
                      <input
                        type="checkbox"
                        value={genreName}
                        checked={selectedGenres.includes(genreName)}
                        onChange={(e) => {
                          const checked = e.target.checked;
                          setSelectedGenres((prev) =>
                            checked ? [...prev, genreName] : prev.filter((g) => g !== genreName)
                          );
                        }}
                      />
                      {genreName}
                    </label>
                  );
                })}
              </div>
            </div>

            <button
              type="button"
              className="clear-filters-btn"
              onClick={() => {
                setIsFree(false);
                setSelectedGenres([]);
                setPriceSort("");
              }}
            >
              Clear Filters
            </button>

          </div>
        </details>

        <button onClick={handleSearch}>Search</button>
      </div>

      {suggestions.length > 0 && (
        <ul className="search-suggestions">
          {suggestions.map((app) => (
            <li key={app.appId} onClick={() => handleSuggestionClick(app)}>
              <img src={app.headerImage} alt={app.name} />
              <div className="suggestion-info">
                <strong>{app.name}</strong>
                <span>{app.price === "0" ? "Free" : app.price}</span>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default SearchBar;
