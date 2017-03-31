using Microsoft.EntityFrameworkCore;

namespace Senparc.Weixin.MP.Sample.MySQL.Models
{
    public class SenparcContext : DbContext
    {
        private readonly string _connectionString;
        public SenparcContext(string connection)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString != null)
            {
                optionsBuilder.UseMySql(_connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        public SenparcContext(DbContextOptions opt) : base(opt)
        { }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<CognitiveEmotion> CognitiveEmotions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>(e =>
            {
                e.HasIndex(x => x.UserName);
                e.HasIndex(x => x.Sex);
            });
        }
    }
}
