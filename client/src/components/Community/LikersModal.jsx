import { Link } from "react-router-dom";

import "../../styles/LikersModal.css"

const LikersModal = ({ likers, onClose }) => (
  <div className="likers-modal-backdrop" onClick={onClose}>
    <div className="likers-modal" onClick={(e) => e.stopPropagation()}>
      <h4>❤️ Liked by:</h4>
      <ul>
        {likers.map((user) => (
          <li key={user.username}>
            <Link to={`/profile/${user.username}`} className="liker-entry">
              <img src={user.profilePicture || "/assets/default-profile.jpg"} alt="Avatar" />
              <span>{user.displayName}</span>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  </div>
);

export default LikersModal;
