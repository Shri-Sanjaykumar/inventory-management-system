using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InternInventory.Models;
using InternInventory.Services;

namespace InternInventory.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null)
        {
            try
            {
                IEnumerable<Vendor> vendors;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    vendors = await _vendorService.SearchVendorsAsync(search);
                    ViewData["SearchTerm"] = search;
                }
                else
                {
                    vendors = await _vendorService.GetAllVendorsAsync();
                }
                return View(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendors index.");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vendor vendor)
        {
            if (!ModelState.IsValid)
            {
                return View(vendor);
            }

            try
            {
                var username = User.Identity?.Name ?? "system";
                await _vendorService.AddVendorAsync(vendor, username);
                TempData["SuccessMessage"] = "Vendor added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vendor: {FirstName} {LastName}", vendor.FirstName, vendor.LastName);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the vendor. Please try again.");
                return View(vendor);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                if (vendor == null)
                {
                    return NotFound();
                }
                return View(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendor for edit: ID {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vendor vendor)
        {
            if (id != vendor.VendorID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(vendor);
            }

            try
            {
                var updated = await _vendorService.UpdateVendorAsync(vendor);
                if (!updated)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Vendor updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor ID: {Id}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the vendor. Please try again.");
                return View(vendor);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _vendorService.DeleteVendorAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Vendor deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vendor ID: {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete vendor. It is referenced by stock receipts.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
