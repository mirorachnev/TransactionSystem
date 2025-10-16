using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransactionSystem.DataAccess.Repositories;
using TransactionSystem.DataAccess.Repositories.Models;

namespace TransactionSystem.ConsoleApp
{
    internal class Program
    {
        public async static Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton<IAccountsRepository, AccountsRepository>();

            using var serviceProvider = builder.Services.BuildServiceProvider();

            var repositoryService = serviceProvider.GetRequiredService<IAccountsRepository>();


            do
            {
                Console.WriteLine();
                Console.WriteLine("Please choose from the available options");
                Console.WriteLine("1 - Add account");
                Console.WriteLine("2 - Get all accounts");
                Console.WriteLine("3 - Get account by id");
                Console.WriteLine("4 - Delete account by id");
                Console.WriteLine("5 - Deposit money");
                Console.WriteLine("6 - Withdraw money");
                Console.WriteLine("7 - Transfer money");
                Console.WriteLine("Press ESC to stop");

                var inputKey = Console.ReadKey().Key;
                Console.WriteLine();
                switch (inputKey) 
                { 
                    case ConsoleKey.D1:
                        AddAccount(repositoryService);
                        break;
                                            
                    case ConsoleKey.D2:
                        GetAllAccounts(repositoryService);
                        break;

                    case ConsoleKey.D3:
                        GetAccountById(repositoryService);
                        break;

                    case ConsoleKey.D4:
                        DeleteAccountById(repositoryService);
                        break;

                    case ConsoleKey.D5:
                        DepositMoney(repositoryService);
                        break;

                    case ConsoleKey.D6:
                        WithdrawMoney(repositoryService);
                        break;

                    case ConsoleKey.D7:
                        TransferMoney(repositoryService);
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);            

        }

        static void AddAccount(IAccountsRepository repository)
        {
            Console.WriteLine("Please enter account id");
            var accountId = Console.ReadLine();
            Console.WriteLine("Please enter account name");
            var accountName = Console.ReadLine();
            Console.WriteLine("Please enter account balance");
            var accountBalanceStr = Console.ReadLine();
            if (!decimal.TryParse(accountBalanceStr, out var accountBalance) || accountBalance < 0)
            {
                Console.WriteLine("Invalid balance value");
                return;
            }
            var account = new AccountData
            {
                AccountId = accountId ?? string.Empty,
                Name = accountName ?? string.Empty,
                Balance = accountBalance
            };
            var result = repository.AddAccountAsync(account).Result;
            if (result)
            {
                Console.WriteLine("Account added successfully");
            }
            else
            {
                Console.WriteLine("Failed to add account");
            }
        }

        static void GetAllAccounts(IAccountsRepository repository)
        {
            var accounts = repository.GetAllAccountsAsync().Result;
            foreach (var account in accounts)
            {
                Console.WriteLine($"Account id - {account.AccountId}, Name - {account.Name}, balance - {account.Balance}");
            }
        }

        static void GetAccountById(IAccountsRepository repository)
        {
            Console.WriteLine("Please enter account id");
            var accountId = Console.ReadLine();
            var account = repository.GetAccountByIdAsync(accountId ?? string.Empty).Result;
            if (account == null)
            {
                Console.WriteLine("Account not found");
            }
            else
            {
                Console.WriteLine($"Account id - {account.AccountId}, Name - {account.Name}, balance - {account.Balance}");
            }
        }

        static void DeleteAccountById(IAccountsRepository repository)
        {
            Console.WriteLine("Please enter account id");
            var accountId = Console.ReadLine();
            var result = repository.RemoveAccountAsync(accountId ?? string.Empty).Result;
            if (result)
            {
                Console.WriteLine("Account deleted successfully");
            }
            else
            {
                Console.WriteLine("Failed to delete account");
            }
        }

        static void DepositMoney(IAccountsRepository repository)
        {
            Console.WriteLine("Please enter account id");
            var accountId = Console.ReadLine();
            Console.WriteLine("Please enter amount to deposit");
            var amountStr = Console.ReadLine();
            if (!decimal.TryParse(amountStr, out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount value");
                return;
            }
            var result = repository.DepositMoneyAsync(accountId ?? string.Empty, amount).Result;
            if (result)
            {
                Console.WriteLine("Deposit successful");
            }
            else
            {
                Console.WriteLine("Failed to deposit money");
            }
        }

        static void WithdrawMoney(IAccountsRepository repository)
        {
            Console.WriteLine("Please enter account id");
            var accountId = Console.ReadLine();
            Console.WriteLine("Please enter amount to withdraw");
            var amountStr = Console.ReadLine();
            if (!decimal.TryParse(amountStr, out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount value");
                return;
            }
            var result = repository.WithdrawMoneyAsync(accountId ?? string.Empty, amount).Result;
            if (result)
            {
                Console.WriteLine("Withdraw successful");
            }
            else
            {
                Console.WriteLine("Failed to withdraw money");
            }
        }

        static void TransferMoney(IAccountsRepository repository)
        {
            Console.WriteLine("Please enter source account id");
            var sourceAccountId = Console.ReadLine();
            Console.WriteLine("Please enter destination account id");
            var destinationAccountId = Console.ReadLine();
            Console.WriteLine("Please enter amount to transfer");
            var amountStr = Console.ReadLine();
            if (!decimal.TryParse(amountStr, out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount value");
                return;
            }
            var withdrawResult = repository.WithdrawMoneyAsync(sourceAccountId ?? string.Empty, amount).Result;
            if (!withdrawResult)
            {
                Console.WriteLine("Failed to withdraw money from source account");
                return;
            }
            var depositResult = repository.DepositMoneyAsync(destinationAccountId ?? string.Empty, amount).Result;
            if (!depositResult)
            {
                // In a real-world scenario, you might want to handle this case more gracefully,
                // such as retrying the withdrawal or logging the failure for manual intervention.
                Console.WriteLine("Failed to deposit money into destination account. Please contact support.");
                return;
            }
            Console.WriteLine("Transfer successful");
        }
    }
}
