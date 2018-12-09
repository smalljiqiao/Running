using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using EFDataCoreDomain.Entities;

namespace EFDataCoreDomain.Contexts
{
    public class QGDDbContexts:DbContext
    {
        public DbSet<Goods_Color_Img_Entity> goods_Color_Img_Entity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=119.23.228.171; Database=HaiBuo2.0_ Production; User ID=hb; Password=hb@2017;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goods_Color_Img_Entity>(option=> {
                option.ToTable("tb_Goods_Color_Img").HasKey(p=>p.Id);
            });
        }

        

    }

}
