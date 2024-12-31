using System;
using System.Security.AccessControl;
using NLog;

namespace LockLib.Log;

public static class Debug
{
    private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();

    static Debug()
    {
        // 配置 NLog
        var config = new NLog.Config.LoggingConfiguration();
        // 创建文件目标
        var fileTarget = new NLog.Targets.FileTarget("LockFailLog")
        {
            FileName = "${basedir}/log.txt",
            Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} | ${level} | ${message}",
            Encoding = System.Text.Encoding.UTF8,
            ConcurrentWrites = true,
            ArchiveAboveSize = 1048576, // 1MB
            ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
            ArchiveDateFormat = "yyyyMMdd",
            KeepFileOpen = false,
            AutoFlush = true,
            CreateDirs = true
        };
        // 创建控制台目标
        var consoleTarget = new NLog.Targets.ConsoleTarget("console")
        {
            Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} | ${level} \n ${message} \n",
            Encoding = System.Text.Encoding.UTF8
        };

        // 添加规则
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
        m_logger.Info("Logging is configured.");
        // 应用配置 
        LogManager.Configuration = config;
    }

    public static void Log(string message)
    {
        m_logger.Info(message);
    }

    public static void Log(Exception exception)
    {
        m_logger.Error(exception);
    }
}