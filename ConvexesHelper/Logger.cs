using log4net;
using log4net.Config;
using System.Reflection;
namespace ConvexHelper
{
    /// <summary>
    /// Central Logger for all, Singleton
    /// </summary>
    public class Logger
    {
        Logger()
        { }

        /// <summary>
        /// Logger Instance
        /// </summary>
        public static ILog Instance
        {
            get
            {
                return Nested.instance.GetLogInstance(Assembly.GetCallingAssembly().GetName().Name);
            }
        }

        ILog GetLogInstance(string name)
        {
            ILog log = LogManager.GetLogger(name);
            return log;
        }

        class Nested
        {
            static Nested()
            {
                XmlConfigurator.Configure();
            }

            internal static readonly Logger instance = new Logger();
        }
    }
    
}


