using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Serialization
{
    public static class SerializationExtensions
    {
        public static TValue GetValue<TValue>(this SerializationInfo info, Expression<Func<TValue>> propertyExpression)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (!(propertyExpression.Body is MemberExpression property))
                throw new ArgumentException(
                    nameof(propertyExpression),
                    "Invalid property expression. Only member accessors are supported (e.g. () => Property)");

            return (TValue)info.GetValue(property.Member.Name, typeof(TValue));
        }

        public static void AddValue<TValue>(this SerializationInfo info, Expression<Func<TValue>> propertyExpression)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (!(propertyExpression.Body is MemberExpression property))
                throw new ArgumentException(nameof(propertyExpression), "Invalid property expression.");

            info.AddValue(property.Member.Name, propertyExpression.Compile()());
        }
    }
}
