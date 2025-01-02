<template>
  <div v-if="isOpen" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
    <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
      <h2 class="text-2xl font-bold mb-4">Edit Booking</h2>
      <form @submit.prevent="updateBooking" class="space-y-4">
        <div>
          <label for="editRoom" class="block text-sm font-medium text-gray-700">Room Number</label>
          <select v-model="selectedRoomId" id="editRoom" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
            <optgroup v-for="type in roomTypes" :label="type.name" :key="type.id">
              <option v-for="room in groupedRooms[type.id]" :value="room.id" :key="room.id">{{ room.number }}</option>
            </optgroup>
          </select>
        </div>
        <div>
          <label for="editCheckInDate" class="block text-sm font-medium text-gray-700">Check-In Date</label>
          <input v-model="formattedCheckInDate" id="editCheckInDate" type="date" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
        </div>
        <div>
          <label for="editCheckOutDate" class="block text-sm font-medium text-gray-700">Check-Out Date</label>
          <input v-model="formattedCheckOutDate" id="editCheckOutDate" type="date" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
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
    booking: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      rooms: [],
      groupedRooms: {},
      roomTypes: [],
      selectedRoomId: null,
      formattedCheckInDate: '',
      formattedCheckOutDate: ''
    };
  },
  watch: {
    booking: {
      immediate: true,
      handler(newBooking) {
        this.formattedCheckInDate = this.formatDateForInput(newBooking.checkInDate);
        this.formattedCheckOutDate = this.formatDateForInput(newBooking.checkOutDate);
        this.selectedRoomId = newBooking.room.id;
      }
    }
  },
  async created() {
    await this.fetchRoomTypes();
    await this.fetchRooms();
  },
  methods: {
    closeModal() {
      this.$emit('close');
    },
    async fetchRooms() {
      try {
        const response = await axiosInstance.get('/admin/available-rooms');
        this.rooms = response.data;
        this.groupRoomsByType();
      } catch (error) {
        console.error('Failed to fetch rooms', error);
      }
    },
    async fetchRoomTypes() {
      try {
        const response = await axiosInstance.get('/admin/room-types');
        this.roomTypes = response.data.map(type => ({ id: type.id, name: type.name, description: type.description, price: type.price }));
      } catch (error) {
        console.error('Failed to fetch room types', error);
      }
    },
    groupRoomsByType() {
      const grouped = this.rooms.reduce((acc, room) => {
        if (!acc[room.roomTypeId]) {
          acc[room.roomTypeId] = [];
        }
        acc[room.roomTypeId].push(room);
        return acc;
      }, {});
      this.groupedRooms = grouped;
    },
    formatDateForInput(date) {
      const localDate = new Date(date);
      localDate.setMinutes(localDate.getMinutes() - localDate.getTimezoneOffset());
      return localDate.toISOString().split('T')[0];
    },
    updateBooking() {
      const updatedBooking = {
        ...this.booking,
        roomId: this.selectedRoomId,
        checkInDate: new Date(this.formattedCheckInDate).toISOString(),
        checkOutDate: new Date(this.formattedCheckOutDate).toISOString()
      };
      this.$emit('update', updatedBooking);
    }
  }
};
</script>
