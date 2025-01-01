<template>
  <div class="container mx-auto mt-8">
    <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Users</h1>
    <div class="text-right mb-4">
      <button @click="openCreateModal" class="py-2 px-4 bg-green-600 text-white font-medium rounded hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">Add User</button>
    </div>
    <div v-if="isLoading" class="text-center">Loading users...</div>
    <div v-if="!isLoading">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Username</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="user in users" :key="user.id">
              <td class="px-6 py-4 whitespace-nowrap">{{ user.userName }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ user.email }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <button @click="openEditModal(user)" class="py-2 px-4 bg-blue-600 text-white font-medium rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">Edit</button>
                <button @click="deleteUser(user.id)" class="py-2 px-4 bg-red-600 text-white font-medium rounded hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 ml-2">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <EditUserModal
      v-if="isEditModalOpen && selectedUser"
      :isOpen="isEditModalOpen"
      :user="selectedUser"
      @close="closeEditModal"
      @update="updateUser"
    />

    <CreateUserModal
      v-if="isCreateModalOpen"
      :isOpen="isCreateModalOpen"
      @close="closeCreateModal"
      @create="createUser"
    />
  </div>
</template>

<script>
import axiosInstance from '../../axiosInstance';
import EditUserModal from '../../components/admin/EditUserModal.vue';
import CreateUserModal from '../../components/admin/CreateUserModal.vue';

export default {
  components: {
    EditUserModal,
    CreateUserModal
  },
  data() {
    return {
      users: [],
      selectedUser: null,
      isEditModalOpen: false,
      isCreateModalOpen: false,
      isLoading: true,
    };
  },
  async created() {
    await this.fetchUsers();
  },
  methods: {
    async fetchUsers() {
      try {
        const response = await axiosInstance.get('/admin/users');
        this.users = response.data;
      } catch (error) {
        console.error('Failed to fetch users', error);
      } finally {
        this.isLoading = false;
      }
    },
    openEditModal(user) {
      this.selectedUser = { ...user };
      this.isEditModalOpen = true;
    },
    closeEditModal() {
      this.isEditModalOpen = false;
      this.selectedUser = null;
    },
    async updateUser(user) {
      try {
        const response = await axiosInstance.put(`/admin/users/${user.id}`, user);
        const updatedUser = response.data;
        this.users = this.users.map(u => (u.id === updatedUser.id ? updatedUser : u));
        this.closeEditModal();
        alert('User updated successfully');
      } catch (error) {
        console.error('Failed to update user', error);
        alert('Failed to update user');
      }
    },
    openCreateModal() {
      this.isCreateModalOpen = true;
    },
    closeCreateModal() {
      this.isCreateModalOpen = false;
    },
    async createUser(user) {
      try {
        const response = await axiosInstance.post('/admin/users', user);
        const newUser = response.data;
        this.users.push(newUser);
        this.closeCreateModal();
        alert('User created successfully');
      } catch (error) {
        console.error('Failed to create user', error);
        alert('Failed to create user');
      }
    },
    async deleteUser(userId) {
      if (confirm('Are you sure you want to delete this user?')) {
        try {
          await axiosInstance.delete(`/admin/users/${userId}`);
          this.users = this.users.filter(user => user.id !== userId);
          alert('User deleted successfully');
        } catch (error) {
          console.error('Failed to delete user', error);
          alert('Failed to delete user');
        }
      }
    }
  }
};
</script>
