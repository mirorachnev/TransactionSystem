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

            await repositoryService.AddAccountAsync(new AccountData { AccountId = "1", Name = "test", Balance = 100 });

            var accounts = await repositoryService.GetAllAccountsAsync();

            foreach (var account in accounts) 
            {
                Console.WriteLine($"Account id - {account.AccountId}, Name - {account.Name}, balance - {account.Balance}");
            }

            Console.ReadKey();

        }
    }
}
