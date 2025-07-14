using N_m3u8DL_RE.Common.Entity;
using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Common.Log;
using N_m3u8DL_RE.Common.Resource;
using N_m3u8DL_RE.Common.Util;
using N_m3u8DL_RE.Export.Entity;
using N_m3u8DL_RE.Parser;
using N_m3u8DL_RE.Parser.Config;
using N_m3u8DL_RE.Parser.Util;
using N_m3u8DL_RE.Processor;
using Spectre.Console;
using System.Text;


namespace N_m3u8DL_RE.Export.Util
{
    public  class ExtractUtil
    {

        public static int GetOrder(StreamSpec streamSpec)
        {
            if (streamSpec.Channels == null) return 0;

            var str = streamSpec.Channels.Split('/')[0];
            return int.TryParse(str, out var order) ? order : 0;
        }
        public static async Task<M3u8Info> ExtractFromUrl(string url)
        {
            ParserConfig parserConfig = new();
            // demo1
            parserConfig.ContentProcessors.Insert(0, new DemoProcessor());
            // demo2
            parserConfig.KeyProcessors.Insert(0, new DemoProcessor2());
            // for www.nowehoryzonty.pl
            parserConfig.UrlProcessors.Insert(0, new NowehoryzontyUrlProcessor());

            // 流提取器配置
            var extractor = new StreamExtractor(parserConfig);
            // 从链接加载内容
            await RetryUtil.WebRequestRetryAsync(async () =>
            {
                await extractor.LoadSourceFromUrlAsync(url);
                return true;
            });
            // 解析流信息
            var streams = await extractor.ExtractStreamsAsync();


            // 全部媒体
            var lists = streams.OrderBy(p => p.MediaType).ThenByDescending(p => p.Bandwidth).ThenByDescending(GetOrder).ToList();
            // 基本流
            var basicStreams = lists.Where(x => x.MediaType is null or MediaType.VIDEO).ToList();
            // 可选音频轨道
            var audios = lists.Where(x => x.MediaType == MediaType.AUDIO).ToList();
            // 可选字幕轨道
            var subs = lists.Where(x => x.MediaType == MediaType.SUBTITLES).ToList();


            return new M3u8Info()
            {
                VideoStreams = basicStreams,
                AudioStreams = audios,
                SubtitleStreams = subs
            };

        }

        public static async  Task<EncryptInfo> ExtractEncryptInfoFromUrl(string m3u8Url,int retryCount)
        {

            string m3u8Content = Encoding.UTF8.GetString(await HTTPUtil.GetBytesAsync(m3u8Url));
            string keyLine = "";

            
            foreach(var line in m3u8Content.Split(new string[] { "\r\n", "\r", "\n" },StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("#EXT-X-KEY"))
                {
                    keyLine = line;
                    break;
                }
            }


            var iv = ParserUtil.GetAttribute(keyLine, "IV");
            var method = ParserUtil.GetAttribute(keyLine, "METHOD");
            var uri = ParserUtil.GetAttribute(keyLine, "URI");

            Logger.Debug("METHOD:{},URI:{},IV:{}", method, uri, iv);

            var encryptInfo = new EncryptInfo(method);

            // IV
            if (!string.IsNullOrEmpty(iv))
            {
                encryptInfo.IV = HexUtil.HexToBytes(iv);
            }


            // KEY
            try
            {
                if (uri.ToLower().StartsWith("base64:"))
                {
                    encryptInfo.Key = Convert.FromBase64String(uri[7..]);
                }
                else if (uri.ToLower().StartsWith("data:;base64,"))
                {
                    encryptInfo.Key = Convert.FromBase64String(uri[13..]);
                }
                else if (uri.ToLower().StartsWith("data:text/plain;base64,"))
                {
                    encryptInfo.Key = Convert.FromBase64String(uri[23..]);
                }
                else if (File.Exists(uri))
                {
                    encryptInfo.Key = File.ReadAllBytes(uri);
                }
                else if (!string.IsNullOrEmpty(uri))
                {
                    var segUrl = ParserUtil.CombineURL(m3u8Url, uri);
                getHttpKey:
                    try
                    {
                        var bytes = HTTPUtil.GetBytesAsync(segUrl).Result;
                        encryptInfo.Key = bytes;
                    }
                    catch (Exception _ex) when (!_ex.Message.Contains("scheme is not supported."))
                    {
                        Logger.WarnMarkUp($"[grey]{_ex.Message.EscapeMarkup()} retryCount: {retryCount}[/]");
                        Thread.Sleep(1000);
                        if (retryCount-- > 0) goto getHttpKey;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ResString.cmd_loadKeyFailed + ": " + ex.Message);
                encryptInfo.Method = EncryptMethod.UNKNOWN;
            }

            return encryptInfo;
        }


    }
}
