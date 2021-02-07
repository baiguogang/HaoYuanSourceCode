using LMH.FileApi.Models;
using System.Configuration;
using System.IO;

namespace LMH.FileApi
{
    public class LoadConfig
    {
        public static  void init()
        {
            StartConfig.UploadAndDownPath = ConfigurationManager.AppSettings.Get("UploadAndDownPath");

            // 如果目录不存在则要先创建
            if (!Directory.Exists(StartConfig.UploadAndDownPath))
            {
                Directory.CreateDirectory(StartConfig.UploadAndDownPath);
            }
        }
    }
}
