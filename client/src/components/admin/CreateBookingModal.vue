<template>
    <div v-if="isOpen" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
      <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
        <h2 class="text-2xl font-bold mb-4">Create Booking</h2>
        <form @submit.prevent="createBooking" class="space-y-4">
          <div>
            <label for="user" class="block text-sm font-medium text-gray-700">User</label>
            <select v-model="selectedUserId" id="user" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
              <option v-for="user in users" :key="user.id" :value="user.id">{{ user.userName }}</option>
            </select>
          </div>
          <div>
            <label for="roomType" class="block text-sm font-medium text-gray-700">Room Type</label>
            <select v-model="roomType" id="roomType" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
              <option v-for="type in roomTypes" :key="type" :value="type">{{ type }}</option>
            </select>
          </div>
          <div>
            <label for="checkInDate" class="block text-sm font-medium text-gray-700">Check-In Date</label>
            <input v-model="checkInDate" id="checkInDate" type="date" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div>
            <label for="checkOutDate" class="block text-sm font-medium text-gray-700">Check-Out Date</label>
            <input v-model="checkOutDate" id="checkOutDate" type="date" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div class="flex justify-end">
            <button @click="closeModal" type="button" class="mr-4 py-2 px-4 bg-gray-600 text-white rounded hover:bg-gray-700 focus:outline-none">Cancel</button>
            <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Create Booking</button>
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
        users: [],
        selectedUserId: '',
        roomType: '',
        checkInDate: '',
        checkOutDate: '',
        roomTypes: []
      };
    },
    async created() {
      await Promise.all([this.fetchUsers(), this.fetchRoomTypes()]);
    },
    methods: {
      async fetchUsers() {
        try {
          const response = await axiosInstance.get('/admin/users');
          this.users = response.data;
        } catch (error) {
          console.error('Failed to fetch users', error);
        }
      },
      async fetchRoomTypes() {
        try {
          const response = await axiosInstance.get('/admin/room-types');
          this.roomTypes = response.data;
        } catch (error) {
          console.error('Failed to fetch room types', error);
        }
      },
      closeModal() {
        this.$emit('close');
      },
      async createBooking() {
        try {
          const bookingData = {
            userId: this.selectedUserId,
            roomType: this.roomType,
            checkInDate: this.checkInDate,
            checkOutDate: this.checkOutDate,
          };
          await axiosInstance.post('/admin/bookings', bookingData);
          alert('Booking created successfully');
          this.$emit('bookingCreated');
          this.$emit('close');
        } catch (error) {
          console.error('Failed to create booking', error);
          alert('Failed to create booking');
        }
      }
    }
  };
  </script>