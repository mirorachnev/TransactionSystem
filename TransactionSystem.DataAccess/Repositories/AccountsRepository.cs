using TransactionSystem.DataAccess.Repositories.Models;

namespace TransactionSystem.DataAccess.Repositories
{
    /// <summary>
    /// Provides an in-memory implementation of the <see cref="IAccountsRepository"/> interface for managing account
    /// data.
    /// </summary>
    /// <remarks>This repository supports asynchronous operations for adding, removing, retrieving, and
    /// updating account data. It ensures thread safety for operations that modify account balances using a <see
    /// cref="SemaphoreSlim"/>.</remarks>
    public class AccountsRepository : IAccountsRepository
    {
        private readonly IDictionary<string, AccountData> _accountsRepository;
        private readonly SemaphoreSlim _semaphoreSlim;

        /// <summary>
        /// Constructor for the <see cref="AccountsRepository"/> class.
        /// </summary>
        public AccountsRepository() 
        {
            _accountsRepository = new Dictionary<string, AccountData>();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        ///<inheritdoc/>
        public async Task<bool> AddAccountAsync(AccountData account)
        {
            return await Task.FromResult(_accountsRepository.TryAdd(account.AccountId, account));
        }

        ///<inheritdoc/>
        public async Task<bool> RemoveAccountAsync(string accountId)
        {
            return await Task.FromResult(_accountsRepository.Remove(accountId));
        }

        ///<inheritdoc/>
        public async Task<AccountData?> GetAccountByIdAsync(string accountId)
        {
            if (_accountsRepository.TryGetValue(accountId, out var accountData))
            {
                return await Task.FromResult(accountData);
            }

            return await Task.FromResult<AccountData?>(default);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<AccountData>> GetAllAccountsAsync()
        {
            return await Task.FromResult(_accountsRepository.Values);
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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
