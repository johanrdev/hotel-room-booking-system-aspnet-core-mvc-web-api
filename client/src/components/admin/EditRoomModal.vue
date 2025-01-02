<template>
  <div v-if="isOpen" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
    <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
      <h2 class="text-2xl font-bold mb-4">Edit Room</h2>
      <form @submit.prevent="submitForm" class="space-y-4">
        <div>
          <label for="number" class="block text-sm font-medium text-gray-700">Room Number</label>
          <input v-model="room.number" id="number" type="text" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" required />
        </div>
        <div>
          <label for="roomType" class="block text-sm font-medium text-gray-700">Room Type</label>
          <select v-model="room.roomTypeId" id="roomType" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
            <option v-for="type in roomTypes" :value="type.id" :key="type.id">{{ type.name }}</option>
          </select>
        </div>
        <div>
          <label for="isAvailable" class="block text-sm font-medium text-gray-700">Is Available</label>
          <input v-model="room.isAvailable" id="isAvailable" type="checkbox" class="mt-1" />
        </div>
        <div class="flex justify-end">
          <button @click="closeModal" type="button" class="mr-4 py-2 px-4 bg-gray-600 text-white rounded hover:bg-gray-700 focus:outline-none">Cancel</button>
          <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Save</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axiosInstance from '../../axiosInstance';

export default {
  props: {
    isOpen: {
      type: Boolean,
      required: true
    },
    room: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      roomTypes: []
    };
  },
  async created() {
    await this.fetchRoomTypes();
  },
  methods: {
    closeModal() {
      this.$emit('close');
    },
    async fetchRoomTypes() {
      try {
        const response = await axiosInstance.get('/admin/room-types');
        this.roomTypes = response.data;
      } catch (error) {
        console.error('Failed to fetch room types', error);
        alert('Failed to fetch room types');
      }
    },
    submitForm() {
      this.$emit('update', { ...this.room });
      this.closeModal();
    }
  }
};
</script>
