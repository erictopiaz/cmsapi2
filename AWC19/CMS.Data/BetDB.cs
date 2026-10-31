using CMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CMS.Data
{
    public interface IBetDB { };
    public class BetDB : DbContext, IMainDB
    {
        public static string CONNECTIONSTRING;
        //public BetDB(DbContextOptions<BetDB> contextOptions) : base(contextOptions)
        //{
        //}
        //public DbSet<LB> LBs { get; set; }
    }
}
