using System;

namespace Dapper.TopHat.Query.Attributes
{
    public class IgnoreAttribute : Attribute
    {
        /// <summary>   Gets or sets the type of the ignore. </summary>
        ///
        /// <value> The type of the ignore. </value>
        public IgnoreType IgnoreType { get; set; }

        /// <summary>   Default constructor. By default both persistence and hydration are ignored </summary>
        public IgnoreAttribute():this(IgnoreType.Both){}

        /// <summary>   Specify which action is to be ignored (persistence or hydration) the default is both.</summary>
        ///
        /// <param name="ignoreType">   Type of the ignore. </param>
        public IgnoreAttribute(IgnoreType ignoreType)
        {
            IgnoreType = ignoreType;
        }
    }
}