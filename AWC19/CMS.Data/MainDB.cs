using CMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CMS.Data
{
    public interface IMainDB { };
    public class Misc { 
        public static string REDIS_CONNECTIONSTRING;
    }
    public class MainDB : DbContext, IMainDB
    {
        public static string CONNECTIONSTRING;
        public static string WITHWRITE_CONNECTIONSTRING;
        public static string DapperConnectionLog;

        //public MainDB(DbContextOptions<MainDB> contextOptions) : base(contextOptions)
        //{
        //}
        //public DbSet<LB> LBs { get; set; }
    }

}
