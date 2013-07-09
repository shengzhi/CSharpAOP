using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace G.W.Y.IO.Helper
{
    public static class StreamHelper
    {
        public static string GetTextForChunked(byte[] bytes, int contentStartIndex, int contentLength, bool isGzip, Encoding coding)
        {
            string html = string.Empty;
            Stream contentStream = new MemoryStream();
            int lastEnterIndex = contentStartIndex;
            for (int i = contentStartIndex; i < contentLength + contentStartIndex; i++)
            {
                if (bytes[i] == 13 && bytes[i + 1] == 10)
                {
                    int chunkLength = int.Parse(coding.GetString(bytes, lastEnterIndex, i - lastEnterIndex).Trim(), System.Globalization.NumberStyles.AllowHexSpecifier);
                    if (isGzip)
                    {
                        if (chunkLength > 0)
                            contentStream.Write(bytes, i + 2, chunkLength);
                    }
                    else
                    {
                        html += coding.GetString(bytes, i + 2, chunkLength);
                    }
                    i = i + 4 + chunkLength;
                    if (i + 1 >= contentLength + contentStartIndex)
                    {
                        //此步骤很有必要，因之前的write操作已将流指向最后，如果要重新读取则必须重置
                        contentStream.Seek(0, SeekOrigin.Begin);
                        html = GetTextForGzip(contentStream, coding);
                    }
                    lastEnterIndex = i;
                }
            }
            return html;
        }

        public static string GetTextForGzip(Stream stream, Encoding coding)
        {
            GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
            StreamReader reader = new StreamReader(gzip, coding);
            string html = reader.ReadToEnd();
            gzip.Close();
            gzip.Dispose();
            return html;
        }
    }
}
