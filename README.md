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



### How to use
