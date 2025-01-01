<template>
  <div>
    <nav class="bg-gray-800 p-4 text-white">
      <div class="container mx-auto flex justify-between items-center">
        <div class="text-2xl font-bold">Hotel Booking System</div>
        <div v-if="authStore.isLoading">
          Loading...
        </div>
        <div v-else>
          <router-link class="mr-4" to="/roomtypes">Rooms</router-link>
          <router-link v-if="!authStore.isAuthenticated" class="mr-4" to="/login">Login</router-link>
          <router-link v-if="!authStore.isAuthenticated" class="mr-4" to="/register">Register</router-link>
          <router-link v-if="authStore.isAuthenticated" class="mr-4" to="/profile">Profile</router-link>
          <router-link v-if="authStore.isAuthenticated && authStore.isAdmin" class="mr-4" to="/admin">Dashboard</router-link>
          <button v-if="authStore.isAuthenticated" @click="logout" class="bg-red-500 px-4 py-2 rounded">Logout</button>
        </div>
      </div>
    </nav>
    <main class="max-w-5xl w-full mx-auto mt-8">
      <router-view></router-view>
    </main>
  </div>
</template>

<script>
import { useAuthStore } from './stores/authStore';
import { onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import axiosInstance from './axiosInstance';

export default {
  setup() {
    const authStore = useAuthStore();
    const router = useRouter();

    onMounted(() => {
      authStore.checkAuth();
    });

    // Watch for changes in isLoading state
    watch(() => authStore.isLoading, (newValue) => {
      if (!newValue) {
        console.log('Auth loading completed');
      }
    });

    const logout = async () => {
      try {
        await axiosInstance.post('/auth/logout');
        authStore.$reset();
        authStore.isLoading = false;
        router.push('/login');
      } catch (error) {
        console.error('Logout failed', error);
        authStore.isLoading = false;
      }
    };

    return {
      authStore,
      logout
    };
  }
};
</script>
