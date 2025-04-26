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
import AddServiceView from "@/views/Add-ServiceView.vue";
import ListAuctionView from "@/views/ListAuctionView.vue";
import DetailAuctionView from "@/views/DetailAuctionView.vue";
import AddPostView from "@/views/Add-PostView.vue";
import AllPostsView from "@/views/AllPostsView.vue";
import TransactionHistoryView from "@/views/TransactionHistoryView.vue";
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
    {
      path: "/add-service",
      name: "add-service",
      component: AddServiceView,
    },
    {
      path: "/list-auction",
      name: "list-auction",
      component: ListAuctionView,
    },
    {
      path: "/auction/:id",
      name: "detail-auction",
      component: DetailAuctionView,
      props: true,
    },
    {
      path: "/add-post",
      name: "add-post",
      component: AddPostView,
    },
    {
      path: "/all-posts",
      name: "all-posts",
      component: AllPostsView,
    },
    {
      path: "/callback",
      name: "OAuthCallback",
      component: LoginView,
    },
    {
      path: "/transactions",
      name: "transactions",
      component: () => import("@/views/ServiceView.vue"),
    },
    {
      path: "/transaction-history",
      name: "transaction-history",
      component: TransactionHistoryView,
    },
  ],
});

export default router;
