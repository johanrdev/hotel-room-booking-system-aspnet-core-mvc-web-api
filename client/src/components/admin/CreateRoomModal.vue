<template>
    <div v-if="isOpen" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
      <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
        <h2 class="text-2xl font-bold mb-4">Create Room</h2>
        <form @submit.prevent="createRoom" class="space-y-4">
          <div>
            <label for="roomNumber" class="block text-sm font-medium text-gray-700">Room Number</label>
            <input v-model="roomNumber" id="roomNumber" type="text" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div>
            <label for="roomType" class="block text-sm font-medium text-gray-700">Room Type</label>
            <input v-model="roomType" id="roomType" type="text" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div>
            <label for="roomPrice" class="block text-sm font-medium text-gray-700">Price</label>
            <input v-model="roomPrice" id="roomPrice" type="number" step="0.01" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div class="flex justify-end">
            <button @click="closeModal" type="button" class="mr-4 py-2 px-4 bg-gray-600 text-white rounded hover:bg-gray-700 focus:outline-none">Cancel</button>
            <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Create Room</button>
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
    }
  },
  data() {
    return {
      roomNumber: '',
      roomType: '',
      roomPrice: '',
    };
  },
  methods: {
    closeModal() {
      this.$emit('close');
    },
    async createRoom() {
      try {
        const roomData = {
          number: this.roomNumber,
          type: this.roomType,
          price: this.roomPrice,
        };
        const response = await axiosInstance.post('/admin/rooms', roomData);
        alert('Room created successfully');
        this.$emit('roomCreated', response.data);
        this.$emit('close');
      } catch (error) {
        console.error('Failed to create room', error);
        alert('Failed to create room');
      }
    }
  }
};
</script>
