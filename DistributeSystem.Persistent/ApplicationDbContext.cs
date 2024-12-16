using DistributeSystem.Domain.Entities;
using DistributeSystem.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        public DbSet<AppUser> AppUses { get; set; }
        public DbSet<Domain.Entities.Identity.Action> Actions { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<ActionInFunction> ActionInFunctions { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
