using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.ContaAzul.Data;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Service;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.ContaAzul.Infrastructure
{
    public class DependecyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        private const string CONTEXT_NAME = "nop_object_context_contaazul";

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ContaAzulService>().As<IContaAzulService>().InstancePerLifetimeScope();
            builder.RegisterType<ContaAzulCustomerService>().As<IContaAzulCustomerService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<ContaAzulObjectContext>(builder, CONTEXT_NAME);


            builder.RegisterType<EfRepository<CustomerContaAzul>>()
            .As<IRepository<CustomerContaAzul>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            ////override required repository with our custom context
            //builder.RegisterType<EfRepository<ContaAzulRecord>>()
            //.As<IRepository<ContaAzulRecord>>()
            //.WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            //.InstancePerLifetimeScope();
        }
    }
}
