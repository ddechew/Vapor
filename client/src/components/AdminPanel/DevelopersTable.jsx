import { useEffect, useState } from "react";

import AddDeveloperModal from "./Modals/AddDeveloperModal";

import adminService from "../../services/adminService";

import "../../styles/AdminTable.css";

const DevelopersTable = () => {
    const [developers, setDevelopers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [editRowId, setEditRowId] = useState(null);
    const [editedDev, setEditedDev] = useState({});
    const [isSaving, setIsSaving] = useState(false);
    const [deletingId, setDeletingId] = useState(null);
    const [showAddModal, setShowAddModal] = useState(false);

    const fetchDevelopers = async () => {
        try {
            const data = await adminService.getDevelopers();
            setDevelopers(data);
        } catch (err) {
            console.error("Failed to fetch developers:", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDevelopers();
    }, []);

    const handleEditClick = (dev) => {
        setEditRowId(dev.developerId);
        setEditedDev({ ...dev });
    };

    const handleCancel = () => {
        setEditRowId(null);
        setEditedDev({});
    };

    const handleFieldChange = (field, value) => {
        setEditedDev(prev => ({ ...prev, [field]: value }));
    };

    const handleSave = async () => {
        if (isSaving) return;
        setIsSaving(true);

        try {
            const original = developers.find(d => d.developerId === editedDev.developerId);
            const hasChanged = editedDev.developerName !== original.developerName;

            if (!hasChanged) {
                alert("No changes made.");
                setEditRowId(null);
                return;
            }

            await adminService.updateDeveloper(editedDev.developerId, editedDev);

            setDevelopers(prev =>
                prev.map(d => (d.developerId === editedDev.developerId ? editedDev : d))
            );

            setEditRowId(null);
            setEditedDev({});
        } catch (err) {
            console.error("Failed to update developer:", err);
            alert("Error updating developer.");
        } finally {
            setIsSaving(false);
        }
    };

    const handleDelete = async (id) => {
        if (deletingId === id) return;
        const confirm = window.confirm("Are you sure you want to delete this developer?");
        if (!confirm) return;

        try {
            setDeletingId(id);
            await adminService.deleteDeveloper(id);
            setDevelopers(prev => prev.filter(d => d.developerId !== id));
        } catch (err) {
            console.error("Failed to delete developer:", err);
            alert("Error deleting developer.");
        } finally {
            setDeletingId(null);
        }
    };

    const exportToJSON = () => {
        const blob = new Blob([JSON.stringify(developers, null, 2)], {
            type: "application/json"
        });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = "developers_export.json";
        link.click();
    };

    const exportToXLSX = async () => {
        try {
            const blob = await adminService.downloadDevelopersExcel();
            const url = window.URL.createObjectURL(new Blob([blob]));
            const link = document.createElement("a");
            link.href = url;
            link.download = "developers_export.xlsx";
            link.click();
        } catch (err) {
            console.error("Failed to export XLSX:", err);
        }
    };

    if (loading) return <p>Loading developers...</p>;

    return (
        <div className="admin-table-container">
            <div className="admin-table-header">
                <h2>🏗️ Developers Table</h2>
                <div className="admin-table-actions">
                    <button onClick={() => setShowAddModal(true)} className="admin-export-btn">➕ Add Developer</button>
                    <button onClick={exportToJSON} className="admin-export-btn">Export JSON</button>
                    <button onClick={exportToXLSX} className="admin-export-btn">Export XLSX</button>
                </div>
            </div>

            <div className="admin-table-scroll">
                <table className="admin-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Developer Name</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {developers.map((d) => (
                            <tr key={d.developerId}>
                                <td>{d.developerId}</td>
                                <td>
                                    {editRowId === d.developerId ? (
                                        <input
                                            value={editedDev.developerName}
                                            onChange={(e) => handleFieldChange("developerName", e.target.value)}
                                        />
                                    ) : (
                                        d.developerName
                                    )}
                                </td>
                                <td>
                                    {editRowId === d.developerId ? (
                                        <>
                                            <button
                                                className="admin-action-btn"
                                                onClick={handleSave}
                                                disabled={isSaving}
                                            >
                                                {isSaving ? "⏳" : "💾"}
                                            </button>
                                            <button className="admin-action-btn" onClick={handleCancel}>❌</button>
                                        </>
                                    ) : (
                                        <>
                                            <button className="admin-action-btn" onClick={() => handleEditClick(d)}>✏️</button>
                                            <button
                                                className="admin-action-btn"
                                                onClick={() => handleDelete(d.developerId)}
                                                disabled={deletingId === d.developerId}
                                            >
                                                {deletingId === d.developerId ? "⏳" : "🗑️"}
                                            </button>
                                        </>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            {showAddModal && (
                <AddDeveloperModal
                    onClose={() => setShowAddModal(false)}
                    onCreate={async (newDev) => {
                        try {
                            const created = await adminService.createDeveloper(newDev);
                            setDevelopers(prev => [...prev, created]);
                            setShowAddModal(false);
                        } catch (err) {
                            console.error("Failed to create developer:", err);
                            alert("Error creating developer.");
                        }
                    }}
                />
            )}

        </div>
    );
};

export default DevelopersTable;
