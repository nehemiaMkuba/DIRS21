using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

public static class ExpressionUtilities
{
    public static object GetValueWithoutCompiling(Expression expression)
    {
        return GetValue(expression, false);
    }

    public static object GetValueWithCompiling(Expression expression)
    {
        return GetValue(expression, true);
    }

    private static object GetValue(Expression expression, bool allowCompile)
    {
        if (expression == null)return null;

        switch (expression)
        {
            case ConstantExpression constantExpression:
                return GetValue(constantExpression);
            case MemberExpression memberExpression:
                return GetValue(memberExpression, allowCompile);
            case MethodCallExpression methodCallExpression:
                return GetValue(methodCallExpression, allowCompile);
        }

        if (allowCompile)
        {
            return GetValueUsingCompile(expression);
        }

        throw new Exception("Couldn't evaluate Expression without compiling: " + expression);
    }

    private static object GetValue(ConstantExpression constantExpression)
    {
        return constantExpression.Value;
    }

    private static object GetValue(MemberExpression memberExpression, bool allowCompile)
    {
        object value = GetValue(memberExpression.Expression, allowCompile);

        MemberInfo member = memberExpression.Member;
        if (member is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(value);
        }

        if (member is PropertyInfo propertyInfo)
        {
            try
            {
                return propertyInfo.GetValue(value);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        throw new Exception("Unknown member type: " + member.GetType());
    }

    private static object GetValue(MethodCallExpression methodCallExpression, bool allowCompile)
    {
        object[] paras = GetArray(methodCallExpression.Arguments, true);
        object obj = GetValue(methodCallExpression.Object, allowCompile);

        try
        {
            return methodCallExpression.Method.Invoke(obj, paras);
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    private static object[] GetArray(IEnumerable<Expression> expressions, bool allowCompile)
    {
        List<object> list = new List<object>();
        foreach (Expression expression in expressions)
        {
            object value = GetValue(expression, allowCompile);
            list.Add(value);
        }

        return list.ToArray();
    }

    private static object GetValueUsingCompile(Expression expression)
    {
        LambdaExpression lambdaExpression = Expression.Lambda(expression);
        Delegate expressionDelegate = lambdaExpression.Compile();
        return expressionDelegate.DynamicInvoke();
    }

}
