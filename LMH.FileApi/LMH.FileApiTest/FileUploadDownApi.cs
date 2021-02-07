using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LMH.FileApiTest
{
    public class FileUploadDownApi
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="mo">参数</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool UploadFilesToApi(FilesUploadApi mo, out string msg)
        {
            if (mo == null || mo == new FilesUploadApi())
            {
                msg = "上传失败：传入参数不能为空！";
                return false;
            }

            if (string.IsNullOrEmpty(mo.Url))
            {
                msg = "上传失败：Api地址不能为空！";
                return false;
            }

            if (string.IsNullOrEmpty(mo.ApiFileSavePath))
            {
                msg = "上传失败：服务器保存地址不能为空！";
                return false;
            }

            if (mo.LocalFiles.Count <= 0)
            {
                msg = "上传失败：未选择文件！";
                return false; ;
            }
            else
            {
                foreach (string it in mo.LocalFiles)
                {
                    if (!File.Exists(it))
                    {
                        msg = "上传失败：不存在文件（" + it + "）！";
                        return false;
                    }
                    if (FileIsUsed(it, out msg))
                    {
                        if (string.IsNullOrWhiteSpace(msg))
                        {
                            msg = "上传失败：文件被占用（" + it + "）！";
                        }

                        return false;
                    }
                }
            }

            try
            {
                string url = $@"{mo.Url}?PassWord=qwer@147258369.&FileSavePath={mo.ApiFileSavePath}";
                HttpClient httpclient = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                foreach (string it in mo.LocalFiles)
                {
                    string filename = Path.GetFileName(it);
                    string ex = Path.GetFileNameWithoutExtension(it);
                    StreamContent streamContent = new StreamContent(new FileStream(it, FileMode.Open, FileAccess.Read, FileShare.Read));
                    content.Add(streamContent, ex, filename);
                }
                HttpResponseMessage message = httpclient.PostAsync(url, content).Result;
                if (message.IsSuccessStatusCode)
                {
                    msg = "上传成功";
                    return true;
                }
                else
                {
                    msg = "上传失败";
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = "上传失败！\r\n " + ex;
                return false;
            }
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="mo">下载参数</param>
        /// <param name="msg">返回消息</param>
        /// <param name="isOpen">下载后是否需要打开</param>
        /// <returns></returns>
        public bool DownFilesFromApi(FilesDownApi mo, out string msg, bool isOpen = false)
        {
            if (string.IsNullOrEmpty(mo.Url))
            {
                msg = "下载失败：api地址不能为空！";

                return false;
            }

            if (string.IsNullOrEmpty(mo.LocalFileSavePath))
            {
                msg = "下载失败：未选择本地保存地址不能为空！";
                return false;
            }
            else
            {
                if (!Directory.Exists(mo.LocalFileSavePath))
                {
                    Directory.CreateDirectory(mo.LocalFileSavePath);
                }
            }

            string fileSavePath = System.Web.HttpUtility.UrlEncode(mo.ApiFileSavePath);
            string url = $@"{mo.Url}?PassWord=qwer@147258369.&FileSavePath={fileSavePath}";
            try
            {
                string fileName = Path.GetFileName(mo.ApiFileSavePath);
                string LocalfilePath = Path.Combine(mo.LocalFileSavePath, fileName);
                if (File.Exists(LocalfilePath))
                {
                    File.Delete(LocalfilePath);
                }
                FileStream fs = new FileStream(LocalfilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                Stream responseStream = response.GetResponseStream();

                byte [] bArr = new byte [1024];
                int iTotalSize = 0;
                int size = responseStream.Read(bArr, 0, bArr.Length);
                while (size > 0)
                {
                    iTotalSize += size;
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, bArr.Length);
                }
                fs.Close();
                responseStream.Close();

                if (isOpen)
                {
                    Task.Run(() => { System.Diagnostics.Process.Start(LocalfilePath); });
                }

                msg = "下载成功";

                return true;
            }
            catch (Exception ex)
            {
                msg = "下载失败\r\n" + ex;
                return false;
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool ExistsFilesToApi(FilesExistsApi mo, out string msg)
        {
            if (string.IsNullOrEmpty(mo.Url))
            {
                msg = "判断失败：api地址不能为空！";

                return false;
            }

            string fileSavePath = System.Web.HttpUtility.UrlEncode(mo.ApiFileSavePath);
            string url = $@"{mo.Url}?PassWord=qwer@147258369.&FileSavePath={fileSavePath}";
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    string strFileStatus = readStream.ReadToEnd();

                    if (strFileStatus.Contains("文件存在"))
                    {
                        msg = "文件存在！";
                        return true;
                    }
                    else
                    {
                        msg = "文件不存在！";
                        return false;
                    }
                }
                else
                {
                    msg = "其他错误！";
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = "文件判断异常！\r\n" + ex;
                return false;
            }
        }
        
        /// <summary>
        /// 返回指示文件是否已被其它程序使用的布尔值
        /// </summary>
        /// <param name="fileFullName">文件的完全限定名，例如：“C:\MyFile.txt”。</param>
        /// <returns>如果文件已被其它程序使用，则为 true；否则为 false。</returns>
        public static bool FileIsUsed(string fileFullName, out string _msg)
        {
            _msg = string.Empty;
            bool result = false;

            //判断文件是否存在，如果不存在，直接返回 false
            if (!File.Exists(fileFullName))
            {
                result = false;

            }
            else
            {
                //如果文件存在，则继续判断文件是否已被其它程序使用
                //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                FileStream fileStream = null;
                try
                {
                    fileStream = File.Open(fileFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                    result = false;
                }
                catch (IOException ex)
                {
                    _msg = ex.Message;
                    result = true;
                }
                catch (Exception ex)
                {
                    _msg = ex.Message;
                    result = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }

            //返回指示文件是否已被其它程序使用的值
            return result;
        }
    }

    /// <summary>
    /// api文件上传方法参数
    /// </summary>
    public class FilesUploadApi
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 文件在服务器保存路径(路径不带文件名，不带盘符名)
        /// </summary>
        public string ApiFileSavePath { get; set; } = string.Empty;

        /// <summary>
        /// 本地需要上传的文件完整路径
        /// </summary>
        public List<string> LocalFiles { get; set; } = new List<string>();
    }

    /// <summary>
    /// api文件下载参数
    /// </summary>
    public class FilesDownApi
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 文件在服务器保存路径(路径带文件名，不带盘符名)
        /// </summary>
        public string ApiFileSavePath { get; set; } = string.Empty;

        /// <summary>
        /// 下载后保存路径（路径不带文件名）
        /// </summary>
        public string LocalFileSavePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// api文件判断是否存在参数
    /// </summary>
    public class FilesExistsApi
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 文件在服务器保存路径(路径带文件名，不带盘符名)
        /// </summary>
        public string ApiFileSavePath { get; set; } = string.Empty;
    }
}
