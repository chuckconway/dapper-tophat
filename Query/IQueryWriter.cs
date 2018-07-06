using System;
using Dapper.TopHat.Query.Expressions;
using Dapper.TopHat.Query.Filters;

namespace Dapper.TopHat.Query
{
    public interface IQueryWriter :  IDisposable 
    {

        /// <summary> Adds a filter to the 'query'. </summary>
        /// <param name="filter">    Type of the filter. </param>
        void AddFilter(IFilter filter);

        /// <summary> Adds an order to 'direction'. </summary>
        /// <param name="column">    The column. </param>
        /// <param name="direction"> The direction. </param>
        void AddOrder(string column, string direction);

        /// <summary> Query if this object contains filter type of where. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        bool ContainsFilterTypeOfWhere();

        ///// <summary> Gets the select. </summary>
        ///// <returns> . </returns>
        //string Query(object instance, string tableName);

        /// <summary> Gets the query. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <returns> . </returns>
        QueryBuilder Query<T>() where T: class, new();

        ///// <summary> Gets the query. </summary>
        ///// <typeparam name="T"> Generic type parameter. </typeparam>
        ///// <returns> . </returns>
        //string Query<T>(string[] columns) where T : class, new();

        ///// <summary> Gets the query. </summary>
        ///// <typeparam name="T"> Generic type parameter. </typeparam>
        ///// <returns> . </returns>
        //string Query<T>(object columns) where T : class, new();

        ///// <summary> Queries. </summary>
        ///// <param name="columns"> The columns. </param>
        ///// <param name="tableName">    The name. </param>
        ///// <returns> . </returns>
        //string Query(string[] columns, string tableName);

        /// <summary> Gets the columns. </summary>
        /// <param name="instance"> The instance. </param>
        /// <returns> The columns. </returns>
        string[] GetColumns(object instance);
    }
}
