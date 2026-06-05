using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InternInventory.Services;

namespace InternInventory.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly IProjectService _projectService;
        private readonly IItemService _itemService;
        private readonly IStockReceiptService _stockReceiptService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IVendorService vendorService,
            IProjectService projectService,
            IItemService itemService,
            IStockReceiptService stockReceiptService,
            ILogger<DashboardController> logger)
        {
            _vendorService = vendorService;
            _projectService = projectService;
            _itemService = itemService;
            _stockReceiptService = stockReceiptService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                var projects = await _projectService.GetAllProjectsAsync();
                var items = await _itemService.GetAllItemsAsync();
                var receipts = await _stockReceiptService.GetAllStockReceiptsAsync();

                ViewBag.TotalVendors = vendors.Count();
                ViewBag.TotalProjects = projects.Count();
                ViewBag.TotalItems = items.Count();
                ViewBag.TotalStockReceipts = receipts.Count();

                // Get recent stock receipts (last 5)
                var recentReceipts = receipts
                    .OrderByDescending(r => r.CreatedOn)
                    .Take(5);

                return View(recentReceipts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard metrics.");
                return View("Error");
            }
        }
    }
}
