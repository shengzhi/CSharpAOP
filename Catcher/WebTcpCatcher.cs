using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using G.W.Y.IO.Logger;
using G.W.Y.IO.Helper;
using Basic.Catcher;

namespace G.W.Y.Catcher
{
    public class WebTcpCatcher : IWebDataCatcher
    {
        private readonly string CONTENTLENGTH = "Content-Length";
        private readonly string CHUNKED = "chunked";
        private readonly string GZIP = "gzip";

        public Encoding Coding { set; get; }
        public string CookiesStr { set; get; }
        private string UriStr { set; get; }
        private string ReferUrl { set; get; }
        public string ResponseHeader { set; get; }
        public FileAgileLogger logger { set; get; }

        private Uri currentUri;
        public Uri CurrentUri
        {
            get
            {
                if (this.currentUri == null)
                {
                    this.currentUri = new Uri(this.ReferUrl);
                }
                return this.currentUri;
            }
        }

        private byte[] dataByte = new byte[300000];
        public byte[] DataByte
        {
            get
            {
                return dataByte;
            }
        }
       

        /// <summary>
        /// 获取消息体
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="indexContentBegin"></param>
        /// <param name="contentLenght"></param>
        private void GetResponseBodyContent(NetworkStream networkStream, out int indexContentBegin, out int contentLenght)
        {
            int size = 2048, totalReadCount = 0, offset = 0;
            contentLenght = 0;

            this.GetResponseHeader(networkStream, out totalReadCount, out indexContentBegin);

            if (this.ResponseHeader.Contains(this.CONTENTLENGTH))//如果返回的消息头里有消息体的长度
            {
                string[] headers = this.ResponseHeader.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = headers.Length - 1; i < headers.Length; i--)
                {
                    if (headers[i].Contains(this.CONTENTLENGTH))
                    {
                        size = int.Parse(headers[i].Split(':')[1]);
                        contentLenght = size;
                        break;
                    }
                }
                lock (networkStream)
                {
                    while (offset + totalReadCount - indexContentBegin < size)
                    {
                        offset += networkStream.Read(this.DataByte, totalReadCount + offset, size - offset);
                    }
                }
            }
            else if (this.ResponseHeader.Contains(this.CHUNKED))
            {
                bool flag = true;
                int offsetTemp = 0;
                lock (networkStream)
                {
                    while (flag)
                    {
                        offsetTemp = networkStream.Read(this.DataByte, totalReadCount + offset, size);
                        if (offsetTemp > 0)
                        {
                            offset += offsetTemp;
                            contentLenght = offset;
                        }
                        for (int i = totalReadCount + offset - 100; i <= totalReadCount + offset; i++)
                        {
                            if (this.DataByte[i] == 13 && this.DataByte[i + 1] == 10 && this.DataByte[i + 2] == 13 && this.DataByte[i + 3] == 10)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取消息头
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="haveReadLength"></param>
        /// <param name="indexContentBegin"></param>
        private void GetResponseHeader(NetworkStream networkStream, out int haveReadLength, out int indexContentBegin)
        {
            int offset = 0, size = 2048;
            indexContentBegin = 0;
            lock (networkStream)
            {
                while (true)
                {
                    haveReadLength = networkStream.Read(this.DataByte, offset, size);
                    for (int i = offset; i < offset + haveReadLength; i++)//Find Response Header
                    {
                        if (this.DataByte[i] == 13 && this.DataByte[i + 1] == 10 && this.DataByte[i + 2] == 13 && this.DataByte[i + 3] == 10)
                        {
                            indexContentBegin = i + 4;
                            break;
                        }
                    }
                    if (haveReadLength > 0)
                    {
                        offset += haveReadLength;
                    }
                    else
                    {
                        break;
                    }
                    if (indexContentBegin != 0)
                    {
                        break;
                    }
                }
            }
            if (offset > 0)
            {
                haveReadLength = offset;
                if (indexContentBegin == 0)
                {
                    //To Write log
                }
                this.ResponseHeader = this.Coding.GetString(this.DataByte, 0, indexContentBegin - 1);
            }
        }

        private TcpClient CreatTcpClient()
        {
            TcpClient clientSocket = new TcpClient();
            clientSocket.Connect(this.CurrentUri.Host, this.CurrentUri.Port);

            string requestHeader = this.SetTcpClientHead().ToString();
            byte[] request = Encoding.UTF8.GetBytes(requestHeader);
            clientSocket.Client.Send(request);
            return clientSocket;
        }

        private StringBuilder SetTcpClientHead()
        {
            StringBuilder headers = new StringBuilder();

            headers.AppendFormat("{0} {1} HTTP/1.1\r\n", "GET", this.CurrentUri.PathAndQuery);
            headers.AppendFormat("Accept:image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*\r\n");
            headers.AppendFormat("Referer:{0}\r\n", this.ReferUrl);
            headers.AppendFormat("Accept-Language:zh-CN\r\n");
            headers.AppendFormat("Connection:Keep-Alive\r\n");
            headers.AppendFormat("Accept-Encoding:gzip, deflate\r\n");
            headers.AppendFormat("Host:{0}\r\n", this.CurrentUri.Host);
            headers.AppendFormat("Cookie:{0}\r\n", this.CookiesStr);

            headers.AppendFormat("User-Agent:Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; Media Center PC 6.0; InfoPath.2; Tablet PC 2.0;)\r\n\r\n");
            return headers;
        }

        public string GetString(string urlstr, string refer)
        {
            this.UriStr = urlstr;
            this.ReferUrl = refer;

            string strHTML = string.Empty;
            int indexContentBegin = 0;
            int bodyLength = 0;
            this.ResponseHeader = string.Empty;
            try
            {
                NetworkStream networkStream = this.CreatTcpClient().GetStream();
                this.GetResponseBodyContent(networkStream, out indexContentBegin, out bodyLength);
                //if use Chunked
                if (this.ResponseHeader.Contains(this.CHUNKED))
                {
                    strHTML = StreamHelper.GetTextForChunked(this.DataByte, indexContentBegin, bodyLength, this.ResponseHeader.Contains(this.GZIP), this.Coding);
                }
                //if use gzip
                if (this.ResponseHeader.Contains(this.GZIP) && !this.ResponseHeader.Contains(this.CHUNKED))
                {
                    Stream contentStream = new MemoryStream(this.DataByte, indexContentBegin, bodyLength);
                    strHTML = StreamHelper.GetTextForGzip(contentStream, this.Coding);
                }
                else if (!this.ResponseHeader.Contains(this.CHUNKED))
                {
                    strHTML = this.Coding.GetString(this.DataByte, indexContentBegin, bodyLength);
                }
                for (int i = 0; i < indexContentBegin + bodyLength + 10; i++)
                {
                    DataByte[i] = 0;
                }
            }
            catch (Exception ee)
            {
                if (this.logger != null)
                {
                    this.logger.LogSimple(ee, "Get PageData", ErrorLevel.Standard);
                }
            }
            return strHTML;
        }

        public string Post(string url, string refer, string data)
        {
            throw new NotImplementedException();
        }
    }
}
