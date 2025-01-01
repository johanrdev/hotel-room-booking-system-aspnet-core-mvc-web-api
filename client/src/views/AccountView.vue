<template>
  <div class="container mx-auto mt-8">
    <h1 class="text-2xl font-bold mb-6 text-center text-gray-800">Account</h1>
    <div v-if="isLoading" class="text-center">Loading account...</div>
    <div v-if="!isLoading">
      <form @submit.prevent="updateAccount" class="space-y-4">
        <div>
          <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
          <input v-model="form.username" id="username" type="text" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
          <input v-model="form.email" id="email" type="email" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div class="flex justify-end">
          <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Update</button>
        </div>
      </form>

      <h2 class="text-xl font-bold mt-8 mb-4">Change Password</h2>
      <form @submit.prevent="changePassword" class="space-y-4">
        <div>
          <label for="currentPassword" class="block text-sm font-medium text-gray-700">Current Password</label>
          <input v-model="passwordForm.currentPassword" id="currentPassword" type="password" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div>
          <label for="newPassword" class="block text-sm font-medium text-gray-700">New Password</label>
          <input v-model="passwordForm.newPassword" id="newPassword" type="password" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div class="flex justify-end">
          <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Change Password</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axiosInstance from '../axiosInstance';

export default {
  data() {
    return {
      form: {
        username: '',
        email: ''
      },
      passwordForm: {
        currentPassword: '',
        newPassword: ''
      },
      isLoading: true,
    };
  },
  async created() {
    await this.fetchAccount();
  },
  methods: {
    async fetchAccount() {
      try {
        const response = await axiosInstance.get('/user/account');
        this.form.username = response.data.username;
        this.form.email = response.data.email;
      } catch (error) {
        console.error('Failed to fetch account', error);
        this.$router.push('/login');
      } finally {
        this.isLoading = false;
      }
    },
    async updateAccount() {
      try {
        await axiosInstance.put('/user/account', this.form);
        alert('Account updated successfully!');
      } catch (error) {
        console.error('Failed to update account', error);
        alert('Failed to update account!');
      }
    },
    async changePassword() {
      try {
        await axiosInstance.put('/user/change-password', this.passwordForm);
        alert('Password changed successfully!');
        this.passwordForm.currentPassword = '';
        this.passwordForm.newPassword = '';
      } catch (error) {
        console.error('Failed to change password', error);
        alert('Failed to change password!');
      }
    }
  }
};
</script>
