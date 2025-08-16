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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        // DbSets for Chat and Notification models
        public DbSet<User> Users { get; set; }
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

            // Category self-referencing relationship
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // MachineManager configurations
            modelBuilder.Entity<MachineManager>()
                .HasIndex(m => m.MachineName)
                .IsUnique();

            // Customer relationships
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.BillingAddress)
                .WithMany(a => a.BillingOrders)
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(a => a.ShippingOrders)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                    .HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

            // PurchaseOrder relationships
            modelBuilder.Entity<PurchaseOrder>()
                    .HasOne(po => po.Supplier)
                    .WithMany(s => s.PurchaseOrders)
                    .HasForeignKey(po => po.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderItem>()
                    .HasOne(poi => poi.PurchaseOrder)
                    .WithMany(po => po.PurchaseOrderItems)
                    .HasForeignKey(poi => poi.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

            // Configure unique indexes
            modelBuilder.Entity<Customer>()
                    .HasIndex(c => c.Email)
                    .IsUnique();

            modelBuilder.Entity<Order>()
                    .HasIndex(o => o.OrderNumber)
                    .IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
                    .HasIndex(po => po.PurchaseOrderNumber)
                    .IsUnique();

            // User configurations
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

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
                .WithMany(u => u.SentMessages)
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
                .WithMany(u => u.Connections)
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
