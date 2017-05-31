using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Configuration;


namespace Han.Infrastructure.Upload
{
    /// <summary>
    /// 图片上传类  作者：苗建龙
    /// </summary>
    public class UploadHelper
    {
        private UploadConfig uploadConfig = null;
        private static string vriualRootPath = null;

        public UploadResult rel = new UploadResult();

        public string VriualRootPath
        {
            get
            {
                //if (vriualRootPath == null)
                //{
                //    //计算当前虚拟目录的相对路径  
                //    string absRootPath =HttpContext.Current.Request.ApplicationPath;//.Server.MapPath("/");
                //    vriualRootPath = HttpContext.Current.Server.MapPath("~/");

                //    vriualRootPath = vriualRootPath.Replace(absRootPath, "/");
                //    vriualRootPath = vriualRootPath.Replace("\\", "/");// \ 变成 /   
                //}
                var path = HttpContext.Current.Request.ApplicationPath + "/";
                return path.Replace("//", "/");
            }
        }

        public UploadHelper(UploadConfig uploadConfig)
        {
            this.uploadConfig = uploadConfig;
        }

        /// <summary> 
        /// 根据GUID获取16位的唯一字符串 
        /// </summary> 
        /// <param name=\"guid\"></param> 
        /// <returns></returns> 
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public static string GetVriualRootPath()
        {
            //计算当前虚拟目录的相对路径  
            string absRootPath = HttpContext.Current.Server.MapPath("/");
            string vriualRootPath = HttpContext.Current.Server.MapPath("~/");

            vriualRootPath = vriualRootPath.Replace(absRootPath, "/");
            vriualRootPath = vriualRootPath.Replace("\\", "/");// \ 变成 / 

            return vriualRootPath;
        }
        /// <summary>
        /// 上传
        /// </summary>
        public UploadResult UpLoad()
        {
            var config = this.uploadConfig;
            var file = config.File;

            if (config.AllowFileEmpty)
            {
                if (file.ContentLength == 0)
                {
                    rel.Success = true;
                    return rel;
                }
            }
            else
            {
                if (file.ContentLength == 0)
                {
                    file = null;
                    rel.ErrMsg = "文件不能为空。";
                    return rel;
                }
            }

            //验证大小
            double fileSize = file.ContentLength;
            if (fileSize > config.AllowMaxLength)
            {
                file = null;
                rel.ErrMsg = string.Format("不允许上传超过{0}M的文件", config.AllowMaxLength / 1048576);
                return rel;
            }

            //if (!CheckFileExtension(file) && config.Chunk == 0)
            //{
            //    file = null;
            //    rel.ErrMsg = "不允许上传此格式的文件";
            //    return rel;
            //}

            try
            {
                //var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                rel.Size = file.ContentLength;
                rel.Type = file.ContentType;
                //rel.Original = parsedContentDisposition.FileName.Replace("\"", string.Empty);
                rel.Original = file.FileName;

                //如果客户端给定了文件名，则直接赋值
                if (!string.IsNullOrEmpty(config.FileName))
                {
                    rel.Name = config.FileName;
                }
                else
                {
                    if (uploadConfig.IsRetainName)
                    {
                        rel.Name = (string.IsNullOrEmpty(config.FileGuid) ? GuidTo16String() : config.FileGuid) + "."
                            + rel.Original;
                    }
                    else
                    {
                        rel.Name = (string.IsNullOrEmpty(config.FileGuid) ? GuidTo16String() : config.FileGuid)
                            + "-0x0" + Path.GetExtension(rel.Original);
                    }
                }

                var subPath = Path.Combine(config.SavePath, config.DatePath ?? "");

                rel.Url = string.Format(VriualRootPath + "fmshared/file/GetFile?fileName={0}&outPutName={2}",
                   HttpUtility.UrlEncode(Path.Combine(config.SavePath, config.DatePath ?? "", rel.Name)),
                   config.SavePath,
                   HttpUtility.UrlEncode(rel.Original));

                rel.FileUrl = Path.Combine(VriualRootPath, uploadConfig.SavePath, uploadConfig.DatePath ?? "", rel.Name).Replace("\\", "/");
                
                rel.PhyPath = Path.Combine(config.AppFilePath, subPath);
                
                var rs = file.InputStream;
                // 计算写入文件的开始位置
                long startPosition = Convert.ToInt32(config.Chunk) * config.ChunkSize;
                // 定义一次接收字节长度
                int bytLen = 4096;
                byte[] nbytes = new byte[bytLen];
                int readSize = rs.Read(nbytes, 0, bytLen);

                // 如何路径不存在，就创建文件路径
                if (!Directory.Exists(rel.PhyPath))
                {
                    Directory.CreateDirectory(rel.PhyPath);
                }

                using (FileStream fs = File.OpenWrite(Path.Combine(rel.PhyPath, rel.Name)))
                {
                    if (fs.CanWrite)
                    {
                        fs.Seek(startPosition, SeekOrigin.Current);
                        while (readSize > 0)
                        {
                            fs.Write(nbytes, 0, readSize);
                            readSize = rs.Read(nbytes, 0, bytLen);
                        }
                        fs.Flush();
                        fs.Close();
                        rs.Dispose();
                        rs.Close();
                        nbytes = null;
                    }
                }
                rel.Success = true;
            }
            catch (Exception ex)
            {
                rel.ErrMsg = "上传异常";
            }
            finally
            {
                file = null;
            }
            return rel;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>string originalImagePath, string thumbnailPath,
        /// <param name="mode">生成缩略图的方式</param> 
        public void MakeThumbnail(int width, int height, string mode)
        {
            string dataStrPath = DateTime.Now.ToString("yyyy/MM/dd/");
            if (!string.IsNullOrEmpty(this.rel.PhyPath))
            {
                System.Drawing.Image originalImage = System.Drawing.Image.FromFile(Path.Combine(rel.PhyPath, rel.Name));
                int towidth = width;
                int toheight = height;

                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;

                switch (mode)
                {
                    case "HW"://指定高宽缩放（不变形）
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            if (originalImage.Width < towidth || originalImage.Height < toheight)
                            {// 低于最低宽度获高度   拉伸至指定宽高
                                //toheight = toheight * originalImage.Width / towidth;
                                break;
                            }
                            else
                            {
                                //toheight = toheight * width / towidth;
                                toheight = originalImage.Height * width / originalImage.Width;
                            }
                        }
                        else
                        {
                            if (originalImage.Width < towidth || originalImage.Height < toheight)
                            {// 低于最低宽度获高度   拉伸至指定宽高
                                //toheight = toheight * originalImage.Width / towidth;
                                break;
                            }
                            else
                            {
                                //towidth = towidth * height / towidth;
                                towidth = originalImage.Width * height / originalImage.Height;
                            }
                        }
                        break;
                    case "HWCut"://指定高宽缩放（可能变形）
                        break;
                    case "W"://指定宽，高按比例  
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形） 
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //新建一个bmp图片
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
                //新建一个画板
                Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);

                try
                {
                    string path_fileName = rel.Name.Replace("-0x0", string.Format("-{0}x{1}", width, height));
                    //以jpg格式保存缩略图
                    bitmap.Save(Path.Combine(rel.PhyPath, path_fileName), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
        }

        /// <summary>
        /// 检查文件类型
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool CheckFileExtension(HttpPostedFileBase file)
        {

            //转换成byte,读取图片MIME类型
            Stream stream;
            //int contentLength = file0.ContentLength; //文件长度
            byte[] fileByte = new byte[2];//contentLength，这里我们只读取文件长度的前两位用于判断就好了，这样速度比较快，剩下的也用不到。
            stream = file.InputStream;
            stream.Read(fileByte, 0, 2);//contentLength，还是取前两位
            //stream.Close();

            string fileFlag = "";
            if (fileByte != null && fileByte.Length > 0)//图片数据是否为空
            {
                fileFlag = fileByte[0].ToString() + fileByte[1].ToString();
            }
            string[] fileTypeStr = { "255216", "7173", "6677", "13780" };//对应的图片格式jpg,gif,bmp,png

            if (Enum.IsDefined(typeof(AllowFileExtension), int.Parse(fileFlag)))
            {
                return true;
            }
            //System.IO.File.Delete(filePath);

            return false;

            //var bx = string.Empty;
            //byte buffer;
            //using (var fs = file.InputStream)
            //{
            //    var r = new BinaryReader(fs);
            //    buffer = r.ReadByte();
            //    bx = buffer.ToString();
            //    buffer = r.ReadByte();
            //    bx += buffer.ToString();
            //    r.Close();
            //}

            //if (Enum.IsDefined(typeof(AllowFileExtension), int.Parse(bx)))
            //{
            //    return true;
            //}
            ////System.IO.File.Delete(filePath);

            //return false;
        }

        public void DelFile(string filePath)
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// 允许的文件扩展名
    /// </summary>
    public enum AllowFileExtension
    {
        JPG = 255216,
        GIF = 7173,
        BMP = 6677,
        PNG = 13780,
        RAR = 8297,
        DOCX = 8075,
        PSD = 5666,
        XLSX = 208207,
        PDF = 3780
    }
}
