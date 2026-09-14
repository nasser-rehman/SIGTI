using SIGTI.Domain.Enums;

namespace SIGTI.Domain.Constants
{
    public static class Roles
    {
        public const string Administrator = nameof(Role.Administrator);
        public const string Technician = nameof(Role.Technician);
        public const string User = nameof(Role.User);

        public const string TechnicalStaff = $"{Administrator},{Technician}";
    }
}
