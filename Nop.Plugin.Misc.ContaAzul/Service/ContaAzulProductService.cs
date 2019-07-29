using Nop.Core.Data;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.ContaAzul.Service
{
    public class ContaAzulProductService: IContaAzulProductService
    {
        private readonly IRepository<ProductContaAzul> _productContaAzulRepository;

        public ContaAzulProductService(IRepository<ProductContaAzul> productContaAzulRepository)
        {
            _productContaAzulRepository = productContaAzulRepository;
        }

        public void InsertProduct(ProductContaAzul productContaAzul)
        {
            if (productContaAzul == null)
                throw new ArgumentNullException("productContaAzul");

            _productContaAzulRepository.Insert(productContaAzul);
        }

        public ProductContaAzul GetProduct(ProductResponse product)
        {
            var query = _productContaAzulRepository.Table;

            query = query.Where(o => o.ContaAzulId == product.id);

            if (query.Count() != 0)
                return query.FirstOrDefault();

            return null;
        }

    }
}
