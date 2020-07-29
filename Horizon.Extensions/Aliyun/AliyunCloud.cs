using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Util;

namespace Horizon.Extensions.Aliyun
{
    public class AliyunCloud
    {
        private static OssClient _clinet;
        private static AliyunConfig _cAliyunConfig;
        public static void CreateInstance(AliyunConfig config)
        {
            if (config==null)
            {
                throw new Exception("misssing aliyun oss config file!!!");
            }
            _cAliyunConfig = config;

            if (_clinet==null)
            {
                _clinet = new OssClient(_cAliyunConfig.EndPoint, _cAliyunConfig.AccessKey, _cAliyunConfig.Secret);
            }
        }

        public static CloudFile UploadFile(byte[] uploadFileBytes,string fileName,out string pathurl)
        {
            var  model = new CloudFile();
            pathurl = "";
            //上传到阿里云
            using (Stream fileStream = new MemoryStream(uploadFileBytes))//转成Stream流
            {
                
                string md5 = OssUtils.ComputeContentMd5(fileStream, fileStream.Length);
                string today = DateTime.Now.ToString("yyyyMM");
                string FileName = fileStream + today + DateTime.Now.Day;//文件名=文件名+当前上传时间
                string FilePath = "Upload/" + today + "/" + FileName;//云文件保存路径

                try
                {
                    //初始化阿里云配置--外网Endpoint、访问ID、访问password

                    //将文件md5值赋值给meat头信息，服务器验证文件MD5
                    var objectMeta = new ObjectMetadata
                    {
                        ContentMd5 = md5,
                    };
                    //文件上传--空间名、文件保存路径、文件流、meta头信息(文件md5) //返回meta头信息(文件md5)
                  var  result = _clinet.PutObject("bucketName", FilePath, fileStream, objectMeta);
                  model.Code = (int)result.HttpStatusCode;


                    //返回给UEditor的插入编辑器的图片的src
                    //Result.Url = "http://bucketName.oss-cn-【外网Endpoint区域】.aliyuncs.com/" + FilePath;
                    pathurl = _cAliyunConfig.EndPoint + FilePath; 
                    //Result.State = UploadState.Success;
                }
                catch (Exception e)
                {
                    model.Code = 500;
                    model.Message = e.Message;
                }
            }
            return model;
        }

        public static List<ListInfo> GetAlkFile()
        {
            
            return  new List<ListInfo>();
        }
    }
}
