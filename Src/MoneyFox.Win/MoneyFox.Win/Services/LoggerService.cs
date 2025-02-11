﻿namespace MoneyFox.Win.Services;

using System.IO;
using Windows.Storage;
using Core.Common;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

public static class LoggerService
{
    public static void Initialize()
    {
        var logPath = Path.Combine(path1: ApplicationData.Current.LocalFolder.Path, path2: LogConfiguration.FileName);
        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.File(
                path: logPath,
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Month,
                retainedFileCountLimit: 12,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}\t[{Level:u3}]\t{Message:lj}\t{Exception}{NewLine}",
                shared: true)
            .CreateLogger();

        Log.Information("Application Startup");
    }
}
