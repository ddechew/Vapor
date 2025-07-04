import { useEffect, useState } from "react";

import AddAppModal from "./Modals/AddAppModal";

import adminService from "../../services/adminService";

import "../../styles/AdminModal.css";
import "../../styles/AdminTable.css";

const AppsTable = () => {
    const [apps, setApps] = useState([]);
    const [appTypes, setAppTypes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showAddModal, setShowAddModal] = useState(false);
    const [editRowId, setEditRowId] = useState(null);
    const [editedApp, setEditedApp] = useState({});
    const [deletingId, setDeletingId] = useState(null);
    const [isSaving, setIsSaving] = useState(false);

    const fetchAll = async () => {
        try {
            const [appsData, typesData] = await Promise.all([
                adminService.getApps(),
                adminService.getAppTypes()
            ]);
            setApps(appsData);
            setAppTypes(typesData);
        } catch (err) {
            console.error("Failed to fetch apps data:", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchAll();
    }, []);

    const handleAddApp = async (newApp) => {
        try {
            await adminService.createApp(newApp);
            await fetchAll(); // refresh UI
            setShowAddModal(false);
        } catch (err) {
            alert(err.message.includes("already exists")
                ? "An app with this name already exists."
                : "Failed to create app: " + (err.message || "Unknown error"));
        }
    };

    const handleEditClick = (app) => {
        setEditRowId(app.appId);
        setEditedApp({ ...app });
    };

    const handleCancel = () => {
        setEditRowId(null);
        setEditedApp({});
    };

    const handleFieldChange = (field, value) => {
        setEditedApp(prev => ({ ...prev, [field]: value }));
    };

    const handleSave = async () => {
        if (isSaving) return;
        setIsSaving(true);
        try {
            await adminService.updateApp(editedApp.appId, editedApp);
            await fetchAll(); 
            handleCancel();
        } catch (err) {
            alert("Error updating app: " + err.message);
        } finally {
            setIsSaving(false);
        }
    };

    const handleDelete = async (appId) => {
        if (!window.confirm("Are you sure you want to delete this app?")) return;
        try {
            setDeletingId(appId);
            await adminService.deleteApp(appId);
            setApps(prev => prev.filter(app => app.appId !== appId));
        } catch (err) {
            alert("Error deleting app: " + err.message);
        } finally {
            setDeletingId(null);
        }
    };


    const exportToJSON = () => {
        const blob = new Blob([JSON.stringify(apps, null, 2)], { type: "application/json" });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = "apps_export.json";
        link.click();
    };


    if (loading) return <p>Loading apps...</p>;

    return (
        <div className="admin-table-container">
            <div className="admin-table-header">
                <h2>🎮 Applications</h2>
                <div className="admin-table-actions">
                    <button className="admin-export-btn" onClick={() => setShowAddModal(true)}>
                        ➕ Add App
                    </button>
                    <button className="admin-export-btn" onClick={exportToJSON}>
                        Export JSON
                    </button>
                </div>
            </div>

            <div className="admin-table-scroll">
                <table className="admin-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Price</th>
                            <th>Type</th>
                            <th>Description</th>
                        </tr>
                    </thead>
                    {apps.map((app) => (
                        <tr key={app.appId}>
                            <td>{app.appId}</td>
                            <td>
                                {editRowId === app.appId ? (
                                    <input value={editedApp.name} onChange={(e) => handleFieldChange("name", e.target.value)} />
                                ) : app.name}
                            </td>
                            <td>
                                {editRowId === app.appId ? (
                                    <input type="number" 
                                           min={0} 
                                           value={editedApp.price} 
                                           onChange={(e) => handleFieldChange("price", parseFloat(e.target.value))} />
                                ) : (app.price === 0 ? "Free" : `€${app.price?.toFixed(2)}`)}
                            </td>
                            <td>
                                {editRowId === app.appId ? (
                                    <>
                                        <select
                                            value={editedApp.appTypeId}
                                            onChange={(e) => handleFieldChange("appTypeId", parseInt(e.target.value))}
                                        >
                                            {appTypes.map(type => (
                                                <option key={type.appTypeId} value={type.appTypeId}>
                                                    {type.typeName}
                                                </option>
                                            ))}
                                        </select>

                                        {editedApp.appTypeId !== 1 && (
                                            <div style={{ marginTop: "4px" }}>
                                                <label style={{ fontSize: "0.85em", marginRight: "5px" }}>Base App:</label>
                                                <select
                                                    value={editedApp.baseAppId || ""}
                                                    onChange={(e) =>
                                                        handleFieldChange(
                                                            "baseAppId",
                                                            e.target.value === "" ? null : parseInt(e.target.value)
                                                        )
                                                    }
                                                >
                                                    <option value="">-- Select Base App --</option>
                                                    {apps
                                                        .filter(a => a.appTypeId === 1 && a.appId !== editedApp.appId)
                                                        .map(base => (
                                                            <option key={base.appId} value={base.appId}>
                                                                {base.name}
                                                            </option>
                                                        ))}
                                                </select>
                                            </div>
                                        )}
                                    </>
                                ) : (
                                    app.appTypeName
                                )}
                            </td>

                            <td style={{ maxWidth: "400px" }}>
                                {editRowId === app.appId ? (
                                    <textarea value={editedApp.description} onChange={(e) => handleFieldChange("description", e.target.value)} />
                                ) : app.description}
                            </td>
                            <td>
                                {editRowId === app.appId ? (
                                    <>
                                        <button onClick={handleSave} className="admin-action-btn">💾</button>
                                        <button onClick={handleCancel} className="admin-action-btn">❌</button>
                                    </>
                                ) : (
                                    <>
                                        <button onClick={() => handleEditClick(app)} className="admin-action-btn">✏️</button>
                                        <button onClick={() => handleDelete(app.appId)} className="admin-action-btn" disabled={deletingId === app.appId}>
                                            {deletingId === app.appId ? "⏳" : "🗑️"}
                                        </button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}

                </table>
            </div>

            {showAddModal && (
                <AddAppModal
                    appTypes={appTypes}
                    existingApps={apps}
                    onClose={() => setShowAddModal(false)}
                    onCreate={handleAddApp}
                />
            )}
        </div>
    );
};

export default AppsTable;
