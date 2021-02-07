using LMH.FileApi.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LMH.FileApi.Controllers
{
    public class ExistsFilesController : ApiController
    {
        public HttpResponseMessage Get([FromUri] ExistsFiles par)
        {
            HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.BadRequest);

            try
            {
                if (par != null && par.PassWord.ToLower() == "qwer@147258369." && !string.IsNullOrEmpty(par.FileSavePath))
                {
                    string LocalPath = Path.Combine(StartConfig.UploadAndDownPath, par.FileSavePath);
                    if (File.Exists(LocalPath))
                    {
                        res = Request.CreateResponse(HttpStatusCode.OK, "文件存在");
                    }
                    else
                    {
                        res = Request.CreateResponse(HttpStatusCode.OK, "文件不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return res;
        }

        public class ExistsFiles
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
