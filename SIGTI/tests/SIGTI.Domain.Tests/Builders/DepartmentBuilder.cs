using System;
using SIGTI.Domain.Entities;

namespace SIGTI.Domain.Tests.Builders
{
    public class DepartmentBuilder
    {
        private string _name = "Departamento de TI";
        private string _description = "Responsável por manter a infraestrutura e sistemas.";
        private bool _isActive = true;

        public DepartmentBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public DepartmentBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public DepartmentBuilder AsDeactivated()
        {
            _isActive = false;
            return this;
        }

        public Department Build()
        {
            var department = new Department(_name, _description);
            if (!_isActive)
                department.Deactivate();

            return department;
        }
    }
}
