using Nop.Data.Mapping;
using Nop.Plugin.Misc.ContaAzul.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Data
{
    public class CustomerContaAzulMap: NopEntityTypeConfiguration<CustomerContaAzul>
    {
        public CustomerContaAzulMap()
        {
            this.ToTable("Customer_ContaAzul");
            this.HasKey(x => x.Id);
        }
    }
}
