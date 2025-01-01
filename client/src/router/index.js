import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import ProfileView from '../views/ProfileView.vue';
import RoomTypeView from '../views/RoomTypeView.vue';
import AdminDashboard from '../views/admin/DashboardView.vue';
import ManageUsers from '../views/admin/ManageUsers.vue';
import ManageBookings from '../views/admin/ManageBookings.vue';
import ManageRooms from '../views/admin/ManageRooms.vue';
import axiosInstance from '../axiosInstance';

const routes = [
  { path: '/login', name: 'Login', component: LoginView },
  { path: '/register', name: 'Register', component: RegisterView },
  { path: '/profile', name: 'Profile', component: ProfileView },
  { path: '/roomtypes', name: 'RoomTypes', component: RoomTypeView },
  { path: '/admin', name: 'Admin', component: AdminDashboard, meta: { requiresAdmin: true } },
  { path: '/admin/manage-users', name: 'ManageUsers', component: ManageUsers, meta: { requiresAdmin: true } },
  { path: '/admin/manage-bookings', name: 'ManageBookings', component: ManageBookings, meta: { requiresAdmin: true } },
  { path: '/admin/manage-rooms', name: 'ManageRooms', component: ManageRooms, meta: { requiresAdmin: true } },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authRequiredForRoutes = ['/profile', '/admin', '/admin/manage-users', '/admin/manage-bookings', '/admin/manage-rooms'];
  const guestOnlyRoutes = ['/login', '/register'];
  const authRequired = authRequiredForRoutes.includes(to.path);
  const guestOnly = guestOnlyRoutes.includes(to.path);

  try {
    const response = await axiosInstance.get('/auth/check-auth');
    const isAuthenticated = response.data.isAuthenticated;
    const isAdmin = response.data.isAdmin;

    if (authRequired && !isAuthenticated) {
      next('/login');
    } else if (guestOnly && isAuthenticated) {
      next('/profile');
    } else if (to.meta.requiresAdmin && !isAdmin) {
      next('/login');
    } else {
      next();
    }
  } catch (error) {
    if (authRequired) {
      next('/login');
    } else {
      next();
    }
  }
});

export default router;
