using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Database : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<UserRoleRelation> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Board>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Board)
                .HasForeignKey(e => e.BoardId)
                .IsRequired();
        }

        public Database(DbContextOptions<Database> options) : base(options)
        {
        }
    }
}
