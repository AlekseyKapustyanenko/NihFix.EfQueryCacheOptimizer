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
        private static Lazy<CacheOptimizedExpressionVisitor> _instance => new Lazy<CacheOptimizedExpressionVisitor>(() => new CacheOptimizedExpressionVisitor());

        internal static CacheOptimizedExpressionVisitor Innstance => _instance.Value;

        protected override Expression VisitBinary(BinaryExpression node)
        {
            bool expressionChanged = false;
            var leftExpression = TransformConstantToPropertyOrGetOriginalExpression(node.Left, ref expressionChanged);
            var rightExpression = TransformConstantToPropertyOrGetOriginalExpression(node.Right, ref expressionChanged);
            if (expressionChanged)
            {
                var newExpression = Expression.MakeBinary(node.NodeType, leftExpression, rightExpression);
                return base.VisitBinary(newExpression);
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Any" && node.Method.DeclaringType == typeof(Enumerable))
            {
                //MemberExpression
                if (node.Arguments[0] is MemberExpression member)
                {
                    if (member.Expression is ConstantExpression constExpr)
                    {
                        var getValueslambda = Expression.Lambda(member);
                        var getValuesMethod = getValueslambda.Compile();
                        var values = (IEnumerable)getValuesMethod.DynamicInvoke();
                        var oldBinaryExpression = ((BinaryExpression)((LambdaExpression)(node.Arguments[1])).Body);
                        var operation = oldBinaryExpression.NodeType;
                        var parameter = oldBinaryExpression.Right;


                        BinaryExpression binaryExpression=null;
                        foreach (var el in values)
                        {
                            var propExpression = ConvertValueToTypedPropertyExpression(el, el.GetType());
                            var expr = Expression.MakeBinary(operation, propExpression, parameter);
                            if (binaryExpression == null)
                            {
                                binaryExpression = expr;
                            }
                            else
                            {
                                binaryExpression = Expression.Or(binaryExpression, expr);
                            }
                        }
                        return base.VisitBinary(binaryExpression);
                    }
                }

            }
            return base.VisitMethodCall(node);
        }



        private Expression TransformConstantToPropertyOrGetOriginalExpression(Expression expression, ref bool isNotOriginal)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                isNotOriginal = true;
                var constantExpression = (ConstantExpression)expression;
                return ConvertValueToTypedPropertyExpression(constantExpression.Value, constantExpression.Type);
            }
            isNotOriginal = false;
            return expression;
        }


        private Expression ConvertValueToTypedPropertyExpression(object value, Type type)
        {
            if (type == typeof(int))
            {
                return Expression.Property(
                    Expression.Constant(new { Value = (int)value }), "Value");
            }
            else if (type == typeof(string))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (string)value }), "Value");
            }
            else if (type == typeof(Guid))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (Guid)value }), "Value");
            }
            else if (type == typeof(DateTime))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (DateTime)value }), "Value");
            }
            else if (type == typeof(float))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (float)value }), "Value");
            }
            else if (type == typeof(double))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (double)value }), "Value");
            }
            else if (type == typeof(bool))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (bool)value }), "Value");
            }
            else if (type == typeof(byte))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (byte)value }), "Value");
            }
            else if (type == typeof(decimal))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (decimal)value }), "Value");
            }
            else if (type == typeof(char))
            {
                return Expression.Property(
                                        Expression.Constant(new { Value = (char)value }), "Value");
            }
            throw new ArgumentException();
        }

    }
}
