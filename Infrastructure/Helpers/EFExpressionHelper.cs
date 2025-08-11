using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Helpers
{
    public static class EFExpressionHelper<TEntity, TUpdate>
    {
        private static readonly ConcurrentDictionary<Type, LambdaExpression> _cache = new();

        public static Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> GetUpdateExpression(TUpdate updateModel)
        {
            var updateType = typeof(TUpdate);

            if (!_cache.TryGetValue(updateType, out var lambda))
            {
                var param = Expression.Parameter(typeof(SetPropertyCalls<TEntity>), "e");
                Expression body = param;

                foreach (var updateProp in updateType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var entityProp = typeof(TEntity).GetProperty(updateProp.Name);
                    if (entityProp == null || entityProp.PropertyType != updateProp.PropertyType)
                        continue;

                    // e => e.PropName
                    var entityParam = Expression.Parameter(typeof(TEntity), "x");
                    var propertyAccess = Expression.Property(entityParam, entityProp);
                    var propertySelector = Expression.Lambda(propertyAccess, entityParam);

                    // e => "constant value"
                    var value = updateProp.GetValue(updateModel);
                    var valueExpr = Expression.Constant(value, updateProp.PropertyType);
                    var valueSelector = Expression.Lambda(valueExpr, entityParam);

                    // e => e.SetProperty(x => x.PropName, x => value)
                    var setPropertyMethod = typeof(SetPropertyCalls<TEntity>)
                        .GetMethods()
                        .First(m => m.Name == "SetProperty" &&
                                    m.IsGenericMethod &&
                                    m.GetParameters().Length == 2);

                    var genericMethod = setPropertyMethod.MakeGenericMethod(updateProp.PropertyType);

                    body = Expression.Call(body, genericMethod, propertySelector, valueSelector);
                }

                lambda = Expression.Lambda<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>(body, param);
                _cache[updateType] = lambda;
            }

            return (Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>)lambda;
        }
    }
}