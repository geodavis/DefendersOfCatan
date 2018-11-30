using DefendersOfCatan.BusinessLogic.Repository;
using DefendersOfCatan.DAL;
using StructureMap;
using StructureMap.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DefendersOfCatan
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Container _container;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

             _container = new Container(_ =>
             {
                 _.Scan(x =>
                 {
                     x.TheCallingAssembly();
                     x.WithDefaultConventions();
                 });
                 _.For<IGameContext>().HybridHttpOrThreadLocalScoped().Use<GameContext>();
                 _.For<IDevelopmentRepository>().Use<DevelopmentRepository>();
                 _.For<IBaseRepository>().Use<BaseRepository>();
                 _.For<IEnemyRepository>().Use<EnemyRepository>();
                 _.For<IGameRepository>().Use<GameRepository>();
                 _.For<IPlayerRepository>().Use<PlayerRepository>();
                 _.For<ITileRepository>().Use<TileRepository>();

                 ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());

             });
        }

        public class StructureMapControllerFactory : DefaultControllerFactory
        {
            protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
            {
                if (controllerType == null)
                {
                    return base.GetControllerInstance(requestContext, controllerType);
                }
                return (Controller)_container.GetInstance(controllerType);
            }
        }
    }
}
