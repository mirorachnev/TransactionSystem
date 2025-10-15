using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly IDictionary<string, AccountData> _accountsRepository;
        private readonly SemaphoreSlim _semaphoreSlim;  

        public AccountsRepository() 
        {
            _accountsRepository = new Dictionary<string, AccountData>();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> AddAccountAsync(AccountData account)
        {
            return await Task.FromResult(_accountsRepository.TryAdd(account.AccountId, account));
        }

        public async Task<bool> RemoveAccountAsync(string accountId)
        {
            return await Task.FromResult(_accountsRepository.Remove(accountId));
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

        public async Task<bool> TransferMoneyAsync(string fromAccountId, string toAccountId, decimal amount)
        {
            if (_accountsRepository.TryGetValue(fromAccountId, out var fromAccount) &&
                _accountsRepository.TryGetValue(toAccountId, out var toAccount) &&
                fromAccount.Balance >= amount)
            {
                await _semaphoreSlim.WaitAsync();
                try
                {  
                    fromAccount.Balance -= amount;
                    toAccount.Balance += amount;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
                    
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> DepositMoneyAsync(string accountId, decimal amount)
        {
            if (_accountsRepository.TryGetValue(accountId, out var accountData))
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    accountData.Balance += amount;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> WithdrawMoneyAsync(string accountId, decimal amount)
        {
            if (_accountsRepository.TryGetValue(accountId, out var accountData))
            {
                if (amount > accountData.Balance)
                    return await Task.FromResult(false);

                await _semaphoreSlim.WaitAsync();
                try
                {
                    accountData.Balance -= amount;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
                
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }
    }
}
