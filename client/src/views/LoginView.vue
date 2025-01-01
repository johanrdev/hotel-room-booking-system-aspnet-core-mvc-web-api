<template>
    <div class="flex items-center justify-center flex-grow">
      <div class="w-full max-w-md p-8 bg-white rounded shadow-md">
        <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Login</h1>
        <form @submit.prevent="login" class="space-y-4">
          <div>
            <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
            <input v-model="username" id="username" type="text" placeholder="Username" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
            <input v-model="password" id="password" type="password" placeholder="Password" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <button type="submit" class="w-full py-2 px-4 bg-indigo-600 text-white font-medium rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Login</button>
        </form>
      </div>
    </div>
  </template>
  
  <script>
  import axiosInstance from '../axiosInstance';
  import { useAuthStore } from '../stores/authStore';
  
  export default {
    data() {
      return {
        username: '',
        password: ''
      };
    },
    methods: {
      async login() {
        try {
          await axiosInstance.post('/auth/login', {
            username: this.username,
            password: this.password
          });
          const authStore = useAuthStore();
          authStore.setAuthenticated(true);
          this.$router.push('/account');
        } catch (error) {
          console.error('Login failed', error);
        }
      }
    }
  };
  </script>
  