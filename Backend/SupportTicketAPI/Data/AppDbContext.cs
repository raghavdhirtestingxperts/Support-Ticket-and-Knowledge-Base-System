using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();
    public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles => Set<KnowledgeBaseArticle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //user
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        //ticket
        modelBuilder.Entity<Ticket>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Status).HasConversion<string>();
            e.Property(t => t.Priority).HasConversion<string>();

            //createdby
            e.HasOne(t => t.CreatedBy)
             .WithMany(u => u.CreatedTickets)
             .HasForeignKey(t => t.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            //assignedto
            e.HasOne(t => t.AssignedTo)
             .WithMany(u => u.AssignedTickets)
             .HasForeignKey(t => t.AssignedToUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        //comment
        modelBuilder.Entity<TicketComment>(e =>
        {
            e.HasKey(c => c.Id);

            e.HasOne(c => c.Ticket)
             .WithMany(t => t.Comments)
             .HasForeignKey(c => c.TicketId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.User)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TicketHistory>(e =>
        {
            e.HasKey(h => h.Id);
            e.Property(h => h.OldStatus).HasConversion<string>();
            e.Property(h => h.NewStatus).HasConversion<string>();

            e.HasOne(h => h.Ticket)
             .WithMany(t => t.History)
             .HasForeignKey(h => h.TicketId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(h => h.ChangedBy)
             .WithMany()
             .HasForeignKey(h => h.ChangedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        //kb
        modelBuilder.Entity<KnowledgeBaseArticle>(e =>
        {
            e.HasKey(a => a.Id);

            e.HasOne(a => a.CreatedBy)
             .WithMany()
             .HasForeignKey(a => a.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        //dates
        var now = new DateTime(2026, 6, 12, 0, 0, 0, DateTimeKind.Utc);

        //users
        //passwordhash
        const string passwordHash = "$2a$11$EjA4/XEdoTEun6S.mVWcCesE7GlBuztGkkPZiLiRWmWKoesKFETtK";

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Alice Smith",
                Email = "alice@demo.com",
                PasswordHash = passwordHash,
                Role = UserRole.Customer,
                TenantId = "default",
                CreatedAt = now
            },
            new User
            {
                Id = 2,
                Name = "Bob Jones",
                Email = "bob@demo.com",
                PasswordHash = passwordHash,
                Role = UserRole.Agent,
                TenantId = "default",
                CreatedAt = now
            },
            new User
            {
                Id = 3,
                Name = "Carol Davis",
                Email = "carol@demo.com",
                PasswordHash = passwordHash,
                Role = UserRole.Supervisor,
                TenantId = "default",
                CreatedAt = now
            }
        );

        //tickets
        modelBuilder.Entity<Ticket>().HasData(
            new Ticket
            {
                Id = 1,
                Title = "Login fails with 401 error",
                Description = "Tried logging in multiple times but it just says 401 unauthorized. I'm sure the password is correct.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.High,
                CreatedAt = now,
                SlaDeadline = now.AddHours(8),
                IsSlaBreached = false,
                CreatedByUserId = 1,
                AssignedToUserId = null
            },
            new Ticket
            {
                Id = 2,
                Title = "Charged twice for subscription",
                Description = "Checked my card statement and I was billed twice this month. Only one order confirmation received.",
                Status = TicketStatus.InProgress,
                Priority = TicketPriority.Critical,
                CreatedAt = now.AddHours(-3),
                SlaDeadline = now.AddHours(-1),
                IsSlaBreached = true,
                CreatedByUserId = 1,
                AssignedToUserId = 2
            },
            new Ticket
            {
                Id = 3,
                Title = "Forgot password reset link not arriving",
                Description = "Requested a password reset but the email never arrived in my inbox or spam.",
                Status = TicketStatus.Resolved,
                Priority = TicketPriority.Low,
                CreatedAt = now.AddDays(-2),
                SlaDeadline = now.AddDays(-2).AddHours(72),
                IsSlaBreached = false,
                CreatedByUserId = 1,
                AssignedToUserId = 2
            },
            new Ticket
            {
                Id = 4,
                Title = "Dark mode support?",
                Description = "Is there any way to turn on dark mode? The white screen is a bit too bright for night use.",
                Status = TicketStatus.PendingCustomer,
                Priority = TicketPriority.Medium,
                CreatedAt = now.AddDays(-1),
                SlaDeadline = now.AddDays(-1).AddHours(24),
                IsSlaBreached = false,
                CreatedByUserId = 1,
                AssignedToUserId = 2
            },
            new Ticket
            {
                Id = 5,
                Title = "CSV export button not working",
                Description = "Clicking the export button does nothing. Tried on Chrome and Safari, no file downloads.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.Medium,
                CreatedAt = now,
                SlaDeadline = now.AddHours(24),
                IsSlaBreached = false,
                CreatedByUserId = 1,
                AssignedToUserId = null
            }
        );

        //historydata
        modelBuilder.Entity<TicketHistory>().HasData(
            new TicketHistory
            {
                Id = 1,
                TicketId = 2,
                OldStatus = TicketStatus.Open,
                NewStatus = TicketStatus.InProgress,
                Note = "Assigned to Bob. Checking payment gateway logs.",
                ChangedAt = now.AddHours(-2),
                ChangedByUserId = 2
            },
            new TicketHistory
            {
                Id = 2,
                TicketId = 3,
                OldStatus = TicketStatus.Open,
                NewStatus = TicketStatus.InProgress,
                Note = "Investigating email logs.",
                ChangedAt = now.AddDays(-2).AddHours(1),
                ChangedByUserId = 2
            },
            new TicketHistory
            {
                Id = 3,
                TicketId = 3,
                OldStatus = TicketStatus.InProgress,
                NewStatus = TicketStatus.Resolved,
                Note = "Manually triggered reset link.",
                ChangedAt = now.AddDays(-1),
                ChangedByUserId = 2
            }
        );

        //kbdata
        modelBuilder.Entity<KnowledgeBaseArticle>().HasData(
            new KnowledgeBaseArticle
            {
                Id = 1,
                Title = "Resetting your password",
                Body = "Click 'Forgot Password' on the login page, type in your email address, and click the link in the reset email. The link is valid for 24 hours.",
                Tags = "password,reset",
                CreatedAt = now.AddDays(-5),
                UpdatedAt = null,
                CreatedByUserId = 2
            },
            new KnowledgeBaseArticle
            {
                Id = 2,
                Title = "SLA response times",
                Body = "Our standard response times are: Critical - 2 hours, High - 8 hours, Medium - 24 hours, Low - 72 hours.",
                Tags = "sla,priority",
                CreatedAt = now.AddDays(-3),
                UpdatedAt = null,
                CreatedByUserId = 2
            }
        );
    }
}
