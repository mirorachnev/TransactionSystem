using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransactionSystem.DataAccess.Repositories;

namespace TransactionSystem.ConsoleApp
{
    internal class Program
    {
        public async static Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.Sources.Clear();
            IHostEnvironment env = builder.Environment;

            builder.Services.AddSingleton<IAccountsRepository, AccountsRepository>();

            using var serviceProvider = builder.Services.BuildServiceProvider();

            var repositoryService = serviceProvider.GetRequiredService<IAccountsRepository>();

            var accounts = await repositoryService.GetAllAccountsAsync();

            Console.ReadKey();

        }
    }
}
