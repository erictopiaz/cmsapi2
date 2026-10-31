using Ching.DistributedTransaction.EFModel.TableModel;
using Ching.DistributedTransaction.EFModel.TableModel.EVO;
using Ching.DistributedTransaction.EFModel.TableModel.Fb;
using Ching.DistributedTransaction.EFModel.TableModel.JDB;
using Ching.DistributedTransaction.EFModel.TableModel.JL;
using Ching.DistributedTransaction.EFModel.TableModel.Spribe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection.Emit;
using Ching.DistributedTransaction.EFModel.TableModel.IM;
using Ching.DistributedTransaction.EFModel.TableModel.TF;
using Ching.DistributedTransaction.EFModel.TableModel.PA;

namespace Ching.DistributedTransaction.EFModel {
    /// <summary>
    /// 记录游戏平台注单请求，防多次处理，进行同步响应
    /// </summary>
    public partial class CommonServiceDbContext : DbContext {
        public CommonServiceDbContext(DbContextOptions<CommonServiceDbContext> options) : base(options) {
            Console.WriteLine($"This is {nameof(CommonServiceDbContext)}  DbContextOptions");
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<GameTranferUnionLog> gameTranferUnionLogs { get; set; }
       
        public virtual DbSet<GameTranferUnionLogWait> gameTranferUnionLogWaits { get; set; }
        public virtual DbSet<order_im_settlement> order_im_settlement { get; set; }
        public virtual DbSet<order_im> order_im { get; set; }
        public virtual DbSet<order_tf> order_tf { get; set; }
        public virtual DbSet<order_pa> order_pa { get; set; } //
        public virtual DbSet<order_pa_win> order_pa_win { get; set; }
        public virtual DbSet<order_pa_lose> order_pa_lose { get; set; }
        public virtual DbSet<order_pa_tie> order_pa_tie { get; set; }
        /// <summary>
        /// 会员单一钱包钱包
        /// </summary>
        public virtual DbSet<UserGameWallet> UserGameWallets { get; set; }
        public virtual DbSet<order_pg> order_pg { get; set; }
        public virtual DbSet<fb_orders> fb_orders { get; set; }
        public virtual DbSet<order_jdb> order_jdb { get; set; }
        public virtual DbSet<order_jdb_cancel> order_jdb_cancel { get; set; }
        public virtual DbSet<order_jl_poker> order_jl_poker { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            
        }
        
    }



}
