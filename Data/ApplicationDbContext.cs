using Microsoft.EntityFrameworkCore;
using AvyyanBackend.Models;

namespace AvyyanBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for Knitfab business models
        public DbSet<MachineManager> MachineManagers { get; set; }

        // DbSets for Authentication
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<PageAccess> PageAccesses { get; set; }
        public DbSet<RolePageAccess> RolePageAccesses { get; set; }

        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatRoomMember> ChatRoomMembers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints

            // MachineManager configurations
            modelBuilder.Entity<MachineManager>()
                .HasIndex(m => m.MachineName)
                .IsUnique();

            // Authentication configurations
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<PageAccess>()
                .HasIndex(p => p.PageUrl)
                .IsUnique();

            // UserRole relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // RolePageAccess relationships
            modelBuilder.Entity<RolePageAccess>()
                .HasOne(rpa => rpa.Role)
                .WithMany(r => r.RolePageAccesses)
                .HasForeignKey(rpa => rpa.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePageAccess>()
                .HasOne(rpa => rpa.PageAccess)
                .WithMany(pa => pa.RolePageAccesses)
                .HasForeignKey(rpa => rpa.PageAccessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePageAccess>()
                .HasOne(rpa => rpa.GrantedByUser)
                .WithMany()
                .HasForeignKey(rpa => rpa.GrantedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Chat Room configurations
            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.CreatedByUser)
                .WithMany()
                .HasForeignKey(cr => cr.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Chat Room Member configurations
            modelBuilder.Entity<ChatRoomMember>()
                .HasOne(crm => crm.ChatRoom)
                .WithMany(cr => cr.Members)
                .HasForeignKey(crm => crm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatRoomMember>()
                .HasOne(crm => crm.User)
                .WithMany(u => u.ChatRoomMemberships)
                .HasForeignKey(crm => crm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for user-room membership
            modelBuilder.Entity<ChatRoomMember>()
                .HasIndex(crm => new { crm.ChatRoomId, crm.UserId })
                .IsUnique();

            // Chat Message configurations
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ChatRoom)
                .WithMany(cr => cr.Messages)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ReplyToMessage)
                .WithMany(cm => cm.Replies)
                .HasForeignKey(cm => cm.ReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message Reaction configurations
            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.Message)
                .WithMany(cm => cm.Reactions)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.User)
                .WithMany()
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for user-message reaction
            modelBuilder.Entity<MessageReaction>()
                .HasIndex(mr => new { mr.MessageId, mr.UserId, mr.Emoji })
                .IsUnique();

            // Notification configurations
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User Connection configurations
            modelBuilder.Entity<UserConnection>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserConnections)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConnection>()
                .HasIndex(uc => uc.ConnectionId)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This will be overridden by the configuration in Program.cs
                // but provides a fallback for design-time operations
                optionsBuilder.UseNpgsql("Host=localhost;Database=AvyyanKnitfab;Username=postgres;Password=system;Port=5432");
            }
        }
    }
}
