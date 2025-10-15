using TransactionSystem.DataAccess.Repositories.Models;

namespace TransactionSystem.DataAccess.Repositories
{
    /// <summary>
    /// Defines a contract for managing account data and performing account-related operations.
    /// </summary>
    /// <remarks>This interface provides methods for retrieving, adding, and removing accounts, as well as
    /// performing financial transactions such as deposits, withdrawals, and transfers. Implementations of this
    /// interface should ensure thread safety and handle any necessary validation or error handling for account
    /// operations.</remarks>
    public interface IAccountsRepository
    {
        /// <summary>
        /// Asynchronously retrieves all account data.
        /// </summary>
        /// <remarks>This method retrieves all accounts without applying any filters. The caller is
        /// responsible  for handling the returned data appropriately, including any necessary filtering or
        /// processing.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of
        /// AccountData representing all accounts.</returns>
        Task<IEnumerable<AccountData>> GetAllAccountsAsync();

        /// <summary>
        /// Asynchronously retrieves account data for the specified account identifier.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to retrieve. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account data if found;
        /// otherwise, <see langword="null"/>.</returns>
        Task<AccountData?> GetAccountByIdAsync(string accountId);

        /// <summary>
        /// Asynchronously adds a new account to the system.
        /// </summary>
        /// <remarks>This method ensures that the account data is validated before adding it to the
        /// system. Duplicate accounts or invalid data may result in the operation returning <see
        /// langword="false"/>.</remarks>
        /// <param name="account">The account data to be added. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the account
        /// was successfully added; otherwise, <see langword="false"/>.</returns>
        Task<bool> AddAccountAsync(AccountData account);

        /// <summary>
        /// Removes the account with the specified identifier asynchronously.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to be removed. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the account
        /// was successfully removed; otherwise, <see langword="false"/>.</returns>
        Task<bool> RemoveAccountAsync(string accountId);

        /// <summary>
        /// Deposits the specified amount of money into the account with the given account ID asynchronously.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to deposit money into. Cannot be null or empty.</param>
        /// <param name="amount">The amount of money to deposit. Must be a positive value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the deposit
        /// was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> DepositMoneyAsync(string accountId, decimal amount);

        /// <summary>
        /// Attempts to withdraw the specified amount of money from the given account asynchronously.
        /// </summary>
        /// <remarks>The withdrawal may fail if the account does not have sufficient funds, the account ID
        /// is invalid, or other conditions prevent the operation. Ensure that the account ID is valid and the account
        /// has enough balance before calling this method.</remarks>
        /// <param name="accountId">The unique identifier of the account from which the money will be withdrawn.</param>
        /// <param name="amount">The amount of money to withdraw. Must be a positive value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the
        /// withdrawal was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> WithdrawMoneyAsync(string accountId, decimal amount);

        /// <summary>
        /// Transfers a specified amount of money from one account to another asynchronously.
        /// </summary>
        /// <remarks>This method performs the transfer operation asynchronously and ensures that the
        /// transaction is processed atomically. The caller should handle any exceptions that may occur during the
        /// operation, such as network failures or invalid account states.</remarks>
        /// <param name="fromAccountId">The unique identifier of the account from which the money will be debited. Cannot be null or empty.</param>
        /// <param name="toAccountId">The unique identifier of the account to which the money will be credited. Cannot be null or empty.</param>
        /// <param name="amount">The amount of money to transfer. Must be a positive value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the transfer
        /// is successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> TransferMoneyAsync(string fromAccountId, string toAccountId, decimal amount);
    }
}
