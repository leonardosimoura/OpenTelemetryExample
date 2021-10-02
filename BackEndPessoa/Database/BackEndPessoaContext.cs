using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndPessoa.Database
{
   
    public class BackEndPessoaContext : DbContext
    {
        public DbSet<Pessoa> Pessoa { get; set; }

        public BackEndPessoaContext(DbContextOptions<BackEndPessoaContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>().HasKey(p => p.PessoaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
