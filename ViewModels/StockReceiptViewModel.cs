using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InternInventory.ViewModels
{
    public class StockReceiptViewModel
    {
        public int StockReceiptID { get; set; }

        [Required(ErrorMessage = "Please select a Vendor.")]
        [Display(Name = "Vendor")]
        public int VendorID { get; set; }

        [Required(ErrorMessage = "Please select a Project.")]
        [Display(Name = "Project")]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Please select an Item.")]
        [Display(Name = "Item")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Receipt Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Receipt Date")]
        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        // Dropdown Lists
        public IEnumerable<SelectListItem>? VendorsList { get; set; }
        public IEnumerable<SelectListItem>? ProjectsList { get; set; }
        public IEnumerable<SelectListItem>? ItemsList { get; set; }
    }
}
