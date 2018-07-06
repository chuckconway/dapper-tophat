using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using Dapper.TopHat.Query;

namespace Dapper.TopHat.Persistence
{
    public class SaveModelService
    {
        private readonly DbConnection _connection;

        public SaveModelService(DbConnection connection)
        {
            _connection = connection;
        }

        private void ExecuteUpdates(IEnumerable<Persist> results)
        {
            foreach (var result in results)
            {
                ExecuteUpdate(result);
            }
        }

        private void ExecuteUpdate(Persist persist)
        {
            var parameters = PopulateParameters(persist);
            _connection.ExecuteAsync(persist.Sql, parameters);
        }

        private static DynamicParameters PopulateParameters(Persist persist)
        {
            var parameters = new DynamicParameters();

            foreach (var p in persist.Parameters)
            {
                parameters.Add(p.Name, p.Value);
            }

            return parameters;
        }

        public TModel Update<TModel>(TModel model, IEnumerable<Persist> results) where TModel : class, new()
        {
            ExecuteUpdates(results);
            return model;
        }

        public TModel Save<TModel>(TModel model, IEnumerable<Persist> results) where TModel : class, new()
        {
            foreach (var persist in results)
            {
                if (persist.HasPrimaryKeysPresent)
                {
                    ExecuteUpdate(persist);
                }
                else
                {
                    InsertModel(model, persist);
                }
            }

            return model;
        }

        private void InsertModel<TModel>(TModel model, Persist persist) where TModel : class, new()
        {
            var parameters = PopulateParameters(persist);
            
            var sql = persist.Sql + "; " + "SELECT SCOPE_IDENTITY();";
            var id = _connection.ExecuteScalar(sql, parameters);

            SetPrimaryKey(model, id);
        }

        private static void SetPrimaryKey<TModel>(TModel model, object id) where TModel : class, new()
        {
            var properties = TypeDescriptor.GetProperties(model);
            var h = new Hydration();

            var keyProperty = properties
                .Cast<PropertyDescriptor>()
                .FirstOrDefault(property => property.GetAttribute<KeyAttribute>() != null);


            var p = new Property(keyProperty.Name, id, keyProperty, model, string.Empty);
            var value = h.ConvertToProperType(id, p);

            keyProperty.SetValue(model, value);
        }
    }
}
