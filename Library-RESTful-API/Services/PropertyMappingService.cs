using Library_RESTful_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() {"Id"} ) },
                { "Genre", new PropertyMappingValue(new List<string>() {"Genre"} ) },
                { "Age", new PropertyMappingValue(new List<string>() {"DateOfBirth"}, true ) },
                { "Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"} ) },
            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }
        
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappinDictionary;
            }

            throw new Exception();
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //spli strings with ','

            //string is seprated by ',' so split it
            var fieldsSplited = fields.Split(',');

            //apply orderby in reverse order
            //otherwise IQueryable will be ordered in wrong order
            foreach (var field in fieldsSplited)
            {
                //triming
                var trimmedField = field.Trim();

                //remove " asc" and " desc" from query
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                //checking property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
