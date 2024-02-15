using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

namespace Assist.Core.Helpers;

public static class ImageHelper
{
    public static Bitmap LoadFromResource(string resourcePath)
    {
        Uri resourceUri;
        if (!resourcePath.StartsWith("avares://"))
        {
            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            resourceUri = new Uri($"avares://{assemblyName}/{resourcePath.TrimStart('/')}");
        }
        else
        {
            resourceUri = new Uri(resourcePath);
        }
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
    
    public static IImage? LoadFromResourceSVG(string svgPath)
    {
        var svgData = SvgSource.Load(svgPath, new Uri("avares://Assist"));
        return new Image { Source = new SvgImage { Source = svgData } }.Source;
    }

    public static async Task<Bitmap?> LoadFromWeb(string resourcePath)
    {
        var uri = new Uri(resourcePath);
        using var httpClient = new HttpClient();
        try
        {
            var data = await httpClient.GetByteArrayAsync(uri);
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{uri}' : {ex.Message}");
            return null;
        }
    }
}