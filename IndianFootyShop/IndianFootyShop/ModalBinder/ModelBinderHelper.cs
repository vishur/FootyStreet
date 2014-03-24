using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndianFootyShop
{
    internal static class ModelBinderHelper
    {
        public static bool IsCollection(this object obj)
        {
            return (obj != null)
                && (obj.GetType() != typeof(String))
                && (typeof(System.Collections.IEnumerable).IsInstanceOfType(obj));
        }

        public static int CollectionGetCount(this object collection)
        {
            if (collection.GetType().IsArray)
                return ((Array)collection).GetLength(0);
            else if (collection.GetType().GetProperty("Count") != null)
                return (int)collection.GetType().GetProperty("Count")
                        .GetValue(collection, null);
            //else if (collection is IEnumerable<LineItem>)
            //    return (int)(collection as IEnumerable<LineItem>).ToList().GetType().GetProperty("Count")
            //        .GetValue((collection as IEnumerable<LineItem>).ToList(), null);
            else return 0;
        }

        public static object CollectionGetItem(this object collection, int index)
        {
            if (collection.GetType().IsArray)
                return ((Array)collection).GetValue(index);
            else if (collection.GetType().GetProperty("Item") != null)
                return collection.GetType().GetProperty("Item")
                    .GetValue(collection, new object[] { index });
            //else if (collection is IEnumerable<LineItem>)
            //    return (collection as IEnumerable<LineItem>).ToList().GetType().GetProperty("Item")
            //        .GetValue((collection as IEnumerable<LineItem>).ToList(), new object[] { index });
            else return null;
        }

        public static void CollectionSetItem(this object collection, int index, object value)
        {
            if (collection.GetType().IsArray)
                ((Array)collection).SetValue(value, index);
            else if (collection.GetType().GetProperty("Item") != null)
                collection.GetType().GetProperty("Item")
                    .SetValue(collection, value, new object[] { index });
            //else if (collection is IEnumerable<LineItem>)
            //{
            //    (collection as IEnumerable<LineItem>).ToList().GetType().GetProperty("Item")
            //        .SetValue((collection as IEnumerable<LineItem>).ToList(), value, new object[] { index });
            //}

        }
    }
}