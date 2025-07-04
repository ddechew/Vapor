import { Link } from "react-router-dom";
import "../styles/RelatedApps.css";

const RelatedApps = ({ relatedApps }) => {
    if (!relatedApps || relatedApps.length === 0) return null;

    return (
        <>
            <hr className="section-divider" />
            <h3 className="description-heading">Additional Content</h3>
            <div className="dlc-container">
                {relatedApps.map((app) => (
                    <Link to={`/appid/${app.appId}`} key={app.appId} className="dlc-card">
                        <img src={app.headerImage} alt={app.name} className="dlc-image" />
                        <p className="dlc-title">{app.name}</p>
                    </Link>
                ))}
            </div>
        </>
    );
};

export default RelatedApps;
