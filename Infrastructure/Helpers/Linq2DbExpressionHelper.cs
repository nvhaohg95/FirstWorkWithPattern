using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class Linq2DbExpressionHelper<TEntity, TUpdate>
    {
        public static Dictionary<Expression<Func<TEntity, object>>, Expression<Func<TEntity, object>>> GetUpdateExpressions(TUpdate updateModel)
        {
            var result = new Dictionary<Expression<Func<TEntity, object>>, Expression<Func<TEntity, object>>>();
            var props = typeof(TUpdate).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var value = prop.GetValue(updateModel);
                if (value == null) continue; // Skip nulls if needed

                // Build: x => x.PropName
                var param = Expression.Parameter(typeof(TEntity), "x");
                var memberAccess = Expression.Convert(Expression.Property(param, prop.Name), typeof(object));
                var keyExpr = Expression.Lambda<Func<TEntity, object>>(memberAccess, param);

                // Build: x => (object)value
                var constant = Expression.Constant(value, typeof(object));
                var valueExpr = Expression.Lambda<Func<TEntity, object>>(constant, param);

                result[keyExpr] = valueExpr;
            }

            return result;
        }
    }
}
