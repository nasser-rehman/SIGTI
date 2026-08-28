using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Domain.Factories
{
    public sealed class UserFactory
    {
        public User Create(
            string name,
            Email email,
            string passwordHash,
            Role role,
            Department department
        )
        {
            return new User(name, email, passwordHash, role, department);
        }

        public User CreateSystemUser(
            string name,
            Email email,
            string passwordHash,
            Role role,
            Department department
        )
        {
            var user = new User(name, email, passwordHash, role, department);
            user.MarkAsSystem();
            return user;
        }
    }
}
