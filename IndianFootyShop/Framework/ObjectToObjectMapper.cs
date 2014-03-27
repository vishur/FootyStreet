using AutoMapper;
using System;
using Microsoft.Practices.ServiceLocation;

namespace Framework
{
    /// <summary>
    /// This class is used for mapping objects to objects.
    /// </summary>
    public static class ObjectToObjectMapper
    {

        private static readonly IServiceLocator _Container = new Container("ObjectToObjectMapper");

        private static readonly object LockObject = new object();

        private static bool _loaded = false;

        /// <summary>
        /// This method is used for translating objects to objects.
        /// </summary>
        /// <param name="source">
        /// The source object for translation.
        /// </param>
        /// <param name="destination">
        /// The destination object for translation.
        /// </param>
        public static void TranslateObject<TSource, TDestination>(TSource source, TDestination destination)
        {
            LoadCustomMaps();

            var adapter = _Container.GetInstance<IMapAdapter<TSource, TDestination>>();
            if (adapter != null)
            {
                adapter.Map(source, destination);
            }
            else if (source is IMap<TDestination>)
            {
                IMap<TDestination> mapper = (IMap<TDestination>)source;
                mapper.Map(destination);
            }
            else if (destination is IMap<TSource>)
            {
                IMap<TSource> mapper = (IMap<TSource>)destination;
                mapper.Map(source);
            }
            else if (typeof(object) == typeof(TSource) || typeof(TDestination) == typeof(object))
            {
                Type sourceType = source.GetType();
                if (sourceType.Assembly.IsDynamic && sourceType.BaseType != null)
                {
                    sourceType = sourceType.BaseType;
                }
                Type destinationType = destination.GetType();
                if (destinationType.Assembly.IsDynamic && destinationType.BaseType != null)
                {
                    destinationType = destinationType.BaseType;
                }
                Mapper.CreateMap(sourceType, destinationType);
                Mapper.Map(source, destination, sourceType, destinationType);
            }
            else
            {
                Mapper.CreateMap<TSource, TDestination>();
                Mapper.Map(source, destination, source.GetType(), typeof(TDestination));
            }
        }

        /// <summary>
        /// This method is used for translating objects to objects.
        /// </summary>
        /// <param name="source">
        /// The source object for translation.
        /// </param>
        /// <returns>TDestination</returns>
        public static TDestination TranslateObject<TSource, TDestination>(TSource source) where TDestination : new()
        {
            var result = new TDestination();
            TranslateObject(source, result);
            return result;
        }



        private static void LoadCustomMaps()
        {
            if (!_loaded)
            {
                lock (LockObject)
                {
                    if (!_loaded)
                    {
                        Mapper.CreateMap<string, bool?>().ConvertUsing(PropertyMapper.StringToNullableBool);
                        Mapper.CreateMap<bool?, string>().ConvertUsing(PropertyMapper.NullableBoolToString);
                        Mapper.CreateMap<string, bool>().ConvertUsing(PropertyMapper.StringToBool);
                        Mapper.CreateMap<bool, string>().ConvertUsing(PropertyMapper.BoolToString);
                        _loaded = true;
                    }
                }
            }
        }
    }
}