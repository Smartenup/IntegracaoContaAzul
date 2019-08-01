using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Lib;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Request;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;

using Nop.Plugin.Misc.ContaAzul.Service;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul
{
    public class ContaAzulSincronizaProdutoTask: ITask
    {
        private IContaAzulProductService _contaAzulProductService;
        private IContaAzulService _contaAzulService;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public ContaAzulSincronizaProdutoTask(IContaAzulProductService contaAzulProductService,
            ISettingService settingService,
            IContaAzulService contaAzulService,
            IProductService productService,
            IStoreService storeService,
            IWorkContext workContext,
            ILogger logger)
        {
            _contaAzulProductService = contaAzulProductService;
            _contaAzulService = contaAzulService;
            _productService = productService;
            _storeService = storeService;
            _logger = logger;
            _settingService = settingService;

        }

        public void Execute()
        {
            _contaAzulService.RefreshToken();

            var products = _productService.SearchProducts(categoryIds: null,
               pageSize: int.MaxValue,
               showHidden: true);

            ProductResponse[] GetProductResponse = null;
            ProductResponse ProductResponse = null;
            CategoryResponse[] CategoryResponse = null;
            var ContaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>();

            var categoria = "?name=Mercadoria para Revenda";

            //busca a categoria no conta azul para obter o id:
            using (var getcategory = new GetCategory(ContaAzulMiscSettings.UseSandbox))
                CategoryResponse = getcategory.CreateAsync(ContaAzulMiscSettings.access_token, categoria).ConfigureAwait(false).GetAwaiter().GetResult();

            foreach (var item in products)
            {
                var product = new ProductMessage();


                product.name = item.Name;
                product.value = Math.Round(item.Price, 1);
                product.cost = Math.Round(item.ProductCost);
                product.available_stock = item.StockQuantity;
                product.net_weight = Math.Round(item.Weight, 3);
                product.category_id = CategoryResponse[0].id;
                product.gross_weight = Math.Round(item.Weight, 3);

                try
                {
                    var filtro = "?name=" + item.Name;

                    using (var getproduct = new GetProduct(ContaAzulMiscSettings.UseSandbox))
                        GetProductResponse = getproduct.CreateAsync(null, ContaAzulMiscSettings.access_token, filtro).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (GetProductResponse.Count() > 0)
                    {
                        var productTable = _contaAzulProductService.GetProduct(GetProductResponse[0]);
                        //caso ele não exista na tabela relacional do banco, insere e atualiza no conta azul
                        if (productTable == null)
                        {
                            using (var productCreation = new ProductCreation(ContaAzulMiscSettings.UseSandbox))
                                ProductResponse = productCreation.CreateAsyncUpdate(product, GetProductResponse[0].id.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

                            if (ProductResponse != null)
                            {
                                var productContaAzul = new ProductContaAzul();

                                productContaAzul.ContaAzulId = ProductResponse.id;
                                productContaAzul.ProductId = item.Id;
                                productContaAzul.DataCriacao = DateTime.Now;
                                _contaAzulProductService.InsertProduct(productContaAzul);
                            }

                        }
                        else
                        {//caso já existe no banco, só verifica se houve alteração e faz o update no conta azul

                            product.id = productTable.ContaAzulId.ToString();
                            product.category_id = null;

                            var objetoContaAzul = JsonConvert.SerializeObject(GetProductResponse[0]);
                            var objetoAtualizar = JsonConvert.SerializeObject(product);

                            if (!objetoAtualizar.Equals(objetoContaAzul))
                            {
                                using (var productCreation = new ProductCreation(ContaAzulMiscSettings.UseSandbox))
                                    ProductResponse = productCreation.CreateAsyncUpdate(product, productTable.ContaAzulId.ToString(), ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();
                            }
                        }
                    }
                    else
                    {//caso ele não exista no conta azul, faz a inserção dele no conta azul e no banco de dados

                        using (var productCreation = new ProductCreation(ContaAzulMiscSettings.UseSandbox))
                            ProductResponse = productCreation.CreateAsync(product, ContaAzulMiscSettings.access_token).ConfigureAwait(false).GetAwaiter().GetResult();

                        if (ProductResponse != null)
                        {
                            var productContaAzul = new ProductContaAzul();

                            productContaAzul.ContaAzulId = ProductResponse.id;
                            productContaAzul.ProductId = item.Id;
                            productContaAzul.DataCriacao = DateTime.Now;
                            _contaAzulProductService.InsertProduct(productContaAzul);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);

                }
            }
        }
    }
}
