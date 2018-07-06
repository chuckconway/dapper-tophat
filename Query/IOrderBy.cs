namespace Dapper.TopHat.Query
{
    public interface IOrderBy<T> where T : class 
    {
        IQuery<T> Asc();

        IQuery<T> Desc();
    }
}
