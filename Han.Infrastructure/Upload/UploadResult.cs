using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Han.Infrastructure.Upload
{
    public class UploadResult
    {
        /// <summary>
        /// 上传是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// 文件显示名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件Url--保存到数据库的路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件正常路径
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// 文件物理路径
        /// </summary>
        public string PhyPath { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string Type { get; set; }

        public UploadResultForUe GetResultForUe()
        {
            return new UploadResultForUe()
            {
                original = Original,
                name = Name,
                url = Url,
                size = Size.ToString(),
                state = Success ? "SUCCESS" : ErrMsg ?? "上传出错了",
                type = Type
            };
        }
    }
}
