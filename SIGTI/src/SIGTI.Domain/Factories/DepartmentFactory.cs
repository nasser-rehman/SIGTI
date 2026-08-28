using SIGTI.Domain.Entities;

namespace SIGTI.Domain.Factories
{
    public sealed class DepartmentFactory
    {
        public Department Create(string name, string description)
        {
            return new Department(name, description);
        }
    }
}
