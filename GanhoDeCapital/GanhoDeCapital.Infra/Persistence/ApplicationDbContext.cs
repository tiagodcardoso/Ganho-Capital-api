using GanhoDeCapital.Core.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace GanhoDeCapital.Infra.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Operation> Operations { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.HasKey(e => e.OperationId);
                entity.Property(e => e.ClientId);
                entity.Property(e => e.ClientName).HasMaxLength(200);
                entity.Property(e => e.ClientCpf).HasMaxLength(14);
                entity.Property(e => e.Tax).HasPrecision(15, 2);
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.OwnsMany(e => e.Transactions, transaction =>
                {
                    transaction.Property(t => t.Operation).HasMaxLength(10).IsRequired();
                    transaction.Property(t => t.UnitCost).HasPrecision(15, 2).IsRequired();
                    transaction.Property(t => t.Quantity).IsRequired();
                });

                entity.HasIndex(e => e.ClientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId);
                entity.Property(e => e.ClientName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.ClientCpf).HasMaxLength(14).IsRequired();

                entity.HasIndex(e => e.ClientCpf);
            });
        }
    }
}
