using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Han.Infrastructure.Upload
{
    public class UploadResultForUe
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string original { get; set; }
        
        /// <summary>
        /// 文件显示名
        /// </summary>
        public string name { get; set; }
        
        /// <summary>
        /// 文件url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 上传状态 SUCCESS:成功  其他为错误消息
        /// </summary>
        public string state { get; set; }
    }
}
