using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Han.Infrastructure.Upload
{
    public class UploadConfig
    {
        /// <summary>
        /// 文件
        /// </summary>
        public HttpPostedFileBase File { get; set; }

        /// <summary>
        /// 文件编号
        /// </summary>
        public string FileGuid { get; set; }

        // public string FileClass { get; set; }

        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool AllowFileEmpty { get; set; } = false;

        /// <summary>
        /// 允许文件类型扩展名
        /// </summary>
        public string AllowFileExtension { get; set; }

        /// <summary>
        /// 允许文件类型扩展名分隔符
        /// </summary>
        public char AllowFileExtensionSplit { get; set; }

        /// <summary>
        /// 允许文件大小
        /// </summary>
        public double AllowMaxLength { get; set; } = 4194304;

        /// <summary>
        /// 文件保存路径--网站绝对路径  前面不加 '~'
        /// </summary>
        // public string FilePath { get; set; }

        /// <summary>
        /// 系统文件保存目录
        /// </summary>
        public string AppFilePath { get; set; }

        /// <summary>
        /// 日期路径
        /// </summary>
        public string DatePath { get; set; }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前块索引
        /// </summary>
        public long Chunk { get; set; }

        /// <summary>
        /// 块总数
        /// </summary>
        public long Chunks { get; set; }

        /// <summary>
        /// 块大小
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// 文件路径名称是否保留原文件名称
        /// </summary>
        public bool IsRetainName { get; set; } = true;
    }
}
