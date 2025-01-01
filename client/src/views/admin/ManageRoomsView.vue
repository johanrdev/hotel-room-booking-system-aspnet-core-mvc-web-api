<template>
  <div class="container mx-auto mt-8">
    <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Rooms</h1>
    <div class="text-right mb-4">
      <button @click="openCreateModal" class="py-2 px-4 bg-green-600 text-white font-medium rounded hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">Add Room</button>
    </div>
    <div v-if="isLoading" class="text-center">Loading rooms...</div>
    <div v-if="!isLoading">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Room Number</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="room in rooms" :key="room.id">
              <td class="px-6 py-4 whitespace-nowrap">{{ room.number }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ room.type }}</td>
              <td class="px-6 py-4 whitespace-nowrap">${{ room.price }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <button @click="openEditModal(room)" class="py-2 px-4 bg-blue-600 text-white font-medium rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">Edit</button>
                <button @click="deleteRoom(room.id)" class="ml-2 py-2 px-4 bg-red-600 text-white font-medium rounded hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    
    <EditRoomModal
      v-if="isEditModalOpen && selectedRoom"
      :isOpen="isEditModalOpen"
      :room="selectedRoom"
      @close="closeEditModal"
      @update="updateRoom"
    />

    <CreateRoomModal
      v-if="isCreateModalOpen"
      :isOpen="isCreateModalOpen"
      @close="closeCreateModal"
      @roomCreated="handleRoomCreated"
    />
  </div>
</template>

<script>
import axiosInstance from '../../axiosInstance';
import EditRoomModal from '../../components/admin/EditRoomModal.vue';
import CreateRoomModal from '../../components/admin/CreateRoomModal.vue';

export default {
components: {
  EditRoomModal,
  CreateRoomModal
},
data() {
  return {
    rooms: [],
    selectedRoom: null,
    isEditModalOpen: false,
    isCreateModalOpen: false,
    isLoading: true,
  };
},
async created() {
  await this.fetchRooms();
},
methods: {
  async fetchRooms() {
    try {
      const response = await axiosInstance.get('/admin/rooms');
      this.rooms = response.data;
    } catch (error) {
      console.error('Failed to fetch rooms', error);
    } finally {
      this.isLoading = false;
    }
  },
  openEditModal(room) {
    this.selectedRoom = { ...room };
    this.isEditModalOpen = true;
  },
  closeEditModal() {
    this.isEditModalOpen = false;
    this.selectedRoom = null;
  },
  async updateRoom(room) {
    try {
      const response = await axiosInstance.put(`/admin/rooms/${room.id}`, room);
      const updatedRoom = response.data;
      this.rooms = this.rooms.map(r => (r.id === updatedRoom.id ? updatedRoom : r));
      this.closeEditModal();
      alert('Room updated successfully');
    } catch (error) {
      console.error('Failed to update room', error);
      alert('Failed to update room');
    }
  },
  openCreateModal() {
    this.isCreateModalOpen = true;
  },
  closeCreateModal() {
    this.isCreateModalOpen = false;
  },
  async createRoom(room) {
    try {
      const response = await axiosInstance.post('/admin/rooms', room);
      const newRoom = response.data;
      this.rooms.push(newRoom); // Ensure reactivity
      this.rooms = [...this.rooms]; // Trigger reactivity
      this.closeCreateModal();
      alert('Room created successfully');
    } catch (error) {
      console.error('Failed to create room', error);
      alert('Failed to create room');
    }
  },
  handleRoomCreated(newRoom) {
    this.rooms.push(newRoom);
    this.rooms = [...this.rooms]; // Trigger reactivity
  },
  async deleteRoom(roomId) {
    if (confirm('Are you sure you want to delete this room?')) {
      try {
        await axiosInstance.delete(`/admin/rooms/${roomId}`);
        this.rooms = this.rooms.filter(room => room.id !== roomId);
        alert('Room deleted successfully');
      } catch (error) {
        console.error('Failed to delete room', error);
        alert('Failed to delete room');
      }
    }
  }
}
};
</script>
