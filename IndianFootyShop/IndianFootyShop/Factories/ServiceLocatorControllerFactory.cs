using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IndianFootyShop
{
    public class ServiceLocatorControllerFactory : IControllerFactory
    {
        private UnityMvcServiceLocator _container;
        private IControllerFactory _innerFactory;

        public ServiceLocatorControllerFactory(UnityMvcServiceLocator container)
            : this(container, new DefaultControllerFactory())
        {
        }

        protected ServiceLocatorControllerFactory(UnityMvcServiceLocator container, IControllerFactory innerFactory)
        {
            _container = container;
            _innerFactory = innerFactory;
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            try
            {
                IController icontroller = _container.GetInstance<IController>(controllerName);
                if (icontroller == null)
                {

                    return _innerFactory.CreateController(requestContext, controllerName);
                }
                else
                {
                    return icontroller;
                }
            }
            catch (Exception ex)
            {
                //Logger.LogErrorData(ex);
                return _innerFactory.CreateController(requestContext, controllerName);
            }
        }

        public void ReleaseController(IController controller)
        {
            _container.Release(controller);
        }


        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return System.Web.SessionState.SessionStateBehavior.Default;
        }
    }
}