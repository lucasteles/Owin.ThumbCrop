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



