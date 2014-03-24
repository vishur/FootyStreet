using System;

namespace Framework.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EntityKeyAttribute : Attribute
    {
        public int Order { get; private set; }

        public EntityKeyAttribute(int order)
        {
            Order = order;
        }
    }
}