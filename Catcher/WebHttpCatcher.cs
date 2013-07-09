using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Specialized;
using System.Net.Cache;
using Basic.Catcher;

namespace G.W.Y.Catcher
{
    public class WebHttpCatcher : IWebDataCatcher,IWebImageCatcher
    {
        #region Field
        private string _cookieString;
        private int _maxtimeout;
        //private string addtionalCookie;
        private bool allowAutoRedirect;
        private CookieContainer cc;
        private Encoding encoder;
        private bool keepalive;
        private string requestCookie;
        private string responseHeaderData;
        private string responseuri;
        private string url;
        private WebProxy proxyObject;
        #endregion

        #region Property

        public bool AllowAutoRedirect
        {
            get
            {
                return this.allowAutoRedirect;
            }
            set
            {
                this.allowAutoRedirect = value;
            }
        }

        public string CookieString
        {
            get
            {
                Uri uri = new Uri(this.url);
                StringBuilder builder = new StringBuilder();
                CookieCollection cookies = this.cc.GetCookies(uri);
                if (cookies.Count == 0)
                {
                    return string.Empty;
                }
                foreach (Cookie cookie in cookies)
                {
                    builder.Append(cookie.ToString());
                    builder.Append(';');
                }
                builder.Remove(builder.Length - 1, 1);
                this._cookieString = builder.ToString();
                return this._cookieString;
            }
            set
            {
                this._cookieString = value;
            }
        }

        public Encoding Encoder
        {
            get
            {
                return this.encoder;
            }
            set
            {
                this.encoder = value;
            }
        }

        public bool KeepAlive
        {
            get
            {
                return this.keepalive;
            }
            set
            {
                this.keepalive = value;
            }
        }

        public int MaxTimeout
        {
            get
            {
                return this._maxtimeout;
            }
            set
            {
                this._maxtimeout = value;
            }
        }

        public string RequestCookie
        {
            get
            {
                return this.requestCookie;
            }
            set
            {
                this.requestCookie = value;
            }
        }

        public string ResponseHeaderData
        {
            get
            {
                return this.responseHeaderData;
            }
            set
            {
                this.responseHeaderData = value;
            }
        }

        public string Responseuri
        {
            get
            {
                return this.responseuri;
            }
            set
            {
                this.responseuri = value;
            }
        }

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        public WebProxy ProxyObject
        {
            get { return proxyObject; }
            set { proxyObject = value; }
        }
        #endregion

        #region ctor

        public WebHttpCatcher(Encoding _encoder)
        {
            this.cc = new CookieContainer();
            this._maxtimeout = 20 * 1000;
            this.encoder = _encoder;
            this.keepalive = true;
            this.allowAutoRedirect = true;
        }

        public WebHttpCatcher(string _url, Encoding _encoder)
            : this(_encoder)
        {
            this.url = _url;
        }

        public WebHttpCatcher(string _url, Encoding _encoder, WebProxy _proxyObject)
            : this(_url, _encoder)
        {
            this.proxyObject = _proxyObject;
        }

        #endregion

        private void Appendcookie(string url, HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(this.requestCookie))
            {
                Uri uri = new Uri(url);
                CookieCollection cookies = this.cc.GetCookies(uri);
                System.Text.RegularExpressions.Match match = null;
                string[] strArray = this.requestCookie.Split(new char[] { ';' });
                if (strArray.Length > 0)
                {
                    lock (cookies.SyncRoot)
                    {
                        string str = strArray[0];
                        if (cookies[str.Split(new char[] { '=' })[0]] == null)
                        {
                            foreach (string str2 in strArray)
                            {
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    match = Regex.Match(str2, "(?<name>[^=]+)=(?<value>[^=].+)");
                                    if (match.Success)
                                    {
                                        request.CookieContainer.Add(uri, new Cookie(match.Groups["name"].Value.Trim(), match.Groups["value"].Value.Trim()));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool CheckValidationResult(object obj, X509Certificate ceft, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }

        public void ClearCookie()
        {
            this.cc = new CookieContainer();
        }

        private HttpWebRequest CreatePostRequest(string url, string refer, byte[] bytes)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (this.proxyObject != null)
            {
                request.Proxy = this.proxyObject;
            }
            Uri uri = new Uri(url);
            request.Timeout = this._maxtimeout;
            request.AllowAutoRedirect = this.allowAutoRedirect;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = this.cc;
            this.Appendcookie(url, request);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("UA-CPU", "x86");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn");
            PropertyInfo property = typeof(WebHeaderCollection).GetProperty("InnerCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                NameValueCollection coolection = property.GetValue(request.Headers, null) as NameValueCollection;
                coolection["Host"] = uri.Host;
                coolection["Content-Length"] = bytes.Length.ToString();
            }
            request.Referer = refer;
            request.ContentLength = bytes.Length;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            if (!string.IsNullOrEmpty(this.requestCookie))
            {
                request.Headers.Add(HttpRequestHeader.Cookie, this.requestCookie);
            }

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            return request;
        }

        private HttpWebRequest CreateGetRequest(string url, string refer)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new NullReferenceException("url is null");
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (this.proxyObject != null)
            {
                request.Proxy = this.proxyObject;
            }
            request.Timeout = this._maxtimeout;
            request.CookieContainer = this.cc;
            this.Appendcookie(url, request);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Accept = "*/*";
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "deflate");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN");
            request.KeepAlive = true;
            if (!string.IsNullOrEmpty(refer))
            {
                request.Referer = refer;
            }
            if (!string.IsNullOrEmpty(this.requestCookie))
            {
                request.Headers.Add(HttpRequestHeader.Cookie, this.requestCookie);
            }
            request.AllowAutoRedirect = this.allowAutoRedirect;
            return request;
        }

        private void SetHeader(WebHeaderCollection header, string name, string value)
        {
            //Activator.CreateInstance(Type.GetType("ss"));
            PropertyInfo property = typeof(WebHeaderCollection).GetProperty("InnerCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                NameValueCollection coolection = property.GetValue(header, null) as NameValueCollection;
                coolection[name] = value;
            }
        }

        public Bitmap GetImg(string url, string refer)
        {
            HttpWebRequest request = this.CreateGetRequest(url, refer);
            Stream responseStream = null;
            Bitmap bitmap = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    responseStream = response.GetResponseStream();
                    if (response.Headers.Get("Content-Encoding") != null)
                    {
                        if (!response.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            responseStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        else
                        {
                            responseStream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                    }
                    else
                    {
                        responseStream = response.GetResponseStream();
                    }
                    if (responseStream != null)
                    {
                        bitmap = (Bitmap)Image.FromStream(responseStream);
                    }
                    if (!string.IsNullOrEmpty(this.responseHeaderData))
                    {
                        this.responseHeaderData = response.Headers.Get(this.responseHeaderData);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return bitmap;
        }

        //public string DownloadString(string refer)
        //{
        //    return this.DownloadString(this.url, refer);
        //}

        public string GetString(string urlstr, string refer)
        {
            HttpWebRequest request = this.CreateGetRequest(urlstr, refer);
            string str = string.Empty;
            StreamReader reader = null;
            Stream stream = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.Headers.Get("Content-Encoding") != null)
                    {
                        if (!response.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        else
                        {
                            stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        reader = new StreamReader(stream, this.encoder);
                    }
                    else
                    {
                        reader = new StreamReader(response.GetResponseStream(), this.encoder);
                    }
                    str = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(this.responseHeaderData))
                    {
                        this.responseHeaderData = response.Headers.Get(this.responseHeaderData);
                        this.responseuri = response.ResponseUri.AbsoluteUri;
                    }
                }
                return str;
            }
            catch (WebException exception)
            {
                if (exception.Status == WebExceptionStatus.Timeout)
                {
                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //public string GetString(string urlstr, string refer, bool checkheader)
        //{
        //    HttpWebRequest request = this.CreateGetRequest(urlstr, refer);
        //    string str = string.Empty;
        //    StreamReader reader = null;
        //    Stream stream = null;
        //    try
        //    {
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            if (response.Headers.Get("Content-Encoding") != null)
        //            {
        //                if (!response.ContentEncoding.ToLower().Contains("deflate"))
        //                {
        //                    stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
        //                }
        //                else
        //                {
        //                    stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
        //                }
        //                reader = new StreamReader(stream, this.encoder);
        //            }
        //            else
        //            {
        //                reader = new StreamReader(response.GetResponseStream(), this.encoder);
        //            }
        //            str = reader.ReadToEnd();
        //            if (checkheader && !string.IsNullOrEmpty(this.responseHeaderData))
        //            {
        //                this.responseHeaderData = response.Headers.Get(this.responseHeaderData);
        //                this.responseuri = response.ResponseUri.AbsoluteUri;
        //            }
        //        }
        //        return str;
        //    }
        //    catch (WebException exception)
        //    {
        //        if (exception.Status == WebExceptionStatus.Timeout)
        //        {
        //            return string.Empty;
        //        }
        //        return string.Empty;
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}

        //public string DownloadStringWithRePath(string rpath, string refer)
        //{
        //    if (string.IsNullOrEmpty(rpath))
        //    {
        //        return this.DownloadString(this.url, refer);
        //    }
        //    if (rpath[0] != '/')
        //    {
        //        rpath = '/' + rpath;
        //    }
        //    string urlstr = this.url + rpath;
        //    return this.DownloadString(urlstr, refer);
        //}

        //public string DownloadStringWithRePath(string rpath, string refer, bool checkheader)
        //{
        //    if (checkheader)
        //    {
        //        return this.DownloadStringWithRePath(rpath, refer);
        //    }
        //    if (string.IsNullOrEmpty(rpath))
        //    {
        //        return this.DownloadString(this.url, refer);
        //    }
        //    if (rpath[0] != '/')
        //    {
        //        rpath = '/' + rpath;
        //    }
        //    string urlstr = this.url + rpath;
        //    return this.DownloadString(urlstr, refer, false);
        //}

        public string Post(string url, string refer, string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            string str = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            Stream stream = null;
            try
            {
                using (response = (HttpWebResponse)this.CreatePostRequest(url, refer, bytes).GetResponse())
                {
                    if (response.Headers.Get("Content-Encoding") != null)
                    {
                        if (!response.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        else
                        {
                            stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        reader = new StreamReader(stream, this.encoder);
                    }
                    else
                    {
                        reader = new StreamReader(response.GetResponseStream(), this.encoder);
                    }
                    str = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(this.responseHeaderData))
                    {
                        this.responseHeaderData = response.Headers.Get(this.responseHeaderData);
                    }
                }
                return str;
            }
            catch (Exception ee)
            {
                File.AppendAllText("登入异常.txt", DateTime.Now.ToString() + ":" + ee.Message + ee.StackTrace);
                return string.Empty;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            // return str;
        }

        //public string PostByReUrl(string rpath, string refer, string data)
        //{
        //    if (string.IsNullOrEmpty(rpath))
        //    {
        //        return this.Post(this.url, refer, data);
        //    }
        //    if (rpath[0] != '/')
        //    {
        //        rpath = '/' + rpath;
        //    }
        //    string url = this.url + rpath;
        //    return this.Post(url, refer, data);
        //}

        private void SetHeaders(HttpWebRequest request, Uri uri, int length)
        {
            SetHeader(request.Headers, "(Request-Line)", string.Format("POST {0} HTTP/1.1", uri.PathAndQuery));
            SetHeader(request.Headers, "Accept", "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*");
            //SetHeader(request.Headers, "Referer", string.Format("http://{0}/app/member/", uri.Host));
            SetHeader(request.Headers, "Referer", string.Format("http://{0}/", uri.Host));
            SetHeader(request.Headers, "Accept-Language", "zh-CN");
            SetHeader(request.Headers, "Content-Type", "application/x-www-form-urlencoded");
            SetHeader(request.Headers, "Accept-Encoding", "gzip, deflate");
            SetHeader(request.Headers, "User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)");
            SetHeader(request.Headers, "Host", uri.Host);
            SetHeader(request.Headers, "Content-Length", length.ToString());
            SetHeader(request.Headers, "Connection", "Keep-Alive");
            SetHeader(request.Headers, "Cache-Control", "no-cache");
            //SetHeader(request.Headers, "Cookie", this.Cookies);
        }
    }
}
