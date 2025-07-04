/* eslint-disable eqeqeq */

import { useEffect, useState, useContext } from "react";

import AddUserModal from "./Modals/AddUserModal";

import AuthContext from "../../context/AuthContext";
import authService from "../../services/authService";
import adminService from "../../services/adminService";

import "../../styles/AdminModal.css"
import "../../styles/AdminTable.css";

const UsersTable = () => {
  const [users, setUsers] = useState([]);
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editRowId, setEditRowId] = useState(null);
  const [editedUser, setEditedUser] = useState({});
  const [deletingId, setDeletingId] = useState(null);
  const [isSaving, setIsSaving] = useState(false);
  const [showAddModal, setShowAddModal] = useState(false);

  const { user, updateUserInfo } = useContext(AuthContext);

  const fetchUsers = async () => {
    try {
      const data = await adminService.getUsers();
      setUsers(data);
    } catch (err) {
      console.error("Failed to fetch users:", err);
    } finally {
      setLoading(false);
    }
  };


  const fetchRoles = async () => {
    try {
      const data = await adminService.getRoles(); 
      setRoles(data);
    } catch (err) {
      console.error("Failed to fetch roles:", err);
    }
  };

  useEffect(() => {
    fetchUsers();
    fetchRoles();
  }, []);

  const handleEditClick = (user) => {
    setEditRowId(user.userId);
    setEditedUser({ ...user });
  };

  const handleCancel = () => {
    setEditRowId(null);
    setEditedUser({});
  };

  const handleSave = async () => {
    if (isSaving) return;

    setIsSaving(true);

    try {
      const original = users.find(u => u.userId === editedUser.userId);
      const hasChanged = Object.keys(editedUser).some(key => editedUser[key] !== original[key]);

      if (!hasChanged) {
        alert("No changes made.");
        setEditRowId(null);
        return;
      }

      await adminService.updateUser(editedUser.userId, editedUser);

      setUsers(prev =>
        prev.map(u => (u.userId == editedUser.userId ? editedUser : u))
      );

      if (user?.userId == editedUser.userId) {
        updateUserInfo({
          displayName: editedUser.displayName,
          wallet: editedUser.wallet,
          profilePicture: editedUser.profilePicture,
          email: editedUser.email,
          points: editedUser.points,
          role: roles.find(r => r.roleId === editedUser.roleId)?.roleName
        })

        authService.regenerateTokens();
      }

      setEditRowId(null);
      setEditedUser({});
    } catch (err) {
      console.error("Failed to save user:", err);
      alert("Error updating user.");
    } finally {
      setIsSaving(false);
    }
  };

  const handleFieldChange = (field, value) => {
    setEditedUser(prev => ({ ...prev, [field]: value }));
  };

  const handleDelete = async (userId) => {
    if (deletingId === userId) return;
    const userToDelete = users.find(u => u.userId == userId);
    const isSelf = user?.userId == userId;

    if (isSelf) {
      alert("You cannot delete your own account from the admin panel.");
      return;
    }

    const confirm = window.confirm(`Are you sure you want to delete "${userToDelete?.username}"?`);
    if (!confirm) return;

    try {
      setDeletingId(userId);
      await adminService.deleteUser(userId);

      setUsers(prev => prev.filter(u => u.userId !== userId));
    } catch (err) {
      console.error("Failed to delete user:", err);
      alert("Error deleting user.");
    } finally {
      setDeletingId(null);
    }
  };

  const exportToJSON = () => {
    const blob = new Blob([JSON.stringify(users, null, 2)], {
      type: "application/json",
    });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "users_export.json";
    link.click();
  };

  const exportToXLSX = async () => {
    try {
      const blob = await adminService.downloadUsersExcel();
      const url = window.URL.createObjectURL(new Blob([blob]));
      const link = document.createElement("a");
      link.href = url;
      link.download = "users_export.xlsx";
      link.click();
    } catch (err) {
      console.error("Failed to export Excel:", err);
    }
  };

  if (loading) return <p>Loading users...</p>;

  return (
    <div className="admin-table-container">
      <div className="admin-table-header">
        <h2>👥 Users Table</h2>
        <div className="admin-table-actions">
          <button onClick={() => setShowAddModal(true)} className="admin-export-btn">
            ➕ Add User
          </button>
          <button onClick={exportToJSON} className="admin-export-btn">Export JSON</button>
          <button onClick={exportToXLSX} className="admin-export-btn">Export XLSX</button>
        </div>
      </div>

      <div className="admin-table-scroll">
        <table className="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Username</th>
              <th>Display Name</th>
              <th>Email</th>
              <th>Wallet</th>
              <th>Points</th>
              <th>Role</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.userId}>
                <td>{u.userId}</td>
                <td>
                  {editRowId === u.userId ? (
                    <input
                      value={editedUser.username}
                      onChange={(e) => handleFieldChange("username", e.target.value)}
                    />
                  ) : (
                    u.username
                  )}
                </td>
                <td>
                  {editRowId === u.userId ? (
                    <input
                      value={editedUser.displayName}
                      onChange={(e) => handleFieldChange("displayName", e.target.value)}
                    />
                  ) : (
                    u.displayName
                  )}
                </td>
                <td>
                  {editRowId === u.userId ? (
                    <input
                      type="email"
                      value={editedUser.email}
                      onChange={(e) => handleFieldChange("email", e.target.value)}
                    />
                  ) : (
                    u.email
                  )}
                </td>
                <td>
                  {editRowId === u.userId ? (
                    <input
                      type="number"
                      value={editedUser.wallet}
                      onChange={(e) => handleFieldChange("wallet", parseFloat(e.target.value))}
                    />
                  ) : (
                    `€${u.wallet?.toFixed(2)}`
                  )}
                </td>
                <td>
                  {editRowId === u.userId ? (
                    <input
                      type="number"
                      value={editedUser.points}
                      onChange={(e) => handleFieldChange("points", parseInt(e.target.value))}
                    />
                  ) : (
                    u.points
                  )}
                </td>
                <td>
                  {editRowId === u.userId ? (
                    <select
                      value={editedUser.roleId}
                      onChange={(e) => handleFieldChange("roleId", parseInt(e.target.value))}
                    >
                      {roles.map((r) => (
                        <option key={r.roleId} value={r.roleId}>{r.roleName}</option>
                      ))}
                    </select>
                  ) : (
                    roles.find(r => r.roleId === u.roleId)?.roleName || "N/A"
                  )}
                </td>

                <td>
                  {editRowId === u.userId ? (
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
                      <button className="admin-action-btn" onClick={() => handleEditClick(u)}>✏️</button>
                      <button
                        className="admin-action-btn"
                        onClick={() => handleDelete(u.userId)}
                        disabled={deletingId === u.userId}
                      >
                        {deletingId === u.userId ? "⏳" : "🗑️"}
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
        <AddUserModal
          roles={roles}
          onClose={() => setShowAddModal(false)}
          onCreate={async (newUser) => {
            try {
              const created = await adminService.createUser(newUser);
              setUsers(prev => [...prev, created]);
              setShowAddModal(false);
            } catch (err) {
              console.error("Failed to create user:", err);
              const msg = err.response?.data?.message || err.message || "Unknown error";
              alert("Error creating user: " + msg);
            }
          }}
        />
      )}
    </div>
  );
};

export default UsersTable;
