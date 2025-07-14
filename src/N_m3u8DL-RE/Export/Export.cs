using N_m3u8DL_RE.CommandLine;
using N_m3u8DL_RE.Common.Entity;
using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Common.Log;
using N_m3u8DL_RE.Common.Resource;
using N_m3u8DL_RE.Common.Util;
using N_m3u8DL_RE.Config;
using N_m3u8DL_RE.DownloadManager;
using N_m3u8DL_RE.Enum;
using N_m3u8DL_RE.Export.Entity;
using N_m3u8DL_RE.Parser;
using N_m3u8DL_RE.Parser.Config;
using N_m3u8DL_RE.Processor;
using N_m3u8DL_RE.Util;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_m3u8DL_RE.Export
{
    public  class Export
    {

        public static int GetOrder(StreamSpec streamSpec)
        {
            if (streamSpec.Channels == null) return 0;

            var str = streamSpec.Channels.Split('/')[0];
            return int.TryParse(str, out var order) ? order : 0;
        }

        private static async Task WriteRawFilesAsync(MyOption option, StreamExtractor extractor, string tmpDir)
        {
            // 写出json文件
            if (option.WriteMetaJson)
            {
                if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);
                Logger.Warn(ResString.writeJson);
                foreach (var item in extractor.RawFiles)
                {
                    var file = Path.Combine(tmpDir, item.Key);
                    if (!File.Exists(file)) await File.WriteAllTextAsync(file, item.Value, Encoding.UTF8);
                }
            }
        }



        public static async Task<M3u8DownloadInfo> DoWorkAsync(MyOption option)
        {


            HTTPUtil.AppHttpClient.Timeout = TimeSpan.FromSeconds(option.HttpRequestTimeout);

            if (option.UseSystemProxy == false)
            {
                HTTPUtil.HttpClientHandler.UseProxy = false;
            }

            if (option.CustomProxy != null)
            {
                HTTPUtil.HttpClientHandler.Proxy = option.CustomProxy;
                HTTPUtil.HttpClientHandler.UseProxy = true;
            }

            // 检查互斥的选项
            if (option is { MuxAfterDone: false, MuxImports.Count: > 0 })
            {
                throw new ArgumentException("MuxAfterDone disabled, MuxImports not allowed!");
            }

            //if (option.UseShakaPackager)
            //{
             //   option.DecryptionEngine = DecryptEngine.SHAKA_PACKAGER;
            //}

            // LivePipeMux开启时 LiveRealTimeMerge必须开启
            if (option is { LivePipeMux: true, LiveRealTimeMerge: false })
            {
                Logger.WarnMarkUp("LivePipeMux detected, forced enable LiveRealTimeMerge");
                option.LiveRealTimeMerge = true;
            }

            // 默认的headers
            var headers = new Dictionary<string, string>()
            {
                ["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36"
            };
            // 添加或替换用户输入的headers
            foreach (var item in option.Headers)
            {
                headers[item.Key] = item.Value;
                Logger.Extra($"User-Defined Header => {item.Key}: {item.Value}");
            }

            var parserConfig = new ParserConfig()
            {
                AppendUrlParams = option.AppendUrlParams,
                UrlProcessorArgs = option.UrlProcessorArgs,
                BaseUrl = option.BaseUrl!,
                Headers = headers,
                CustomMethod = option.CustomHLSMethod,
                CustomeKey = option.CustomHLSKey,
                CustomeIV = option.CustomHLSIv,
            };

            if (option.AllowHlsMultiExtMap)
            {
                parserConfig.CustomParserArgs.Add("AllowHlsMultiExtMap", "true");
            }

            // demo1
            parserConfig.ContentProcessors.Insert(0, new DemoProcessor());
            // demo2
            parserConfig.KeyProcessors.Insert(0, new DemoProcessor2());
            // for www.nowehoryzonty.pl
            parserConfig.UrlProcessors.Insert(0, new NowehoryzontyUrlProcessor());


            var url = option.Input;

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

            // 尝试从URL或文件读取文件名
            if (string.IsNullOrEmpty(option.SaveName))
            {
                option.SaveName = OtherUtil.GetFileNameFromInput(option.Input);
            }

            // 生成文件夹
            var tmpDir = Path.Combine(option.TmpDir ?? Environment.CurrentDirectory, $"{option.SaveName ?? DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}");
            // 记录文件
            extractor.RawFiles["meta.json"] = GlobalUtil.ConvertToJson(lists);
            // 写出文件
            await WriteRawFilesAsync(option, extractor, tmpDir);

            Logger.Info(ResString.streamsInfo, lists.Count, basicStreams.Count, audios.Count, subs.Count);

            foreach (var item in lists)
            {
                Logger.InfoMarkUp(item.ToString());
            }

            var selectedStreams = new List<StreamSpec>();
            if (option.DropVideoFilter != null || option.DropAudioFilter != null || option.DropSubtitleFilter != null)
            {
                basicStreams = FilterUtil.DoFilterDrop(basicStreams, option.DropVideoFilter);
                audios = FilterUtil.DoFilterDrop(audios, option.DropAudioFilter);
                subs = FilterUtil.DoFilterDrop(subs, option.DropSubtitleFilter);
                lists = basicStreams.Concat(audios).Concat(subs).ToList();
            }

            if (option.DropVideoFilter != null) Logger.Extra($"DropVideoFilter => {option.DropVideoFilter}");
            if (option.DropAudioFilter != null) Logger.Extra($"DropAudioFilter => {option.DropAudioFilter}");
            if (option.DropSubtitleFilter != null) Logger.Extra($"DropSubtitleFilter => {option.DropSubtitleFilter}");
            if (option.VideoFilter != null) Logger.Extra($"VideoFilter => {option.VideoFilter}");
            if (option.AudioFilter != null) Logger.Extra($"AudioFilter => {option.AudioFilter}");
            if (option.SubtitleFilter != null) Logger.Extra($"SubtitleFilter => {option.SubtitleFilter}");

            if (option.AutoSelect)
            {
                if (basicStreams.Count != 0)
                    selectedStreams.Add(basicStreams.First());
                var langs = audios.DistinctBy(a => a.Language).Select(a => a.Language);
                foreach (var lang in langs)
                {
                    selectedStreams.Add(audios.Where(a => a.Language == lang).OrderByDescending(a => a.Bandwidth).ThenByDescending(GetOrder).First());
                }
                selectedStreams.AddRange(subs);
            }
            else if (option.SubOnly)
            {
                selectedStreams.AddRange(subs);
            }
            else if (option.VideoFilter != null || option.AudioFilter != null || option.SubtitleFilter != null)
            {
                basicStreams = FilterUtil.DoFilterKeep(basicStreams, option.VideoFilter);
                audios = FilterUtil.DoFilterKeep(audios, option.AudioFilter);
                subs = FilterUtil.DoFilterKeep(subs, option.SubtitleFilter);
                selectedStreams = basicStreams.Concat(audios).Concat(subs).ToList();
            }
            else
            {
                // 展示交互式选择框
                selectedStreams = FilterUtil.SelectStreams(lists);
            }

            if (selectedStreams.Count == 0)
                throw new Exception(ResString.noStreamsToDownload);

            // HLS: 选中流中若有没加载出playlist的，加载playlist
            // DASH/MSS: 加载playlist (调用url预处理器)
            if (selectedStreams.Any(s => s.Playlist == null) || extractor.ExtractorType == ExtractorType.MPEG_DASH || extractor.ExtractorType == ExtractorType.MSS)
                await extractor.FetchPlayListAsync(selectedStreams);

            // 直播检测
            var livingFlag = selectedStreams.Any(s => s.Playlist?.IsLive == true) && !option.LivePerformAsVod;
            if (livingFlag)
            {
                Logger.WarnMarkUp($"[white on darkorange3_1]{ResString.liveFound}[/]");
            }

            // 无法识别的加密方式，自动开启二进制合并
            if (selectedStreams.Any(s => s.Playlist!.MediaParts.Any(p => p.MediaSegments.Any(m => m.EncryptInfo.Method == EncryptMethod.UNKNOWN))))
            {
                Logger.WarnMarkUp($"[darkorange3_1]{ResString.autoBinaryMerge3}[/]");
                option.BinaryMerge = true;
            }

            // 应用用户自定义的分片范围
            if (!livingFlag)
                FilterUtil.ApplyCustomRange(selectedStreams, option.CustomRange);

            // 应用用户自定义的广告分片关键字
            FilterUtil.CleanAd(selectedStreams, option.AdKeywords);

            // 记录文件
            extractor.RawFiles["meta_selected.json"] = GlobalUtil.ConvertToJson(selectedStreams);

            Logger.Info(ResString.selectedStream);
            foreach (var item in selectedStreams)
            {
                Logger.InfoMarkUp(item.ToString());
            }

            return new M3u8DownloadInfo()
            {
                Option= option,
                ParserConfig = parserConfig,
                StreamExtractor = extractor,
                SelectedStreams = selectedStreams,
                LivingFlag = livingFlag
            };
          

        }

        private static async Task DownloadM3U8(MyOption option, ParserConfig parserConfig, StreamExtractor extractor, string tmpDir, List<StreamSpec> selectedStreams, bool livingFlag)
        {
            Logger.InfoMarkUp(ResString.saveName + $"[deepskyblue1]{option.SaveName.EscapeMarkup()}[/]");

            // 开始MuxAfterDone后自动使用二进制版
            if (option is { BinaryMerge: false, MuxAfterDone: true })
            {
                option.BinaryMerge = true;
                Logger.WarnMarkUp($"[darkorange3_1]{ResString.autoBinaryMerge6}[/]");
            }

            // 下载配置
            var downloadConfig = new DownloaderConfig()
            {
                MyOptions = option,
                DirPrefix = tmpDir,
                Headers = parserConfig.Headers, // 使用命令行解析得到的Headers
            };

            var result = false;

            if (extractor.ExtractorType == ExtractorType.HTTP_LIVE)
            {
                var sldm = new HTTPLiveRecordManager(downloadConfig, selectedStreams, extractor);
                result = await sldm.StartRecordAsync();
            }
            else if (!livingFlag)
            {
                // 开始下载
                var sdm = new SimpleDownloadManager(downloadConfig, selectedStreams, extractor);
                result = await sdm.StartDownloadAsync();
            }
            else
            {
                var sldm = new SimpleLiveRecordManager2(downloadConfig, selectedStreams, extractor);
                result = await sldm.StartRecordAsync();
            }

            if (result)
            {
                Logger.InfoMarkUp("[white on green]Done[/]");
            }
            else
            {
                Logger.ErrorMarkUp("[white on red]Failed[/]");
                Environment.ExitCode = 1;
            }
        }
    }
}
