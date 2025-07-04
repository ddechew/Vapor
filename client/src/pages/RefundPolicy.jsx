import "../styles/About.css"; 

const RefundPolicy = () => {
  return (
    <div className="about-container">
      <h1>Refund Policy</h1>

      <section className="about-section">
        <h2>💸 Our Commitment</h2>
        <p>
          At Vapor, we want you to be satisfied with your purchases. This Refund Policy outlines when and how you may request a refund.
        </p>
      </section>

      <section className="about-section">
        <h2>🕒 Refund Eligibility</h2>
        <ul>
          <li>Refunds are available within <strong>14 days</strong> of purchase if the game has been played for less than <strong>2 hours</strong>.</li>
          <li>In-app purchases, DLCs, and other additional content may have different eligibility conditions.</li>
          <li>Gift purchases can only be refunded to the original purchaser.</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>🚫 Non-Refundable Items</h2>
        <ul>
          <li>Games that have been significantly used (played more than 2 hours).</li>
          <li>Items purchased during promotional events that explicitly state "non-refundable."</li>
          <li>Wallet top-ups or point redemptions are non-refundable.</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>📋 How to Request a Refund</h2>
        <ul>
          <li>Go to your Purchase History and locate the item you wish to refund.</li>
          <li>Click on "Request Refund" and provide a brief reason for your request.</li>
          <li>Our support team will review and process your request within 7 business days.</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>⚖️ Final Decision</h2>
        <p>
          All refund decisions are made at the sole discretion of Vapor. Abuse of the refund system may result in loss of refund privileges or account suspension.
        </p>
      </section>

      <section className="about-section">
        <h2>📬 Need Help?</h2>
        <p>
          If you have questions or concerns regarding refunds, please contact our support team for assistance.
        </p>
      </section>
    </div>
  );
};

export default RefundPolicy;
