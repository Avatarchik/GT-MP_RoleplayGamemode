using MySql.Data.Entity;
using Roleplay.Server.Models;
using System.Data.Common;
using System.Data.Entity;

namespace Roleplay
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Database : DbContext
    {
        #region Database Tables
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<OwnedVehicle> OwnedVehicles { get; set; }
        public DbSet<Garage> Garages { get; set; }
        public DbSet<ItemInfo> ItemInformations { get; set; }
        #endregion Database Tables

        public Database()
            : base("MYSQL_CONN")
        {
            this.Database.Initialize(false);
        }

        public Database(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}