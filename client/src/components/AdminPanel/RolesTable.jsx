import { useEffect, useState } from "react";

import adminService from "../../services/adminService";

import "../../styles/AdminTable.css";

const RolesTable = () => {
    const [roles, setRoles] = useState([]);
    const [loading, setLoading] = useState(true);
    const [editRowId, setEditRowId] = useState(null);
    const [editedRole, setEditedRole] = useState({});
    const [deletingId, setDeletingId] = useState(null);
    const [isSaving, setIsSaving] = useState(false);

    const fetchRoles = async () => {
        try {
            const data = await adminService.getRoles();
            setRoles(data);
        } catch (err) {
            console.error("Failed to fetch roles:", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRoles();
    }, []);

    const handleEditClick = (role) => {
        setEditRowId(role.roleId);
        setEditedRole({ ...role });
    };

    const handleCancel = () => {
        setEditRowId(null);
        setEditedRole({});
    };

    const handleFieldChange = (field, value) => {
        setEditedRole(prev => ({ ...prev, [field]: value }));
    };

    const handleSave = async () => {
        if (isSaving) return;
        setIsSaving(true);

        try {
            const original = roles.find(r => r.roleId === editedRole.roleId);
            const hasChanged = Object.keys(editedRole).some(key => editedRole[key] !== original[key]);

            if (!hasChanged) {
                alert("No changes made.");
                setEditRowId(null);
                return;
            }

            await adminService.updateRole(editedRole.roleId, editedRole);

            setRoles(prev =>
                prev.map(r => (r.roleId === editedRole.roleId ? editedRole : r))
            );

            setEditRowId(null);
            setEditedRole({});
        } catch (err) {
            console.error("Failed to save role:", err);
            alert("Error updating role.");
        } finally {
            setIsSaving(false);
        }
    };

    const handleDelete = async (roleId) => {
        if (deletingId === roleId) return;
        const confirm = window.confirm("Are you sure you want to delete this role?");
        if (!confirm) return;

        try {
            setDeletingId(roleId);
            await adminService.deleteRole(roleId);
            setRoles(prev => prev.filter(r => r.roleId !== roleId));
        } catch (err) {
            console.error("Failed to delete role:", err);
            alert("Error deleting role.");
        } finally {
            setDeletingId(null);
        }
    };

    const exportToJSON = () => {
        const blob = new Blob([JSON.stringify(roles, null, 2)], {
            type: "application/json",
        });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = "roles_export.json";
        link.click();
    };

    const exportToXLSX = async () => {
        try {
          const blob = await adminService.downloadRolesExcel();
          const url = window.URL.createObjectURL(new Blob([blob]));
          const link = document.createElement("a");
          link.href = url;
          link.download = "roles_export.xlsx";
          link.click();
        } catch (err) {
          console.error("Failed to export Excel:", err);
        }
    };      

    if (loading) return <p>Loading roles...</p>;

    return (
        <div className="admin-table-container">
            <div className="admin-table-header">
                <h2>🛡️ Roles Table</h2>
                <div className="admin-table-actions">
                    <button onClick={exportToJSON} className="admin-export-btn">Export JSON</button>
                    <button onClick={exportToXLSX} className="admin-export-btn">Export XLSX</button>
                </div>
            </div>

            <div className="admin-table-scroll">
                <table className="admin-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Role Name</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {roles.map((r) => (
                            <tr key={r.roleId}>
                                <td>{r.roleId}</td>
                                <td>
                                    {editRowId === r.roleId ? (
                                        <input
                                            value={editedRole.roleName}
                                            onChange={(e) => handleFieldChange("roleName", e.target.value)}
                                        />
                                    ) : (
                                        r.roleName
                                    )}
                                </td>
                                <td>
                                    {editRowId === r.roleId ? (
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
                                            <button className="admin-action-btn" onClick={() => handleEditClick(r)}>✏️</button>
                                            <button
                                                className="admin-action-btn"
                                                onClick={() => handleDelete(r.roleId)}
                                                disabled={deletingId === r.roleId}
                                            >
                                                {deletingId === r.roleId ? "⏳" : "🗑️"}
                                            </button>
                                        </>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default RolesTable;
