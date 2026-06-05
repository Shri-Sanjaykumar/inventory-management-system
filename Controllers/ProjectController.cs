using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InternInventory.Models;
using InternInventory.Services;

namespace InternInventory.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null)
        {
            try
            {
                IEnumerable<Project> projects;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    projects = await _projectService.SearchProjectsAsync(search);
                    ViewData["SearchTerm"] = search;
                }
                else
                {
                    projects = await _projectService.GetAllProjectsAsync();
                }
                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects index.");
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
        public async Task<IActionResult> Create(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            try
            {
                var username = User.Identity?.Name ?? "system";
                await _projectService.AddProjectAsync(project, username);
                TempData["SuccessMessage"] = "Project added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project: {ProjectName}", project.ProjectName);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the project. Please try again.");
                return View(project);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project for edit: ID {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.ProjectID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            try
            {
                var updated = await _projectService.UpdateProjectAsync(project);
                if (!updated)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Project updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project ID: {Id}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the project. Please try again.");
                return View(project);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Administrator,Backend Developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _projectService.DeleteProjectAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Project deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project ID: {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete project. It is referenced by stock receipts.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
