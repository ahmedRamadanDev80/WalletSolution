using System;


namespace Wallet.Core.Entities
{
    public sealed class WalletEntity
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public long Balance { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();


        private WalletEntity() { }


        public static WalletEntity Create(Guid userId)
        {
            return new WalletEntity { Id = Guid.NewGuid(), UserId = userId, Balance = 0 };
        }


        public void AddPoints(long amount)
        {
            if (amount <= 0) throw new ArgumentException("amount must be > 0", nameof(amount));
            Balance += amount;
        }


        public void BurnPoints(long amount)
        {
            if (amount <= 0) throw new ArgumentException("amount must be > 0", nameof(amount));
            if (Balance < amount) throw new InvalidOperationException("Insufficient points");
            Balance -= amount;
        }
    }
}