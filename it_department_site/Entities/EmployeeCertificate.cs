using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("employee_certificates")]
    public class EmployeeCertificate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Required]
        [Column("photo_url")]
        public string PhotoUrl { get; set; } = string.Empty;

        [MaxLength(128)]
        [Column("drive_file_id")]
        public string? DriveFileId { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [MaxLength(128)]
        [Column("mime_type")]
        public string? MimeType { get; set; }

        [MaxLength(260)]
        [Column("file_name")]
        public string? FileName { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
