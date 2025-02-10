using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

using TodoApi.Models;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filter<T> filter)
    {
        foreach (var condition in filter.Conditions)
        {
            string propertyName = condition.Key;
            var filters = condition.Value;

            PropertyInfo? property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            // TODO: Maybe raise an exception that the filter is invalid
            if (property == null) continue;

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression? finalExpression = null;

            foreach (var filterCondition in filters)
            {
                string operatorKey = filterCondition.Key;
                object? filterValue = filterCondition.Value;

                if (filterValue is JsonElement jsonElement)
                {
                    filterValue = ConvertJsonElement(jsonElement, property.PropertyType);
                }

                Expression propertyAccess = Expression.Property(parameter, property);
                Expression constant = Expression.Constant(Convert.ChangeType(filterValue, property.PropertyType));

                Expression? conditionExpression = operatorKey switch
                {
                    "eq" => Expression.Equal(propertyAccess, constant),
                    "gt" => Expression.GreaterThan(propertyAccess, constant),
                    "lt" => Expression.LessThan(propertyAccess, constant),
                    "like" => Expression.Call(propertyAccess, "Contains", Type.EmptyTypes, constant),
                    "in" => Expression.Call(Expression.Constant(filterValue), typeof(List<>).MakeGenericType(property.PropertyType).GetMethod("Contains")!, propertyAccess),
                    _ => null
                };

                if (conditionExpression != null)
                {
                    finalExpression = finalExpression == null
                        ? conditionExpression
                        : Expression.AndAlso(finalExpression, conditionExpression);
                }
            }

            if (finalExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
                query = query.Where(lambda);
            }
        }

        return query;
    }

    private static object ConvertJsonElement(JsonElement jsonElement, Type targetType)
    {
        return targetType switch
        {
            var t when t == typeof(int) => jsonElement.GetInt32(),
            var t when t == typeof(bool) => jsonElement.GetBoolean(),
            var t when t == typeof(DateTime) => jsonElement.GetDateTime(),
            var t when t == typeof(string) => jsonElement.GetString() ?? string.Empty,
            var t when t == typeof(double) => jsonElement.GetDouble(),
            _ => throw new InvalidOperationException($"Unsupported conversion type: {targetType.Name}")
        };
    }
}
