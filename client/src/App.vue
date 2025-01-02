<template>
  <div class="relative flex-grow flex flex-col" @click="handleClickOutside">
    <nav class="fixed top-0 left-0 right-0 z-30 flex justify-center bg-gray-800 p-4 h-16 text-white">
      <div class="container mx-auto flex justify-between items-center">
        <router-link to="/" class="text-lg font-bold">Hotel Booking System</router-link>
        <div v-if="authStore.isLoading">
          Loading...
        </div>
        <div class="hidden sm:flex items-center" v-else>
          <router-link class="mr-4" to="/room-types">Rooms</router-link>
          <router-link v-if="!authStore.isAuthenticated" class="mr-4" to="/login">Login</router-link>
          <router-link v-if="!authStore.isAuthenticated" class="mr-4" to="/register">Register</router-link>
          <div v-if="authStore.isAuthenticated" class="relative inline-block text-left">
            <button @click.stop="toggleDropdown" ref="dropdownButton" type="button" class="inline-flex justify-center w-full rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-gray-100 focus:ring-indigo-500">
              {{ authStore.username }}
              <svg class="-mr-1 ml-2 h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 011.414 1.414l-4 4a 1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </button>

            <div v-if="dropdownOpen" ref="dropdownMenu" class="origin-top-right absolute top-full mt-2 right-0 w-56 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5 focus:outline-none">
              <div class="py-1" role="menu" aria-orientation="vertical" aria-labelledby="options-menu">
                <router-link @click.native="closeDropdown" to="/account" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" role="menuitem">My Account</router-link>
                <router-link @click.native="closeDropdown" to="/bookings" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" role="menuitem">My Bookings</router-link>
                <router-link v-if="authStore.isAdmin" @click.native="closeDropdown" to="/admin" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" role="menuitem">Dashboard</router-link>
                <button @click="logout" class="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">Logout</button>
              </div>
            </div>
          </div>
        </div>
        
        <div class="flex justify-center items-center sm:hidden">
          <button @click.stop="toggleMobileMenu" ref="mobileMenuButton" class="text-gray-500 hover:text-white focus:outline-none focus:text-white">
            <svg v-if="!isMobileMenuOpen" class="h-6 w-6" fill="none" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" viewBox="0 0 24 24" stroke="currentColor">
              <path d="M4 6h16M4 12h16M4 18h16"></path>
            </svg>
            <svg v-else class="h-6 w-6" fill="none" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" viewBox="0 0 24 24" stroke="currentColor">
              <path d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
      </div>

      <div class="fixed top-16 left-0 right-0 z-30 flex flex-col sm:hidden bg-gray-900" :class="{'block': isMobileMenuOpen, 'hidden': !isMobileMenuOpen}" ref="mobileMenu">
        <div class="container mx-auto p-2">
          <router-link @click.native="closeMobileMenu" to="/room-types" class="block text-sm p-2 text-white hover:bg-yellow-600" role="menuitem">Rooms</router-link>
          <router-link v-if="!authStore.isAuthenticated" @click.native="closeMobileMenu" to="/login" class="block text-sm p-2 text-white hover:bg-yellow-600" role="menuitem">Login</router-link>
          <router-link v-if="!authStore.isAuthenticated" @click.native="closeMobileMenu" to="/register" class="block text-sm p-2 text-white hover:bg-yellow-600" role="menuitem">Register</router-link>
          <router-link v-if="authStore.isAuthenticated" @click.native="closeMobileMenu" to="/account" class="block text-sm p-2 text-white hover:bg-yellow-600" role="menuitem">My Account</router-link>
          <router-link v-if="authStore.isAuthenticated" @click.native="closeMobileMenu" to="/bookings" class="block text-sm p-2 text-white hover:bg-yellow-600" role="menuitem">My Bookings</router-link>
          <router-link v-if="authStore.isAuthenticated && authStore.isAdmin" @click.native="closeMobileMenu" to="/admin" class="block text-sm p-2 text-white hover:bg-yellow-600" role="menuitem">Dashboard</router-link>
          <button @click="logout" class="block w-full text-left p-2 text-sm text-white hover:bg-yellow-600">Logout</button>
        </div>
      </div>
    </nav>
    <main class="max-w-5xl w-full mx-auto flex flex-col flex-grow mt-24">
      <router-view></router-view>
    </main>
  </div>
</template>

<script>
import { useAuthStore } from './stores/authStore';
import { onMounted, onBeforeUnmount, watch, ref } from 'vue';
import { useRouter } from 'vue-router';
import axiosInstance from './axiosInstance';

export default {
  setup() {
    const authStore = useAuthStore();
    const router = useRouter();
    const dropdownOpen = ref(false);
    const isMobileMenuOpen = ref(false);
    const dropdownMenu = ref(null);
    const dropdownButton = ref(null);
    const mobileMenu = ref(null);
    const mobileMenuButton = ref(null);

    onMounted(() => {
      authStore.checkAuth();
      document.addEventListener('click', handleClickOutside);
    });

    onBeforeUnmount(() => {
      document.removeEventListener('click', handleClickOutside);
    });

    watch(() => authStore.isLoading, (newValue) => {
      if (!newValue) {
        console.log('Auth loading completed');
      }
    });

    const toggleDropdown = () => {
      dropdownOpen.value = !dropdownOpen.value;
    };

    const closeDropdown = () => {
      dropdownOpen.value = false;
    };

    const toggleMobileMenu = () => {
      isMobileMenuOpen.value = !isMobileMenuOpen.value;
    };

    const closeMobileMenu = () => {
      isMobileMenuOpen.value = false;
    };

    const handleClickOutside = (event) => {
      if (
        dropdownMenu.value &&
        !dropdownMenu.value.contains(event.target) &&
        dropdownButton.value &&
        !dropdownButton.value.contains(event.target)
      ) {
        closeDropdown();
      }

      if (
        mobileMenu.value &&
        !mobileMenu.value.contains(event.target) &&
        mobileMenuButton.value &&
        !mobileMenuButton.value.contains(event.target)
      ) {
        closeMobileMenu();
      }
    };

    const logout = async () => {
      try {
        await axiosInstance.post('/auth/logout');
        authStore.$reset();
        authStore.isLoading = false;
        closeDropdown();
        closeMobileMenu();
        router.push('/login');
      } catch (error) {
        console.error('Logout failed', error);
        authStore.isLoading = false;
        closeDropdown();
        closeMobileMenu();
      }
    };

    return {
      authStore,
      toggleDropdown,
      closeDropdown,
      toggleMobileMenu,
      closeMobileMenu,
      handleClickOutside,
      logout,
      dropdownOpen,
      isMobileMenuOpen,
      dropdownMenu,
      dropdownButton,
      mobileMenu,
      mobileMenuButton
    };
  }
};
</script>
