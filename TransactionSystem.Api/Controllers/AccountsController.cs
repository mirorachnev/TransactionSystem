using Microsoft.AspNetCore.Mvc;
using TransactionSystem.Api.Repositories;
using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsRepository _accountsRepository;
        
        public AccountsController(IAccountsRepository accountsRepository) 
        {
            _accountsRepository = accountsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountData>>> GetAllAccountsAsync()
        {
            return Ok(await _accountsRepository.GetAllAccountsAsync());
        }

        [HttpGet("{accountId}")]
        public async Task<ActionResult<AccountData>> GetAccountByIdAsync(string accountId)
        {
            var result = await _accountsRepository.GetAccountByIdAsync(accountId);

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

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
    }
}
