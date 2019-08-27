using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer.Visitor
{
    public class CacheOptimizedExpressionVisitor : ExpressionVisitor
    {
        protected override Expression VisitConstant(ConstantExpression node)
        {
            return ConvertValueToTypedPropertyExpression(node.Value, node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var optimizeIsSuccses = false;
            BinaryExpression optimizedExpression = null;
            if (node.Method.DeclaringType == typeof(Enumerable))
            {

                switch (node.Method.Name)
                {
                    case "Any":
                        optimizeIsSuccses = TryOptimizeAny(node, out optimizedExpression);
                        break;
                    case "Contains":
                        optimizeIsSuccses = TryOptimizeContains(node, out optimizedExpression);
                        break;
                    case "All":
                        optimizeIsSuccses = TryOptimizeAll(node, out optimizedExpression);
                        break;
                }
            }
            return optimizeIsSuccses ? VisitBinary(optimizedExpression) : base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Type == typeof(bool))
            {
                var trueVal = ConvertValueToTypedPropertyExpression(true, typeof(bool));
                var binary = Expression.MakeBinary(ExpressionType.Equal, node, trueVal);
                return binary;

            }
            return base.VisitMember(node);
        }


        private bool TryOptimizeAny(MethodCallExpression node, out BinaryExpression expression)
        {
            if (TryGetCollectionFromMethod(node, out var values) && node.Arguments.Count == 2)
            {
                var lambda = (LambdaExpression)node.Arguments[1];
                var oldBinaryExpression = (BinaryExpression)lambda.Body;
                var lambdaParam = lambda.Parameters[0];
                var operation = oldBinaryExpression.NodeType;
                var selectorParameter = (MemberExpression)(oldBinaryExpression.Right == lambdaParam ?
                    oldBinaryExpression.Left : oldBinaryExpression.Right);
                var binaryExpression = MakeBinaryExpressionFromCollection(values, selectorParameter, operation, Expression.Or);
                expression = binaryExpression;
                return true;
            }
            else
            {
                expression = null;
                return false;
            }
        }

        private bool TryOptimizeContains(MethodCallExpression node, out BinaryExpression expression)
        {
            if (TryGetCollectionFromMethod(node, out var values) && node.Arguments.Count == 2)
            {
                var selector = (MemberExpression)node.Arguments[1];
                var binaryExpression = MakeBinaryExpressionFromCollection(values, selector, ExpressionType.Equal, Expression.Or);
                expression = binaryExpression;
                return true;
            }
            else
            {
                expression = null;
                return false;
            }
        }

        private bool TryOptimizeAll(MethodCallExpression node, out BinaryExpression expression)
        {
            if (TryGetCollectionFromMethod(node, out var values) && node.Arguments.Count == 2)
            {
                var lambda = (LambdaExpression)node.Arguments[1];
                var oldBinaryExpression = (BinaryExpression)lambda.Body;
                var lambdaParam = lambda.Parameters[0];
                var operation = oldBinaryExpression.NodeType;
                var selectorParameter = (MemberExpression)(oldBinaryExpression.Right == lambdaParam ?
                    oldBinaryExpression.Left : oldBinaryExpression.Right);
                var binaryExpression = MakeBinaryExpressionFromCollection(values, selectorParameter, operation, Expression.And);
                expression = binaryExpression;
                return true;
            }
            else
            {
                expression = null;
                return false;
            }
        }

        private Expression ConvertValueToTypedPropertyExpression(object value, Type type)
        {
            return Expression.Convert(Expression.Property(
                                        Expression.Constant(new { Value = value }), "Value"), type);
        }

        private bool TryGetCollectionFromMethod(MethodCallExpression node, out IEnumerable collection)
        {
            if (node.Arguments[0] is MemberExpression member && member.Expression.NodeType == ExpressionType.Constant)
            {

                var getValueslambda = Expression.Lambda(member);
                var getValuesMethod = getValueslambda.Compile();
                collection = (IEnumerable)getValuesMethod.DynamicInvoke();
                return true;
            }
            collection = null;
            return false;
        }

        private BinaryExpression MakeBinaryExpressionFromCollection(
            IEnumerable values,
            MemberExpression selector,
            ExpressionType operation,
            Func<Expression, Expression,
                BinaryExpression> binaryMethod)
        {


            BinaryExpression binaryExpression = null;
            foreach (var el in values)
            {
                var propExpression = Expression.Constant(el);
                var expr = Expression.MakeBinary(operation, propExpression, selector);
                if (binaryExpression == null)
                {
                    binaryExpression = expr;
                }
                else
                {
                    binaryExpression = binaryMethod(binaryExpression, expr);
                }
            }
            return binaryExpression;
        }
    }
}
