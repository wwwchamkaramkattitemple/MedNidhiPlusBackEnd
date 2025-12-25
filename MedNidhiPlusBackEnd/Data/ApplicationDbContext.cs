using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MedNidhiPlusBackEnd.API.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<User> Users { get; set; }


    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<AppointmentType> AppointmentTypes { get; set; }
    public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    public DbSet<SystemSetting> SystemSettings { get; set; }

    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<MedicineCategory> MedicineCategories { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------------
        //  Appointment Relationships
        // -------------------------

        // Patient ? Appointments
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Doctor ? Appointments  (IMPORTANT: Fix WithMany here)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Department (optional)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Department)
            .WithMany()
            .HasForeignKey(a => a.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // -------------------------
        //  Seed Appointment Types
        // -------------------------
        modelBuilder.Entity<AppointmentType>().HasData(
            new AppointmentType { Id = 1, TypeName = "Consultation", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentType { Id = 2, TypeName = "Check-up", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentType { Id = 3, TypeName = "Follow-up", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentType { Id = 4, TypeName = "Procedure", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentType { Id = 5, TypeName = "Emergency", CreatedAt = new DateTime(2024, 01, 01) }
        );

        // -------------------------
        //  Seed Appointment Statuses
        // -------------------------
        modelBuilder.Entity<AppointmentStatus>().HasData(
            new AppointmentStatus { Id = 1, StatusName = "Scheduled", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentStatus { Id = 2, StatusName = "Confirmed", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentStatus { Id = 3, StatusName = "In Progress", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentStatus { Id = 4, StatusName = "Completed", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentStatus { Id = 5, StatusName = "Cancelled", CreatedAt = new DateTime(2024, 01, 01) },
            new AppointmentStatus { Id = 6, StatusName = "No Show", CreatedAt = new DateTime(2024, 01, 01) }
        );

        // -------------------------
        //  Invoice
        // -------------------------
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Patient)
            .WithMany(p => p.Invoices)
            .HasForeignKey(i => i.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InvoiceItem>()
            .HasOne(ii => ii.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SystemSetting>()
        .HasIndex(x => x.Id)
        .IsUnique();
    }

}