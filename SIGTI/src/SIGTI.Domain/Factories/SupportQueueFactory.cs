using SIGTI.Domain.Entities;

namespace SIGTI.Domain.Factories
{
    public sealed class SupportQueueFactory
    {
        public SupportQueue Create(string name, string description)
        {
            return new SupportQueue(name, description);
        }
    }
}
