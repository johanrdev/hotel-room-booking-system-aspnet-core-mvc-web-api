import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import AccountView from '../views/AccountView.vue';
import RoomTypeView from '../views/RoomTypeView.vue';
import AdminDashboardView from '../views/admin/DashboardView.vue';
import ManageUsersView from '../views/admin/ManageUsersView.vue';
import ManageBookingsView from '../views/admin/ManageBookingsView.vue';
import ManageRoomsView from '../views/admin/ManageRoomsView.vue';
import UserBookingsView from '../views/UserBookingsView.vue';
import axiosInstance from '../axiosInstance';

const routes = [
  { path: '/', name: 'Home', component: HomeView },
  { path: '/login', name: 'Login', component: LoginView },
  { path: '/register', name: 'Register', component: RegisterView },
  { path: '/account', name: 'Account', component: AccountView },
  { path: '/bookings', name: 'MyBookings', component: UserBookingsView },
  { path: '/roomtypes', name: 'RoomTypes', component: RoomTypeView },
  { path: '/admin', name: 'Admin', component: AdminDashboardView, meta: { requiresAdmin: true } },
  { path: '/admin/manage-users', name: 'ManageUsers', component: ManageUsersView, meta: { requiresAdmin: true } },
  { path: '/admin/manage-bookings', name: 'ManageBookings', component: ManageBookingsView, meta: { requiresAdmin: true } },
  { path: '/admin/manage-rooms', name: 'ManageRooms', component: ManageRoomsView, meta: { requiresAdmin: true } },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authRequiredForRoutes = ['/account', '/bookings', '/admin', '/admin/manage-users', '/admin/manage-bookings', '/admin/manage-rooms'];
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
      next('/account');
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
