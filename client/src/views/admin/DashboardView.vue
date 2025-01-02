<template>
  <div class="container mx-auto mt-8">
    <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Admin Dashboard</h1>
    <div v-if="isLoading" class="text-center">Loading dashboard data...</div>
    <div v-if="!isLoading">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="p-4 bg-white rounded shadow-md flex items-center">
          <span class="fas fa-users text-4xl mr-4"></span>
          <div>
            <h2 class="text-xl font-bold">Total Users</h2>
            <p class="text-2xl">{{ dashboardData.totalUsers }}</p>
            <router-link to="/admin/manage-users" class="text-blue-500 underline">Manage Users</router-link>
          </div>
        </div>
        <div class="p-4 bg-white rounded shadow-md flex items-center">
          <span class="fas fa-calendar-check text-4xl mr-4"></span>
          <div>
            <h2 class="text-xl font-bold">Total Bookings</h2>
            <p class="text-2xl">{{ dashboardData.totalBookings }}</p>
            <router-link to="/admin/manage-bookings" class="text-blue-500 underline">Manage Bookings</router-link>
          </div>
        </div>
        <div class="p-4 bg-white rounded shadow-md flex items-center">
          <span class="fas fa-hotel text-4xl mr-4"></span>
          <div>
            <h2 class="text-xl font-bold">Total Rooms</h2>
            <p class="text-2xl">{{ dashboardData.totalRooms }}</p>
            <router-link to="/admin/manage-rooms" class="text-blue-500 underline">Manage Rooms</router-link>
          </div>
        </div>
        <div class="p-4 bg-white rounded shadow-md flex items-center">
          <span class="fas fa-bed text-4xl mr-4"></span>
          <div>
            <h2 class="text-xl font-bold">Total Room Types</h2>
            <p class="text-2xl">{{ dashboardData.totalRoomTypes }}</p>
            <router-link to="/admin/manage-room-types" class="text-blue-500 underline">Manage Room Types</router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axiosInstance from '../../axiosInstance';

export default {
  data() {
    return {
      dashboardData: {},
      isLoading: true,
    };
  },
  async created() {
    await this.fetchDashboardData();
  },
  methods: {
    async fetchDashboardData() {
      try {
        const response = await axiosInstance.get('/admin/dashboard');
        this.dashboardData = response.data;
      } catch (error) {
        console.error('Failed to fetch dashboard data', error);
      } finally {
        this.isLoading = false;
      }
    }
  }
};
</script>
