using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public class SiteSettingsService : ISiteSettingsService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<SiteSettingsService> _logger;
    private readonly string _settingsFilePath;
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public SiteSettingsService(IWebHostEnvironment env, ILogger<SiteSettingsService> logger)
    {
        _env = env;
        _logger = logger;
        _settingsFilePath = Path.Combine(_env.ContentRootPath, "site_settings.json");
    }

    public async Task<SiteSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                var defaultSettings = new SiteSettingsDto();
                defaultSettings.Maintenance.LastSettingsUpdatedAt = DateTime.UtcNow;
                var json = JsonSerializer.Serialize(defaultSettings, _jsonOptions);
                await File.WriteAllTextAsync(_settingsFilePath, json, cancellationToken);
                return defaultSettings;
            }

            var fileContent = await File.ReadAllTextAsync(_settingsFilePath, cancellationToken);
            var settings = JsonSerializer.Deserialize<SiteSettingsDto>(fileContent, _jsonOptions);
            return settings ?? new SiteSettingsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read site_settings.json. Returning default settings.");
            return new SiteSettingsDto();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<SiteSettingsDto> UpdateSettingsAsync(SiteSettingsDto newSettings, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            newSettings.Maintenance.LastSettingsUpdatedAt = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(newSettings, _jsonOptions);
            await File.WriteAllTextAsync(_settingsFilePath, json, cancellationToken);
            _logger.LogInformation("Site settings updated successfully at {Path}", _settingsFilePath);
            return newSettings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save site_settings.json");
            throw;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> ClearCacheAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var settings = await GetSettingsInternalAsync(cancellationToken);
            settings.Maintenance.LastCacheClearedAt = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            await File.WriteAllTextAsync(_settingsFilePath, json, cancellationToken);
            _logger.LogInformation("System cache timestamp cleared successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cache timestamp");
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<SiteSettingsDto> GetSettingsInternalAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_settingsFilePath))
        {
            return new SiteSettingsDto();
        }
        var fileContent = await File.ReadAllTextAsync(_settingsFilePath, cancellationToken);
        return JsonSerializer.Deserialize<SiteSettingsDto>(fileContent, _jsonOptions) ?? new SiteSettingsDto();
    }
}
