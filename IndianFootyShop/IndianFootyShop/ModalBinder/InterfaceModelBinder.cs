using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace IndianFootyShop
{
    /// <summary>
    /// Custom model binder to bind Interfaces or Abstract class using UnityContainer
    /// </summary>
    public class InterfaceModelBinder : DefaultModelBinder
    {
        IServiceLocator serviceLocator = null;

        public InterfaceModelBinder(IServiceLocator ServiceLocator)
        {
            serviceLocator = ServiceLocator;
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if ((bindingContext.Model.IsCollection()) && (bindingContext.Model.CollectionGetCount() > 0))
                return this.BindCollection(controllerContext, bindingContext);
            else
                return base.BindModel(controllerContext, bindingContext);
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            //If Abstract class or Interface then only use UnityContainer
            //otherwise use default.
            if (modelType.IsAbstract || modelType.IsInterface)
            {
                var model = serviceLocator.GetInstance(modelType);
                // return model;
                return base.CreateModel(controllerContext, bindingContext, model.GetType());
            }
            else
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
        }



        private object BindCollection(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object collection = bindingContext.Model;
            Type collectionMemberType = typeof(Object);
            if (collection.GetType().IsGenericType)
                collectionMemberType =
                    collection.GetType().GetGenericArguments()[0];
            int count = collection.CollectionGetCount();
            for (int index = 0; index < count; index++)
            {
                // Create a BindingContext for the collection member:   
                ModelBindingContext innerContext = new ModelBindingContext();
                object member = collection.CollectionGetItem(index);
                Type memberType =
                    (member == null) ? collectionMemberType : member.GetType();
                innerContext.ModelMetadata =
                    ModelMetadataProviders.Current.GetMetadataForType(
                        delegate() { return member; },
                        memberType);
                innerContext.ModelName =
                    String.Format("{0}[{1}]", bindingContext.ModelName, index);
                innerContext.ModelState = bindingContext.ModelState;
                innerContext.PropertyFilter = bindingContext.PropertyFilter;
                innerContext.ValueProvider = bindingContext.ValueProvider;

                // Bind the collection member:   
                IModelBinder binder = Binders.GetBinder(memberType);
                object boundMember =
                    binder.BindModel(controllerContext, innerContext) ?? member;
                collection.CollectionSetItem(index, boundMember);
            }

            // Return the collection:   
            return collection;
        }

        private object BindCollectionProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, object Model)
        {
            object collection = Model;
            Type collectionMemberType = typeof(Object);
            if (collection.GetType().IsGenericType)
                collectionMemberType =
                    collection.GetType().GetGenericArguments()[0];
            int count = collection.CollectionGetCount();
            for (int index = 0; index < count; index++)
            {
                // Create a BindingContext for the collection member:   
                ModelBindingContext innerContext = new ModelBindingContext();
                object member = collection.CollectionGetItem(index);
                Type memberType =
                    (member == null) ? collectionMemberType : member.GetType();
                innerContext.ModelMetadata =
                    ModelMetadataProviders.Current.GetMetadataForType(
                        delegate() { return member; },
                        memberType);
                innerContext.ModelName =
                    String.Format("{0}[{1}]", bindingContext.ModelName, index);
                innerContext.ModelState = bindingContext.ModelState;
                innerContext.PropertyFilter = bindingContext.PropertyFilter;
                innerContext.ValueProvider = bindingContext.ValueProvider;

                // Bind the collection member:   
                IModelBinder binder = Binders.GetBinder(memberType);
                object boundMember =
                    binder.BindModel(controllerContext, innerContext) ?? member;
                collection.CollectionSetItem(index, boundMember);
            }

            // Return the collection:   
            return collection;
        }

    }
}