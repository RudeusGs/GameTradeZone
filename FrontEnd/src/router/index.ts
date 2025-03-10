import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import LoginView from "@/views/LoginView.vue";
import RegisterView from "@/views/RegisterView.vue";
import ProfileView from "@/views/ProfileView.vue";
import PurchasedView from "@/views/PurchasedView.vue";
import PolicyView from "@/views/PolicyView.vue";
import RechargeView from "@/views/RechargeView.vue";
import HPurchasedView from "@/views/HPurchasedView.vue";
import AddAccountView from "@/views/Add-AccountView.vue";
import ForumsView from "@/views/ForumsView.vue";
import PostDetailsView from "@/views/PostDetailsView.vue";
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
    },
    {
      path: "/register",
      name: "register",
      component: RegisterView,
    },
    {
      path: "/profile",
      name: "profile",
      component: ProfileView,
    },
    {
      path: "/purchased",
      name: "purchased",
      component: PurchasedView,
    },
    {
      path: "/policy",
      name: "policy",
      component: PolicyView,
    },
    {
      path: "/recharge",
      name: "recharge",
      component: RechargeView,
    },
    {
      path: "/hpurcharsed",
      name: "hpurcharsed",
      component: HPurchasedView,
    },
    {
      path: "/add-account",
      name: "add-account",
      component: AddAccountView,
    },
    {
      path: "/forums",
      name: "forums",
      component: ForumsView,
    },
    {
      path: "/post/:id",
      name: "post-details",
      component: PostDetailsView,
    },
  ],
});

export default router;
