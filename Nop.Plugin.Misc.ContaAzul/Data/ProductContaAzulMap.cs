using Nop.Data.Mapping;
using Nop.Plugin.Misc.ContaAzul.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Data
{
    public class ProductContaAzulMap: NopEntityTypeConfiguration<ProductContaAzul>
    {
        public ProductContaAzulMap()
        {
            this.ToTable("Product_ContaAzul");
            this.HasKey(x => x.Id);
        }
    }
}
