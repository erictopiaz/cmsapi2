using Ching.DistributedTransaction.EFModel.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel
{
    /// <summary>
    /// 代码里会传入指定链接
    /// </summary>
    public partial class AmountRecordDbContext : DbContext
    {
        private string _sqlString = "";
        /// <summary>
        /// 透过注入方式传入链接
        /// </summary>
        /// <param name="options"></param>
        public AmountRecordDbContext(DbContextOptions<AmountRecordDbContext> options) : base(options)
        {
            Console.WriteLine($"This is {nameof(AmountRecordDbContext)}  DbContextOptions");
        }
        /// <summary>
        /// 直接传入链接
        /// </summary>
        /// <param name="postgreSql"></param>
        public AmountRecordDbContext(string postgreSql) 
        {
            _sqlString = postgreSql;
        }


        public virtual DbSet<User> User { get; set; }
        /// <summary>
        /// 会员单一钱包钱包
        /// </summary>
        public virtual DbSet<UserGameWalletPostgre> UserGameWalletPostgres { get; set; }
        //
       
        public virtual DbSet<GameTranferUnionLogPostgre> gameTranferUnionLogPostgre { get; set; }

        public virtual DbSet<GameTranferUnionLogWait> gameTranferUnionLogWaits { get; set; }
        public virtual DbSet<GameWalletTranferPostgre> GameWalletTranferPostgres { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_sqlString))
                optionsBuilder.UseNpgsql(_sqlString);
        }

    }

}
