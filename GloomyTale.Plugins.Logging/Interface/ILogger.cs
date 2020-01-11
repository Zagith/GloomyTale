using System;
using System.Runtime.CompilerServices;

namespace GloomyTale.Plugins.Logging.Interface
{
    public interface ILogger
    {
        void Debug(string data, [CallerMemberName]string memberName = "");

        void Debug(string msg);

        void DebugFormat(string msg, params object[] objs);

        void Info(string msg);
        void InfoFormat(string msg, params object[] objs);

        void Warn(string msg);
        void WarnFormat(string msg, params object[] objs);

        void Error(Exception ex, [CallerMemberName]string memberName = "");

        void Error(string msg, Exception ex);

        void ErrorFormat(string msg, Exception ex, params object[] objs);

        void Error(string data, Exception ex = null, [CallerMemberName]string memberName = "");

        void Fatal(string msg, Exception ex);

        void Fatal(string data, Exception ex = null, [CallerMemberName]string memberName = "");

        void Info(string message, Exception ex = null, [CallerMemberName]string memberName = "");

        void LogEvent(string logEvent, string data, Exception ex = null, [CallerMemberName]string memberName = "");

        void LogEventError(string logEvent, string data, Exception ex = null, [CallerMemberName]string memberName = "");

        void LogUserEvent(string logEvent, string caller, string data);

        void LogUserEventDebug(string logEvent, string caller, string data);

        void LogUserEventError(string logEvent, string caller, string data, Exception ex);

        void Warn(string data, Exception innerException = null, [CallerMemberName]string memberName = "");
    }
}
