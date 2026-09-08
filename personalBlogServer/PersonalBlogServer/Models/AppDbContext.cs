using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PersonalBlogServer.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserEducation> UserEducations { get; set; }

    public virtual DbSet<UserExperience> UserExperiences { get; set; }

    public virtual DbSet<UserSkill> UserSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=PersonalBlogDB;Username=postgres;Password=Quoc@123");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "categories_name_key").IsUnique();

            entity.HasIndex(e => e.Slug, "categories_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(120)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comments_pkey");

            entity.ToTable("comments");

            entity.HasIndex(e => e.PostId, "idx_comments_post_id");

            entity.HasIndex(e => e.UserId, "idx_comments_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.GuestEmail)
                .HasMaxLength(255)
                .HasColumnName("guest_email");
            entity.Property(e => e.GuestName)
                .HasMaxLength(100)
                .HasColumnName("guest_name");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("fk_comments_post");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_comments_user");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("posts_pkey");

            entity.ToTable("posts");

            entity.HasIndex(e => e.AuthorId, "idx_posts_author_id");

            entity.HasIndex(e => e.CategoryId, "idx_posts_category_id");

            entity.HasIndex(e => e.PublishedAt, "idx_posts_published_at");

            entity.HasIndex(e => e.Status, "idx_posts_status");

            entity.HasIndex(e => e.Slug, "posts_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Excerpt).HasColumnName("excerpt");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("published_at");
            entity.Property(e => e.Slug)
                .HasMaxLength(300)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.ThumbnailUrl).HasColumnName("thumbnail_url");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.ViewCount)
                .HasDefaultValue(0)
                .HasColumnName("view_count");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_posts_author");

            entity.HasOne(d => d.Category).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_posts_category");

            entity.HasMany(d => d.Tags).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("fk_post_tags_tag"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .HasConstraintName("fk_post_tags_post"),
                    j =>
                    {
                        j.HasKey("PostId", "TagId").HasName("post_tags_pkey");
                        j.ToTable("post_tags");
                        j.HasIndex(new[] { "TagId" }, "idx_post_tags_tag_id");
                        j.IndexerProperty<int>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.HasIndex(e => e.Name, "tags_name_key").IsUnique();

            entity.HasIndex(e => e.Slug, "tags_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(120)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AboutStory)
                .HasDefaultValueSql("'Mình bắt đầu hành trình lập trình từ niềm say mê sáng tạo các giao diện web trực quan và logic backend mạnh mẽ. Với kinh nghiệm làm việc cùng hệ sinh thái Vue.js và ASP.NET Core, mình luôn hướng đến việc viết mã nguồn sạch (clean code), tối ưu hóa trải nghiệm người dùng và xây dựng kiến trúc hệ thống bền vững, dễ mở rộng.'::text")
                .HasColumnName("about_story");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.Bio)
                .HasDefaultValueSql("'Lập trình viên đam mê xây dựng các sản phẩm web hiện đại, hiệu năng cao và trải nghiệm người dùng mượt mà.'::text")
                .HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CvUrl)
                .HasDefaultValueSql("'https://example.com/cv-phanduongquoc.pdf'::text")
                .HasColumnName("cv_url");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(150)
                .HasColumnName("display_name");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FacebookUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'https://facebook.com'::character varying")
                .HasColumnName("facebook_url");
            entity.Property(e => e.GithubUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'https://github.com/phanduongquoc'::character varying")
                .HasColumnName("github_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(150)
                .HasDefaultValueSql("'Fullstack Web Developer'::character varying")
                .HasColumnName("job_title");
            entity.Property(e => e.LinkedinUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'https://linkedin.com/in/phanduongquoc'::character varying")
                .HasColumnName("linkedin_url");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .HasDefaultValueSql("'Hồ Chí Minh, Việt Nam'::character varying")
                .HasColumnName("location");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasDefaultValueSql("'0987 654 321'::character varying")
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Author'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.TwitterUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'https://twitter.com'::character varying")
                .HasColumnName("twitter_url");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'https://pdq-personal-blog.vercel.app'::character varying")
                .HasColumnName("website_url");
            entity.Property(e => e.YearsOfExperience)
                .HasDefaultValue(3)
                .HasColumnName("years_of_experience");
        });

        modelBuilder.Entity<UserEducation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_educations_pkey");

            entity.ToTable("user_educations");

            entity.HasIndex(e => e.UserId, "idx_user_educations_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Degree)
                .HasMaxLength(200)
                .HasColumnName("degree");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.EndYear)
                .HasMaxLength(20)
                .HasColumnName("end_year");
            entity.Property(e => e.Institution)
                .HasMaxLength(200)
                .HasColumnName("institution");
            entity.Property(e => e.StartYear)
                .HasMaxLength(20)
                .HasColumnName("start_year");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserEducations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_educations_user");
        });

        modelBuilder.Entity<UserExperience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_experiences_pkey");

            entity.ToTable("user_experiences");

            entity.HasIndex(e => e.UserId, "idx_user_experiences_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Company)
                .HasMaxLength(150)
                .HasColumnName("company");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.EndDate)
                .HasMaxLength(50)
                .HasColumnName("end_date");
            entity.Property(e => e.IsCurrent)
                .HasDefaultValue(false)
                .HasColumnName("is_current");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .HasColumnName("location");
            entity.Property(e => e.Role)
                .HasMaxLength(150)
                .HasColumnName("role");
            entity.Property(e => e.StartDate)
                .HasMaxLength(50)
                .HasColumnName("start_date");
            entity.Property(e => e.Technologies)
                .HasMaxLength(300)
                .HasColumnName("technologies");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserExperiences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_experiences_user");
        });

        modelBuilder.Entity<UserSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_skills_pkey");

            entity.ToTable("user_skills");

            entity.HasIndex(e => e.Category, "idx_user_skills_category");

            entity.HasIndex(e => e.UserId, "idx_user_skills_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .HasColumnName("icon");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Proficiency)
                .HasDefaultValue(80)
                .HasColumnName("proficiency");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserSkills)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_skills_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
