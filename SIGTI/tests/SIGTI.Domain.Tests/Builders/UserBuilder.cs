using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Domain.Tests.Builders
{
    public class UserBuilder
    {
        private string _name = "Usuário Teste";
        private string _email = "example@example.com";
        private Email? _emailObject;
        private string _passwordHash = "hashteste";
        private Role _role = Role.Technician;
        private Department _department = new DepartmentBuilder().Build();
        private bool _isActive = true;

        public UserBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public UserBuilder WithInvalidLongName()
        {
            _name = new string('A', 101);
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;
            _emailObject = null;
            return this;
        }

        public UserBuilder WithEmail(Email email)
        {
            _emailObject = email;
            return this;
        }

        public UserBuilder WithPasswordHash(string passwordHash)
        {
            _passwordHash = passwordHash;
            return this;
        }

        public UserBuilder WithRole(Role role)
        {
            _role = role;
            return this;
        }

        public UserBuilder WithDepartmentId(Department department)
        {
            _department = department;
            return this;
        }

        public UserBuilder WithEmptyDepartment()
        {
            _department = null!;
            return this;
        }

        public UserBuilder AsDeactivated()
        {
            _isActive = false;
            return this;
        }

        public User Build()
        {
            var email = _emailObject ?? new Email(_email);
            var user = new User(_name, email, _passwordHash, _role, _department);

            if (!_isActive)
                user.Deactivate();

            return user;
        }
    }
}
