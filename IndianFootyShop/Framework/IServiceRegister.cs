using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public interface IServiceRegister
    {
        void RegisterMultipleInstances<T>(params T[] instances);

        void RegisterMultipleInstances(Type type, params object[] instances);

        void RegisterInstance(Type type, object instance);

        void RegisterInstance<T>(T instance);

        void RegisterType(Type fromType, Type toType);
    }
}
