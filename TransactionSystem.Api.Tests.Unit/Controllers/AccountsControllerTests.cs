using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionSystem.Api.Controllers;
using TransactionSystem.Api.Repositories;
using TransactionSystem.Api.Repositories.Models;

namespace TransactionSystem.Api.Tests.Unit.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
        private readonly AccountsController _accountsController;

        public AccountsControllerTests()
        {
            _accountsRepositoryMock = new Mock<IAccountsRepository>();
            _accountsController = new AccountsController(_accountsRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAccountsAsyncOkTest()
        {
            var accounts = new[]
             {
                new AccountData { AccountId = "1", Balance = 100, Name = "Test" },
                new AccountData { AccountId = "2", Balance = 200, Name = "Test" },
                new AccountData { AccountId = "3", Balance = 300, Name = "Test" }
            };

            _accountsRepositoryMock.Setup(repo => repo.GetAllAccountsAsync())
                .ReturnsAsync(accounts);

            var result = await _accountsController.GetAllAccountsAsync();

            var objectResult = result.Result as OkObjectResult;
            var resultAccounts = objectResult?.Value as IEnumerable<AccountData>;

            resultAccounts.Should().BeEquivalentTo(accounts);
        }

        [Fact]
        public async Task GetAllAccountsAsyncOkEmptyDataTest()
        {
            _accountsRepositoryMock.Setup(repo => repo.GetAllAccountsAsync())
                .ReturnsAsync([]);

            var result = await _accountsController.GetAllAccountsAsync();

            var objectResult = result.Result as OkObjectResult;
            var resultAccounts = objectResult?.Value as IEnumerable<AccountData>;

            resultAccounts.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAccountByIdAsyncOkTest()
        {
            var account = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            _accountsRepositoryMock.Setup(repo => repo.GetAccountByIdAsync("1"))
                .ReturnsAsync(account);
            var result = await _accountsController.GetAccountByIdAsync("1");
            var objectResult = result.Result as OkObjectResult;
            var resultAccount = objectResult?.Value as AccountData;
            resultAccount.Should().BeEquivalentTo(account);
        }

        [Fact]
        public async Task GetAccountByIdAsyncNotFoundTest()
        {
            _accountsRepositoryMock.Setup(repo => repo.GetAccountByIdAsync("nonexistent"))
                .ReturnsAsync((AccountData?)null);
            var result = await _accountsController.GetAccountByIdAsync("nonexistent");
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAccountAsyncOkTest()
        {
            var account = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            _accountsRepositoryMock.Setup(repo => repo.AddAccountAsync(account))
                .ReturnsAsync(true);
            var result = await _accountsController.CreateAccountAsync(account);
            var objectResult = result.Result as OkObjectResult;
            var resultAccount = objectResult?.Value as AccountData;
            resultAccount.Should().BeEquivalentTo(account);
        }

        [Fact]
        public async Task CreateAccountAsyncBadRequestTest()
        {
            var account = new AccountData { AccountId = "1", Balance = -100, Name = "Test" };
            var result = await _accountsController.CreateAccountAsync(account);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAccountAsyncInternalServerErrorTest()
        {
            var account = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            _accountsRepositoryMock.Setup(repo => repo.AddAccountAsync(account))
                .ReturnsAsync(false);
            var result = await _accountsController.CreateAccountAsync(account);
            var objectResult = result.Result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult?.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task RemoveAccountAsyncOkTest()
        {
            _accountsRepositoryMock.Setup(repo => repo.RemoveAccountAsync("1"))
                .ReturnsAsync(true);
            var result = await _accountsController.RemoveAccountAsync("1");
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveAccountAsyncInternalServerErrorTest()
        {
            _accountsRepositoryMock.Setup(repo => repo.RemoveAccountAsync("1"))
                .ReturnsAsync(false);
            var result = await _accountsController.RemoveAccountAsync("1");
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult?.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task RemoveAccountAsyncNotFoundTest()
        {
            _accountsRepositoryMock.Setup(repo => repo.RemoveAccountAsync("nonexistent"))
                .ReturnsAsync(false);
            var result = await _accountsController.RemoveAccountAsync("nonexistent");
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult?.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task DepositMoneyAsyncOkTest()
        {

        }

        [Fact]
        public async Task DepositMoneyAsyncNotFoundTest()
        {

        }

        [Fact]
        public async Task DepositMoneyAsyncNegativeAmountTest()
        {

        }

        [Fact]
        public async Task WithdrawMoneyAsyncOkTest()
        {

        }

        [Fact]
        public async Task WithdrawMoneyAsyncNotFoundTest()
        {

        }

        [Fact]
        public async Task WithdrawMoneyAsyncNegativeAmountTest()
        {

        }

        [Fact]
        public async Task TransferMoneyAsyncOkTest()
        {

        }

        [Fact]
        public async Task TransferMoneyAsyncNotFoundTest()
        {

        }

        [Fact]
        public async Task TransferMoneyAsyncNegativeAmountTest()
        {

        }
    }
}
