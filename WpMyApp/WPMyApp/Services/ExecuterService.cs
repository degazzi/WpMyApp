using WpMyApp.Data;
using WpMyApp.Models;

namespace WpMyApp.Services
{
    public class ExecuterService
    {
        private readonly ExecuterRepository _repository;

        public ExecuterService(ExecuterRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Executer>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Executer> GetAsync(string id) => _repository.GetByIdAsync(id);

        public Task CreateAsync(Executer executer) => _repository.CreateAsync(executer);

        public Task UpdateAsync(string id, Executer executer) =>
            _repository.UpdateAsync(id, executer);

        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}