using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InternInventory.Models;
using InternInventory.Services;

namespace InternInventory.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null)
        {
            try
            {
                IEnumerable<Item> items;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    items = await _itemService.SearchItemsAsync(search);
                    ViewData["SearchTerm"] = search;
                }
                else
                {
                    items = await _itemService.GetAllItemsAsync();
                }
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items index.");
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
        public async Task<IActionResult> Create(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            try
            {
                await _itemService.AddItemAsync(item);
                TempData["SuccessMessage"] = "Item added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item: {ItemName}", item.ItemName);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the item. Please try again.");
                return View(item);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item for edit: ID {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item)
        {
            if (id != item.ItemID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            try
            {
                var updated = await _itemService.UpdateItemAsync(item);
                if (!updated)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Item updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item ID: {Id}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the item. Please try again.");
                return View(item);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _itemService.DeleteItemAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Item deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item ID: {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete item. It is referenced by stock receipts.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
