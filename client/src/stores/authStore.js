import { defineStore } from 'pinia';
import axiosInstance from '../axiosInstance';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    isAuthenticated: false,
    isLoading: true,
    isAdmin: false,
  }),
  actions: {
    async checkAuth() {
      try {
        const response = await axiosInstance.get('/auth/check-auth');
        this.isAuthenticated = response.data.isAuthenticated;
        this.isAdmin = response.data.isAdmin;
      } catch (error) {
        this.isAuthenticated = false;
        this.isAdmin = false;
      } finally {
        this.isLoading = false;
      }
    },
    setAuthenticated(isAuthenticated) {
      this.isAuthenticated = isAuthenticated;
      this.checkAuth();
    },
    $reset() {
      this.isAuthenticated = false;
      this.isLoading = true;
      this.isAdmin = false;
    }
  }
});
