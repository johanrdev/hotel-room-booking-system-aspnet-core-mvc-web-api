<template>
  <div>
      <h1 class="text-3xl font-bold mb-6 text-center text-gray-800">Room Types</h1>
      <div v-if="isLoading" class="text-center">Loading room types...</div>
      <div v-if="!isLoading && roomTypes.length === 0" class="text-center">No room types available</div>
      <div v-if="!isLoading && roomTypes.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div v-for="type in roomTypes" :key="type.id" class="p-4 bg-white rounded shadow-md">
              <img :src="`/images/${type.imageUrl}` || '/images/default.png'" alt="Room Type Image" class="h-32 w-full object-cover rounded mb-4" />
              <h2 class="text-xl font-bold text-center">{{ type.name }}</h2>
              <p class="text-center">{{ type.description }}</p>
              <p class="text-center">Price per night: ${{ type.price }}</p>
              <button @click="handleBooking(type)" class="mt-4 py-2 px-4 w-full bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none">Book Now</button>
          </div>
      </div>

      <CreateBookingModal v-if="showBookingModal" :roomType="selectedRoomType" @close="closeModal" />
  </div>
</template>

<script>
import axiosInstance from '../axiosInstance';
import { useAuthStore } from '../stores/authStore';
import CreateBookingModal from '../components/CreateBookingModal.vue';

export default {
  components: {
      CreateBookingModal,
  },
  data() {
      return {
          roomTypes: [],
          isLoading: true,
          showBookingModal: false,
          selectedRoomType: null,
      };
  },
  async created() {
      try {
          const response = await axiosInstance.get('/room/roomtypes');
          this.roomTypes = response.data;
      } catch (error) {
          console.error('Failed to fetch room types', error);
      } finally {
          this.isLoading = false;
      }
  },
  methods: {
      handleBooking(type) {
          const authStore = useAuthStore();
          if (authStore.isAuthenticated) {
              this.selectedRoomType = type;
              this.showBookingModal = true;
          } else {
              this.$router.push('/login');
          }
      },
      closeModal() {
          this.showBookingModal = false;
          this.selectedRoomType = null;
      }
  }
};
</script>