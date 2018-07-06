namespace Dapper.TopHat.Persistence
{
    public class SqlResult
    {
        public string Sql { get; set; }

        public bool HasPrimaryKeysPresent { get; set; }
    }
}