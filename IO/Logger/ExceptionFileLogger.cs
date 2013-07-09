using System;
using System.Collections.Generic;
using System.Text;

namespace G.W.Y.IO.Logger
{
    public class ExceptionFileLogger : IExceptionLogger
    {
        protected IAgileLogger agileLogger;
        protected ErrorLevel errorLevel = ErrorLevel.Standard;
        public IAgileLogger AgileLogger
        {
            set
            {
                this.agileLogger = value;
            }
        }
        public ErrorLevel ErrorLevel
        {
            set
            {
                this.errorLevel = value;
            }
        }
        public ExceptionFileLogger()
        {
        }
        public ExceptionFileLogger(IAgileLogger logger)
            : this(logger, ErrorLevel.Standard)
        {
        }
        public ExceptionFileLogger(IAgileLogger logger, ErrorLevel _errorLevel)
        {
            this.agileLogger = logger;
            this.errorLevel = _errorLevel;
        }
        public void Log(Exception ee, string methodPath, Type[] genericTypes, string[] argumentNames, object[] argumentValues)
        {
            StringBuilder stringBuilder = new StringBuilder(methodPath);
            if (genericTypes != null && genericTypes.Length > 0)
            {
                stringBuilder.Append("<");
                for (int i = 0; i < genericTypes.Length; i++)
                {
                    stringBuilder.Append(genericTypes[i].ToString());
                    if (i != genericTypes.Length - 1)
                    {
                        stringBuilder.Append(",");
                    }
                }
                stringBuilder.Append(">");
            }
            string text = stringBuilder.ToString();
            if (argumentNames != null && argumentNames.Length > 0)
            {
                StringBuilder stringBuilder2 = new StringBuilder("<Parameters>");
                for (int i = 0; i < argumentNames.Length; i++)
                {
                    stringBuilder2.Append(string.Format("<{0}>{1}</{0}>", argumentNames[i], (argumentValues[i] == null) ? "NULL" : argumentValues[i].ToString()));
                }
                stringBuilder2.Append("</Parameters>");
                text = text + "@" + stringBuilder2.ToString();
            }
            this.agileLogger.Log(ee, text, this.errorLevel);
        }
    }
}
