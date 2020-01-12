/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using System.Net;
namespace IFramework.Net
{
    public class FileDownLoader_Http : FileDownLoader
    {
        private FileStream fileStream;
        private Stream stream;
        private HttpWebResponse response;
        private string tempFileExt = ".temp";
        /// ��ʱ�ļ�ȫ·��
        private string tempSaveFilePath;

        private long currentLength;
        public override long CurrentLength { get { return currentLength; } }
        public virtual long FileLength { get; private set; }
        public override float Progress
        {
            get
            {
                if (FileLength > 0)
                    return Math.Max(Math.Min(1, (float)currentLength / FileLength), 0);
                return 0;
            }
        }


        public FileDownLoader_Http(string url, string SaveDir) : base(url, SaveDir)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", SaveDir, FileNameWithoutExt, tempFileExt);
        }
        public override void DownLoad()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Method = "GET";
            if (File.Exists(tempSaveFilePath))
            {
                //��֮ǰ��������һ���֣���������
                fileStream = File.OpenWrite(tempSaveFilePath);
                currentLength = fileStream.Length;
                fileStream.Seek(currentLength, SeekOrigin.Current);
                //�������ص��ļ���ȡ����ʼλ��
                request.AddRange((int)currentLength);
            }
            else
            {
                //��һ������
                fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
                currentLength = 0;
            }

            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
           
            //�ܵ��ļ���С=��ǰ��Ҫ���ص�+�����ص�
            FileLength = response.ContentLength + currentLength;

            IsDownLoading = true;
            int lengthOnce;
            int bufferMaxLength = 1024 * 20;

            while (currentLength < FileLength)
            {
                byte[] buffer = new byte[bufferMaxLength];
                if (!stream.CanRead) break;
                //��д����
                lengthOnce = stream.Read(buffer, 0, buffer.Length);
                currentLength += lengthOnce;
                fileStream.Write(buffer, 0, lengthOnce);
            }
            IsDownLoading = false;
            response.Close();
            stream.Close();
            fileStream.Close();
            //��ʱ�ļ�תΪ���յ������ļ�
            if (File.Exists(SaveFilePath))
                File.Delete(SaveFilePath);
            File.Move(tempSaveFilePath, SaveFilePath);
            Compelete();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (response!=null) response.Close();
            if (stream!=null) stream.Close();
            if (fileStream!=null) fileStream.Close();
        }
    }
}