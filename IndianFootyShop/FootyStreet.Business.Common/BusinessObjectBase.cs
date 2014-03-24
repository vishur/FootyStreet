using FootyStreet.Utilities;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootyStreet.Business.Common
{
    public abstract class BusinessObjectBase
    {
        private ISessionContainer dataContainer;
        private IServiceLocator container;

        public BusinessObjectBase(IServiceLocator Container, ISessionContainer DataContainer)
        {
            dataContainer = DataContainer;
            container = Container;

        }

        protected TModel GetBuinessObjects<TModel>(string Key)
           where TModel : class
        {
            return (dataContainer.GetSessionData<TModel>(Key));
        }

        protected TModel GetBuinessObjects<TModel>()
            where TModel : class
        {
            return (dataContainer.GetSessionData<TModel>());
        }

        protected void SetBusinessObject<TModel>(TModel DataItem, string Key)
            where TModel : class
        {
            dataContainer.SetSessionData<TModel>(DataItem, Key);
        }

        protected void RemoveObject<TModel>(string Key)
            where TModel : class
        {
            dataContainer.RemoveSessionData(Key);
        }

        protected void RemoveSessionObjects()
        {
            dataContainer.RemoveSessionData();
        }

        protected void RemoveSessionObjects(IList<String> keysToRetain)
        {
            dataContainer.RemoveSessionData(keysToRetain);
        }

    }
}
