using System.ComponentModel;

namespace Dapper.TopHat.Query
{
    public class Property
    {
        /// <summary> Constructor. </summary>
        /// <param name="name">               The name. </param>
        /// <param name="value">              The value. </param>
        /// <param name="propertyDescriptor"> The information. </param>
        /// <param name="instance">           The instance. </param>
        /// <param name="lineage">            The lineage. </param>
        public Property(string name, object value, PropertyDescriptor propertyDescriptor, object instance, string lineage)
        {
            Name = name;
            Value = value;
            PropertyDescriptor = propertyDescriptor;
            Instance = instance;
            Lineage = lineage;
        }

        /// <summary> Gets or sets the name. </summary>
        /// <value> The name. </value>
        public string Name { get; private set; }

        /// <summary> Gets or sets the value. </summary>
        /// <value> The value. </value>
        public object Value { get; set; }

        /// <summary> Gets or sets the information. </summary>
        /// <value> The information. </value>
        public PropertyDescriptor PropertyDescriptor { get; private set; }

        /// <summary> Gets or sets the lineage. </summary>
        /// <value> The lineage. </value>
        public string Lineage { get; set; }

        /// <summary> Gets or sets the instance. </summary>
        /// <value> The instance. </value>
        public object Instance { get; private set; }
    }
}