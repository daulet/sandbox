using System;
using System.Linq.Expressions;

// Taken from awesome Wintelect AzureStorage library: https://github.com/Wintellect/AzureStorage

namespace Echo.Sample.RecordingSample
{
    /// <summary>A helper struct that returns the string name for a type's property.</summary>
    /// <typeparam name="T">The type defining the property you wish to convert to a string.</typeparam>
    public struct PropertyName<T>
    {
        /// <summary>Returns a type's property name as a string.</summary>
        /// <param name="propertyExpression">An expression that returns the desired property.</param>
        /// <returns>The property name as a string.</returns>
        public String this[LambdaExpression propertyExpression]
        {
            get
            {
                Expression body = propertyExpression.Body;
                MemberExpression me = (body is UnaryExpression)
                    ? (MemberExpression)((UnaryExpression)body).Operand
                    : (MemberExpression)body;
                return me.Member.Name;
            }
        }

        /// <summary>Returns a type's property name as a string.</summary>
        /// <param name="propertyExpression">An expression that returns the desired property.</param>
        /// <returns>The property name as a string.</returns>
        public String this[Expression<Func<T, Object>> propertyExpression]
        {
            get
            {
                return this[(LambdaExpression)propertyExpression];
            }
        }

        /// <summary>Returns several types property names as a string collection.</summary>
        /// <param name="propertyExpressions">The expressions; each returns a desired property.</param>
        /// <returns>The property names as a string collection.</returns>
        public String[] this[params Expression<Func<T, Object>>[] propertyExpressions]
        {
            get
            {
                var propertyNames = new String[propertyExpressions.Length];
                for (Int32 i = 0; i < propertyNames.Length; i++) propertyNames[i] = this[(LambdaExpression)propertyExpressions[i]];
                return propertyNames;
            }
        }
    }
}
