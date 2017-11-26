using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

// Taken from awesome Wintelect AzureStorage library: https://github.com/Wintellect/AzureStorage

namespace Echo.Sample.RecordingSample
{
    /// <summary>Used to perform an equals (EQ) or not equals (NE) comparison.</summary>
    public enum EqualOp
    {
        /// <summary>Equal to.</summary>
        EQ,
        /// <summary>Not equal to.</summary>
        NE
    }
    /// <summary>Used to perform an equals (EQ), not equals (NE), greater-than (GT), 
    /// greater-than-or-equals to (GE), less-than (LT), or less-than-or-equals to (LE) comparison.</summary>
    public enum CompareOp
    {
        /// <summary>Equal to.</summary>
        EQ = EqualOp.EQ,
        /// <summary>Not equal to.</summary>
        NE = EqualOp.NE,
        /// <summary>Greater than.</summary>
        GT,
        /// <summary>Greater than or equal to.</summary>
        GE,
        /// <summary>Less than.</summary>
        LT,
        /// <summary>Less than or equal to.</summary>
        LE
    }
    /// <summary>Used to perform an range comparison.</summary>
    public enum RangeOp
    {
        /// <summary>Equivalent of GE and LT.</summary>
        IncludeLowerExcludeUpper = (CompareOp.GE << 8) | CompareOp.LT,
        /// <summary>Equivalent of GE and LT.</summary>
        StartsWith = IncludeLowerExcludeUpper,
        /// <summary>Equivalent of GE and LE.</summary>
        IncludeLowerIncludeUpper = (CompareOp.GE << 8) | CompareOp.LE,
        /// <summary>Equivalent of GT and LT.</summary>
        ExcludeLowerExcludeUpper = (CompareOp.GT << 8) | CompareOp.LT,
        /// <summary>Equivalent of GE and LE.</summary>
        ExcludeLowerIncludeUpper = (CompareOp.GT << 8) | CompareOp.LE
    }

    /// <summary>Use this light-weight struct to help you create a table query filter string.</summary>
    /// <typeparam name="TEntity">The whose properties you want to include in the filter string.</typeparam>
    public struct TableFilterBuilder<TEntity>
    {
        private static PropertyName<TEntity> s_propName = new PropertyName<TEntity>();
        private readonly String m_filter;
        private TableFilterBuilder(String filter) { m_filter = filter; }
        /// <summary>The string to be assigned to TableQuery's FilterString property.</summary>
        public override String ToString() { return m_filter; }

        /// <summary>Implicitly converts a TableFilterBuilder to a String (for TableQuery's FilterString property).</summary>
        /// <param name="fe">The TableFilterBuilder to convert.</param>
        /// <returns>The string to be assigned to TableQuery's FilterString property.</returns>
        public static implicit operator String(TableFilterBuilder<TEntity> fe) { return fe.ToString(); }
        private String BuildPropertyExpr(LambdaExpression property, CompareOp compareOp, String value)
        {
            if (value == null) throw new ArgumentNullException("value");

            const String ops = "eqnegtgeltle";  // Must be in same order as CompareOps enum
            String expr = String.Format("({0} {1} {2})", s_propName[property], ops.Substring(2 * (Int32)compareOp, 2), value);
            return expr;
        }

        private TableFilterBuilder<TEntity> Conjunction(String expr, [CallerMemberName] String conjunction = null)
        {
            if (this.m_filter != null)
            { // If previous expression, apply conjunction
                conjunction = conjunction == "And" ? " and " : " or ";
                expr = new StringBuilder("(").Append(this.m_filter).Append(conjunction).Append(expr).Append(")").ToString();
            }
            return new TableFilterBuilder<TEntity>(expr);
        }

        private static String NextStringKey(String @string)
        {
            return @string.Substring(0, @string.Length - 1)
               + ((Char)(@string[@string.Length - 1] + 1)).ToString();
        }
        #region Value formatters
        private String Format(Boolean value) { return value ? "true" : "false"; }
        private String Format(DateTimeOffset value)
        {
            return String.Format("datetime'{0}'", value.UtcDateTime.ToString("o"));
        }
        private String Format(Guid value)
        {
            return String.Format("guid'{0}'", value.ToString());
        }
        private String Format(Byte[] value)
        {
            var sb = new StringBuilder(value.Length * 2);
            foreach (Byte b in value) sb.Append(b.ToString("x2"));
            return String.Format("X'{0}'", sb);
        }
        #endregion

        #region And Members
        /// <summary>Ands a TableFilterBuilder expression with another.</summary>
        /// <param name="other">The other TableFilterBuidler expression.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(TableFilterBuilder<TEntity> other)
        {
            return Conjunction(other);
        }

        /// <summary>Ands the current expression with a new Boolean expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Boolean>> property, EqualOp equalOp, Boolean value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new Boolean expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Boolean?>> property, EqualOp equalOp, Boolean value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new Int32 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Int32>> property, CompareOp compareOp, Int32 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ands the current expression with a new Int32 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Int32?>> property, CompareOp compareOp, Int32 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ands the current expression with a new Int64 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Int64>> property, CompareOp compareOp, Int64 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ands the current expression with a new Int64 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Int64?>> property, CompareOp compareOp, Int64 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ands the current expression with a new Double expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Double>> property, CompareOp compareOp, Double value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ands the current expression with a new Double expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Double?>> property, CompareOp compareOp, Double value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ands the current expression with a new DateTimeOffset expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, DateTimeOffset>> property, CompareOp compareOp, DateTimeOffset value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new DateTimeOffset expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, DateTimeOffset?>> property, CompareOp compareOp, DateTimeOffset value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new Guid expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Guid>> property, EqualOp equalOp, Guid value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new Guid expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Guid?>> property, EqualOp equalOp, Guid value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new byte array expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, Byte[]>> property, EqualOp equalOp, Byte[] value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ands the current expression with a new String expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, String>> property, CompareOp compareOp, String value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, "'" + value + "'"));
        }

        /// <summary>Ands the current expression with a new String expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="lowerValue">The lower value to compare the string property to.</param>
        /// <param name="rangeOp">The kind of comparison operation to perform.</param>
        /// <param name="upperValue">The upper value to compare the string property to.</param>
        /// <returns>The resulting expression.</returns>      
        public TableFilterBuilder<TEntity> And(Expression<Func<TEntity, String>> property, String lowerValue, RangeOp rangeOp = RangeOp.StartsWith, String upperValue = null)
        {
            upperValue = upperValue ?? NextStringKey(lowerValue);
            CompareOp lowerCompareOp = (CompareOp)((Int32)rangeOp >> 8);
            CompareOp upperCompareOp = (CompareOp)((Int32)rangeOp & 0xFF);
            return Conjunction(new TableFilterBuilder<TEntity>().And(property, lowerCompareOp, lowerValue).And(property, upperCompareOp, upperValue));
        }
        #endregion

        #region Or Members
        /// <summary>Ors a TableFilterBuilder expression with another.</summary>
        /// <param name="other">The other TableFilterBuidler expression.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(TableFilterBuilder<TEntity> other) { return Conjunction(other); }

        /// <summary>Ors the current expression with a new Boolean expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Boolean>> property, EqualOp equalOp, Boolean value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new Boolean expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Boolean?>> property, EqualOp equalOp, Boolean value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new Int32 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Int32>> property, CompareOp compareOp, Int32 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ors the current expression with a new Int32 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Int32?>> property, CompareOp compareOp, Int32 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ors the current expression with a new Int64 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Int64>> property, CompareOp compareOp, Int64 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ors the current expression with a new Int64 expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Int64?>> property, CompareOp compareOp, Int64 value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ors the current expression with a new Double expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Double>> property, CompareOp compareOp, Double value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ors the current expression with a new Double expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Double?>> property, CompareOp compareOp, Double value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, value.ToString()));
        }

        /// <summary>Ors the current expression with a new DateTimeOffset expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, DateTimeOffset>> property, CompareOp compareOp, DateTimeOffset value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new DateTimeOffset expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, DateTimeOffset?>> property, CompareOp compareOp, DateTimeOffset value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new Guid expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Guid>> property, EqualOp equalOp, Guid value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new Guid expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Guid?>> property, EqualOp equalOp, Guid value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new byte array expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="equalOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, Byte[]>> property, EqualOp equalOp, Byte[] value)
        {
            return Conjunction(BuildPropertyExpr(property, (CompareOp)equalOp, Format(value)));
        }

        /// <summary>Ors the current expression with a new String expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="compareOp">The kind of comparison operation to perform.</param>
        /// <param name="value">The value to compare the property to.</param>
        /// <returns>The resulting expression.</returns>
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, String>> property, CompareOp compareOp, String value)
        {
            return Conjunction(BuildPropertyExpr(property, compareOp, "'" + value + "'"));
        }

        /// <summary>Ors the current expression with a new String expression.</summary>
        /// <param name="property">A lambda expression identifying the type's property.</param>
        /// <param name="lowerValue">The lower value to compare the string property to.</param>
        /// <param name="rangeOp">The kind of comparison operation to perform.</param>
        /// <param name="upperValue">The upper value to compare the string property to.</param>
        /// <returns>The resulting expression.</returns>      
        public TableFilterBuilder<TEntity> Or(Expression<Func<TEntity, String>> property, String lowerValue, RangeOp rangeOp = RangeOp.StartsWith, String upperValue = null)
        {
            upperValue = upperValue ?? NextStringKey(lowerValue);
            CompareOp lowerCompareOp = (CompareOp)((Int32)rangeOp >> 8);
            CompareOp upperCompareOp = (CompareOp)((Int32)rangeOp & 0xFF);
            return Conjunction(new TableFilterBuilder<TEntity>().Or(property, lowerCompareOp, lowerValue).And(property, upperCompareOp, upperValue));
        }
        #endregion
    }
}
