
namespace Han.DbLight.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq.Expressions;

    using Han.Infrastructure;
    using Han.Infrastructure.Extensions;
    using Han.DbLight.Extensions;

    

    /// <summary>
    /// Dictionary≤Œ ˝”≥…‰£¨
    /// </summary>
    public class DictionaryParameterMapper : IParameterMapper
    {
        public DictionaryParameterMapper(IDbTypeConverter dbTypeConverter)
        {
            this.dbTypeConverter = dbTypeConverter;
        }

        private IDbTypeConverter dbTypeConverter;
        public void AssignParameters(DbCommand command, object[] parameterValues)
        {
            IDictionary<string,object> names = parameterValues[0] as Dictionary<string,object>;
            dbTypeConverter.AddParams(command,names);
        }
        
    }
    
   
    //public class PrimitiveObjectParameterMapper : IParameterMapper
    //{
        
    //    public void AssignParameters(DbCommand command, object[] parameterValues)
    //    {

    //       Func<object> f=  (Func<object>)parameterValues[0];
    //       var property = f.ToExpression();
    //       string propertyName = property.GetPropertySymbol().Replace(".", ""); 
    //       object value = property.Compile()();
    //       IDictionary<string, object> names =new Dictionary<string, object>();
    //        names[propertyName] = value;
    //        command.AddParams(names);
    //    }

    //}
}