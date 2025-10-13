using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAccountsAsync()
        {
            // Placeholder for getting accounts logic
            return Ok(await _accountsRepository.GetAllAccountsAsync());
        }

        [HttpPost("/{accountId}")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] AccountData accountData)
        {
            // Placeholder for creating an account logic
            
            var result = await _accountsRepository.AddAccountAsync(accountData);
            if (!result)
            {
                return BadRequest("Could not create account."); ;
            }

            return Ok(accountData);
        }
    }
}
