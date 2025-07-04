import { useState } from "react";
import "../../../styles/AdminModal.css";

const AddUserModal = ({ roles, onClose, onCreate }) => {
    const [formData, setFormData] = useState({
        username: "",
        displayName: "",
        email: "",
        wallet: 0,
        points: 0,
        roleId: roles.length > 0 ? roles[0].roleId : 1
    });

    const handleChange = (field, value) => {
        setFormData(prev => ({ ...prev, [field]: value }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onCreate(formData); 
    };

    return (
        <div className="admin-modal-backdrop">
            <div className="admin-modal">
                <h3>Create New User</h3>
                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="Username"
                        value={formData.username}
                        onChange={e => handleChange("username", e.target.value)}
                        required
                    />
                    <input
                        type="password"
                        placeholder="Temporary Password"
                        value={formData.password}
                        onChange={e => handleChange("password", e.target.value)}
                        required
                    />
                    <input
                        type="text"
                        placeholder="Display Name"
                        value={formData.displayName}
                        onChange={e => handleChange("displayName", e.target.value)}
                        required
                    />
                    <input
                        type="email"
                        placeholder="Email"
                        value={formData.email}
                        onChange={e => handleChange("email", e.target.value)}
                        required
                    />
                    <label>Wallet (€)</label>
                    <input
                        type="number"
                        placeholder="Wallet"
                        value={formData.wallet}
                        onChange={e => handleChange("wallet", parseFloat(e.target.value))}
                    />

                    <label>Points</label>
                    <input
                        type="number"
                        placeholder="Points"
                        value={formData.points}
                        onChange={e => handleChange("points", parseInt(e.target.value))}
                    />

                    <select
                        value={formData.roleId}
                        onChange={e => handleChange("roleId", parseInt(e.target.value))}
                    >
                        {roles.map(role => (
                            <option key={role.roleId} value={role.roleId}>
                                {role.roleName}
                            </option>
                        ))}
                    </select>

                    <div className="admin-modal-actions">
                        <button type="submit" className="admin-action-btn">✅ Add</button>
                        <button type="button" className="admin-action-btn" onClick={onClose}>❌ Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AddUserModal;
