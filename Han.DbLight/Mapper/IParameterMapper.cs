
namespace Han.DbLight.Mapper
{
    using System.Data.Common;

    /// <summary>
    /// ����ӳ��
    /// </summary>
    public interface IParameterMapper
    {
        //
        // Parameters:
        //   command:
        //
        //   parameterValues:
        void AssignParameters(DbCommand command, object[] parameterValues);
    }
}