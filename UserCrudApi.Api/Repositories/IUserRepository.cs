using UserCrudApi.Api.Models;

namespace UserCrudApi.Api.Repositories;

public interface IUserRepository
{
    IReadOnlyCollection<User> GetAll();

    User? GetById(int id);

    User Create(string name, string email);

    bool EmailExists(string email, int? excludingUserId = null);

    bool Update(int id, string name, string email);

    bool Delete(int id);
}
