namespace SIGTI.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key) : base($"{entityName} {key} não foi encontrado.")
        {
        }
        public NotFoundException(string message) : base(message)
        {

        }

        public NotFoundException() : base()
        {
        }

        public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }

}

