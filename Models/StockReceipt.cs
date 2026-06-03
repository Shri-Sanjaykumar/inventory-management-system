using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternInventory.Models
{
    [Table("StockReceipts", Schema = "dbo")]
    public class StockReceipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockReceiptID { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        public int VendorID { get; set; }

        [Required(ErrorMessage = "Project is required.")]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Item is required.")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Receipt Date is required.")]
        [DataType(DataType.Date)]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation Properties for EF Core relations
        [ForeignKey("VendorID")]
        public virtual Vendor? Vendor { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project? Project { get; set; }

        [ForeignKey("ItemID")]
        public virtual Item? Item { get; set; }
    }
}
