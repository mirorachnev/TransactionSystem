using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Repositories
{
    public interface IAccountsRepository
    {
        Task<IEnumerable<AccountData>> GetAllAccountsAsync();

        Task<AccountData?> GetAccountByIdAsync(string accountId);

        Task<bool> AddAccountAsync(AccountData account);

        Task<bool> RemoveAccountAsync(string accountId);

        Task<bool> DepositMoneyAsync(string accountId, decimal amount);

        Task<bool> WithdrawMoneyAsync(string accountId, decimal amount);

        Task<bool> TransferMoneyAsync(string fromAccountId, string toAccountId, decimal amount);
    }
}
