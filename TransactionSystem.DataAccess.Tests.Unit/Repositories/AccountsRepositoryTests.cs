using FluentAssertions;
using TransactionSystem.DataAccess.Repositories;
using TransactionSystem.DataAccess.Repositories.Models;

namespace TransactionSystem.DataAccess.Tests.Unit.Repositories
{
    public class AccountsRepositoryTests
    {
        public AccountsRepositoryTests() { }

        [Fact]
        public async Task GetAllAccountsAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();

            var accounts = new[]
            {
                new AccountData { AccountId = "1", Balance = 100, Name = "Test" },
                new AccountData { AccountId = "2", Balance = 200, Name = "Test" },
                new AccountData { AccountId = "3", Balance = 300, Name = "Test" }
            };

            await accountsRepository.AddAccountAsync(accounts[0]);
            await accountsRepository.AddAccountAsync(accounts[1]);
            await accountsRepository.AddAccountAsync(accounts[2]);

            var result = await accountsRepository.GetAllAccountsAsync();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(accounts);
        }

        [Fact]
        public async Task GetAllAccountsAsyncOkEmptyDataTest()
        {
            var accountsRepository = new AccountsRepository();

            var result = await accountsRepository.GetAllAccountsAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAccountByIdAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();
            var account = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);
            var result = await accountsRepository.GetAccountByIdAsync("1");
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(account);
        }

        [Fact]
        public async Task GetAccountByIdAsyncNotFoundTest()
        {
            var accountsRepository = new AccountsRepository();
            var result = await accountsRepository.GetAccountByIdAsync("nonexistent");
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddAccountAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();
            var account = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            var result = await accountsRepository.AddAccountAsync(account);
            result.Should().BeTrue();
            var retrievedAccount = await accountsRepository.GetAccountByIdAsync("1");
            retrievedAccount.Should().NotBeNull();
            retrievedAccount.Should().BeEquivalentTo(account);
        }

        [Fact]
        public async Task AddAccountAsyncDuplicateIdTest()
        {
            var accountsRepository = new AccountsRepository();
            var account1 = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            var account2 = new AccountData { AccountId = "1", Balance = 200, Name = "Test2" };
            var result1 = await accountsRepository.AddAccountAsync(account1);
            var result2 = await accountsRepository.AddAccountAsync(account2);
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            var retrievedAccount = await accountsRepository.GetAccountByIdAsync("1");
            retrievedAccount.Should().NotBeNull();
            retrievedAccount.Should().BeEquivalentTo(account1);
        }

        [Fact]
        public async Task RemoveAccountAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();
            var account = new AccountData { AccountId = "1", Balance = 100, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);
            var result = await accountsRepository.RemoveAccountAsync("1");
            result.Should().BeTrue();
            var retrievedAccount = await accountsRepository.GetAccountByIdAsync("1");
            retrievedAccount.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAccountAsyncNotFoundTest()
        {
            var accountsRepository = new AccountsRepository();
            var result = await accountsRepository.RemoveAccountAsync("nonexistent");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DepositMoneyAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();
            var initialBalance = 100;
            var account = new AccountData { AccountId = "1", Balance = initialBalance, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);

            var amount = 100;

            await accountsRepository.DepositMoneyAsync(account.AccountId, amount);

            var result = await accountsRepository.GetAccountByIdAsync(account.AccountId);

            result.Should().NotBeNull();
            result?.Balance.Should().Be(amount + initialBalance);
        }

        [Fact]
        public async Task DepositMoneyAsyncNotFoundTest()
        {
            var accountsRepository = new AccountsRepository();
            var initialBalance = 100;
            var account = new AccountData { AccountId = "1", Balance = initialBalance, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);

            var amount = 10;

            var result = await accountsRepository.DepositMoneyAsync("2", amount);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task WithdrawMoneyAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();
            var initialBalance = 100;
            var account = new AccountData { AccountId = "1", Balance = initialBalance, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);
            var amount = 50;
            await accountsRepository.WithdrawMoneyAsync(account.AccountId, amount);
            var result = await accountsRepository.GetAccountByIdAsync(account.AccountId);
            result.Should().NotBeNull();
            result?.Balance.Should().Be(initialBalance - amount);
        }

        [Fact]
        public async Task WithdrawMoneyAsyncNotFoundTest()
        {
            var accountsRepository = new AccountsRepository();
            var initialBalance = 100;
            var account = new AccountData { AccountId = "1", Balance = initialBalance, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);
            var amount = 50;
            var result = await accountsRepository.WithdrawMoneyAsync("2", amount);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task WithdrawMoneyAsyncInsufficientFundsTest()
        {
            var accountsRepository = new AccountsRepository();
            var initialBalance = 100;
            var account = new AccountData { AccountId = "1", Balance = initialBalance, Name = "Test" };
            await accountsRepository.AddAccountAsync(account);
            var amount = 150;
            var result = await accountsRepository.WithdrawMoneyAsync(account.AccountId, amount);
            result.Should().BeFalse();
            var retrievedAccount = await accountsRepository.GetAccountByIdAsync(account.AccountId);
            retrievedAccount.Should().NotBeNull();
            retrievedAccount?.Balance.Should().Be(initialBalance);
        }

        [Fact]
        public async Task TransferMoneyAsyncOkTest()
        {
            var accountsRepository = new AccountsRepository();
            var accountFrom = new AccountData { AccountId = "1", Balance = 200, Name = "Test" };
            var accountTo = new AccountData { AccountId = "2", Balance = 100, Name = "Test" };
            await accountsRepository.AddAccountAsync(accountFrom);
            await accountsRepository.AddAccountAsync(accountTo);
            var amount = 50;
            var result = await accountsRepository.TransferMoneyAsync(accountFrom.AccountId, accountTo.AccountId, amount);
            result.Should().BeTrue();
            var updatedAccountFrom = await accountsRepository.GetAccountByIdAsync(accountFrom.AccountId);
            var updatedAccountTo = await accountsRepository.GetAccountByIdAsync(accountTo.AccountId);
            updatedAccountFrom.Should().NotBeNull();
            updatedAccountTo.Should().NotBeNull();
            updatedAccountFrom?.Balance.Should().Be(150);
            updatedAccountTo?.Balance.Should().Be(150);
        }

        [Fact]
        public async Task TransferMoneyAsyncInsufficientFundsTest()
        {
            var accountsRepository = new AccountsRepository();
            var accountFrom = new AccountData { AccountId = "1", Balance = 50, Name = "Test" };
            var accountTo = new AccountData { AccountId = "2", Balance = 100, Name = "Test" };
            await accountsRepository.AddAccountAsync(accountFrom);
            await accountsRepository.AddAccountAsync(accountTo);
            var amount = 100;
            var result = await accountsRepository.TransferMoneyAsync(accountFrom.AccountId, accountTo.AccountId, amount);
            result.Should().BeFalse();
            var updatedAccountFrom = await accountsRepository.GetAccountByIdAsync(accountFrom.AccountId);
            var updatedAccountTo = await accountsRepository.GetAccountByIdAsync(accountTo.AccountId);
            updatedAccountFrom.Should().NotBeNull();
            updatedAccountTo.Should().NotBeNull();
            updatedAccountFrom?.Balance.Should().Be(50);
            updatedAccountTo?.Balance.Should().Be(100);
        }
    }
}
