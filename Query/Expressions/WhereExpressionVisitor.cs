using System;
using System.Linq.Expressions;

namespace Dapper.TopHat.Query.Expressions
{
    public class WhereExpressionVisitor : ExpressionVisitorBase<QueryBuilder>
    {
        private string paramenterName = null;
        private object parameterValue = null;

        /// <summary> Visit binary. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="node">    The node. </param>
        protected override void VisitBinary(Context context, BinaryExpression node)
        {

            context.State.Builder.Append("(");

            //Get Name
            var builder = Visit(node.Left, context.State);
            
            var binaryExpressions = new VisitBinaryExpressions();
            var s = binaryExpressions.Visit(node);
            builder.Builder.Append(s);

            //Get Value
            builder = Visit(node.Right, context.State);
            builder.Builder.Append(")");

            if (parameterValue != null)
            {
                builder.AddParameter(paramenterName, parameterValue);
            }

            paramenterName = null;
            parameterValue = null;
        }

        /// <summary> Visit unary. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="node">    The node. </param>
        protected override void VisitUnary(Context context, UnaryExpression node)
        {
            if(node.NodeType == ExpressionType.Not)
            {
                context.State.Builder.Append(" NOT ");
            }

            base.VisitUnary(context, node);
        }

        /// <summary> Visit member. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="node">    The node. </param>
        protected override void VisitMember(Context context, MemberExpression node)
        {
            var visitMember = new VisitMemberExpressions();
            var val = visitMember.Visit(context.State, node);

            if (string.IsNullOrEmpty(paramenterName))
            {
                paramenterName = Convert.ToString(val);
                context.State.Builder.Append(val);
            }
            else
            {
                parameterValue = val;
                context.State.Builder.Append($"@{paramenterName}");
            }
        }

        /// <summary> Visit constant. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="node">    The node. </param>
        protected override void VisitConstant(Context context, ConstantExpression node)
        {
            var visitConstant = new VisitConstantExpression();
            var val = visitConstant.Visit(context.State, node);
            context.State.Builder.Append(val);
        }
    }
}
