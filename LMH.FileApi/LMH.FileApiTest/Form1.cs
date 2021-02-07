using System;
using System.IO;
using System.Windows.Forms;

namespace LMH.FileApiTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //文件上传
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FileUploadDownApi FileUpload = new FileUploadDownApi();

                FilesUploadApi mo = new FilesUploadApi();
                mo.Url = @"http://172.16.0.91:8012/api/UploadFiles";
                mo.ApiFileSavePath = @"测试\20210205";
                
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Multiselect = true;
                if (fileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                foreach (var it in fileDialog.FileNames)
                {
                    mo.LocalFiles.Add(it);
                }

                string msg = "";
                FileUpload.UploadFilesToApi(mo, out msg);

                MessageBox.Show(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("上传失败！\r\n "+ex);
            }
        }

        //文件下载
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FileUploadDownApi FileDown = new FileUploadDownApi();

                FilesDownApi mo = new FilesDownApi();
                mo.Url = @"http://172.16.0.91:8012/api/DownFiles";
                mo.ApiFileSavePath = @"测试\20210205\正则表达式深入浅出.pdf";
                mo.LocalFileSavePath = Application.StartupPath + "\\" + Path.GetFileName(mo.ApiFileSavePath);

                string msg = "";
                FileDown.DownFilesFromApi(mo, out msg, true);

                MessageBox.Show(msg);
            }
            catch (Exception)
            {
                MessageBox.Show("下载失败");
            }
        }

        //文件是否存在
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FileUploadDownApi FileDown = new FileUploadDownApi();

                FilesExistsApi mo = new FilesExistsApi();
                mo.Url = @"http://172.16.0.91:8012/api/ExistsFiles";
                mo.ApiFileSavePath = @"测试\20210205\正则表达式深入浅出.pdf";

                string msg = "";
                FileDown.ExistsFilesToApi(mo, out msg);

                MessageBox.Show(msg);
            }
            catch (Exception)
            {
                MessageBox.Show("判断文件是否存在失败");
            }
        }
    }
}
