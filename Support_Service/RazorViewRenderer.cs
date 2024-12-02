using System;
using RazorLight;

namespace Ecommerce_Product.Support_Serive;

public class RazorViewRenderer
{
    private readonly IRazorLightEngine _engine;

    public RazorViewRenderer()
    {
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(Program))
            .UseMemoryCachingProvider()
            .Build();        
    }

    public async Task<string> RenderViewToStringAsync<T>(string viewName, T model)
    {
        return await _engine.CompileRenderAsync(viewName, model);
    }
}