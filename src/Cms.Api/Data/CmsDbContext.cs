using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Cms.Api.Data
{
    public partial class CmsDbContext : DbContext
    {
        public CmsDbContext()
            : base("name=CmsDbContext")
        {
        }

        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CommandInFunction> CommandInFunctions { get; set; }
        public virtual DbSet<Command> Commands { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<KnowledgeBas> KnowledgeBases { get; set; }
        public virtual DbSet<LabelInKnowledgeBas> LabelInKnowledgeBases { get; set; }
        public virtual DbSet<Label> Labels { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>()
                .Property(e => e.Action)
                .IsUnicode(false);

            modelBuilder.Entity<ActivityLog>()
                .Property(e => e.EntityName)
                .IsUnicode(false);

            modelBuilder.Entity<ActivityLog>()
                .Property(e => e.EntityId)
                .IsUnicode(false);

            modelBuilder.Entity<ActivityLog>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRoleClaim>()
                .Property(e => e.RoleId)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRole>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetRoleClaims)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUserClaim>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetUserLogin>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetUser>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserTokens)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUserToken>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<Attachment>()
                .Property(e => e.FileType)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.SeoAlias)
                .IsUnicode(false);

            modelBuilder.Entity<CommandInFunction>()
                .Property(e => e.CommandId)
                .IsUnicode(false);

            modelBuilder.Entity<CommandInFunction>()
                .Property(e => e.FunctionId)
                .IsUnicode(false);

            modelBuilder.Entity<Command>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Comment>()
                .Property(e => e.OwnerUserId)
                .IsUnicode(false);

            modelBuilder.Entity<Function>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Function>()
                .Property(e => e.ParentId)
                .IsUnicode(false);

            modelBuilder.Entity<Function>()
                .Property(e => e.Icon)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBas>()
                .Property(e => e.SeoAlias)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBas>()
                .Property(e => e.OwnerUserId)
                .IsUnicode(false);

            modelBuilder.Entity<LabelInKnowledgeBas>()
                .Property(e => e.LabelId)
                .IsUnicode(false);

            modelBuilder.Entity<Label>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Permission>()
                .Property(e => e.FunctionId)
                .IsUnicode(false);

            modelBuilder.Entity<Permission>()
                .Property(e => e.RoleId)
                .IsUnicode(false);

            modelBuilder.Entity<Permission>()
                .Property(e => e.CommandId)
                .IsUnicode(false);

            modelBuilder.Entity<Report>()
                .Property(e => e.ReportUserId)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.UserId)
                .IsUnicode(false);
        }
    }
}
