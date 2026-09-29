using System;
using SIGTI.Domain.Entities;

namespace SIGTI.Domain.Tests.Builders
{
    public class SupportQueueBuilder
    {
        private string _name = "Fila N1";
        private string _description =
            "Atendimento de primeiro nível (helpdesk).";

        private bool _isActive = true;

        public SupportQueueBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public SupportQueueBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public SupportQueueBuilder AsDeactivated()
        {
            _isActive = false;
            return this;
        }

        public SupportQueue Build()
        {
            var queue = new SupportQueue(_name, _description);
            if (!_isActive)
                queue.Deactivate();

            return queue;
        }
    }
}
