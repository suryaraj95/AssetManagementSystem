import { create } from 'zustand';
import apiClient from '../api/client';

const useNotificationStore = create((set, get) => ({
  notifications: [],
  unreadCount: 0,
  
  fetchNotifications: async () => {
    try {
      const response = await apiClient.get('/notifications');
      const data = response.data;
      set({ 
        notifications: data, 
        unreadCount: data.filter((n) => !n.isRead).length 
      });
    } catch (error) {
      console.error('Failed to fetch notifications', error);
    }
  },

  markAsRead: async (id) => {
    try {
      await apiClient.put(`/notifications/${id}/read`);
      set((state) => {
        const updated = state.notifications.map((n) => 
          n.id === id ? { ...n, isRead: true } : n
        );
        return { 
          notifications: updated, 
          unreadCount: updated.filter((n) => !n.isRead).length 
        };
      });
    } catch (error) {
      console.error('Failed to mark read', error);
    }
  },

  markAllAsRead: async () => {
    try {
      await apiClient.put(`/notifications/read-all`);
      set((state) => {
        const updated = state.notifications.map((n) => ({ ...n, isRead: true }));
        return { notifications: updated, unreadCount: 0 };
      });
    } catch (error) {
      console.error('Failed to mark all read', error);
    }
  }
}));

export default useNotificationStore;
