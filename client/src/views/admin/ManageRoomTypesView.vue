<template>
    <div class="container mx-auto mt-8">
      <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Room Types</h1>
      <div class="text-right mb-4">
        <button @click="openCreateModal" class="py-2 px-4 bg-green-600 text-white font-medium rounded hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">Add Room Type</button>
      </div>
      <div v-if="isLoading" class="text-center">Loading room types...</div>
      <div v-if="!isLoading">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type Name</th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price</th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="roomType in roomTypes" :key="roomType.id">
                <td class="px-6 py-4 whitespace-nowrap">{{ roomType.name }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ roomType.price }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <button @click="openEditModal(roomType)" class="py-2 px-4 bg-blue-600 text-white font-medium rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">Edit</button>
                  <button @click="deleteRoomType(roomType.id)" class="ml-2 py-2 px-4 bg-red-600 text-white font-medium rounded hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
  
      <EditRoomTypeModal
        v-if="isEditModalOpen && selectedRoomType"
        :isOpen="isEditModalOpen"
        :roomType="selectedRoomType"
        @close="closeEditModal"
        @update="updateRoomType"
      />
  
      <CreateRoomTypeModal
        v-if="isCreateModalOpen"
        :isOpen="isCreateModalOpen"
        @close="closeCreateModal"
        @roomTypeCreated="handleRoomTypeCreated"
      />
    </div>
  </template>
  
  <script>
  import axiosInstance from '../../axiosInstance';
  import EditRoomTypeModal from '../../components/admin/EditRoomTypeModal.vue';
  import CreateRoomTypeModal from '../../components/admin/CreateRoomTypeModal.vue';
  
  export default {
    components: {
      EditRoomTypeModal,
      CreateRoomTypeModal
    },
    data() {
      return {
        roomTypes: [],
        selectedRoomType: null,
        isEditModalOpen: false,
        isCreateModalOpen: false,
        isLoading: true,
      };
    },
    async created() {
      await this.fetchRoomTypes();
    },
    methods: {
      async fetchRoomTypes() {
        try {
          const response = await axiosInstance.get('/admin/room-types');
          this.roomTypes = response.data;
        } catch (error) {
          console.error('Failed to fetch room types', error);
        } finally {
          this.isLoading = false;
        }
      },
      openEditModal(roomType) {
        this.selectedRoomType = { ...roomType };
        this.isEditModalOpen = true;
      },
      closeEditModal() {
        this.isEditModalOpen = false;
        this.selectedRoomType = null;
      },
      async updateRoomType(updatedRoomType) {
        try {
          const response = await axiosInstance.put(`/admin/room-types/${updatedRoomType.id}`, updatedRoomType);
          const updatedRoomTypeData = response.data;
  
          this.roomTypes = this.roomTypes.map(rt => (rt.id === updatedRoomTypeData.id ? updatedRoomTypeData : rt));
          this.closeEditModal();
          alert('Room type updated successfully');
        } catch (error) {
          console.error('Failed to update room type', error);
          alert('Failed to update room type');
        }
      },
      openCreateModal() {
        this.isCreateModalOpen = true;
      },
      closeCreateModal() {
        this.isCreateModalOpen = false;
      },
      async createRoomType(roomType) {
        try {
          const response = await axiosInstance.post('/admin/room-types', roomType);
          const newRoomType = response.data;
  
          this.roomTypes.push(newRoomType);
          this.roomTypes = [...this.roomTypes];
          this.closeCreateModal();
          alert('Room type created successfully');
        } catch (error) {
          console.error('Failed to create room type', error);
          alert('Failed to create room type');
        }
      },
      handleRoomTypeCreated(newRoomType) {
        this.roomTypes.push(newRoomType);
        this.roomTypes = [...this.roomTypes];
      },
      async deleteRoomType(roomTypeId) {
        if (confirm('Are you sure you want to delete this room type?')) {
          try {
            await axiosInstance.delete(`/admin/room-types/${roomTypeId}`);
            this.roomTypes = this.roomTypes.filter(roomType => roomType.id !== roomTypeId);
            alert('Room type deleted successfully');
          } catch (error) {
            console.error('Failed to delete room type', error);
            alert('Failed to delete room type');
          }
        }
      }
    }
  };
  </script>
  