namespace SIS.MvcFramework.Mapping
{
    using System.Collections.Generic;
    using System.Linq;

    public static class MappingExtensions
    {
        public static IEnumerable<TDestination> To<TDestination>(this IEnumerable<object> collection)
        {
            return collection
                .Select(elem => ModelMapper.ProjectTo<TDestination>(elem));
        }
    }
}
