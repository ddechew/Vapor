import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import userService from "../services/userService";

import "../styles/PurchaseHistory.css";

const PurchaseHistory = () => {
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchHistory = async () => {
      try {
        const data = await userService.getPurchaseHistory();
        setHistory(data);
      } catch (err) {
        console.error("Failed to load purchase history:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchHistory();
  }, []);

  if (loading) return <p>Loading purchase history...</p>;
  if (history.length === 0) return <p>You have no purchases yet.</p>;

  return (
    <div className="purchase-history-container">
      <h2>Your Purchase History</h2>
      <table className="purchase-history-table">
        <thead>
          <tr>
            <th>App</th>
            <th>Date</th>
            <th>Payment</th>
            <th>Price</th>
            <th>Wallet Δ</th>
            <th>Wallet Balance</th>
          </tr>
        </thead>
        <tbody>
          {history.map((entry) => (
            <tr key={`${entry.purchaseDate}-${entry.appId}`}>
              <td className="app-info">
                {entry.appId == null ? (
                  <span className="app-link">💰 Wallet Top-Up</span>
                ) : (
                  <Link to={`/appid/${entry.appId}`} className="app-link">
                    {entry.appHeaderImage && (
                      <img
                        src={entry.appHeaderImage}
                        alt={entry.appName}
                        className="app-thumbnail"
                      />
                    )}
                    {entry.appName}
                  </Link>
                )}
              </td>

              <td>{new Date(entry.purchaseDate).toLocaleString()}</td>
              <td>{entry.paymentMethod}</td>
              <td>{entry.priceAtPurchase.toFixed(2)}€</td>
              <td>
                {entry.walletChange !== null
                  ? `${entry.walletChange > 0 ? "+" : ""}${entry.walletChange.toFixed(2)}€`
                  : "—"}

              </td>
              <td>
                {entry.walletBalanceAfter !== null
                  ? `${entry.walletBalanceAfter.toFixed(2)}€`
                  : "—"}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PurchaseHistory;
