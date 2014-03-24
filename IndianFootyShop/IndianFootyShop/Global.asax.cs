using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IndianFootyShop
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ////Loding the Unity Container from Web Config
            IUnityContainer containerUnity = UnityContainerBootstrapper.Bootstrap();

            UnityMvcServiceLocator unityMVCServiceLcator = new UnityMvcServiceLocator(containerUnity);
            HttpContext.Current.Cache["UnityServiceLocator"] = unityMVCServiceLcator;

            var factory = new ServiceLocatorControllerFactory(unityMVCServiceLcator);
            ControllerBuilder.Current.SetControllerFactory(factory);
            DependencyResolver.SetResolver(unityMVCServiceLcator);

            //Add the InterfaceModelBinder
            ModelBinders.Binders.DefaultBinder = new InterfaceModelBinder(unityMVCServiceLcator);
        }
    }
}