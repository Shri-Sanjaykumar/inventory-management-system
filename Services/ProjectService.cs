using InternInventory.Models;
using InternInventory.Repositories;

namespace InternInventory.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.OrderBy(p => p.ProjectName);
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm)
        {
            var projects = await _projectRepository.SearchByProjectNameAsync(searchTerm);
            return projects.OrderBy(p => p.ProjectName);
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task<bool> AddProjectAsync(Project project, string currentUserName)
        {
            project.CreatedBy = currentUserName;
            project.CreatedOn = DateTime.UtcNow;

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            var existing = await _projectRepository.GetByIdAsync(project.ProjectID);
            if (existing == null) return false;

            existing.ProjectName = project.ProjectName.Trim();
            existing.AddressLine1 = project.AddressLine1.Trim();
            existing.City = project.City.Trim();
            existing.State = project.State.Trim();
            existing.Pincode = project.Pincode.Trim();

            await _projectRepository.UpdateAsync(existing);
            await _projectRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var existing = await _projectRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _projectRepository.DeleteAsync(id);
            await _projectRepository.SaveAsync();
            return true;
        }
    }
}
