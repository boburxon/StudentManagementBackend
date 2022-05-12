using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementBackend.Entities
{

    public class Student
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? ProfilePhoto { get; set; }

        public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; }

    }

}
