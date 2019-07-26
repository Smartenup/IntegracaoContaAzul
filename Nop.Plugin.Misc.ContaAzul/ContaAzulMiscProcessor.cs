using Nop.Core.Data;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.ContaAzul.Data;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ContaAzul
{
    public class ContaAzulMiscProcessor : BasePlugin, IMiscPlugin
    {
        private ContaAzulObjectContext _context;
        private IRepository<CustomerContaAzul> _customer;

        public ContaAzulMiscProcessor(ContaAzulObjectContext context, IRepository<CustomerContaAzul> customer)
        {
            _context = context;
            _customer = customer;
        }

        //public void ManageSiteMap(SiteMapNode rootNode)
        //{
        //    var menuItem = new SiteMapNode()
        //    {
        //        SystemName = "Misc.IntegracaoContaAzul",
        //        Title = "Integracao Conta Azul",
        //        ControllerName = "MiscIntegracaoContaAzul",
        //        ActionName = "Configure",
        //        Visible = true,
        //        RouteValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.IntegracaoContaAzul.Controllers" }, { "area", null } },
        //    };
        //    var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
        //    if (pluginNode != null)
        //        pluginNode.ChildNodes.Add(menuItem);
        //    else
        //        rootNode.ChildNodes.Add(menuItem);
        //}

        public override void Install()
        {
            _context.Install();
            base.Install();
        }


        public override void Uninstall()
        {
            _context.Uninstall();
            base.Uninstall();
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "MiscContaAzul";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.ContaAzul.Controllers" }, { "area", null } };
        }
    }
}
