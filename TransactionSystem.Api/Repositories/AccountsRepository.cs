using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly IDictionary<string, AccountData> _accountsRepository;
        
        public AccountsRepository() 
        {
            _accountsRepository = new Dictionary<string, AccountData>();
        }

        public async Task<bool> AddAccountAsync(AccountData account)
        {
            return await Task.FromResult(_accountsRepository.TryAdd(account.AccountId, account));
        }

        public async Task<AccountData?> GetAccountByIdAsync(string accountId)
        {
            if (_accountsRepository.TryGetValue(accountId, out var accountData))
            {
                return await Task.FromResult(accountData);
            }

            return await Task.FromResult<AccountData?>(default);
        }

        public async Task<IEnumerable<AccountData>> GetAllAccountsAsync()
        {
            return await Task.FromResult(_accountsRepository.Values);
        }
    }
}
