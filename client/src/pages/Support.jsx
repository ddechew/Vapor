import "../styles/About.css"; 

const Support = () => {
  return (
    <div className="about-container">
      <h1>Support</h1>

      <section className="about-section">
        <h2>🛠️ We're Here to Help</h2>
        <p>
          If you encounter any issues or have questions about Vapor, our support team is ready to assist you.
        </p>
      </section>

      <section className="about-section">
        <h2>📚 Common Support Topics</h2>
        <ul>
          <li>Account management and login issues</li>
          <li>Payment problems or wallet inquiries</li>
          <li>Game download and installation support</li>
          <li>Community guidelines and post/report concerns</li>
          <li>Point rewards and redemption questions</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>✉️ How to Contact Support</h2>
        <ul>
          <li>Visit our in-app Support Center (coming soon)</li>
          <li>Send us an email at <strong>support@vaporplatform.com</strong></li>
          <li>Open a support ticket directly from your account settings</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>⏳ Response Times</h2>
        <p>
          We aim to respond to all inquiries within <strong>24 to 48 hours</strong>. During busy periods, responses may take slightly longer.
        </p>
      </section>

      <section className="about-section">
        <h2>📢 Important Notes</h2>
        <ul>
          <li>Please provide as much detail as possible when contacting support (screenshots, error messages, etc.).</li>
          <li>For refund-related questions, please review our <a href="/refund-policy">Refund Policy</a> before submitting a request.</li>
        </ul>
      </section>
    </div>
  );
};

export default Support;
