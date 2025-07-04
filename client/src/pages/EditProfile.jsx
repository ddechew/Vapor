import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";

import ImageCropper from "../components/ImageCropper";

import AuthContext from "../context/AuthContext";
import userService from "../services/userService";
import authService from "../services/authService";

import "../styles/EditProfile.css";

const EditProfile = () => {
    const { user, updateUserInfo } = useContext(AuthContext);
    const [displayName, setDisplayName] = useState(user?.displayName || "");
    const [profilePicture, setProfilePicture] = useState(user?.profilePicture || "");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);
    const [cropping, setCropping] = useState(false);
    const [croppedBlob, setCroppedBlob] = useState(null);
    const [imageToCrop, setImageToCrop] = useState(null);
    const navigate = useNavigate();

    const originalDisplayName = user?.displayName || "";
    const originalProfilePicture = user?.profilePicture || "";
    const hasChanges = displayName !== originalDisplayName || profilePicture !== originalProfilePicture;

    const handleFileSelect = (e) => {
        const file = e.target.files[0];
        if (file) {
            const imageUrl = URL.createObjectURL(file);
            setImageToCrop(imageUrl);
            setCropping(true);
        }
    };

    const handleCropDone = async (blob) => {
        setCropping(false);
        setImageToCrop(null);
        setCroppedBlob(blob);
        const previewUrl = URL.createObjectURL(blob);
        setProfilePicture(previewUrl);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess(false);

        let uploadedImageUrl = profilePicture;

        try {
            if (croppedBlob) {
                const { imageUrl } = await userService.uploadProfilePicture(croppedBlob);
                uploadedImageUrl = imageUrl;
            }

            await userService.updateProfile({
                displayName,
                profilePicture: uploadedImageUrl,
            });

            const updatedUser = { ...user, displayName, profilePicture: uploadedImageUrl };

            updateUserInfo(updatedUser);
            
            
            setCropping(false);
            setSuccess(true);
            
            await authService.regenerateTokens();
            setTimeout(() => navigate(`/profile/${user.username}`), 2000);
        } catch (err) {
            setError(err.message || "❌ Failed to update profile.");
        }
    };

    return (
        <div className="edit-profile-wrapper">
            <div className="edit-profile-container">
                <h2>Edit Your Profile</h2>
                <form onSubmit={handleSubmit}>
                    <label>Display Name:</label>
                    <input
                        type="text"
                        value={displayName}
                        onChange={(e) => setDisplayName(e.target.value)}
                        required
                    />
                    <label>Upload New Profile Picture:</label>
                    <input type="file" accept="image/*" onChange={handleFileSelect} />

                    {profilePicture && !cropping && (
                        <div className="image-preview-wrapper">
                            <p>Current Profile Picture:</p>
                            <img
                                src={profilePicture}
                                alt="Selected Profile"
                                className="profile-preview"
                                style={{ width: 128, height: 128, borderRadius: "50%", objectFit: "cover" }}
                            />
                        </div>
                    )}

                    <button
                        type="submit"
                        className="btn btn-primary"
                        disabled={!hasChanges}
                        style={{ opacity: hasChanges ? 1 : 0.5, cursor: hasChanges ? "pointer" : "not-allowed" }}
                    >
                        Save Changes
                    </button>
                </form>

                {success && <p className="success-msg">✅ Profile updated successfully! Redirecting...</p>}
                {error && <p className="error-msg">❌ {error}</p>}

                {cropping ? (
                    <ImageCropper
                        image={imageToCrop}
                        onCropDone={handleCropDone}
                        onCancel={() => setCropping(false)}
                    />
                ) : null}
            </div>
        </div>
    );
};

export default EditProfile;