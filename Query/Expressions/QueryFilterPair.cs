using System.Collections.Generic;
using System.Text;

namespace Dapper.TopHat.Query.Expressions
{
    public class QueryBuilder
    {
        public QueryBuilder()
        {
            
        }

        public QueryBuilder(string sql)
        {
            Builder = new StringBuilder(sql);
        }

        public StringBuilder Builder { get; private set; } = new StringBuilder();

        public Criteron CurrentNode { get; private set; }

        public List<Criteron> Parameters { get; } = new List<Criteron>();

        public void AddParameter(string name, object value)
        {
            Parameters.Add(new Criteron(name, value));
        }

        public void SetNameForCurrentNode(string node)
        {
            CurrentNode.Key = node;
        }

        public void SetValueForCurrentNode(string node)
        {
            CurrentNode.Key = node;
        }

        public QueryBuilder ResetSql(string sql)
        {
            this.Builder = new StringBuilder();
            Builder.Append(sql);

            return this;
        }
    }

    public class Criteron
    {
        public Criteron() { }

        public Criteron(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public object Value { get; set; }
    } 
}
