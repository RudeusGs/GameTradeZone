import baseApi from "./base.api";
const forumStatsApi = {
  getForumStats: async () => {
    return await baseApi.get("/ForumStats/getForumStats");
  },
  getPostStats: async () => {
    return await baseApi.get("/ForumStats/getPostStats");
  },
  getUserStats: async () => {
    return await baseApi.get("/ForumStats/getUserStats");
  },
};
