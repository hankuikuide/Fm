using System;

namespace Han.Infrastructure
{
    public class LogHelper
    {
        private Type type;

        public LogHelper(Type type)
        {
            this.type = type;
        }

        public void Info(string v)
        {
            Log.Logger.Log(Log.Level.Info, v);
        }
        public void Debug(string v)
        {
            Log.Logger.Log(Log.Level.Debug, v);
        }
        public void Error(string v, Exception ex)
        {
            Log.Logger.LogException(ex);
        }

        public void Error(string msg)
        {
            Log.Logger.Log(Log.Level.Error, msg);
        }
    }
}