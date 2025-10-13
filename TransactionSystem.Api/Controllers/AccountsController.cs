using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TransactionSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAccounts()
        {
            // Placeholder for getting accounts logic
            return Ok(new[] { "Account1", "Account2", "Hello World" });
        }
    }
}
