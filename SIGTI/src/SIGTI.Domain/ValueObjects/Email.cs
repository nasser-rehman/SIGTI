using System.Net.Mail;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Domain.ValueObjects
{
    public sealed record Email
    {
        private const int MaxLength = 254;
        public string Value { get; }

        public Email(string value)
        {
            value = value.Trim();

            if (!IsValid(value))
                throw new DomainException("O E-mail informado é invalido.");

            Value = value;
        }

        private static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || (email.Length > MaxLength))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
