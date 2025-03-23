using Daric.Database;
using Daric.Database.SqlServer;
using Daric.Domain.Customers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Daric.Infrastructure.SqlServer
{
    internal class DaricDbContext(ISqlServerDataConfig config, IServiceProvider scopedServiceProvider, ILogger<DaricDbContext> logger)
            : SqlServerDbContext<DaricDbContext>(config, scopedServiceProvider, logger)
    {

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

        }
    }
}
