import baseApi from "./base.api"; // Adjust path if needed

const transactionApi = {
  getRechargeHistory: async () => {
    // Use the full path from Swagger
    return await baseApi.get("/RechargeBank/recharge");
  },
  getWithdrawHistory: async () => {
    // Use the full path from Swagger
    return await baseApi.get("/RechargeBank/withdraws");
  },
  // Add other transaction-related API calls if needed
};

export default transactionApi;
