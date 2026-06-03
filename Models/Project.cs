using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternInventory.Models
{
    [Table("Projects", Schema = "dbo")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Project Name is required.")]
        [StringLength(150, ErrorMessage = "Project Name cannot exceed 150 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [StringLength(250, ErrorMessage = "Address Line 1 cannot exceed 250 characters.")]
        public string AddressLine1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(20, ErrorMessage = "Pincode cannot exceed 20 characters.")]
        public string Pincode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
