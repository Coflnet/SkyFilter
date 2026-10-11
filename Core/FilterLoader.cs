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
        await RefreshLoop(stoppingToken);
    }

    /// <summary>How often the rune list is reloaded</summary>
    internal TimeSpan RefreshInterval = TimeSpan.FromHours(1);
    /// <summary>Retry interval while only the seeded runes are known</summary>
    internal TimeSpan RetryInterval = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Reloads the known runes so new ones appear without a restart
    /// </summary>
    internal async Task RefreshLoop(CancellationToken stoppingToken)
    {
        var failing = false;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(engine.RunesLoaded ? RefreshInterval : RetryInterval, stoppingToken);
                // WaitAsync so shutdown never waits on a hanging api call
                if (await engine.RefreshRuneAliases(provider, stoppingToken).WaitAsync(stoppingToken))
                {
                    if (failing)
                        logger.LogWarning("Rune filter refresh recovered");
                    failing = false;
                }
                else
                {
                    // warn once, repeated failures while the items api is down are only debug
                    logger.Log(failing ? LogLevel.Debug : LogLevel.Warning, "Could not refresh rune filters, no usable item list received");
                    failing = true;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (System.Exception e)
            {
                logger.LogError(e, "Error refreshing rune filters");
            }
        }
    }
}
