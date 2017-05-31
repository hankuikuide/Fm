
namespace Han.DbLight
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Han.DbLight.External;

    public interface IWhereQueryTranslator
    {
        string Translate(Expression expression);

        List<Constraint> GetConstraints();
        IDictionary<string, object> DbParams { get; set; }

        bool IsUseAlias { get; set; }

        Table Table { get; set; }//如果增加关联需要多个表信息
        
        ColumnMapperStrategy ColumnMapperStrategy { get; set; }
    }
}