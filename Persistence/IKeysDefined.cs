
using System.Collections.Generic;

namespace Dapper.TopHat.Persistence
{
    public interface IKeysDefined
    {
        bool PrimaryKeysExist { get; }

        Persist GenerateSql(string name, object instance, IList<Query.Property> primaryKeys, IList<Query.Property> properties);
    }
}