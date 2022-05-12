using Microsoft.EntityFrameworkCore;
using StudentManagementBackend.Entities;

namespace StudentManagementBackend.Utilities;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<AttendanceLog> AttendanceLogs { get; set; }
}