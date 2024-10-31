using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;

namespace SofarBMS.Helper
{
    public class LogHelper
    {
        #region 自动日志（按照Config配置）
        public static string Build(string msg)
        {
            var info = DateTime.Now + " " + msg;
            return info;
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("InfoLog");
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
            log = null;
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message">错误日志</param>
        public static void Error(string message, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("Error");
            if (log.IsInfoEnabled)
            {
                log.Error(message, ex);
            }
            log = null;
        }
        /// <summary>
        /// 故障和告警日志
        /// </summary>
        /// <param name="message"></param>
        public static void FaultAndWarning(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("FaultAndWarningLog");
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
            log = null;
        }
        /// <summary>
        /// 程序更新日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Download(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("DownloadLog");
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
            log = null;
        }
        #endregion


        #region 手动日志（手动命名日志文件）
        //public enum LogType
        //{
        //    None,
        //    Info,
        //    Error,
        //    Download
        //}

        /// <summary>
        /// 日志对象的字典。key=日志文件名，value=ilog日志对象
        /// </summary>
        static Dictionary<string, log4net.ILog> _ilogDict;

        //public static bool CreateNewLog = false;
        private static string LogFileName = "";
        public static string SubDirectory = "Info";
        static log4net.ILog _currentLogger = null;

        /// <summary>
        /// 公共方法，记录日志到固定的文件名
        /// </summary>
        /// <param name="logFileName">日志要保存的文件名</param>
        /// <param name="logInfo">日志信息</param>
        public static void AddLog(string logInfo)
        {
            _currentLogger?.Info(logInfo);
        }

        /// <summary>
        /// 初始化日志对象
        /// </summary>
        /// <param name="logFileName">日志文件名，日志对象</param>
        private static void InitNewLog1(string logFileName)
        {
            if (_ilogDict == null)
            {
                _ilogDict = new Dictionary<string, log4net.ILog>();
            }


            try
            {
                if (!_ilogDict.ContainsKey(logFileName))
                {
                    //创建日志目录
                    log4net.LogManager.CreateRepository(logFileName);
                    //获取日志对象
                    log4net.ILog logger = log4net.LogManager.GetLogger(logFileName, logFileName);

                    _ilogDict.Add(logFileName, logger);

                    //配置输出日志格式。%m表示message即日志信息。%n表示newline换行
                    log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout(@"%m%n");
                    layout.ActivateOptions();

                    //配置日志级别为所有级别
                    LevelMatchFilter filter = new LevelMatchFilter();
                    filter.LevelToMatch = Level.All;
                    filter.ActivateOptions();

                    //配置日志【循环附加，累加】
                    RollingFileAppender appender = new RollingFileAppender();

                    appender.File = $"Log//{SubDirectory}//";

                    appender.ImmediateFlush = true;
                    appender.MaxSizeRollBackups = 0;
                    appender.StaticLogFileName = false;

                    appender.DatePattern = $"yyyy-MM-dd HH时mm分{LogFileName}&quot;.log&quot;";
                    appender.LockingModel = new FileAppender.MinimalLock();
                    appender.CountDirection = 0;
                    appender.PreserveLogFileNameExtension = true;

                    appender.AddFilter(filter);
                    appender.Layout = layout;
                    appender.AppendToFile = true;
                    appender.ActivateOptions();

                    //配置缓存，增加日志效率
                    var bfa = new BufferingForwardingAppender();
                    bfa.AddAppender(appender);
                    bfa.BufferSize = 500;
                    bfa.Lossy = false;
                    bfa.Fix = FixFlags.None;
                    bfa.ActivateOptions();

                    log4net.Config.BasicConfigurator.Configure(log4net.LogManager.GetRepository(logFileName), bfa);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static void CreateNewLogger()
        {
            try
            {
                LogFileName = DateTime.Now.ToString("yyyy-MM-dd HH时mm分ss秒");

                // Pattern Layout
                PatternLayout layout = new PatternLayout("%date{yyyy-MM-dd HH:mm:ss.fff}%newline%message%newline%newline");
                //PatternLayout layout = new PatternLayout("%date{yyyy-MM-dd HH:mm:ss.fff} %-5level[%L] %message% %F%newline");
                // Level Filter
                //LevelMatchFilter filter = new LevelMatchFilter();
                //filter.LevelToMatch = Level.All;
                //filter.ActivateOptions();
                // File Appender
                //RollingFileAppender appender = new RollingFileAppender();
                FileAppender appender = new FileAppender();
                appender.Encoding = Encoding.Unicode;
                appender.Threshold = Level.All;
                // 目录
                appender.File = $"Log\\{SubDirectory}\\{LogFileName}.log";
                // 立即写入磁盘
                appender.ImmediateFlush = true;
                // true：追加到文件；false：覆盖文件
                appender.AppendToFile = true;
                // 新的日期或者文件大小达到上限，新建一个文件
                //appender.RollingStyle = RollingFileAppender.RollingMode.Size;
                // 文件大小达到上限，新建文件时，文件编号放到文件后缀前面
                //appender.PreserveLogFileNameExtension = true;
                // 时间模式
                //appender.DatePattern = $"yyyy-MM-dd HH时mm分ss秒'.log'";
                // 最小锁定模型以允许多个进程可以写入同一个文件
                appender.LockingModel = new FileAppender.MinimalLock();
                appender.Name = $"{LogFileName}Appender";
                //appender.AddFilter(filter);
                appender.Layout = layout;
                // 文件大小上限
                //appender.MaximumFileSize = "10MB";
                //appender.MaxFileSize = 10 * 1024 * 1024;
                // 设置无限备份=-1 ，最大备份数为30
                //appender.MaxSizeRollBackups = 30;
                //appender.StaticLogFileName = false;
                appender.ActivateOptions();
                //create instance
                string repositoryName = $"{LogFileName}Repository";
                ILoggerRepository repository = LoggerManager.CreateRepository(repositoryName);
                BasicConfigurator.Configure(repository, appender);
                //After the log instance initialization, we can get the instance from the LogManager by the special log instance name. Then you can start your logging trip.
                _currentLogger = LogManager.GetLogger(repositoryName, LogFileName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        #endregion
    }//class
}
