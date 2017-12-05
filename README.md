[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![Nuget](https://img.shields.io/nuget/v/Owin.ThumbCrop.svg)](https://www.nuget.org/packages/Owin.ThumbCrop/)

# Owin.ThumbCrop

is a middleware for create thumb based in your webapp assets

### Installation

Install from Nuget

```powershell
Install-Package Owin.ThumbCrop
```

Install from dotnet core CLI

```powershell
dotnet add package Owin.ThumbCrop 
```
### Configurations

Just add the middleware in owin pipeline, ***before*** `UseStaticFiles` and `UseMvc`

```cs
  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseThumbMiddleware();

            app.UseStaticFiles();
            app.UseMvc(routes => {...});
        }
```

Its possible to pass a configurations to the `UseThumbMiddleware` method as a instance of `ThumbCropConfig` or on an overload wich receives an `Action<ThumbCropConfig>`
  
```cs
          app.UseThumbMiddleware(config => {
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
| CacheManager    | IThumbCacheManager | Cache manager                                                  | `InMemoryCacheManager` |


### How to use

Just put the `UrlPattern` at the end of your image URL and pass the transformation parameters

```
~/images/cat.jpg.thumb.axd?width=200&height=250&refpoint=TopLeft&crop&vignette
```
#### Original:
![Original](https://raw.githubusercontent.com/lucasteles/Owin.ThumbCrop/master/images/cat.jpg)

#### Thumb:
![Thumb](https://raw.githubusercontent.com/lucasteles/Owin.ThumbCrop/master/images/cat_thumb.png)|


### Thumb Options

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

