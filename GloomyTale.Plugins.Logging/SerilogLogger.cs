using Serilog;
using Serilog.Events;
using System;
using System.Runtime.CompilerServices;
using ILogger = GloomyTale.Plugins.Logging.Interface.ILogger;

namespace GloomyTale.Plugins.Logging
{
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogLogger() =>
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"logs/{DateTime.Now:yyyyMMddHHmmss}.log", LogEventLevel.Information, flushToDiskInterval: TimeSpan.FromMinutes(5))
                .CreateLogger();

        /// <summary>
        /// Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="memberName"></param>
        public void Debug(string data, [CallerMemberName]string memberName = "")
        {
            _logger.Debug($"[{memberName}]: {data}");
        }

        public void Debug(string msg)
        {
            _logger.Debug(msg);
        }

        public void DebugFormat(string msg, params object[] objs)
        {
            _logger.Debug(msg, objs);
        }

        public void Info(string msg)
        {
            _logger.Information(msg);
        }

        public void InfoFormat(string msg, params object[] objs)
        {
            _logger.Information(msg, objs);
        }

        public void Warn(string msg)
        {
            _logger.Warning(msg);
        }

        public void WarnFormat(string msg, params object[] objs)
        {
            _logger.Warning(msg, objs);
        }

        /// <summary>
        /// Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="ex"></param>
        public void Error(Exception ex, [CallerMemberName]string memberName = "")
        {
            _logger.Error($"[{memberName}]: {ex.Message}", ex);
        }

        public void Error(string data, Exception ex = null, [CallerMemberName]string memberName = "")
        {
            if (ex != null)
            {
                _logger.Error($"[{memberName}]: {data} {ex.InnerException}", ex);
            }
            else
            {
                _logger.Error($"[{memberName}]: {data}");
            }
        }

        public void Error(string msg, Exception ex)
        {
            _logger.Error(ex, msg);
        }

        public void ErrorFormat(string msg, Exception ex, params object[] objs)
        {
            _logger.Error(ex, msg, objs);
        }

        public void Fatal(string msg, Exception ex)
        {
            _logger.Fatal(ex, msg);
        }

        /// <summary>
        /// Wraps up the fatal message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public void Fatal(string data, Exception ex = null, [CallerMemberName]string memberName = "")
        {
            if (ex != null)
            {
                _logger.Fatal($"[{memberName}]: {data} {ex.InnerException}", ex);
            }
            else
            {
                _logger.Fatal($"[{memberName}]: {data}");
            }
        }

        /// <summary>
        /// Wraps up the info message with the CallerMemberName
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public void Info(string message, Exception ex = null, [CallerMemberName]string memberName = "")
        {
            if (ex != null)
            {
                _logger.Information($"[{memberName}]: {message}", ex);
            }
            else
            {
                _logger.Information($"[{memberName}]: {message}");
            }
        }

        /// <summary>
        /// Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public void LogEvent(string logEvent, string data, Exception ex = null, [CallerMemberName]string memberName = "")
        {
            if (ex != null)
            {
                _logger.Information($"[{memberName}]: [{logEvent}]{data}");
            }
            else
            {
                _logger.Information($"[{memberName}]: [{logEvent}]{data}", ex);
            }
        }

        /// <summary>
        /// Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public void LogEventError(string logEvent, string data, Exception ex = null, [CallerMemberName]string memberName = "")
        {
            if (ex != null)
            {
                _logger.Error($"[{memberName}]: [{logEvent}]{data}", ex);
            }
            else
            {
                _logger.Error($"[{memberName}]: [{logEvent}]{data}");
            }
        }

        /// <summary>
        /// Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        public void LogUserEvent(string logEvent, string caller, string data)
        {
            _logger.Information($"[{logEvent}][{caller}]{data}");
        }
        /// <summary>
        /// Wraps up the message with the CallerMemberName
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        public void LogUserEventDebug(string logEvent, string caller, string data)
        {
            _logger.Debug($"[{logEvent}][{caller}]{data}");
        }
        /// <summary>
        /// Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        public void LogUserEventError(string logEvent, string caller, string data, Exception ex)
        {
            _logger.Error($"[{logEvent}][{caller}]{data}", ex);
        }
        /// <summary>
        /// Wraps up the warn message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="innerException"></param>
        /// <param name="memberName"></param>
        public void Warn(string data, Exception innerException = null, [CallerMemberName]string memberName = "")
        {
            if (innerException != null)
            {
                _logger.Warning($"[{memberName}]: {data} {innerException.InnerException}", innerException);
            }
            else
            {
                _logger.Warning($"[{memberName}]: {data}");
            }
        }
    }
}
