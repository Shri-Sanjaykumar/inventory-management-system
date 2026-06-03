using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InternInventory.Models;
using InternInventory.Services;
using InternInventory.ViewModels;

namespace InternInventory.Controllers
{
    [Authorize]
    public class StockReceiptController : Controller
    {
        private readonly IStockReceiptService _stockReceiptService;
        private readonly IVendorService _vendorService;
        private readonly IProjectService _projectService;
        private readonly IItemService _itemService;
        private readonly ILogger<StockReceiptController> _logger;

        public StockReceiptController(
            IStockReceiptService stockReceiptService,
            IVendorService vendorService,
            IProjectService projectService,
            IItemService itemService,
            ILogger<StockReceiptController> logger)
        {
            _stockReceiptService = stockReceiptService;
            _vendorService = vendorService;
            _projectService = projectService;
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null)
        {
            try
            {
                IEnumerable<StockReceipt> receipts;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    receipts = await _stockReceiptService.SearchStockReceiptsAsync(search);
                    ViewData["SearchTerm"] = search;
                }
                else
                {
                    receipts = await _stockReceiptService.GetAllStockReceiptsAsync();
                }
                return View(receipts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock receipts index.");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = new StockReceiptViewModel();
                await PopulateDropdownListsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dropdown lists for create stock receipt.");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockReceiptViewModel model)
        {
            // Server-side validation for business rules
            if (model.ReceiptDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("ReceiptDate", "Receipt Date cannot be in the future.");
            }

            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownListsAsync(model);
                return View(model);
            }

            try
            {
                var receipt = new StockReceipt
                {
                    VendorID = model.VendorID,
                    ProjectID = model.ProjectID,
                    ItemID = model.ItemID,
                    ReceiptDate = model.ReceiptDate,
                    Quantity = model.Quantity
                };

                var username = User.Identity?.Name ?? "system";
                await _stockReceiptService.AddStockReceiptAsync(receipt, username);
                
                TempData["SuccessMessage"] = "Stock receipt added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownListsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving stock receipt.");
                ModelState.AddModelError(string.Empty, "An unexpected database error occurred. Please try again.");
                await PopulateDropdownListsAsync(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var receipt = await _stockReceiptService.GetStockReceiptByIdAsync(id);
                if (receipt == null)
                {
                    return NotFound();
                }

                var model = new StockReceiptViewModel
                {
                    StockReceiptID = receipt.StockReceiptID,
                    VendorID = receipt.VendorID,
                    ProjectID = receipt.ProjectID,
                    ItemID = receipt.ItemID,
                    ReceiptDate = receipt.ReceiptDate,
                    Quantity = receipt.Quantity
                };

                await PopulateDropdownListsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock receipt for edit: ID {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockReceiptViewModel model)
        {
            if (id != model.StockReceiptID)
            {
                return BadRequest();
            }

            if (model.ReceiptDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("ReceiptDate", "Receipt Date cannot be in the future.");
            }

            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownListsAsync(model);
                return View(model);
            }

            try
            {
                var receipt = new StockReceipt
                {
                    StockReceiptID = model.StockReceiptID,
                    VendorID = model.VendorID,
                    ProjectID = model.ProjectID,
                    ItemID = model.ItemID,
                    ReceiptDate = model.ReceiptDate,
                    Quantity = model.Quantity
                };

                var updated = await _stockReceiptService.UpdateStockReceiptAsync(receipt);
                if (!updated)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Stock receipt updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownListsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock receipt ID: {Id}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the stock receipt.");
                await PopulateDropdownListsAsync(model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _stockReceiptService.DeleteStockReceiptAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Stock receipt deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock receipt ID: {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the stock receipt.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownListsAsync(StockReceiptViewModel model)
        {
            // Vendor sorting by FirstName
            var vendors = await _vendorService.GetAllVendorsAsync();
            model.VendorsList = vendors.Select(v => new SelectListItem
            {
                Value = v.VendorID.ToString(),
                Text = $"{v.FirstName} {v.LastName}"
            });

            // Project sorting by ProjectName
            var projects = await _projectService.GetAllProjectsAsync();
            model.ProjectsList = projects.Select(p => new SelectListItem
            {
                Value = p.ProjectID.ToString(),
                Text = p.ProjectName
            });

            // Item sorting by ItemName
            var items = await _itemService.GetAllItemsAsync();
            model.ItemsList = items.Select(i => new SelectListItem
            {
                Value = i.ItemID.ToString(),
                Text = i.ItemName
            });
        }
    }
}
