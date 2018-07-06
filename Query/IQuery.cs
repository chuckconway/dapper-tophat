using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper.TopHat.Query.Restrictions;

namespace Dapper.TopHat.Query
{
    public interface IQuery<T> where T : class 
    {
        /// <summary>
        /// Queries the specified where.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        IQuery<T> Where(Expression<Func<T, bool>> @where);

        /// <summary>
        /// Queries the specified where.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        IQuery<T> Where(string @where);

        IQuery<T> And(Expression<Func<T, bool>> and);

        IQuery<T> And(string and);

        IQuery<T> Or(Expression<Func<T, bool>> or);

        IQuery<T> Or(params IRestriction[] restrictions);

        IQuery<T> Or(string or);

        IQuery<T> Like(Expression<Func<T, bool>> like);

        IQuery<T> Like(string like);

        IOrderBy<T> OrderBy(Expression<Func<T, object>> field);

        T First();

        Task<T> FirstAsync();

        T Single();

        Task<T> SingleAsync();

        T SingleOrDefault();

        Task<T> SingleOrDefaultAsync();

        T FirstOrDefault();

        Task<T> FirstOrDefaultAsync();

        IEnumerable<T> ToEnumerable();

        Task<IEnumerable<T>> ToEnumerableAsync();
    }
}
