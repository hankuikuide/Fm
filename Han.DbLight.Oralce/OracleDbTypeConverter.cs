#region copyright
// <copyright file="OracleDbTypeConverter.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-12-28</datecreated>
#endregion
namespace Han.DbLight.Oracle
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OracleDbTypeConverter : IDbTypeConverter
    {
        public virtual string ToDbString(object obj, Func<object, string> customConverter = null)
        {
            if (obj == null)
                return "NULL";
            Type type = obj.GetType();
            if (type == typeof(string))
            {
                string s = obj as string;
                return String.Format("'{0}'", s.Replace("'", "''"));
            }
            if (type == typeof(DateTime))
            {
                DateTime d = (DateTime)obj;
                return String.Format("to_date('{0}', 'dd/mm/yyyy hh24:mi')", d.ToString("dd/MM/yyyy HH:mm"));
            }
            if (type == typeof(bool))
            {
                bool v = (bool)obj;
                if (v) return "1";
                else return "0";
            }
            //
            if (type.IsEnum)
            {
                return Convert.ToInt32(obj).ToString();
            }
            return obj.ToString();
        }
        public virtual string FormatDbType(IDbDataParameter prm)
        {
            IDbDataParameter p = prm as IDbDataParameter;
            if (p.Value == null || p.Value == DBNull.Value)
                return "NULL";
            switch (p.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    string s = p.Value as string;
                    return String.Format("'{0}'", s.Replace("'", "''"));

                case DbType.Binary:
                    byte[] b = p.Value as byte[];
                    return HexString(b);

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                    DateTime d = (DateTime)p.Value;
                    return String.Format("to_date('{0}', 'dd/mm/yyyy hh24:mi')", d.ToString("dd/MM/yyyy HH:mm"));

                default:
                    return p.Value.ToString();
            }
        }

        protected string HexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
                sb.AppendFormat("{0:X2}", bytes[i]);
            return sb.ToString();
        }

        //http://www.devart.com/dotconnect/oracle/docs/DataTypeMapping.html
        //http://docs.oracle.com/html/B10961_01/features.htm#1024984
        //每个数据库提供程序有不同的映射方式，ms oracle client 与odp.oracle client不同
        //因此默认实现不自定义映射
        public virtual DbType ClrToDbType(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual Type DbTypeToClr(DbType dbType)
        {
            throw new NotImplementedException();
        }
        public virtual DbParameter GetDbParameter(DbCommand cmd, string name, object value)
        {
            // OracleParameter parameter=
            DbParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = name;

            if (value == null)
            {

                //ms oracle cilent 必须显示转换
                parameter.Value = DBNull.Value;
            }

            else
            {
                var type = value.GetType();
                if (type.IsEnum)
                {
                   // parameter.Value = (int)value;
                    parameter.Value = Convert.ToString((int)value, 16);
                }
                else if (type == typeof(bool))
                {
                    bool v = (bool)value;
                    if (v) parameter.Value = "1";
                    else parameter.Value = "0";
                }
                else
                {
                    parameter.Value = value;
                }

            }

            return parameter;
        }


        public void AddParam(DbCommand cmd, object value)
        {
            var name = string.Format(":{0}", cmd.Parameters.Count);
            DbParameter p = this.GetDbParameter(cmd, name, value);
            cmd.Parameters.Add(p);
        }

        public void AddParams(DbCommand cmd, IDictionary<string, object> dbParameters)
        {
            //dbtype 根据clr 类型推断
            //I think it was working earlier with ODT.NET without null-checks, but have not confirmed it. Apparently System.Data.OracleClient is dropping parameters with null-value.
            //http://stackoverflow.com/questions/5678905/ora-01008-with-all-variables-bound
            foreach (var kv in dbParameters)
            {
                DbParameter parameter = this.GetDbParameter(cmd, kv.Key, kv.Value);


                cmd.Parameters.Add(parameter);
            }
        }
        public IDictionary<string, object> ToDicParams(params object[] args)
        {
            IDictionary<string, object> dbs = new Dictionary<string, object>();
            int i = 0;
            foreach (object item in args)
            {
                dbs.Add(i.ToString(), item);
                i++;
            }
            return dbs;
        }
        public void AddParams(DbCommand cmd, params object[] args)
        {
            foreach (object item in args)
            {
                AddParam(cmd, item);
            }
        }
    }
}
