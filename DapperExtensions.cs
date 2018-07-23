using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper.TopHat.Persistence;
using Dapper.TopHat.Query;

namespace Dapper.TopHat
{
    public static class DapperExtensions
    {
        public static TModel Save<TModel>(this DbConnection connection, TModel model, Expression<Func<TModel, bool>> @where = null, DbTransaction transaction = null) where TModel : class, new()
        {
            ValidateAttributes(model);

            IPersistence persistence = new Persistence.Persistence();
            var saveService = new SaveModelService(connection, transaction);
            var results = (where != null ? persistence.Persist(model, @where) : persistence.Persist<TModel>(model));

            if (where != null) // We know this is an update
            {
                saveService.Update(model, results);
            }
            else // We don't know if this is an update or not and must check the primary keys
            {
                saveService.Save(model, results);
            }

            return model;
        }

        public static async Task<TModel> SaveAsync<TModel>(this DbConnection connection, TModel model, Expression<Func<TModel, bool>> @where = null, DbTransaction transaction = null) where TModel : class, new()
        {
            ValidateAttributes(model);

            IPersistence persistence = new Persistence.Persistence();
            var saveService = new SaveModelService(connection, transaction);
            var results = (where != null ? persistence.Persist(model, @where) : persistence.Persist<TModel>(model));
            TModel saved;

            if (where != null) // We know this is an update
            {
                saved = await saveService.UpdateAsync(model, results);
            }
            else // We don't know if this is an update or not and must check the primary keys
            {
                saved = await saveService.SaveAsync(model, results);
            }

            return saved;
        }

        private static void ValidateAttributes<TModel>(TModel model) where TModel : class, new()
        {
            var attributes = model.GetType().GetProperties().Where(s => s.GetCustomAttributes<KeyAttribute>().Any())
                .Select(s => s).ToList();

            if (attributes.Count != 1)
            {
                throw new Exception("The save method can't be used with methods that don't have the [Key] on a property.");
            }

            var attribute = attributes.FirstOrDefault();

            if (attribute.PropertyType != typeof(int))
            {
                throw new Exception("Saving non int values as a primary key is not supported");
            }
        }

        public static IQuery<TModel> Where<TModel>(this DbConnection connection, Expression<Func<TModel, bool>> @where, DbTransaction transaction = null) where TModel : class, new ()
        {
            return new Query<TModel>(connection, new QueryWriter(), transaction).Where(@where);
        }

        public static IQuery<TModel> All<TModel>(this DbConnection connection, DbTransaction transaction = null) where TModel : class, new()
        {
            return new Query<TModel>(connection, new QueryWriter(), transaction);
        }
    }
}
