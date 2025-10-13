namespace TransactionSystem.Api.Repositories.Models
{
    public class AccountData
    {
        public required string AccountId { get; set; } 
        
        public required string Name { get; set; }

        public required decimal Balance { get; set; }
    }
}
