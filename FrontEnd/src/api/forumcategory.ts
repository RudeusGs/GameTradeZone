import baseApi from "./base.api";

const forumCategoryApi = {
  createCategory: async (data: any) => {
    // Assuming data contains category details
    return await baseApi.post("/forumscategory/create", data);
  },
  getAllCategories: async () => {
    return await baseApi.get("/forumscategory/getall");
  },
  deleteCategory: async (id: string) => {
    // Assuming ID is passed for deletion
    return await baseApi.delete(`/forumscategory/delete/${id}`); // Or pass ID in data/params if needed
  },
  getAllCategoryStats: async () => {
    return await baseApi.get("/forumscategory/getallstat");
  },
};

export default forumCategoryApi;
