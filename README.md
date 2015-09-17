# Jean Philippe Youtube Url Downloader (JeanPhilippe_Youtube-dl)
A Windows Runtime library, that allow you to get downloadable url of Youtube video link.

## Overview
JeanPhilippe_Youtube-dl is a library for Windows Runtime (WinRT), written in C#, that allow you to get downloadable url of Youtube video link.

## Target platforms

- Windows 8.1
- Windows Phone 8.1

## NuGet

[JeanPhilippe_Youtube-dl at NuGet](https://www.nuget.org/packages/JeanPhilippe_Youtube-dl/)

    Install-Package JeanPhilippe_Youtube-dl

## License

The JeanPhilippe_Youtube-dl URL-extraction code is licensed under the [MIT License](http://opensource.org/licenses/MIT)

## Example code

**How to get the downloadable Url of a video and  to download it**
```c#
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using System.Threading.Tasks;
using JeanPhilippe_Youtube-dl;

// Your Youtube video url
string Url = "insert video url";

//You need to create Youtube ApiKey
//Go to https://developers.google.com/youtube/v3/getting-started , follow the instructions
//And create your Youtube ApiKey 
string ApiKey = " your Youtube Apikey here";

string ApplicationName = "Name of your application";

//This function will return the video file when it'll finish to download it
private async Task<StorageFile> Video_Downloading(string videoUrl, string fileName)
{
  ProcessDownload process = new ProcessDownload(ApiKey, ApplicationName);
  var LinkList = await DownloadableLink.ObtainLinks(VideoUrl);
  VideoModels video = LinkList.First(s => s.Resolution == 360 && s.VideoQuality == VideoQuality.Mp4);

  StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName,
    CreationCollisionOption.ReplaceExisting);

  Uri source;
  Uri.TryCreate(video.DownloadableLink.Trim(), UriKind.RelativeOrAbsolute, out source);

  BackgroundDownloader downloader = new BackgroundDownloader();
  DownloadOperation operation = downloader.CreateDownload(source, file);

  await operation.StartAsync();

  return file;
}

 // The file that represents your video file downloaded
 var f = await Video_Downloading(Url, "MyVideo");
```
