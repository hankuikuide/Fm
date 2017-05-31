using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Han.DbLight.TableMetadata.External
{
    /// <summary>
    /// 组合属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompositeAttribute : Attribute
    {
    }
}
