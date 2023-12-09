using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Coflnet.Sky.Filter;
public class FilterLoader : BackgroundService
{
    private FilterEngine engine;
    private IServiceProvider provider;

    public FilterLoader(FilterEngine engine, IServiceProvider provider)
    {
        this.engine = engine;
        this.provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await engine.Load(provider);
    }
}
