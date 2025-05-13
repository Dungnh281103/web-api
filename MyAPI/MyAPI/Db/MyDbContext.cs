using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
namespace MyAPI.Db
{
    public class MyDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        private readonly IServiceProvider _serviceProvider;

        public MyDbContext(DbContextOptions<MyDbContext> options, IServiceProvider serviceProvider = null)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        public DbSet<Story> Stories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<SavedStory> SavedStories { get; set; }
        public DbSet<ReadingHistory> ReadingHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.Story)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.StoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .IsRequired(false)              // ← make the FK optional
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Rating>()
                .HasOne(r => r.Story)
                .WithMany(s => s.Ratings)
                .HasForeignKey(r => r.StoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Chapter>()
                .HasOne(c => c.Story)
                .WithMany(s => s.Chapters)
                .HasForeignKey(c => c.StoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Report>()
                .HasOne(r => r.Story)
                .WithMany()
                .HasForeignKey(r => r.StoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Report>()
                .HasOne(r => r.Chapter)
                .WithMany()
                .HasForeignKey(r => r.ChapterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Story>()
                .HasMany(s => s.Categories)
                .WithMany(c => c.Stories)
                .UsingEntity(j => j.ToTable("StoryCategories"));

            builder.Entity<Story>()
                .HasMany(s => s.Tags)
                .WithMany(t => t.Stories)
                .UsingEntity(j => j.ToTable("StoryTags"));

            builder.Entity<ReadingHistory>(eb =>
            {
                // Composite PK (user+story) hoặc bạn có thể dùng PK auto-increment
                eb.HasKey(r => new { r.UserId, r.StoryId });

                // Unique index (không bắt buộc nếu đã là PK)
                eb.HasIndex(r => new { r.UserId, r.StoryId })
                  .IsUnique();

                // FK → AppUser
                eb.HasOne(r => r.User)
                  .WithMany(u => u.ReadingHistories)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

                // FK → Story
                eb.HasOne(r => r.Story)
                  .WithMany(s => s.ReadingHistories)
                  .HasForeignKey(r => r.StoryId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

        }

        // Thêm method này để gọi seeder sau khi database được tạo
        public void EnsureSeedData()
        {
            if (_serviceProvider != null)
            {
                // Gọi hàm SeedData từ lớp DataSeeder
                DataSeeder.SeedData(_serviceProvider).Wait();
            }
        }
    }
}