import "../styles/About.css"; 

const TermsOfService = () => {
  return (
    <div className="about-container">
      <h1>Terms of Service</h1>

      <section className="about-section">
        <h2>🧾 Acceptance of Terms</h2>
        <p>
          By using Vapor, you agree to these Terms of Service. Please read them carefully. If you do not agree, you should not use the platform.
        </p>
      </section>

      <section className="about-section">
        <h2>🕹️ Use of the Platform</h2>
        <ul>
          <li>You must create an account to purchase, download, and access certain features.</li>
          <li>You are responsible for maintaining the confidentiality of your account information.</li>
          <li>You agree to use the platform in compliance with all applicable laws and regulations.</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>🛒 Purchases and Refunds</h2>
        <ul>
          <li>All purchases are final unless stated otherwise in our Refund Policy.</li>
          <li>Prices are subject to change without notice.</li>
          <li>Any promotions or discounts are subject to specific terms outlined at the time of the offer.</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>🚫 Prohibited Activities</h2>
        <ul>
          <li>Unauthorized sharing or resale of games or account information.</li>
          <li>Using the platform for unlawful purposes.</li>
          <li>Disrupting, hacking, or damaging the service or other users’ experience.</li>
        </ul>
      </section>

      <section className="about-section">
        <h2>🔧 Changes to the Service</h2>
        <p>
          We reserve the right to modify, suspend, or discontinue the platform (or any part of it) at any time with or without notice.
        </p>
      </section>

      <section className="about-section">
        <h2>📅 Changes to the Terms</h2>
        <p>
          We may update these Terms of Service from time to time. We will notify users of significant changes through the platform or via email.
        </p>
      </section>

      <section className="about-section">
        <h2>📬 Contact Us</h2>
        <p>
          If you have any questions about these Terms of Service, please reach out to our support team.
        </p>
      </section>
    </div>
  );
};

export default TermsOfService;
