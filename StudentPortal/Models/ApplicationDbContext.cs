using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<AddDropHistory> AddDropHistories { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<StudentEvaluation> StudentEvaluations { get; set; }
        public DbSet<StudentStatement> StudentStatements { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Log> Logs { get; set; }

        public DbSet<Admin> Admins { get; set; } // Added Admin DbSet
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // You can configure additional relationships, indexes, etc. here.
            base.OnModelCreating(modelBuilder);
        }
    }
}
