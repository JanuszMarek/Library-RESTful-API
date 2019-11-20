using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Library_RESTful_API.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObjectList = new List<ExpandoObject>();

            //crate a list of PropertyInfo objects on TSource
            var propertyInfoList = new List<PropertyInfo>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                //if empty then all public properties should be in ExpandObject
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // only wanted public fields should be in ExpandObject

                //split fields
                var fieldSplited = fields.Split(',');

                foreach (var field in fieldSplited)
                {
                    //delete spaces
                    var propertyName = field.Trim();

                    //get properties
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} can not be found");
                    }

                    //add properties to list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            //run through the source objects
            foreach(TSource sourceObject in source)
            {
                //ExpandoObject hold selected prop and values
                var dataShapedObject = new ExpandoObject();

                //Get value of properties to return
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //add to ExpandObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                //add to main list
                expandoObjectList.Add(dataShapedObject);
            }

            //return list
            return expandoObjectList;
        }
    }
}
