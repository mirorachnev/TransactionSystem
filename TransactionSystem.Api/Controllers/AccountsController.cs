using Microsoft.AspNetCore.Mvc;
using TransactionSystem.Api.Repositories;
using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Controllers
{
    /// <summary>
    /// Accounts controller to manage bank accounts and transactions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsRepository _accountsRepository;

        /// <summary>
        /// Constructor for AccountsController.
        /// </summary>
        /// <param name="accountsRepository">Accounts Repository</param>
        public AccountsController(IAccountsRepository accountsRepository) 
        {
            _accountsRepository = accountsRepository;
        }

        /// <summary>
        /// Retrieves all account data asynchronously.
        /// </summary>
        /// <remarks>This method fetches a collection of all accounts from the repository and returns them
        /// as an HTTP 200 OK response.</remarks>
        /// <returns>An <see cref="ActionResult{T}"/> containing an <see cref="IEnumerable{T}"/> of <see cref="AccountData"/>
        /// objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountData>>> GetAllAccountsAsync()
        {
            return Ok(await _accountsRepository.GetAllAccountsAsync());
        }

        /// <summary>
        /// Retrieves account details for the specified account ID.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch account data from the
        /// repository. Ensure that the <paramref name="accountId"/> is a valid, non-empty string.</remarks>
        /// <param name="accountId">The unique identifier of the account to retrieve.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing the account details as an <see cref="AccountData"/> object if
        /// the account is found; otherwise, a <see cref="NotFoundResult"/>.</returns>
        [HttpGet("{accountId}")]
        public async Task<ActionResult<AccountData>> GetAccountByIdAsync(string accountId)
        {
            var result = await _accountsRepository.GetAccountByIdAsync(accountId);

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        /// <summary>
        /// Creates a new account with the specified account data.
        /// </summary>
        /// <remarks>This method validates the initial balance of the account and ensures it is
        /// non-negative.  If the account creation fails due to a server-side issue, an appropriate error response is
        /// returned.</remarks>
        /// <param name="accountData">The account data to create the new account. The <see cref="AccountData.Balance"/> property must not be
        /// negative.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing the created <see cref="AccountData"/> if the operation is
        /// successful. Returns a <see cref="BadRequestObjectResult"/> if the initial balance is negative. Returns a
        /// <see cref="StatusCodeResult"/> with status code 500 if the account could not be created due to a server
        /// error.</returns>
        [HttpPost]
        public async Task<ActionResult<AccountData>> CreateAccountAsync([FromBody] AccountData accountData)
        {
            if (accountData.Balance < 0)
                return BadRequest("Initial balance cannot be negative");

            var result = await _accountsRepository.AddAccountAsync(accountData);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Account with id {accountData.AccountId} cannot be created");
            }

            return Ok(accountData);
        }

        /// <summary>
        /// Deletes the account with the specified identifier.
        /// </summary>
        /// <remarks>This method attempts to delete the account identified by <paramref name="accountId"/>
        /// using the underlying repository. If the deletion fails, a 500 Internal Server Error  response is returned
        /// with an appropriate error message.</remarks>
        /// <param name="accountId">The unique identifier of the account to be deleted. Cannot be null or empty.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.  Returns <see cref="OkResult"/> if the
        /// account is successfully deleted, or a  <see cref="StatusCodeResult"/> with status code 500 if the deletion
        /// fails.</returns>
        [HttpDelete("{accountId}")] 
        public async Task<ActionResult> RemoveAccountAsync(string accountId)
        {
            var result = await _accountsRepository.RemoveAccountAsync(accountId);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Account with id {accountId} cannot be deleted");
            }
            
            return Ok();
        }

        /// <summary>
        /// Deposits the specified amount of money into the account with the given account ID.
        /// </summary>
        /// <remarks>This method performs a validation to ensure the deposit amount is positive.  If the
        /// deposit operation fails due to an internal error, a 500 status code is returned.</remarks>
        /// <param name="accountId">The unique identifier of the account to deposit money into.</param>
        /// <param name="amount">The amount of money to deposit. Must be a positive value.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="BadRequestObjectResult"/> if the <paramref name="amount"/> is not positive.  Returns <see
        /// cref="OkResult"/> if the deposit is successful.  Returns <see cref="ObjectResult"/> with a status code of
        /// 500 if the deposit operation fails.</returns>
        [HttpPut("deposit/{accountId}")]
        public async Task<ActionResult> DepositMoneyAsync(string accountId, decimal amount)
        { 
            if (amount <= 0)
                return BadRequest("Deposit amount must be positive");

            var result = await _accountsRepository.DepositMoneyAsync(accountId, amount);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Deposit to account with id {accountId} failed");
            }

            return Ok();
        }

        /// <summary>
        /// Processes a withdrawal request for the specified account.
        /// </summary>
        /// <remarks>This method performs a withdrawal operation by delegating the request to the accounts
        /// repository.  Ensure that the <paramref name="accountId"/> is valid and the account has sufficient funds
        /// before calling this method.</remarks>
        /// <param name="accountId">The unique identifier of the account from which the money will be withdrawn.</param>
        /// <param name="amount">The amount of money to withdraw. Must be greater than zero.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="BadRequestObjectResult"/> if the amount is not positive,  <see cref="StatusCodeResult"/> with status
        /// 500 if the withdrawal fails,  or <see cref="OkResult"/> if the operation succeeds.</returns>
        [HttpPut("withdraw/{accountId}")]
        public async Task<ActionResult> WithdrawMoneyAsync(string accountId, decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Withdraw amount must be positive");

            var result = await _accountsRepository.WithdrawMoneyAsync(accountId, amount);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Withdraw from account with id {accountId} failed");
            }

            return Ok();
        }

        /// <summary>
        /// Transfers a specified amount of money from one account to another.
        /// </summary>
        /// <remarks>This method performs basic validation on the input parameters and delegates the
        /// transfer operation to the accounts repository. Ensure that the provided account IDs are valid and that the
        /// accounts exist in the system.</remarks>
        /// <param name="fromAccountId">The unique identifier of the account from which the money will be transferred.</param>
        /// <param name="toAccountId">The unique identifier of the account to which the money will be transferred.</param>
        /// <param name="amount">The amount of money to transfer. Must be a positive value.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns <see
        /// cref="BadRequestObjectResult"/> if the transfer amount is not positive or if the source and destination
        /// accounts are the same. Returns <see cref="StatusCodeResult"/> with status code 500 if the transfer fails due
        /// to an internal error. Returns <see cref="OkResult"/> if the transfer is successful.</returns>
        [HttpPut("transfer/{fromAccountId}/{toAccountId}")]
        public async Task<ActionResult> TransferMoneyAsync(string fromAccountId, string toAccountId, decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Transfer amount must be positive");
            
            if (fromAccountId == toAccountId)
                return BadRequest("Cannot transfer to the same account");

            var result = await _accountsRepository.TransferMoneyAsync(fromAccountId, toAccountId, amount);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Transfer from account with id {fromAccountId} to account with id {toAccountId} failed");
            }
            
            return Ok();
        }
    }
}
