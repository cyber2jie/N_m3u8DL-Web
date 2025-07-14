using N_m3u8DL_RE.Common.Log;
using N_m3u8DL_RE.Common.Util;
using N_m3u8DL_RE.Util;
using System.Net.Http.Headers;

namespace N_m3u8DL_RE.Export.Util;

public  static class DownUtil
{
    private static readonly HttpClient AppHttpClient = HTTPUtil.AppHttpClient;


    public static async Task<byte[]> DownloadAsync(string url,CancellationTokenSource cancellationTokenSource, Dictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

        if (headers != null)
        {
            foreach (var item in headers)
            {
                request.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
        }
        Logger.Debug(request.Headers.ToString());
        try
        {
            
            using var response = await AppHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token);
            if (((int)response.StatusCode).ToString().StartsWith("30"))
            {
                HttpResponseHeaders respHeaders = response.Headers;
                Logger.Debug(respHeaders.ToString());
                if (respHeaders.Location != null)
                {
                    var redirectedUrl = "";
                    if (!respHeaders.Location.IsAbsoluteUri)
                    {
                        Uri uri1 = new Uri(url);
                        Uri uri2 = new Uri(uri1, respHeaders.Location);
                        redirectedUrl = uri2.ToString();
                    }
                    else
                    {
                        redirectedUrl = respHeaders.Location.AbsoluteUri;
                    }
                    return await DownloadAsync(redirectedUrl,cancellationTokenSource, headers);
                }
            }
            response.EnsureSuccessStatusCode();
            var contentLength = response.Content.Headers.ContentLength;

            using var stream = new MemoryStream();
            using var responseStream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token);
            var buffer = new byte[16 * 1024];
            var size = 0;

            size = await responseStream.ReadAsync(buffer, cancellationTokenSource.Token);
            await stream.WriteAsync(buffer.AsMemory(0, size));
            // 检测imageHeader
            bool imageHeader = ImageHeaderUtil.IsImageHeader(buffer);
            // 检测GZip（For DDP Audio）
            bool gZipHeader = buffer.Length > 2 && buffer[0] == 0x1f && buffer[1] == 0x8b;

            while ((size = await responseStream.ReadAsync(buffer, cancellationTokenSource.Token)) > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, size));
               
            }

            return stream.ToArray();
        }
        catch (OperationCanceledException oce) when (oce.CancellationToken == cancellationTokenSource.Token)
        {
           
            throw new Exception("Download speed too slow!");
        }
    }
}