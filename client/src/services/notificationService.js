import fetchWithAuth from '../utils/fetchWithAuth';

const NOTIF_API_URL = 'https://localhost:7003/api/notification';

const notificationService = {
    async getAll() {
        const res = await fetchWithAuth(`${NOTIF_API_URL}`);
        if (!res.ok) throw new Error('Failed to fetch notifications');
        return await res.json();
    },

    async markAsRead(id) {
        const res = await fetchWithAuth(`${NOTIF_API_URL}/mark-as-read/${id}`, {
            method: 'POST',
        });
        if (!res.ok) throw new Error('Failed to mark notification as read');
    },

    async clearAll() {
        const res = await fetchWithAuth(`${NOTIF_API_URL}/clear`, {
            method: 'DELETE',
        });
        if (!res.ok) throw new Error('Failed to clear notifications');
    }
};

export default notificationService;
