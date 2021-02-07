using LMH.FileApi.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace LMH.FileApi.Controllers
{
    public class UploadFilesController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> UploadFileAsync([FromUri] UploadFilePara uppara)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                if (uppara == null || string.IsNullOrEmpty(uppara.FileSavePath) || string.IsNullOrEmpty(uppara.PassWord) || uppara.PassWord.ToLower() != "qwer@147258369.")
                {
                    return new BadRequestErrorMessageResult("传入参数错误！", this);
                }

                string fileSavetmpLocation = HttpContext.Current.Server.MapPath("~/Resources/tmp/" + Guid.NewGuid());
                if (!Directory.Exists(fileSavetmpLocation))
                {
                    Directory.CreateDirectory(fileSavetmpLocation);
                }

                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSavetmpLocation);
                await Request.Content.ReadAsMultipartAsync(provider);

                string fileSaveLocation = Path.Combine(StartConfig.UploadAndDownPath, uppara.FileSavePath);
                foreach (MultipartFileData file in provider.FileData)
                {
                    DirectoryInfo directory = new DirectoryInfo(fileSaveLocation);
                    if (!directory.Exists)
                    {
                        directory.Create();
                    }
                    FileInfo Fi = new FileInfo(file.LocalFileName);
                    File.Copy(file.LocalFileName, Path.Combine(directory.FullName, Fi.Name), true);
                }
                Directory.Delete(fileSavetmpLocation, true);
                return Ok("Success:文件上传成功！");
            }
            catch (Exception exp)
            {
                return InternalServerError(exp);
            }
        }
    }

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        string sPath = string.Empty;
        public CustomMultipartFormDataStreamProvider(string path) : base(path)
        {
            sPath = path;
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileFullPath = Path.Combine(sPath, headers.ContentDisposition.FileName.Replace("\"", ""));
            FileInfo Fi = new FileInfo(fileFullPath);
            if (!Fi.Directory.Exists)
            {
                Fi.Directory.Create();
            }
            return fileFullPath;
        }
    }

    /// <summary>
    /// 文件上传接口参数
    /// </summary>
    public class UploadFilePara
    {
        public string FileSavePath { get; set; } = string.Empty;

        public string PassWord { get; set; } = string.Empty;
    }
}
