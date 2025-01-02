<template>
    <div v-if="isOpen" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
      <div class="bg-white p-8 rounded shadow-md w-full max-w-md">
        <h2 class="text-2xl font-bold mb-4">Edit Room Type</h2>
        <form @submit.prevent="emitUpdateEvent" class="space-y-4">
          <div>
            <label for="roomTypeName" class="block text-sm font-medium text-gray-700">Room Type Name</label>
            <input v-model="roomTypeName" id="roomTypeName" type="text" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div>
            <label for="roomTypeDescription" class="block text-sm font-medium text-gray-700">Description</label>
            <textarea v-model="roomTypeDescription" id="roomTypeDescription" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"></textarea>
          </div>
          <div>
            <label for="roomTypePrice" class="block text-sm font-medium text-gray-700">Price</label>
            <input v-model="roomTypePrice" id="roomTypePrice" type="number" class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
          </div>
          <div class="flex justify-end">
            <button @click="closeModal" type="button" class="mr-4 py-2 px-4 bg-gray-600 text-white rounded hover:bg-gray-700 focus:outline-none">Cancel</button>
            <button type="submit" class="py-2 px-4 bg-indigo-600 text-white font-medium rounded hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">Update Room Type</button>
          </div>
        </form>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    props: {
      isOpen: {
        type: Boolean,
        required: true
      },
      roomType: {
        type: Object,
        required: true
      }
    },
    data() {
      return {
        roomTypeName: this.roomType.name,
        roomTypeDescription: this.roomType.description,
        roomTypePrice: this.roomType.price
      };
    },
    methods: {
      closeModal() {
        this.$emit('close');
      },
      emitUpdateEvent() {
        const updatedRoomType = {
          id: this.roomType.id,
          name: this.roomTypeName,
          description: this.roomTypeDescription,
          price: this.roomTypePrice
        };
        this.$emit('update', updatedRoomType);
        this.closeModal();
      }
    }
  };
  </script>
  