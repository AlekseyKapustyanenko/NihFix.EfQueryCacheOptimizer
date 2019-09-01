using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace NihFix.EfQueryCacheOptimizer.Visitor
{
    public class CacheOptimizedExpressionVisitor : ExpressionVisitor
    {
        private readonly IOptimizationConfig _optimizationConfig;

        public CacheOptimizedExpressionVisitor(IOptimizationConfig optimizationConfig)
        {
            _optimizationConfig = optimizationConfig;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return ConvertValueToTypedPropertyExpression(node.Value, node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var optimizeIsSuccess = false;
            BinaryExpression optimizedExpression = null;
            if (node.Method.DeclaringType == typeof(Enumerable))
            {
                switch (node.Method.Name)
                {
                    case "Any":
                        optimizeIsSuccess = TryOptimizeAny(node, out optimizedExpression);
                        break;
                    case "Contains":
                        optimizeIsSuccess = TryOptimizeContains(node, out optimizedExpression);
                        break;
                    case "All":
                        optimizeIsSuccess = TryOptimizeAll(node, out optimizedExpression);
                        break;
                }
            }

            return optimizeIsSuccess ? VisitBinary(optimizedExpression) : base.VisitMethodCall(node);
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
                var lambda = (LambdaExpression) node.Arguments[1];
                var oldBinaryExpression = (BinaryExpression) lambda.Body;
                var lambdaParam = lambda.Parameters[0];
                var operation = oldBinaryExpression.NodeType;
                var selectorParameter = (MemberExpression) (oldBinaryExpression.Right == lambdaParam
                    ? oldBinaryExpression.Left
                    : oldBinaryExpression.Right);
                var binaryExpression =
                    MakeBinaryExpressionFromCollection(values, selectorParameter, operation, Expression.Or);
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
                var selector = (MemberExpression) node.Arguments[1];
                var binaryExpression =
                    MakeBinaryExpressionFromCollection(values, selector, ExpressionType.Equal, Expression.Or);
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
                var lambda = (LambdaExpression) node.Arguments[1];
                var oldBinaryExpression = (BinaryExpression) lambda.Body;
                var lambdaParam = lambda.Parameters[0];
                var operation = oldBinaryExpression.NodeType;
                var selectorParameter = (MemberExpression) (oldBinaryExpression.Right == lambdaParam
                    ? oldBinaryExpression.Left
                    : oldBinaryExpression.Right);
                var binaryExpression =
                    MakeBinaryExpressionFromCollection(values, selectorParameter, operation, Expression.And);
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
            if (type == typeof(int))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (int) value}), "Value");
            }
            else if (type == typeof(string))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (string) value}), "Value");
            }
            else if (type == typeof(Guid))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (Guid) value}), "Value");
            }
            else if (type == typeof(DateTime))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (DateTime) value}), "Value");
            }
            else if (type == typeof(float))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (float) value}), "Value");
            }
            else if (type == typeof(double))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (double) value}), "Value");
            }
            else if (type == typeof(bool))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (bool) value}), "Value");
            }
            else if (type == typeof(byte))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (byte) value}), "Value");
            }
            else if (type == typeof(decimal))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (decimal) value}), "Value");
            }
            else if (type == typeof(char))
            {
                return Expression.Property(
                    Expression.Constant(new {Value = (char) value}), "Value");
            }
            else
            {
                return Expression.Convert(Expression.Property(
                    Expression.Constant(new {Value = value}), "Value"), type);
            }
        }

        private bool TryGetCollectionFromMethod(MethodCallExpression node, out IEnumerable collection)
        {
            if (node.Arguments[0] is MemberExpression member && member.Expression.NodeType == ExpressionType.Constant)
            {
                var getValuesLambda = Expression.Lambda(member);
                var getValuesMethod = getValuesLambda.Compile();
                collection = GetOptimalEnumerable((IEnumerable) getValuesMethod.DynamicInvoke());
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
                binaryExpression = binaryExpression == null ? expr : binaryMethod(binaryExpression, expr);
            }

            return binaryExpression;
        }

        private IEnumerable GetOptimalEnumerable(IEnumerable enumerable)
        {
            var typedEnumerable = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            var enumerableLength = typedEnumerable.Length;
            if (enumerableLength >= _optimizationConfig.OptimalCollectionSize || enumerableLength == 0)
            {
                return typedEnumerable;
            }
            else
            {
                var additionalElementsCount = _optimizationConfig.OptimalCollectionSize - enumerableLength;
                var additionalElements = Enumerable.Repeat(typedEnumerable.First(), additionalElementsCount);
                return typedEnumerable.Concat(additionalElements);
            }
        }
    }
}