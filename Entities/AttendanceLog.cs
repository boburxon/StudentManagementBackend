using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace StudentManagementBackend.Entities
{

    public enum AttendanceLogType
    {
        Enter,
        Leave
    }

    public class AttendanceLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public AttendanceLogType LogType { get; set; }

        [JsonIgnore]
        public Student Student { get; set; }
        
        public DateTime CreatedDate { get; set; }

    }

}
