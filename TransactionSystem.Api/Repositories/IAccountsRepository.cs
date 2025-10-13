using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Repositories
{
    public interface IAccountsRepository
    {
        Task<IEnumerable<AccountData>> GetAllAccountsAsync();

        Task<AccountData?> GetAccountByIdAsync(string accountId);

        Task<bool> AddAccountAsync(AccountData account);
    }
}
