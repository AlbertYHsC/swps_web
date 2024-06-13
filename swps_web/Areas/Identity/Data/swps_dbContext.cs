using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using swps_web.Areas.Identity.Data;
using swps_web.Models;

namespace swps_web.Data;

public class swps_dbContext : IdentityUserContext<swps_webUser>
{
    public swps_dbContext(DbContextOptions<swps_dbContext> options)
        : base(options)
    {
    }

    public DbSet<swps_web.Models.Device> Device { get; set; }
    public DbSet<swps_web.Models.Record> Record { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<swps_webUser>(b =>
        {
            var indexEmail = b.HasIndex(e => e.NormalizedEmail).Metadata;
            b.Metadata.RemoveIndex(indexEmail.Properties);
            b.Property(e => e.Email).HasColumnName("DeviceSN");
            b.Property(e => e.NormalizedEmail).HasColumnName("NormalizedDeviceSN");
            b.HasIndex(e => e.NormalizedEmail).HasDatabaseName("NormalizedDeviceSNIndex").IsUnique();

            b.Property(e => e.PhoneNumber).HasColumnName("RecoveryCode");

            b.HasMany<Device>().WithOne().HasForeignKey(d => d.UserId).IsRequired();
            b.HasMany<Record>().WithOne().HasForeignKey(r => r.UserId).IsRequired();
        });

        builder.Entity<swps_webUser>()
            .Ignore(e => e.EmailConfirmed)
            .Ignore(e => e.PhoneNumberConfirmed)
            .Ignore(e => e.TwoFactorEnabled);

        builder.Entity<Device>(b =>
        {
            b.HasKey(d => d.Id);
            b.HasIndex(e => e.DeviceSN).HasDatabaseName("DeviceSNIndex").IsUnique();

            b.HasMany<Record>().WithOne().HasForeignKey(r => r.DeviceId).IsRequired();

            b.ToTable("EdgeDevices");
        });
        
        builder.Entity<Record>(b =>
        {
            b.HasKey(r => r.Id);

            b.HasIndex(e => e.DetectTime).HasDatabaseName("DetectTimeIndex");

            b.ToTable("SensorRecords");
        });
    }
}
