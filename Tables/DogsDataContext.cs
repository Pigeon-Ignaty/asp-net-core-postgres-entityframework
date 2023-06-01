using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kinological_club.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace kinological_club.Tables
{
    public class DogsDataContext : DbContext
    {
        public DogsDataContext(DbContextOptions<DogsDataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();

            modelBuilder.Entity<ShortenNameViewModel>()
                .HasNoKey()
                .ToView(null); // Указывает, что это представление, а не таблица
            modelBuilder.Entity<SelectionViewModel>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }



        public DbSet<Dogs> Dogs { get; set; }
        public DbSet<Archive> Archive { get; set; }
        public DbSet<Award> Award { get; set; }
        public DbSet<Exhibition> Exhibition { get; set; }
        public DbSet<Owners> Owners { get; set; }
        public DbSet<Auth> Auth { get; set; }
        public DbSet<SelectionViewModel> SelectionViewModel { get; set; }

        public DbSet<ShortenNameViewModel> ShortenName { get; set; }
    }


}
