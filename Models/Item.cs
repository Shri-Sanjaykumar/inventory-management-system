using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternInventory.Models
{
    [Table("Items", Schema = "dbo")]
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Item Name is required.")]
        [StringLength(150, ErrorMessage = "Item Name cannot exceed 150 characters.")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit of Measure is required.")]
        [StringLength(50, ErrorMessage = "Unit of Measure cannot exceed 50 characters.")]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opening Balance is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Opening Balance must be a non-negative number.")]
        public int OpeningBalance { get; set; }

        [StringLength(1000, ErrorMessage = "Detailed Description cannot exceed 1000 characters.")]
        public string? DetailedDescription { get; set; }
    }
}
