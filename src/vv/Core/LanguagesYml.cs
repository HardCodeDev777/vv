using vv.CLI;
using vv.Utils;
using YamlDotNet.RepresentationModel;

namespace vv.Core;

internal static class LanguagesYml
{
    private const string DOWNLOAD_URL = 
        "https://raw.githubusercontent.com/github-linguist/linguist/refs/heads/main/lib/linguist/languages.yml";
    private static Color DefaultColorIfNotFound => Color.FromHex("#383c42");

    private static readonly HashSet<string> IgnoredExtraLangs = new()
    {
        "Markdown", "MSBuild", "YAML", "Visual Studio Solution",
        "Plain Text", "TOML", "XAML", "JSON", "XML", "SVG"
    };

    public static string LangsFilePath => 
        Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "languages.yml");

    public static Dictionary<LangData, string> GetLangsColors(List<LangData> langsDatas, bool ignoreExtra)
    {
        var yamlString = File.ReadAllText(LangsFilePath);

        if (string.IsNullOrEmpty(yamlString))
            throw new UserException($"Couldn't find {LangsFilePath}!");

        var yaml = new YamlStream();
        using var reader = new StringReader(yamlString);
        yaml.Load(reader);

        var root = (YamlMappingNode)yaml.Documents[0].RootNode;

        var result = new Dictionary<LangData, string>();

        foreach(var langData in langsDatas)
        {
            if (ignoreExtra && IgnoredExtraLangs.Contains(langData.Name)) continue;

            if (root.Children.ContainsKey(langData.Name))
            {
                var langNode = (YamlMappingNode)root.Children[langData.Name];
                var color = langNode.Children["color"].ToString();

                result.Add(langData, color);
            }
            else result.Add(langData, DefaultColorIfNotFound.ToHex());         
        }

        return result;
    }

    public static async Task UpdateLanguagesYmlFile()
    {
        if (File.Exists(LangsFilePath)) File.Delete(LangsFilePath);

        var newFileBytes = await HttpUtils.DownloadFileBytes(DOWNLOAD_URL);

        File.WriteAllBytes(LangsFilePath, newFileBytes);
    }
}