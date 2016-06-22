﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Forum.Entity;

namespace Wodsoft.ComBoost.Forum
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }
        
        public DataContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=(local);database=ComBoostForum;uid=sa;pwd=123@abc");
            }
        }

        public DbSet<Member> Member { get; set; }
    }
}
