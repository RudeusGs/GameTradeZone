import { createRouter, createWebHistory } from "vue-router";
import WithdrawRequestsView from "@/views/WithdrawRequestsView.vue";
import HomeView from '../views/HomeView.vue'; // Trang admin hoặc home
import LoginView from '@/views/LoginView.vue';
import AdminWebsiteAccount from '@/views/AdminWebsiteAccount.vue';
import GameInforView from '@/views/GameInforView.vue';
import GameAccountView from '@/views/GameAccountView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      redirect: "/admin",
    },
    {
      path: "/admin",
      name: "admin",
      component: HomeView,
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
    },
    {
      path: "/website-account",
      name: "website-account",
      component: AdminWebsiteAccount,
    },
    {
      path: "/game-infor",
      name: "game-infor",
      component: GameInforView,
    },
    {
      path: "/withdraw-requests",
      name: "withdraw-requests",
      component: WithdrawRequestsView,
    },
    {
      path: '/game-account',
      name: 'game-account',
      component: GameAccountView,
    },
  ],
});

export default router;
