[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![Nuget](https://img.shields.io/nuget/v/Owin.ThumbCrop.svg)](https://www.nuget.org/packages/Owin.ThumbCrop/)

# Owin.ThumbCrop

is a middleware for create thumb based in your local or storage webapp images

### Installation

Install from Nuget

```powershell
Install-Package Owin.ThumbCrop
```

Install from dotnet core CLI

```powershell
dotnet add package Owin.ThumbCrop
```

---
### How to use

Just put the `UrlPattern` at the end of your image URL and pass the transformation parameters

```
~/images/cat.jpg.thumb.axd?width=200&height=250&refpoint=TopLeft&crop&vignette
```
##### Original:
![Original](https://raw.githubusercontent.com/lucasteles/Owin.ThumbCrop/master/images/cat.jpg)

##### Thumb:
![Thumb](https://raw.githubusercontent.com/lucasteles/Owin.ThumbCrop/master/images/cat_thumb.png)|


---
### Configurations

Just add the middleware in owin pipeline, ***before*** `UseStaticFiles` and `UseMvc`

```cs
  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseThumbCrop();

            app.UseStaticFiles();
            app.UseMvc(routes => {...});
        }
```

Its possible to pass a configurations to the `UseThumbMiddleware` method as a instance of `ThumbCropConfig` or on an overload which receives an `Action<ThumbCropConfig>`

```cs
          app.UseThumbCrop(config => {
                config.UseCache = true;
                config.CacheExpireTime = TimeSpan.FromDays(1);
            });
```

The configuration options are

| Name            | Type               | Description                                                    | Default                |
|-----------------|--------------------|----------------------------------------------------------------|------------------------|
| DefaultWidth    | int                | Default thumb width                                            | 100                    |
| DefaultHeight   | int                | Default thumb height                                           | 100                    |
| UseCache        | bool               | Defines if will use cache                                      | false                  |
| UrlPattern      | string             | The pattern in end of URL to trigger the image transformations | ".thumb.axd"           |
| CacheExpireTime | Timespan           | Time for the cache expires                                     | 1 hour                 |
| NotFoundFile    | string             | File to replace a not known request URL                        | null                   |
| ImageSources    | IEnumerable<IImageSource\>| Defines the source of the image files                   | `LocalFileImageSource` |
| CacheManager    | IThumbCacheManager | Cache manager                                                  | `InMemoryCacheManager` |

---
### Thumb Options

Thats options which can be used in the query string

| Name       | Type           | Values                                                                                                             | Description                                      |
|------------|----------------|--------------------------------------------------------------------------------------------------------------------|--------------------------------------------------|
| width      | int            |                                                                                                                    | Defines the width                                |
| height     | int            |                                                                                                                    | Defines the height                               |
| crop       | bool           |                                                                                                                    | Defines if will crop on resize                   |
| refpoint   | ReferencePoint | TopLeft,  TopCenter,  TopRight,  MiddleLeft,  MiddleCenter,  MiddleRight,  BottomLeft,  BottomCenter,  BottomRight | Defines the reference point when resize and crop |
| flip       | FlipType       | Horizontal, Vertical                                                                                               | Flip the image based on parameter                |
| opacity    | int            | 0 ... 100                                                                                                          | Set the image opacity                            |
| rotate     | int            | 0 ... 360                                                                                                          | Rotate image                                     |
| hue        | int            |                                                                                                                    | Set image hue                                    |
| oil        | int            |                                                                                                                    | Apply Oil Filter                                 |
| pixelate   | int            |                                                                                                                    | Apply pixelate filter                            |
| contrast   | int            |                                                                                                                    | Apply contrast                                   |
| brightness | int            |                                                                                                                    | Apply brightness                                 |
| blur       | bool           |                                                                                                                    | Apply blur                                       |
| sharpening | bool           |                                                                                                                    | Apply sharpening                                 |
| invert     | bool           |                                                                                                                    | Invert image colors                              |
| glow       | bool           |                                                                                                                    | Apply Glow                                       |
| BlackWhite | bool           |                                                                                                                    | Set image as black and white only                |
| Grayscale  | bool           |                                                                                                                    | Set image as grayscale                           |
| Lomograph  | bool           |                                                                                                                    | Apply Lomograph filter                           |
| Polaroid   | bool           |                                                                                                                    | Apply Polaroid filter                            |
| Sepia      | bool           |                                                                                                                    | Apply Sepia filter                               |
| Kodachrome | bool           |                                                                                                                    |  Apply Kodachrome filter                         |
| Vignette   | bool           |                                                                                                                    | Apply Vignette filter                            |




---
### Image Sources

You can define the file data sources for any kind of storage, the default used data source is a local source ( webapp file system ).


As an example you can add the `Owin.ThumbCrop.AzureStorage` in your project from NuGet


```powershell
Install-Package Owin.ThumbCrop.AzureStorage
```

or dotnet core CLI

```powershell
dotnet add package Owin.ThumbCrop.AzureStorage
```


And configure your owin middleware to use de Azure storage data source,

```cs
app.UseThumbCrop(config =>
{
    config.ImageSources = new[]
    {
        new AzureStorageImageSource(connectionString,  allowedContainers: new []{ "cantainer1", "container2" })
    };
});

```

With this it will get the base image directly from the azure storage with the url `http://server/containerName/blobName`


Because its possible to define a collection of sources, the middleware will use the first source which responds successfully.

If you want to first search in a local file system and if the file not exists then search in the storage, you have to simple put the sources in order

```cs
app.UseThumbCrop(config =>
{
    config.ImageSources = new IImageSource[]
    {
        new LocalFileImageSource(),
        new AzureStorageImageSource(connectionString, containerName),
    };
});

```

For other types of storage or sources just implement the `IImageSource` interface and add in the ImageSources collection inside config
