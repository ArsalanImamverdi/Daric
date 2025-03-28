using Daric.Database;
using Daric.Database.SqlServer;
using Daric.Domain.Accounts;
using Daric.Domain.Bonuses;
using Daric.Domain.Customers;
using Daric.Domain.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Daric.Infrastructure.SqlServer
{
    internal class DaricDbContext(ISqlServerDataConfig config, IServiceProvider scopedServiceProvider, ILogger<DaricDbContext> logger)
            : SqlServerDbContext<DaricDbContext>(config, scopedServiceProvider, logger)
    {

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Bonus> Bonuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<long>("Account.AccountNumber", a =>
            {
                a.StartsAt(1_000_000_001);
                a.IncrementsBy(1);
            });
            modelBuilder.HasSequence<long>("Account.TrackingCode", a =>
            {
                a.StartsAt(1_000);
                a.IncrementsBy(1);
            });

            modelBuilder.Entity<Customer>().Property(p => p.FirstName).HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(p => p.LastName).HasMaxLength(500);

            modelBuilder.Entity<Customer>().OwnsMany(c => c.Addresses, a =>
            {
                a.Property(p => p.Street).HasColumnName(nameof(Address.Street)).HasMaxLength(100);
                a.Property(p => p.Country).HasColumnName(nameof(Address.Country)).HasMaxLength(100);
                a.Property(p => p.City).HasColumnName(nameof(Address.City)).HasMaxLength(100);
                a.Property(p => p.PostalCode).HasColumnName(nameof(Address.PostalCode)).HasMaxLength(20);
            });
            modelBuilder.Entity<Customer>().OwnsMany(c => c.Phones, p =>
            {
                p.Property(p => p.CountryCode).HasColumnName(nameof(Phone.CountryCode)).HasMaxLength(5);
                p.Property(p => p.PhoneNumber).HasColumnName(nameof(Phone.PhoneNumber)).HasMaxLength(10);
                p.Property(p => p.PhoneType).HasColumnName(nameof(Phone.PhoneType)).HasMaxLength(10);
            });

            modelBuilder.Entity<Transaction>().Property(p => p.TransactionType).HasConversion<string>().HasMaxLength(10);
            modelBuilder.Entity<Transaction>().HasIndex(e => e.CreatedAt);
            modelBuilder.Entity<Transaction>().Property(p => p.PerformBy).HasConversion<string>().HasMaxLength(10);
            modelBuilder.Entity<Transaction>().Property(p => p.Status).HasConversion<string>().HasMaxLength(10);
            modelBuilder.Entity<Transaction>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>().Property(p => p.Description).HasMaxLength(1000);


            modelBuilder.Entity<Account>().Property(p => p.AccountNumber).HasMaxLength(11);
            modelBuilder.Entity<Account>().Property(p => p.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<Account>().HasIndex(e => e.AccountNumber).IsUnique();

            modelBuilder.Entity<Bonus>().HasIndex(e => e.Type);
            modelBuilder.Entity<Bonus>().Property(p => p.Type).HasConversion<string>().HasMaxLength(50);
            modelBuilder.Entity<Bonus>().Property(p => p.Amount).HasPrecision(18, 2);
        }
    }
}
