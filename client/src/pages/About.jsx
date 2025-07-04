import { useEffect, useState } from "react";

import LoadingSpinner from "../components/LoadingSpinner";

import "../styles/About.css"; 

const About = () => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(false);
  }, []);

  if (loading) return <LoadingSpinner />;
  return (
    <div className="about-container">
      <h1>About Vapor</h1>

      <section className="about-section">
        <h2>🎮 What is Vapor?</h2>
        <p>
          Vapor is your next-generation game store experience — offering an
          intuitive platform for discovering, purchasing, and managing your
          favorite games. Inspired by Steam, we combine a modern UI with
          powerful features such as cart syncing, wallet payments, media
          previews, and a customizable game library.
        </p>
      </section>

      <section className="about-section">
        <h2>💡 How the Point System Works</h2>
        <ul>
          <li>
            🪙 <strong>Earn points</strong> for every paid game you purchase.
            For every <strong>1.00€</strong> you spend, you receive{" "}
            <strong>1 point</strong>.
          </li>
          <li>
            🔁 <strong>Points are added</strong> automatically after a
            successful purchase.
          </li>
          <li>
            🛍️ Once you reach a certain amount of points (e.g.{" "}
            <strong>100 points</strong>), you can use them to get discounts on
            future purchases.
          </li>
          <li>
            💥 You can choose to <strong>redeem your points</strong> at
            checkout if you have enough to unlock a reward tier.
          </li>
        </ul>
      </section>

      <section className="about-section">
        <h2>📜 Example</h2>
        <p>
          If you buy a game that costs <strong>29.99€</strong>, you earn{" "}
          <strong>29 points</strong>. Once you collect 100 points, you'll be
          able to redeem them for a <strong>5€ discount</strong>.
        </p>
      </section>

      <section className="about-section">
        <h2>📌 Notes</h2>
        <ul>
          <li>Points do not expire.</li>
          <li>Free games do not generate points.</li>
          <li>
            Discounts via points can't be stacked with other promotions.
          </li>
        </ul>
      </section>
    </div>
  );
};

export default About;
