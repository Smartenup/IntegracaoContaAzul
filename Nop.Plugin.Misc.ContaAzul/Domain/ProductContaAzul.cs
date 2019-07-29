using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Domain
{
    public class ProductContaAzul: BaseEntity
    {
        public int ProductId { get; set; }

        public Guid ContaAzulId { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
