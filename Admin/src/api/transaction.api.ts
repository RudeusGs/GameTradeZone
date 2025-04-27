import baseApi from "./base.api"; // Assuming you have a baseApi setup

// Define interfaces based on C# models if needed, e.g.:
// interface WithdrawMoneyModel { ... }

const transactionApi = {
  getAllTransactions: async () => {
    return await baseApi.get("/RechargeBank/transactions");
  },

  getAllWithdrawRequests: async () => {
    return await baseApi.get("/RechargeBank/all-withdraws");
  },

  confirmWithdrawal: async (id: number) => {
    return await baseApi.post(`/RechargeBank/withdraw/${id}`, null, {
      params: {
        id: 0,
      },
    });
  },

  getUserWithdrawHistory: async () => {
    return await baseApi.get("/RechargeBank/withdraws");
  },
  getUserRechargeHistory: async () => {
    return await baseApi.get("/RechargeBank/recharge");
  },
};

export default transactionApi;
