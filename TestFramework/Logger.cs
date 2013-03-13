using System.Reflection;
using log4net;
using log4net.Config;

namespace TestFramework
{
    /// <summary>
    ///     Central Logger for all, Singleton
    /// </summary>
    public class Logger
    {
        private Logger()
        {
        }

        /// <summary>
        ///     Logger Instance
        /// </summary>
        public static ILog Instance
        {
            get { return Nested.instance.GetLogInstance(Assembly.GetCallingAssembly().GetName().Name); }
        }

        private ILog GetLogInstance(string name)
        {
            ILog log = LogManager.GetLogger(name);
            return log;
        }

        private class Nested
        {
            internal static readonly Logger instance = new Logger();

            static Nested()
            {
                XmlConfigurator.Configure();
            }
        }
    }
}