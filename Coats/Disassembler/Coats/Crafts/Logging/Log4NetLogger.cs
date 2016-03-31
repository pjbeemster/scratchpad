namespace Coats.Crafts.Logging
{
    using DD4T.ContentModel.Logging;
    using log4net;
    using System;

    public class Log4NetLogger : ILogWrapper
    {
        public void Critical(string message, params object[] parameters)
        {
            LogManager.GetLogger(typeof(Log4NetLogger)).FatalFormat(message, parameters);
        }

        public void Critical(string message, LoggingCategory category, params object[] parameters)
        {
            LogManager.GetLogger(category.ToString()).FatalFormat(message, parameters);
        }

        public void Debug(string message, params object[] parameters)
        {
            ILog logger = LogManager.GetLogger(typeof(Log4NetLogger));
            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat(message, parameters);
            }
        }

        public void Debug(string message, LoggingCategory category, params object[] parameters)
        {
            ILog logger = LogManager.GetLogger(category.ToString());
            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat(message, parameters);
            }
        }

        public void Error(string message, params object[] parameters)
        {
            LogManager.GetLogger(typeof(Log4NetLogger)).ErrorFormat(message, parameters);
        }

        public void Error(string message, LoggingCategory category, params object[] parameters)
        {
            LogManager.GetLogger(category.ToString()).ErrorFormat(message, parameters);
        }

        public void Information(string message, params object[] parameters)
        {
            LogManager.GetLogger(typeof(Log4NetLogger)).InfoFormat(message, parameters);
        }

        public void Information(string message, LoggingCategory category, params object[] parameters)
        {
            LogManager.GetLogger(category.ToString()).InfoFormat(message, parameters);
        }

        public void Warning(string message, params object[] parameters)
        {
            LogManager.GetLogger(typeof(Log4NetLogger)).WarnFormat(message, parameters);
        }

        public void Warning(string message, LoggingCategory category, params object[] parameters)
        {
            LogManager.GetLogger(category.ToString()).WarnFormat(message, parameters);
        }
    }
}

