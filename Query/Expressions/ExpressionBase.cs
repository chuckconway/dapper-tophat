using System;

namespace Dapper.TopHat.Query.Expressions
{
    public abstract class ExpressionBase 
    {
        /// <summary> Replace '= null' with IS NULL. </summary>
        /// <param name="val">         The value. </param>
        /// <param name="twoFromTail"> The two from tail. </param>
        /// <param name="state">       The state. </param>
        /// <returns> . </returns>
        protected object ReplaceEqualsNullWithIsNull(object val, int twoFromTail, QueryBuilder state)
        {
            if (val is string && Convert.ToString(val).Contains("null"))
            {
                //remove hanging equals sign
                state.Builder.Remove(twoFromTail, 2);
                val = "IS NULL";
            }

            return val;
        }

        /// <summary> Gets a value. </summary>
        /// <param name="state"> The state. </param>
        /// <param name="eval">  The eval. </param>
        /// <returns> The value. </returns>
        protected object GetRightHandValue(QueryBuilder state, Func<char, object> eval)
        {
            const int skipBlankSpace = 2;

            //Length is 1 based, index is 0 based. Then skip one more space for the extra space to see if this is on the right-side of an equal-sign.
            var twoFromTail = (state.Builder.Length > 2 ? state.Builder.Length - skipBlankSpace : 0);

            var c = state.Builder[twoFromTail];

            var val = eval(c);
            return ReplaceEqualsNullWithIsNull(val, twoFromTail, state);
        }
    }
}
