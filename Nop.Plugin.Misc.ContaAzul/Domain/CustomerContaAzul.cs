using Nop.Core;
using System;

namespace Nop.Plugin.Misc.ContaAzul.Domain
{
    public class CustomerContaAzul: BaseEntity
    {
        public int CustomerId { get; set; }

        public Guid ContaAzulId { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
