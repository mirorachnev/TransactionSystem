namespace TransactionSystem.Api.Repositories.Models
{
    /// <summary>
    /// Represents account information, including the account identifier, name, and balance.
    /// </summary>
    /// <remarks>This class is used to store and manage data related to an account.  All properties are
    /// required and must be set before using an instance of this class.</remarks>
    public class AccountData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the account.
        /// </summary>
        public required string AccountId { get; set; } 
        
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the current balance of the account.
        /// </summary>
        public required decimal Balance { get; set; }
    }
}
