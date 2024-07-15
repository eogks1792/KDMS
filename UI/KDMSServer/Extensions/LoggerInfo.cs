
using KDMSServer.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Windows.Threading;

namespace KDMSServer;

public static class LoggerInfo
{
    public static void ServerLog(this ILogger logger, string msg)
    {
        logger.Information(msg);
        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
        {
            var model = App.Current.Services.GetService<MainViewModel>()!;
            if (model == null)
                return;

            if (model.ServerLogView.Count > 100)
                model.ServerLogView.RemoveAt(100);

            model.ServerLogView.Insert(0, new LogInfo()
            {
                LogTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                LogMessage = msg
            });
        }));
    }

    public static void DbLog(this ILogger logger, string msg)
    {
        logger.Information(msg);
        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
        {
            var model = App.Current.Services.GetService<MainViewModel>()!; 
            if (model == null)
                return;

            if (model.DbLogView.Count > 100)
                model.DbLogView.RemoveAt(100);

            model.DbLogView.Insert(0, new LogInfo()
            {
                LogTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                LogMessage = msg
            });
        }));
    }


}
