
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using Han.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DbParameterCreater
    {
        /// <summary>
        /// 产生参数key
        /// </summary>
        /// <returns></returns>
        public string GenerateKey<T>(Expression<Func<T>> expression)
        {
            if (expression.Body is MemberExpression)
            {
              //  Expression<Func<TResult>> memberExpression = expression as Expression<Func<T>>;
                //针对属性
                string propertyName = expression.GetPropertySymbol().Replace(".", ""); //防止重名回溯属性父对象
                return propertyName;

            }
            else
            {
                //对其他类型表达式获取key
                MemberExpression memberExpression = expression.GetRightMostMember();
                var memberName= memberExpression.ToPath();
                return memberName;
            }
        }
        public string GenerateKey(Expression expression)
        {
            MemberExpression memberExpression = expression.GetRightMostMember();
            var memberName = memberExpression.ToPath();
            return memberName;
        }
    }
}
