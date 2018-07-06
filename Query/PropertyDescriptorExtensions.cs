using System;
using System.ComponentModel;
using System.Linq;

namespace Dapper.TopHat.Query
{
    public static class PropertyDescriptorExtensions
    {
        /// <summary> A PropertyDescriptor extension method that gets an attribute. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="property"> The information. </param>
        /// <returns> The attribute&lt; t&gt; </returns>
        public static T GetAttribute<T>(this PropertyDescriptor property) where T : Attribute
        {
            T ofType = property.Attributes.OfType<T>().FirstOrDefault();
            return ofType;
        }

        ///// <summary> Gets a name. </summary>
        ///// <typeparam name="T"> Generic type parameter. </typeparam>
        ///// <param name="property"> The information. </param>
        ///// <returns> The name. </returns>
        //public static string GetName<T>(this PropertyDescriptor property) where T : Attribute, IName
        //{
        //    IName val = GetAttribute<T>(property);
        //    return (val == null ? property.Name : val.Name);
        //}
    }
} 
