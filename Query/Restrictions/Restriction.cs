using System;

namespace Dapper.TopHat.Query.Restrictions
{
    public class Restriction
    {
        /// <summary> Likes. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="left">  The left. </param>
        /// <param name="right"> The right. </param>
        /// <returns> . </returns>
        public IRestriction Like<T>(Action<T> left, string right)
        {
            return new LikeRestriction<T>(left, right);
        }
    }
}
