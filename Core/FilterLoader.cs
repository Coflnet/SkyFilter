using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Coflnet.Sky.Filter;
public class FilterLoader : BackgroundService
{
    private FilterEngine engine;
    private IServiceProvider provider;
    private ILogger<FilterLoader> logger;

    public FilterLoader(FilterEngine engine, IServiceProvider provider, ILogger<FilterLoader> logger)
    {
        this.engine = engine;
        this.provider = provider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await engine.Load(provider);
                break;
            }
            catch (System.Exception e)
            {
                logger.LogError(e, "Error loading filters");
            }
            await Task.Delay(1000 * 60, stoppingToken);
        }
        logger.LogInformation("Additional filters loaded.");
    }
}
