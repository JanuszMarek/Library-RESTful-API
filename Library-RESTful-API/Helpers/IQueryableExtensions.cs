using Library_RESTful_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Library_RESTful_API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if(string.IsNullOrWhiteSpace(orderBy))
            {
                //no sorting
                return source;
            }

            //string is seprated by ',' so split it
            var orderBySplit = orderBy.Split(',');

            //apply orderby in reverse order
            //otherwise IQueryable will be ordered in wrong order
            foreach(var orderByClause in orderBySplit.Reverse())
            {
                //triming
                var trimmedOrderBy = orderByClause.Trim();

                //checking desc or asc
                var orderDescending = trimmedOrderBy.EndsWith(" desc");

                //remove " asc" and " desc" from query
                var indexOfFirstSpace = trimmedOrderBy.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderBy : trimmedOrderBy.Remove(indexOfFirstSpace);

                //checking property
                if(!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];

                if(propertyMappingValue == null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue));
                }

                //Run revers to orderby correct order
                foreach(var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    //revert sort order if necessary
                    if(propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }
            }
            return source;
        }
    }
}
