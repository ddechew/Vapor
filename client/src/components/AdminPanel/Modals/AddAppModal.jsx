import { useState } from "react";
import "../../../styles/AdminModal.css";

const AddAppModal = ({ appTypes, existingApps, onClose, onCreate }) => {
    const [formData, setFormData] = useState({
        appName: "",
        price: 0,
        description: "",
        appTypeId: appTypes.length > 0 ? appTypes[0].appTypeId : 1,
        baseAppId: null
    });

    const handleChange = (field, value) => {
        setFormData(prev => ({ ...prev, [field]: value }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onCreate(formData);
    };

    const isRelatedApp = formData.appTypeId !== 1;

    return (
        <div className="admin-modal-backdrop">
            <div className="admin-modal">
                <h3>Create New App</h3>
                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="App Name"
                        value={formData.appName}
                        onChange={(e) => handleChange("appName", e.target.value)}
                        required
                    />

                    <textarea
                        placeholder="Description"
                        value={formData.description}
                        onChange={(e) => handleChange("description", e.target.value)}
                        required
                        rows={4}
                    />

                    <label>Price (€)</label>
                    <input
                        type="number"
                        min={0}
                        value={formData.price}
                        onChange={(e) => handleChange("price", parseFloat(e.target.value))}
                        required
                    />

                    <label>Application Type</label>
                    <select
                        value={formData.appTypeId}
                        onChange={(e) => handleChange("appTypeId", parseInt(e.target.value))}
                    >
                        {appTypes.map(type => (
                            <option key={type.appTypeId} value={type.appTypeId}>
                                {type.typeName}
                            </option>
                        ))}
                    </select>

                    {isRelatedApp && (
                        <>
                            <label>Base App</label>
                            <select
                                value={formData.baseAppId || ""}
                                onChange={(e) => handleChange("baseAppId", e.target.value === "" ? null : parseInt(e.target.value))}
                            >
                                <option value="">-- Select Base App --</option>
                                {existingApps.map(app => (
                                    <option key={app.appId} value={app.appId}>
                                        {app.name}
                                    </option>
                                ))}
                            </select>
                        </>
                    )}

                    <div className="admin-modal-actions">
                        <button type="submit" className="admin-action-btn">✅ Add</button>
                        <button type="button" className="admin-action-btn" onClick={onClose}>❌ Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AddAppModal;
