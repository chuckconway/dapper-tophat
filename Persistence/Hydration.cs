using System;
using System.ComponentModel;
using Dapper.TopHat.Query;

namespace Dapper.TopHat.Persistence
{
    public class Hydration
    {
        /// <summary> Converts this object to a proper type. </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the requested operation is invalid. </exception>
        /// <param name="value">    The value. </param>
        /// <param name="property"> The property. </param>
        /// <returns> The given data converted to a proper type. </returns>
        public object ConvertToProperType(object value, Property property)
        {
            //TODO:Intercept Data Property Materialize here.

            if (property.PropertyDescriptor.PropertyType.IsEnum)
            {
                value = Enum.Parse(property.PropertyDescriptor.PropertyType, Convert.ToString(value), true);
            }

            if (property.PropertyDescriptor.PropertyType.IsValueType && value == null && Nullable.GetUnderlyingType(property.PropertyDescriptor.PropertyType) == null)
            {
                throw new InvalidOperationException(
                    $"Can't cast the property {property.Name}, which is of type '{property.PropertyDescriptor.PropertyType.Name}', to a 'null' value, consider using a Nullable type");
            }

            if (value != null && value.GetType() != property.PropertyDescriptor.PropertyType)
            {
                var converter = TypeDescriptor.GetConverter(property.PropertyDescriptor.PropertyType);

                try
                {
                    var propType = property.PropertyDescriptor.PropertyType;

                    value = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>)
                         ? Convert.ChangeType(value, Nullable.GetUnderlyingType(propType))
                         : converter.ConvertTo(value, propType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Can't cast the property {property.Name}, which is of type '{property.PropertyDescriptor.PropertyType.Name}' to type of {value.GetType().Name}.", ex);
                }

            }

            return value;
        }
    }
}
