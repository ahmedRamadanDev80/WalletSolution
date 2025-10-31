using System;


namespace Wallet.Core.Entities
{
    public sealed class WalletTransaction
    {
        public Guid Id { get; private set; }
        public Guid WalletId { get; private set; }
        public string Type { get; private set; }
        public long Amount { get; private set; }
        public long BalanceAfter { get; private set; }
        public string? Description { get; private set; }
        public string? ExternalReference { get; private set; }
        public DateTime CreatedAt { get; private set; }


        private WalletTransaction() { }


        public WalletTransaction(Guid walletId, string type, long amount, long balanceAfter, string? externalRef, string? description)
        {
            Id = Guid.NewGuid();
            WalletId = walletId;
            Type = type;
            Amount = amount;
            BalanceAfter = balanceAfter;
            ExternalReference = externalRef;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
    }
}