using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Entities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class EntityForeignKeyAttribute : ForeignKeyAttribute 
    {
        public int Order { get; private set; }

        public EntityForeignKeyAttribute(string name, int order) : base(name)
        {
            Order = order;
        }
    }
}
