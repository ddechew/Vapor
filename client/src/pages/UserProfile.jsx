import { useEffect, useState, useContext } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import userService from "../services/userService";
import AuthContext from "../context/AuthContext";
import LoadingSpinner from "../components/LoadingSpinner"
import "../styles/UserProfile.css";

const UserProfile = () => {
    const { username } = useParams();
    const { user: loggedInUser } = useContext(AuthContext);
    const navigate = useNavigate();

    const [profile, setProfile] = useState(null);
    const [library, setLibrary] = useState([]);
    const [loading, setLoading] = useState(true);
    const isOwnProfile = loggedInUser?.username === username;

    useEffect(() => {
        const fetchData = async () => {
            try {
                const profileData = await userService.getUserByUsername(username);
                setProfile(profileData);

                const libraryData = await userService.getUserLibraryByUsername(username);
                const filtered = libraryData.filter(item => item.appTypeId === 1); 
                setLibrary(filtered);
            } catch (err) {
                console.error("Failed to load profile:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [username]);

    if (loading) return <LoadingSpinner/>;
    if (!profile) return <div className="container">User not found.</div>;

    return (
        <div className="profile-container">
            <div className="profile-header">
                <img
                    src={profile.profilePicture}
                    alt="Profile"
                    className="profile-picture"
                />
                <div>
                    <h2>{profile.displayName || profile.username}</h2>
                    <p>@{profile.username}</p>
                </div>
                {isOwnProfile && (
                    <button
                    onClick={() => navigate("/account/edit")}
                    className="edit-profile-btn"
                  >
                    Edit Profile
                  </button>
                )}
            </div>

            <div className="profile-library">
                <h3>Owned Games</h3>
                {library.length === 0 ? (
                    <p>This user doesn't own any games yet.</p>
                ) : (
                    <div className="profile-app-list">
                        {library.map(app => (
                            <Link to={`/appid/${app.appId}`} className="profile-app-link" key={app.appId}>
                                <div className="profile-app-row">
                                    <img src={app.headerImage} alt={app.appName} />
                                    <div className="profile-app-info">
                                        <h4>{app.appName}</h4>
                                        <p className="purchase-date">
                                            Purchased on:{" "}
                                            {new Date(app.purchaseDate).toLocaleDateString(undefined, {
                                                year: "numeric",
                                                month: "short",
                                                day: "numeric"
                                            })}
                                        </p>
                                    </div>
                                </div>
                            </Link>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default UserProfile;
