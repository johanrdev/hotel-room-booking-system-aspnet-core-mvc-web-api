<template>
  <div class="container mx-auto mt-8">
    <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Bookings</h1>
    <div class="text-right mb-4">
      <button @click="openCreateModal" class="py-2 px-4 bg-green-600 text-white font-medium rounded hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">Add Booking</button>
    </div>
    <div v-if="isLoading" class="text-center">Loading bookings...</div>
    <div v-if="!isLoading">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Room</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Room Type</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price per night</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">User</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Check-In Date</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Check-Out Date</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="booking in bookings" :key="booking.id">
              <td class="px-6 py-4 whitespace-nowrap">{{ booking.room?.number || 'N/A' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ booking.room?.roomType?.name || 'N/A' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">${{ booking.room?.roomType?.price || 'N/A' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ booking.user?.userName || 'N/A' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ formatDate(booking.checkInDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">{{ formatDate(booking.checkOutDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <button @click="openEditModal(booking)" class="py-2 px-4 bg-blue-600 text-white font-medium rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">Edit</button>
                <button @click="deleteBooking(booking.id)" class="ml-2 py-2 px-4 bg-red-600 text-white font-medium rounded hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <EditBookingModal
      v-if="isEditModalOpen && selectedBooking"
      :isOpen="isEditModalOpen"
      :booking="selectedBooking"
      @close="closeEditModal"
      @update="updateBooking"
    />

    <CreateBookingModal
      v-if="isCreateModalOpen"
      :isOpen="isCreateModalOpen"
      @close="closeCreateModal"
      @bookingCreated="fetchBookings"
    />
  </div>
</template>

<script>
import axiosInstance from '../../axiosInstance';
import EditBookingModal from '../../components/admin/EditBookingModal.vue';
import CreateBookingModal from '../../components/admin/CreateBookingModal.vue';

export default {
  components: {
    EditBookingModal,
    CreateBookingModal
  },
  data() {
    return {
      bookings: [],
      selectedBooking: null,
      isEditModalOpen: false,
      isCreateModalOpen: false,
      isLoading: true,
    };
  },
  async created() {
    await this.fetchBookings();
  },
  methods: {
    async fetchBookings() {
      try {
        const response = await axiosInstance.get('/admin/bookings');
        this.bookings = response.data.map(booking => ({
          ...booking,
          checkInDate: this.formatDateForInput(booking.checkInDate),
          checkOutDate: this.formatDateForInput(booking.checkOutDate)
        }));
      } catch (error) {
        console.error('Failed to fetch bookings', error);
      } finally {
        this.isLoading = false;
      }
    },
    openEditModal(booking) {
      this.selectedBooking = { ...booking };
      this.isEditModalOpen = true;
    },
    closeEditModal() {
      this.isEditModalOpen = false;
      this.selectedBooking = null;
    },
    async updateBooking(booking) {
      try {
        const response = await axiosInstance.put(`/admin/bookings/${booking.id}`, {
          ...booking,
          checkInDate: booking.checkInDate,
          checkOutDate: booking.checkOutDate
        });
        const updatedBooking = response.data;
        
        const roomResponse = await axiosInstance.get(`/admin/rooms/${updatedBooking.roomId}`);
        updatedBooking.room = roomResponse.data;
        
        this.bookings = this.bookings.map(b => (b.id === updatedBooking.id ? updatedBooking : b));
        this.closeEditModal();
        alert('Booking updated successfully');
      } catch (error) {
        console.error('Failed to update booking', error);
        alert('Failed to update booking');
      }
    },
    async deleteBooking(bookingId) {
      if (confirm('Are you sure you want to delete this booking?')) {
        try {
          await axiosInstance.delete(`/admin/bookings/${bookingId}`);
          this.bookings = this.bookings.filter(booking => booking.id !== bookingId);
          alert('Booking deleted successfully');
        } catch (error) {
          console.error('Failed to delete booking', error);
          alert('Failed to delete booking');
        }
      }
    },
    openCreateModal() {
      this.isCreateModalOpen = true;
    },
    closeCreateModal() {
      this.isCreateModalOpen = false;
    },
    formatDateForInput(date) {
      const localDate = new Date(date);
      localDate.setMinutes(localDate.getMinutes() - localDate.getTimezoneOffset());
      return localDate.toISOString().split('T')[0];
    },
    formatDate(date) {
      const options = { year: 'numeric', month: 'long', day: 'numeric' };
      const localDate = new Date(date);
      localDate.setMinutes(localDate.getMinutes() - localDate.getTimezoneOffset());
      return localDate.toLocaleDateString(undefined, options);
    }
  }
};
</script>
