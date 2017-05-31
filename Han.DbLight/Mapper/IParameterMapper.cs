
namespace Han.DbLight.Mapper
{
    using System.Data.Common;

    /// <summary>
    /// ≤Œ ˝”≥…‰
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