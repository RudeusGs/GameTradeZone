using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.RechargeBank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Interfaces
{
    public interface IRechargeBankService
    {
        Task<ApiResult> GetAllRechargeBankTransactions();
        Task<ApiResult> GetAllWithdrawMoneyRequest();
        Task<ApiResult> WithdrawMoneyRequest(WithdrawMoneyModel model);
        Task<ApiResult> WithdrawMoneyConfirmRequest(int id);
        Task<ApiResult> GetAllWithDrawTransactionsByUser();
        Task<ApiResult> GetAllRechargeBankTransactionsByUser();
    }
}
