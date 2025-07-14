namespace N_m3u8DL_Web.Config;

using Tomlyn;


public class Config
{

    public Web Web { get; set;}
    public Admin Admin { get; set; }
    public Storage Storage { get; set; }

    public Download Download { get; set; }


     static Config Init(string configFile)
    {
        try
        {
            var text = File.ReadAllText(configFile);
            return Tomlyn.Toml.ToModel<Config>(text);
        }catch(Exception ex) {
            throw new Exception($"无法解析配置文件: {configFile},{ex.Message}", ex);
        }

    }
    public static string GetConfigPath(string fileName)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }

   
    public static Config Parse(string configFile)
    {
        return Init(GetConfigPath(configFile));
    }
}

public class Web
{
    public string Name { get; set;}
    public string RootPath { get; set;}
    public string RunUrl { get; set; }

}

public class Admin
{

    public bool Auth { get; set; } = true;
    public string User { get; set; }
    public string Password { get; set; }

    public string Certificate { get; set; }
}

public class Storage
{
    public string Path { get; set; }

}

public class Download
{

    public int HttpRequestTimeout { get; set; }

    public bool UseSystemProxy { get; set;} = true;

    public int TaskQueueSize { get; set; } = 10;

    public int TaskWorkers { get; set; } = 5;

    public int TaskDownloadThread { get; set; } = 5;

    public int TaskLoadTimeout { get; set; } = 30;

}