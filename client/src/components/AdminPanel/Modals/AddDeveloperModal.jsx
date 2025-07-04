import { useState } from "react";
import "../../../styles/AdminModal.css";

const AddDeveloperModal = ({ onClose, onCreate }) => {
  const [developerName, setDeveloperName] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!developerName.trim()) {
      alert("Developer name cannot be empty.");
      return;
    }
    onCreate({ developerName: developerName.trim() });
  };

  return (
    <div className="admin-modal-backdrop">
      <div className="admin-modal">
        <h3>Create New Developer</h3>
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            placeholder="Developer Name"
            value={developerName}
            onChange={(e) => setDeveloperName(e.target.value)}
            required
          />
          <div className="admin-modal-actions">
            <button type="submit" className="admin-action-btn">✅ Add</button>
            <button type="button" className="admin-action-btn" onClick={onClose}>❌ Cancel</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddDeveloperModal;
