#region copyright
// <copyright file="Logger.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.Log
{
    using System;

    using NLog;
    using NLog.Config;
    using NLog.Targets;
    public enum Level
    {
        Trace,

        Debug,

        Info,

        Warn,

        Error,

        Fatal
    }

    public static class Logger
    {
        #region Static Fields

        private static NLog.Logger Nlogger = null;

        #endregion

        /// <summary>
        /// 配置Nlog读取配置文件
        /// </summary>
        /// <param name="logsPath"></param>
        public static void LoggergConfiguration(string logsPath)
        {
            #region Step 1.Create configuration object
            var config = new LoggingConfiguration();
            #endregion

            #region Step 2. Create targets and add them to the configuration 
            var outPutTarget = new DebuggerTarget()
            {
                Name = "Debugger",
                Layout = "${longdate} ${level:uppercase=false} ${message}"
            };

            config.AddTarget(outPutTarget);

            var fileErrorTarget = new FileTarget()
            {
                ArchiveAboveSize = 10240,
                Name = "FileError",
                Layout = "${longdate} ${level:uppercase=false} ${message}",
                FileName = logsPath + "/${shortdate}/${shortdate}_error.log"
            };

            config.AddTarget(fileErrorTarget);
            #endregion

            #region Step 4. Define rules
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, outPutTarget));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, fileErrorTarget));
            #endregion

            #region  Step 5. Activate the configuration
            LogManager.Configuration = config;
            config.Reload();
            #endregion

            Nlogger = LogManager.GetCurrentClassLogger();

        }

        /// <summary>
        /// 修改nlog配置文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void UpdateLoggergConfiguration(string logsPath, bool logFullMode)
        {
            // 获取Nlog配置文件
            var config = LogManager.Configuration;

            // 添加Debug日志级别记录Target
            if (logFullMode)
            {
                #region 添加target
                var debugFileTarget = new FileTarget()
                {
                    ArchiveAboveSize = 1024 * 1024 * 5,
                    Name = "FileDebug",
                    Layout = "${longdate} ${level:uppercase=false} ${message}",
                    FileName = logsPath + "/${shortdate}/${shortdate}.log"
                };

                config.AddTarget(debugFileTarget);
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, debugFileTarget));
                #endregion
            }
            else
            {
                #region 如果存在，就移除
                if (config.FindTargetByName("FileDebug") != null)
                {
                    //先移除Target, 再移除Rule
                    config.RemoveTarget("FileDebug");
                    config.LoggingRules.RemoveAt(2);
                }
                #endregion
            }

            #region 激活新的配置文件
            LogManager.Configuration = config;
            LogManager.Configuration.Reload();

            #endregion
        }


        #region 通过静态构造函数，配置Nlog
        //static Logger()
        //{

        ////Step 1.Create configuration object
        //var config = new LoggingConfiguration();

        //// Step 2. Create targets and add them to the configuration 
        //var consoleTarget = new DebuggerTarget();
        //config.AddTarget("console", consoleTarget);

        //var fileTarget = new FileTarget();
        //config.AddTarget("file", fileTarget);

        //var fileErrorTarget = new FileTarget();
        //config.AddTarget("file", fileErrorTarget);

        //// Step 3. Set target properties 
        //consoleTarget.Layout = @"${longdate} ${level:uppercase=false} ${message}";

        ////配置日常日志文件路径及格式
        ////fileTarget.FileName = "${basedir}/logs/${shortdate}/${shortdate}.log";
        //fileTarget.FileName = "c:/logs/${shortdate}/${shortdate}.log";

        //fileTarget.Layout = "${longdate} ${level:uppercase=false} ${message}";

        ////配置错误日志文件路径及格式
        //fileErrorTarget.FileName = "${basedir}/logs/${shortdate}/${shortdate}_error.log";
        //fileErrorTarget.Layout = "${longdate} ${level:uppercase=false} ${message}";

        //// Step 4. Define rules
        //config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));

        //////是否开启全量模式
        //config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));


        //config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, fileErrorTarget));

        //// Step 5. Activate the configuration
        //LogManager.Configuration = config;

        //}
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 当messageCallback 结果不为 null时记录日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="messageCallback"></param>
        public static void Log(Level level, Func<string> messageCallback)
        {
            var message = messageCallback();
            if (message != null)
                Log(level, message);
        }
        /// <summary>
        /// 记录所有异常信息，包括inneerexcepiton
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            Nlogger.Log(LogLevel.Error, ex.ToString);
        }

        public static void Log(Level level, string message)
        {
            if (Nlogger == null)
            {
                return;
            }
            LogLevel l;
            switch (level)
            {
                case Level.Trace:
                    l = LogLevel.Trace;
                    break;
                case Level.Debug:
                    l = LogLevel.Debug;
                    break;
                case Level.Info:
                    l = LogLevel.Info;
                    break;
                case Level.Warn:
                    l = LogLevel.Warn;
                    break;
                case Level.Error:
                    l = LogLevel.Error;
                    break;
                case Level.Fatal:
                    l = LogLevel.Fatal;
                    break;
                default:
                    l = LogLevel.Trace;
                    break;
            }

            Nlogger.Log(l, message);
        }

        #endregion
    }
}