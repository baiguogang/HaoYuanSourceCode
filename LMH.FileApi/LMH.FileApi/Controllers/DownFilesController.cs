using LMH.FileApi.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace LMH.FileApi.Controllers
{
    public class DownFilesController : ApiController
    {
        public async Task<HttpResponseMessage> Get([FromUri] GetFilePara par)
        {
            try
            {
                if (par != null && par.PassWord.ToLower() == "qwer@147258369." && !string.IsNullOrEmpty(par.FileSavePath))
                {
                    string LocalPath = Path.Combine(StartConfig.UploadAndDownPath, par.FileSavePath);
                    if (File.Exists(LocalPath))
                    {
                        string filename = Path.GetFileName(LocalPath);
                        var stream = new FileStream(LocalPath, FileMode.Open);
                        HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StreamContent(stream)
                        };
                        resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = filename
                        };
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        resp.Content.Headers.ContentLength = stream.Length;
                        return await Task.FromResult(resp);
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public class GetFilePara
        {
            /// <summary>
            /// 密码
            /// </summary>
            public string PassWord { get; set; }

            /// <summary>
            /// 文件路径
            /// </summary>
            public string FileSavePath { get; set; }
        }
    }
}
