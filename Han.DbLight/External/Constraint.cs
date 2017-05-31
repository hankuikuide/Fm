namespace Han.DbLight.External
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;

    using Han.Infrastructure;

    public class Constraint
    {
        /// <summary>
        /// The query that this constraint is operating on
        /// </summary>
        public SqlQuery query;

        public Constraint() { }

        public int Test { get; set; }

      


        #region props

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        private string _tableName = String.Empty;

        private ConstraintType condition = ConstraintType.Where;
        private string parameterName;

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public ConstraintType Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        public string TableName
        {
            get { return this._tableName; }
            set { this._tableName = value; }
        }

        /// <summary>
        /// 右表达式属性链
        /// </summary>
        private List<string> rightProperyChian = new List<string>();
        /// <summary>
        /// 左表达式属性链，需要生成属性名称
        /// </summary>
        private List<string> leftProperyChian = new List<string>();
        public string Test1 { get; set; }
        public string TableAlias { get; set; }
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; }

        public string ProperyName { get; set; }
       

     

        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        /// <value>The comparison.</value>
        public Comparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        public object ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        /// <value>The start value.</value>
        public object StartValue { get; set; }

        /// <summary>
        /// Gets or sets the end value.
        /// </summary>
        /// <value>The end value.</value>
        public object EndValue { get; set; }

        /// <summary>
        /// Gets or sets the in values.
        /// </summary>
        /// <value>The in values.</value>
        public IEnumerable InValues { get; set; }

       
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName
        {
            get { return this.parameterName ?? this.ColumnName; }
            set { this.parameterName = value; }
        }

        /// <summary>
        /// Gets or sets the type of the db.
        /// </summary>
        /// <value>The type of the db.</value>
        public DbType DbType { get; set; }

        /// <summary>
        /// 右表达式属性链
        /// </summary>
        public List<string> RightProperyChian
        {
            get
            {
                return this.rightProperyChian;
            }
            set
            {
                this.rightProperyChian = value;
            }
        }

        /// <summary>
        /// 左表达式属性链，需要生成属性名称
        /// </summary>
        public List<string> LeftProperyChian
        {
            get
            {
                return this.leftProperyChian;
            }
            set
            {
                this.leftProperyChian = value;
            }
        }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        /// <param name="comp">The comp.</param>
        /// <returns></returns>
        public static string GetComparisonOperator(Comparison comp)
        {
            string sOut;
            switch (comp)
            {
                case Comparison.Blank:
                    sOut = SqlComparison.BLANK;
                    break;
                case Comparison.GreaterThan:
                    sOut = SqlComparison.GREATER;
                    break;
                case Comparison.GreaterOrEquals:
                    sOut = SqlComparison.GREATER_OR_EQUAL;
                    break;
                case Comparison.LessThan:
                    sOut = SqlComparison.LESS;
                    break;
                case Comparison.LessOrEquals:
                    sOut = SqlComparison.LESS_OR_EQUAL;
                    break;
                case Comparison.Like:
                    sOut = SqlComparison.LIKE;
                    break;
                case Comparison.NotEquals:
                    sOut = SqlComparison.NOT_EQUAL;
                    break;
                case Comparison.NotLike:
                    sOut = SqlComparison.NOT_LIKE;
                    break;
                case Comparison.Is:
                    sOut = SqlComparison.IS;
                    break;
                case Comparison.IsNot:
                    sOut = SqlComparison.IS_NOT;
                    break;
                case Comparison.OpenParentheses:
                    sOut = "(";
                    break;
                case Comparison.CloseParentheses:
                    sOut = ")";
                    break;
                case Comparison.In:
                    sOut = " IN ";
                    break;
                case Comparison.NotIn:
                    sOut = " NOT IN ";
                    break;
                case Comparison.StartsWith:
                    sOut = SqlComparison.LIKE;
                    break;
                case Comparison.EndsWith:
                    sOut = SqlComparison.LIKE;
                    break;
                default:
                    sOut = SqlComparison.EQUAL;
                    break;
            }
            return sOut;
        }

        #endregion


        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (obj is Constraint)
            {
                Constraint compareTo = (Constraint)obj;
                return compareTo.ParameterName.Matches(this.parameterName) &&
                       compareTo.ParameterValue.Equals(this.ParameterValue);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

     

      
    }
}