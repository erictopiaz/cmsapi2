using Ching.DistributedTransaction.EFModel.TableModel;
using Ching.DistributedTransaction.EFModel.TableModel.EVO;
using Ching.DistributedTransaction.EFModel.TableModel.JDB;
using Ching.DistributedTransaction.EFModel.TableModel.JL;
using Ching.DistributedTransaction.EFModel.TableModel.Spribe;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ching.DistributedTransaction.EFModel {
    /// <summary>
    /// 游戏公共注单
    /// </summary>
    public partial class GameBetOrderDbContext : DbContext {
        public GameBetOrderDbContext(DbContextOptions<GameBetOrderDbContext> options) : base(options) {
            Console.WriteLine($"This is {nameof(GameBetOrderDbContext)}  DbContextOptions");
        }
        //
        public virtual DbSet<PG_BetOrderLog_wd> PG_BetOrderLog_wd { get; set; }
        public virtual DbSet<JL_OrderLog_withdraw> JL_OrderLog_withdraw { get; set; }
        public virtual DbSet<JL_OrderLog_settle> JL_OrderLog_settle { get; set; }
        public virtual DbSet<JL_OrderLog_Cancel> JL_OrderLog_Cancel { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) {



            #region pg
            builder.Entity<PG_BetOrderLog_wd>();

            #endregion

            #region spribe

            builder.Entity<Spribe_BetOrderLog_withdraw>();
            builder.Entity<Spribe_BetOrderLog_deposit>();
            builder.Entity<Spribe_BetOrderLog_rollback>();

            #endregion

            #region jdb
            builder.Entity<JDB_BetOrderLog_rollback>();
            builder.Entity<JDB_BetOrderLog_wd>();
            #endregion

            #region EVO
            builder.Entity<EVO_BetOrderLog_withdraw>();
            builder.Entity<EVO_BetOrderLog_deposit>();
            builder.Entity<EVO_BetOrderLog_rollback>();
            #endregion

            #region JL
            builder.Entity<JL_OrderLog_withdraw>().Ignore(x => x.freeSpinData).Ignore(x => x.token).Ignore(x => x.isFreeRound);
            builder.Entity<JL_OrderLog_settle>();
            builder.Entity<JL_OrderLog_Cancel>();
            #endregion

            var decimalProps = builder.Model
                        .GetEntityTypes()
                        .SelectMany(t => t.GetProperties())
                        .Where(p => (System.Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

            foreach (var property in decimalProps) {
                property.SetPrecision(18);
                property.SetScale(4);
            }

        }

        ///// <summary>
        ///// 游戏注单表
        ///// </summary>
        //public virtual DbSet<BetOrderModel> BetOrderModels { get; set; }


        ///// <summary>
        ///// 游戏注单表
        ///// </summary>
        //public virtual DbSet<TurboWithdrawBetOrderModel> TurboWithdrawBetOrderModels { get; set; }
        ///// <summary>
        ///// 游戏注单表
        ///// </summary>
        //public virtual DbSet<TurboDepositBetOrderModel> TurboDepositBetOrderModels { get; set; }
        ///// <summary>
        ///// 游戏注单表
        ///// </summary>
        //public virtual DbSet<TurboRollbackBetOrderModel> TurboRollbackBetOrderModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            
        }
    }
}
