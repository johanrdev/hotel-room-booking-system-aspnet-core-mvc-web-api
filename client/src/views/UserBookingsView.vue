<template>
  <div class="container mx-auto mt-8">
    <h1 class="text-2xl font-bold mb-6 text-center text-gray-800">My Bookings</h1>

    <div v-if="isLoading" class="text-center">Loading bookings...</div>
    <div v-if="!isLoading && bookings.length === 0" class="text-center">No bookings available</div>
    <div v-if="!isLoading && bookings.length > 0">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Room</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Room Type</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price per Night</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Check-In Date</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Check-Out Date</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Number of Days</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total Price</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="booking in bookings" :key="booking.id">
              <td class="px-6 py-4 whitespace-nowrap">{{ booking.room.number }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ booking.room.roomType?.name || 'N/A' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">${{ booking.room.roomType?.price || 'N/A' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ formatDate(booking.checkInDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ formatDate(booking.checkOutDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ calculateDays(booking.checkInDate, booking.checkOutDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">${{ calculateTotalPrice(booking.room.roomType?.price, booking.checkInDate, booking.checkOutDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <button @click="deleteBooking(booking.id)" class="py-2 px-4 bg-red-600 text-white font-medium rounded hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axiosInstance from '../axiosInstance';

export default {
  data() {
    return {
      bookings: [],
      isLoading: true,
    };
  },
  async created() {
    await this.fetchBookings();
  },
  methods: {
    async fetchBookings() {
      try {
        const response = await axiosInstance.get('/booking/user');
        this.bookings = response.data;
      } catch (error) {
        console.error('Failed to fetch bookings', error);
      } finally {
        this.isLoading = false;
      }
    },
    formatDate(date) {
      const options = { year: 'numeric', month: 'long', day: 'numeric' };
      return new Date(date).toLocaleDateString(undefined, options);
    },
    calculateDays(checkInDate, checkOutDate) {
      const checkIn = new Date(checkInDate);
      const checkOut = new Date(checkOutDate);
      const timeDifference = checkOut.getTime() - checkIn.getTime();
      return Math.ceil(timeDifference / (1000 * 3600 * 24));
    },
    calculateTotalPrice(pricePerNight, checkInDate, checkOutDate) {
      if (!pricePerNight) {
        return 'N/A';
      }
      const days = this.calculateDays(checkInDate, checkOutDate);
      return (pricePerNight * days).toFixed(2);
    },
    async deleteBooking(bookingId) {
      if (confirm("Are you sure you want to delete this booking?")) {
        try {
          await axiosInstance.delete(`/booking/${bookingId}`);
          this.bookings = this.bookings.filter(booking => booking.id !== bookingId);
          alert('Booking deleted successfully');
        } catch (error) {
          console.error('Failed to delete booking', error);
          alert('Failed to delete booking');
        }
      }
    }
  }
};
</script>
