using InternInventory.Models;

namespace InternInventory.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<bool> AddProjectAsync(Project project, string currentUserName);
        Task<bool> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int id);
    }
}
