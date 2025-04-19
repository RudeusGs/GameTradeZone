import baseApi from "./base.api";

const forumApi = {
  likePost: async (postId: string) => {
    return await baseApi.post(`/PostInfo/like/${postId}`, null);
  },
  commentOnPost: async (data: any) => {
    // Assuming data contains postId and comment content
    return await baseApi.post("/PostInfo/comment", data);
  },
  getPostComments: async (postId: string) => {
    return await baseApi.get(`/PostInfo/comments/${postId}`);
  },
  deleteComment: async (commentId: string) => {
    return await baseApi.delete(`/PostInfo/comment/${commentId}`);
  },
  updateComment: async (commentId: string, data: any) => {
    // Assuming data contains updated comment content
    return await baseApi.post(`/PostInfo/comment/${commentId}`, data);
  },
  unlikePost: async (postId: string) => {
    return await baseApi.post(`/PostInfo/unlike/${postId}`, null);
  },
  createPost: async (data: any) => {
    // Assuming data contains post details
    return await baseApi.post("/posts/create", data);
  },
  getPostById: async (id: string) => {
    return await baseApi.get(`/posts/get/${id}`);
  },
  getLatestPosts: async () => {
    return await baseApi.get("/posts/latest");
  },
  deletePost: async (id: string) => {
    return await baseApi.delete(`/posts/delete/${id}`);
  },
  getPostsByCategory: async (categoryId: string) => {
    return await baseApi.get(`/posts/by-category/${categoryId}`);
  },
  getPostsByUser: async (userId: string) => {
    return await baseApi.get(`/posts/by-user/${userId}`);
  },
  updatePost: async (postId: string, data: any) => {
    // Assuming data contains updated post details
    return await baseApi.post(`/posts/update/${postId}`, data);
  },
  GetAllCategoryPostCount: async () => {
    // Removed data parameter as GET usually doesn't need it for this type of request
    return await baseApi.get("/forumscategory/getallcategorypostcount");
  },
  GetPostCount: async () => {
    // Removed data parameter as GET usually doesn't need it for this type of request
    return await baseApi.get("/posts/getpostcount");
  },
};

export default forumApi;
