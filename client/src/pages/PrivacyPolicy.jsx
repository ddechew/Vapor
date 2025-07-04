import "../styles/About.css"; 

const PrivacyPolicy = () => {
  return (
    <div className="about-container">
      <h1>Privacy Policy</h1>

      <section className="about-section">
        <h2>🔒 Your Privacy Matters</h2>
        <p>
          At Vapor, we are committed to protecting your personal information. This Privacy Policy explains how we collect, use, and protect your data when you use our platform.
        </p>
      </section>

      <section className="about-section">
        <h2>📋 Information We Collect</h2>
        <ul>
          <li>Account information (such as your username, email address, and profile picture)</li>
          <li>Purchase history and wallet transactions</li>
          <li>Game library and activity within the community</li>
          <li>Technical data (such as IP address, device information, browser type)</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>🛡️ How We Use Your Information</h2>
        <ul>
          <li>To manage your account and deliver purchased games</li>
          <li>To process payments securely</li>
          <li>To personalize your experience (e.g., point rewards, recommendations)</li>
          <li>To provide customer support and communicate with you</li>
          <li>To improve the platform’s functionality and security</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>🤝 Data Sharing</h2>
        <p>
          We do not sell your personal data. We only share information with trusted partners necessary for payment processing, fraud prevention, and technical support.
        </p>
      </section>

      <section className="about-section">
        <h2>⚙️ Your Choices</h2>
        <ul>
          <li>You can update your profile and preferences at any time.</li>
          <li>You can request deletion of your account and data by contacting support.</li>
          <li>You can opt out of marketing emails (if any are implemented).</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>📅 Changes to This Policy</h2>
        <p>
          We may update our Privacy Policy from time to time. Any changes will be posted on this page, and major updates will be communicated to users via email or app notifications.
        </p>
      </section>

      <section className="about-section">
        <h2>📬 Contact Us</h2>
        <p>
          If you have any questions about this Privacy Policy, please contact our support team.
        </p>
      </section>
    </div>
  );
};

export default PrivacyPolicy;
