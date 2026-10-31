using Ching.DistributedTransaction.EFModel.TableModel;
using Ching.DistributedTransaction.EFModel.TableModel.Fb;
using Ching.DistributedTransaction.EFModel.TableModel.JL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel
{
    /// <summary>
    /// 记录游戏平台注单请求，防多次处理，进行同步响应
    /// 配置给mssql
    /// </summary>
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Console.WriteLine($"This is {nameof(ApplicationDbContext)}  DbContextOptions");
        }

        public virtual DbSet<GameTranferUnionLog> gameTranferUnionLogs { get; set; }

        public virtual DbSet<GameTranferUnionLogWait> gameTranferUnionLogWaits { get; set; }
        /// <summary>
        /// 会员单一钱包钱包
        /// </summary>
        public virtual DbSet<UserGameWallet> UserGameWallets { get; set; }
        public virtual DbSet<ofUser> ofUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

    }

}
