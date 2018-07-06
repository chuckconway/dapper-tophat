using System;

namespace Dapper.TopHat.Query.Restrictions
{
    public class LikeRestriction<T> : IRestriction
    {
        private readonly Action<T> _left;
        private readonly string _right;

        /// <summary> Constructor. </summary>
        /// <param name="left">  The left. </param>
        /// <param name="right"> The right. </param>
        public LikeRestriction(Action<T> left, string right)
        {
            _left = left;
            _right = right;
        }

        /// <summary> Gets the sql. </summary>
        /// <returns> . </returns>
        public string Sql()
        {
            return string.Empty;
        }
    }
}
