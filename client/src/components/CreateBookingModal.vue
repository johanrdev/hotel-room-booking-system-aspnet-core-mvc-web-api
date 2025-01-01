<template>
  <div class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
    <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
      <h2 class="text-2xl font-bold mb-4">Create Booking for {{ roomType.name }}</h2>
      <form @submit.prevent="createBooking" class="space-y-4">
        <div>
          <label for="checkInDate" class="block text-sm font-medium text-gray-700">Check-In Date</label>
          <input v-model="checkInDate" id="checkInDate" type="date" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div>
          <label for="checkOutDate" class="block text-sm font-medium text-gray-700">Check-Out Date</label>
          <input v-model="checkOutDate" id="checkOutDate" type="date" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div class="flex justify-end">
          <button @click="$emit('close')" type="button" class="mr-4 py-2 px-4 bg-gray-600 text-white rounded hover:bg-gray-700 focus:outline-none">Cancel</button>
          <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Create Booking</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axiosInstance from '../axiosInstance';

export default {
  props: {
    roomType: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      checkInDate: '',
      checkOutDate: ''
    };
  },
  methods: {
    async createBooking() {
      try {
        const bookingData = {
          roomType: this.roomType.name,
          checkInDate: this.checkInDate,
          checkOutDate: this.checkOutDate,
        };
        await axiosInstance.post('/booking', bookingData);
        alert('Booking created successfully');
        this.$emit('close');
      } catch (error) {
        console.error('Failed to create booking', error);
        alert('Failed to create booking');
      }
    }
  }
};
</script>
