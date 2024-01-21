using Common.Models;
using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Database : DbContext
    {
        private readonly IEncryptionProvider _provider;

        public DbSet<Board> Boards { get; set; }
        public DbSet<UserRoleRelation> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(this._provider);

            modelBuilder.Entity<Board>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Board)
                .HasForeignKey(e => e.BoardId)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        public Database(DbContextOptions<Database> options, IConfiguration configuration) : base(options)
        {
            this._provider = new GenerateEncryptionProvider("AAECAwQFBgcICQoLDA0ODw==");

        }
    }
}
