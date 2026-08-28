using SIGTI.Domain.Entities;
using System;

namespace SIGTI.Domain.Tests.Builders
{
    public class SupportQueueBuilder
    {
        private string _name = "Fila N1";
        private string _description = "Atendimento de primeiro nível (helpdesk).";

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

        public SupportQueue Build()
        {
            return new SupportQueue(_name, _description);
        }
    }
}
