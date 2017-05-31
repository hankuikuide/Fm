
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using Han.Infrastructure;
    using Han.Infrastructure.Extensions;
    using Han.Infrastructure.Reflection;
    using Han.DbLight.External;
    using Han.DbLight.TableMetadata;

    using ExpressionVisitor = System.Linq.Expressions.ExpressionVisitor;

    /// <summary>
    /// todo 不同数据库不同
    /// </summary>
    public class WhereQueryTranslator : ExpressionVisitor, IWhereQueryTranslator
    {
       // private Constraint currentConstraint1;
        private StringBuilder sb;
        private string orderBy = string.Empty;
     
        private string whereClause = string.Empty;
        private IDictionary<string, object> dbParams = new Dictionary<string, object>();
        public List<Constraint> GetConstraints()
        {
            return binaryCons;
        }
        public Table Table { get; set; }
        private SqlLog sqlLog;
        //public WhereQueryTranslator()
        //{}
        public WhereQueryTranslator(DatabaseInfo databaseInfo)
        {
            this.databaseInfo = databaseInfo;
          
            sqlLog=new SqlLog(databaseInfo);
        }
        public IDictionary<string, object> DbParams
        {
            get { return this.dbParams; }
            set
            {
                dbParams = value;
            }
        }
        public ColumnMapperStrategy ColumnMapperStrategy { get; set; }
       
             /// <summary>
        /// 是否使用别名类型参数名作为别名
        /// </summary>
        public bool IsUseAlias { get; set; }
        public string WhereClause
        {
            get
            {
                return this.whereClause;
            }
        }

        private DatabaseInfo databaseInfo;
        private DbParameterCreater dbParameterCreater=new DbParameterCreater();
        public DatabaseInfo DatabaseInfo
        {
            get
            {
                return this.databaseInfo;
            }
            set
            {
                this.databaseInfo = value;
            }
        }

       

        public string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            this.whereClause = this.sb.ToString();
            return this.whereClause;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            
            //if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            //{
            //    this.Visit(m.Arguments[0]);
            //    LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
            //    this.Visit(lambda.Body);
            //    return m;
            //}

             Constraint currentConstraint = this.GetCurrent();//bug 第一次visitmethodcall时，为空conditionBuilder.AppendByAutoGen(null, (User u) => u.UserCode.Contains(userCode),ColumnMapperStrategy.ColumnAttribute,false);
            if (m.Method.Name == "Contains")
            {
                Expression nextExpression = m.Arguments[0];
                string contain=null;
                if(nextExpression is ConstantExpression)
                {
                    ConstantExpression sizeExpression = (ConstantExpression)nextExpression;
                    contain = sizeExpression.Value.ToString();
                }
               
                if (nextExpression is MemberExpression)
                {
                   contain= this.GetValue(nextExpression as MemberExpression) as string;
                }
               
               
                if (!string.IsNullOrEmpty(contain))
                {
                    //方法调用没有左表达式。方法对象设置为左
                    isLeft = true;
                    Visit(m.Object);
                    if (leftProperyChian.Count == 1)
                    {
                        string key = currentConstraint.TableAlias+ leftProperyChian[0];
                        sb.AppendFormat(" {0} like {1}",this.GetColumnName(leftProperyChian[0]), this.DatabaseInfo.SqlDialect.DbParameterConstant + key);
                        dbParams[key] = string.Format("%{0}%", contain);
                    }
                    leftProperyChian.Clear();
                    isLeft = false;
                    currentConstraint.ParameterValue = contain;
                    currentConstraint.Comparison = Comparison.Like;
                }
              
            }
            else
            {
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name)); 
            }
           return m;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            Constraint currentConstraint = this.GetCurrent();
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    currentConstraint.Comparison=Comparison.IsNot;
                    this.sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        private bool CanAppend(StringBuilder sb)
        {
            var s = sb.ToString();
            var l = s.LastOrDefault();
            if (s.Length != 0 && l != '(')
            {
                return true;
            }
            return false;
        }

        private List<Constraint> binaryCons = new List<Constraint>();
        private int beCount = 0;//binary 总数
        private int i = 0;
        private Constraint GetCurrent()
        {
            if (binaryCons.Count==0)
            {
                binaryCons.Add(new Constraint());
            }
            if (i == 0) i++;
            return binaryCons[i - 1];
        }

        private bool isLeft = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {

            Constraint cons=new Constraint();
            cons.Test = beCount;
            binaryCons.Add(cons);
            beCount++;
            i++;
           // this.sb.Append("(");
            isLeft = true;
            this.Visit(b.Left);
            //非属性路径，只有u.Name格式
            if (leftProperyChian.Count==1)
                sb.Append(this.GetColumnName(leftProperyChian[0]));
            Constraint currentConstraint=this.GetCurrent();
            if (this.CanAppend(this.sb))
            {
                
                switch (b.NodeType)
                {
                    case ExpressionType.And:
                        currentConstraint.Condition = ConstraintType.And;
                        this.sb.Append(" AND ");
                        break;

                    case ExpressionType.AndAlso:
                        currentConstraint.Condition = ConstraintType.And;
                        this.sb.Append(" AND ");
                        break;

                    case ExpressionType.Or:
                        currentConstraint.Condition = ConstraintType.Or;
                        this.sb.Append(" OR ");
                        break;

                    case ExpressionType.OrElse:
                        this.sb.Append(" OR ");
                        currentConstraint.Condition = ConstraintType.Or;
                        break;

                    case ExpressionType.Equal:
                        if (this.IsNullConstant(b.Right))
                        {
                            currentConstraint.Comparison = Comparison.Is;
                            this.sb.Append(" IS ");
                        }
                        else
                        {
                            this.sb.Append(" = ");
                            currentConstraint.Comparison = Comparison.Equals;
                        }
                        break;

                    case ExpressionType.NotEqual:
                        if (this.IsNullConstant(b.Right))
                        {
                            this.sb.Append(" IS NOT ");
                            currentConstraint.Comparison = Comparison.IsNot;
                        }
                        else
                        {
                            currentConstraint.Comparison = Comparison.NotEquals;
                            this.sb.Append(" <> ");
                        }
                        break;

                    case ExpressionType.LessThan:
                        this.sb.Append(" < ");
                        currentConstraint.Comparison = Comparison.LessThan;
                        break;

                    case ExpressionType.LessThanOrEqual:
                        this.sb.Append(" <= ");
                        currentConstraint.Comparison = Comparison.LessOrEquals;
                        break;

                    case ExpressionType.GreaterThan:
                        this.sb.Append(" > ");
                        currentConstraint.Comparison = Comparison.GreaterThan;
                        break;

                    case ExpressionType.GreaterThanOrEqual:
                        this.sb.Append(" >= ");
                        currentConstraint.Comparison = Comparison.GreaterOrEquals;
                        break;

                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

                }
            }

            isLeft = false;
            this.Visit(b.Right);
            i--;//解析完一个binary
           
            leftProperyChian.Clear();
            rightProperyChian.Clear();
            //解析表达式完成，处理空值sql
            //(PatientId = ..)
          //  this.sb.Append(")");
            string reg = @"\([^\(]*?\.\.\)";
            this.sb = new StringBuilder(Regex.Replace(this.sb.ToString(), reg, ""));
            return b;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            //if (node.Type == typeof(TSrc))
            //{
            //    return parameter;
            //}
            //else
            return base.VisitParameter(node);
        }
     
        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            Constraint currentConstraint =this.GetCurrent();
            string key =null;
            if (leftProperyChian.Count != 0)
            {
                key = this.GetKey();
            }
            else
            {
                throw new Exception("左边表达式不能为空");
            }
            if (q == null && c.Value == null)
            {
                dbParams[key] = null;
                this.sb.Append(this.DatabaseInfo.SqlDialect.DbParameterConstant+key);
            }
            else if (q == null)
            {
                currentConstraint.ParameterValue = c.Value;
               
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                   
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

                    default:
                        dbParams[key] = c.Value;
                        this.sb.Append(this.DatabaseInfo.SqlDialect.DbParameterConstant + key);
                        break;
                }
            }

            return c;
        }
        /// <summary>
        /// 根据属性名称获取列名
        /// </summary>
        /// <param name="properyName"></param>
        /// <returns></returns>
        private string GetColumnName(string properyName)
        {
            Constraint constraint = this.GetCurrent();
            if(constraint.TableAlias!=null)
            {
                return constraint.TableAlias+"."+Table.Columns.Where(col => col.PropertyName == properyName).First().ColumnName;
            }
            else
            {
                return Table.Columns.Where(col => col.PropertyName == properyName).First().ColumnName;
            }
         
           // return properyName;
        }
        private string GetKey()
        {
            string leftmemberPath = null;
            Constraint currentConstraint = this.GetCurrent();
            if (leftProperyChian.Count != 0)
            {
                for (int j = 0; j < leftProperyChian.Count; j++)
                {
                    leftmemberPath += leftProperyChian[j];

                }
                leftmemberPath = leftmemberPath.Replace(".", "");
                if (currentConstraint.TableAlias != null) return currentConstraint.TableAlias + leftmemberPath;
                else
                {
                    return leftmemberPath;
                }
            }
            else
            {
                throw new Exception();
            }
           
           
        }
        
        /// <summary>
        /// 右表达式属性链
        /// </summary>
        private  List<string> rightProperyChian=new List<string>();
        /// <summary>
        /// 左表达式属性链，需要生成属性名称
        /// </summary>
        private List<string> leftProperyChian = new List<string>();
        private object GetValue(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }
       
        private bool innerConstant = false;
        protected override Expression VisitMember(MemberExpression m)
        {
            Constraint constraint = this.GetCurrent();
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                string columnName = "";
                string properyName = "";
                properyName = m.Member.Name;
                if (this.ColumnMapperStrategy == ColumnMapperStrategy.ColumnAttribute)
                {
                    PropertyInfo propertyInfo = (PropertyInfo)m.Member;
                   
                    var colAttr = propertyInfo.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();
                    columnName = colAttr.ColumnName;
                }
                else
                {
                    columnName = m.Member.Name;
                }
                var parm = m.Expression as ParameterExpression;
                if (IsUseAlias&&parm != null)
                {
                    constraint.TableAlias = parm.Name;
                    columnName = parm.Name + "." + columnName;
                    //this.sb.Append(parm.Name + "." + columnName);
                }
                else
                {
                    //this.sb.Append(columnName);
                }
                if(isLeft)
                {
                    leftProperyChian.Insert(0, properyName);
                }
               
                else
                {
                    //properyChian
                }
                constraint.ColumnName = columnName;
                return m;
            }
            else if (m.Expression != null && m.Expression.NodeType == ExpressionType.MemberAccess)
            {

               // 处理属性值。处理堆栈.
                MemberExpression innerMember = (MemberExpression)m.Expression;

                if (isLeft)
                {
                   // this.sb.Append(m.Member.Name);
                    leftProperyChian.Insert(0, m.Member.Name);
                }
                else
                {
                    rightProperyChian.Insert(0, m.Member.Name);
                }
               
                constraint.Test1 += m.Member.Name;
                
                if(innerMember.Expression.NodeType==ExpressionType.Constant)
                {
                  // rightProperyChian.Insert(0, innerMember.Member.Name);

                    innerConstant = true;
                }
                Visit(innerMember);
                if(innerMember.Member is PropertyInfo)
                {
                  
                    //right属性路径已经通过filed查询到了最终结果，不处理中间属性值
                    if (isLeft)
                    {
                        string leftmemberPath = "";
                        if (leftProperyChian.Count != 0)
                        {
                            for (int j = 0; j < leftProperyChian.Count; j++)
                            {
                                leftmemberPath += "." + leftProperyChian[j];

                            }
                            leftmemberPath = leftmemberPath.Remove(0, 1);
                            sb.Append(leftmemberPath);
                        }
                    }
                   
                }
                else
                {
                    if(!isLeft)
                    {
                        object outerObj = GetValue(innerMember);
                        string memberPath = null;
                        string key = null;
                        if (rightProperyChian.Count != 0)
                        {
                            for (int j = 0; j < rightProperyChian.Count; j++)
                            {
                                if (j != 0)
                                {
                                    memberPath += "." + rightProperyChian[j];
                                }
                                key += rightProperyChian[j];

                            }
                        }
                        memberPath = memberPath.Remove(0, 1);
                        var value = Reflection.GetPropertyValueByPath(outerObj, memberPath);
                        this.dbParams[key] = value; //空值使用参数化查询
                        this.sb.Append(this.DatabaseInfo.SqlDialect.DbParameterConstant + key);
                    }
                    else
                    {
                        throw new Exception("左表达式不应该是filedinfo");
                    }
                   
                    
                    
                }
               

                return m;
            }
            else if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
               // object value=null;
                if (isLeft)
                {
                    throw new Exception("应该不支持此种表达式？？");
                   // leftProperyChian.Insert(0, m.Member.Name);

                }
                else
                {
                    //处理parameter.Order.UserInfo.Name。
                    rightProperyChian.Insert(0, m.Member.Name);
                    constraint.Test1 += m.Member.Name;
                    //结束parameter.Order.UserInfo.Name。
                    //加标记少计算express，提高性能
                    if( !innerConstant)
                    {
                        //下面 处理user.ID=userId//userId为filed，并且为基本类型
                        var value = GetValue(m);
                        if (value.GetType().IsPrimitive())
                        {
                            if (rightProperyChian.Count == 1)
                            {
                                string key = rightProperyChian[0];
                                this.dbParams[key] = value;
                                sb.Append(this.DatabaseInfo.SqlDialect.DbParameterConstant + key);
                            }
                            else
                            {
                                throw new Exception("解析错误");
                            }
                        }
                        
                    }
                    innerConstant = false;
                   
                }

              
                return m;
            }
            else
            {
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
            }
           
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {
            UnaryExpression unary = (UnaryExpression)expression.Arguments[1];
            LambdaExpression lambdaExpression = (LambdaExpression)unary.Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            MemberExpression body = lambdaExpression.Body as MemberExpression;
            if (body != null)
            {
                if (string.IsNullOrEmpty(this.orderBy))
                {
                    this.orderBy = string.Format("{0} {1}", body.Member.Name, order);
                }
                else
                {
                    this.orderBy = string.Format("{0}, {1} {2}", this.orderBy, body.Member.Name, order);
                }

                return true;
            }

            return false;
        }

    
      
    }
}