using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Coats.Crafts.SmartTartget;
using log4net;
using System.IO;
using System.Xml.Xsl;

namespace Coats.Crafts.SmartTarget.FASXmlPollerService
{
    public partial class FASXmlPoller : ServiceBase
    {
        private readonly Timer _timer;
        private const int DefaultPollingIntervalMinutes = 1;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(FASXmlPoller));
        private static XslCompiledTransform _xslCompiledTransform = new XslCompiledTransform(false);

        private enum PollOrRespondType
        {
            Poll = 1,
            Respond = 2
        };

        /// <summary>
        /// Get polling interval from app.config.
        /// If not found, default to DefaultPollingIntervalMinutes
        /// </summary>
        private int PollingIntervalMinutes
        {
            get
            {
                try
                {
                    return Properties.Settings.Default.PollingIntervalMinutes;
                }
                catch (Exception)
                {
                    return DefaultPollingIntervalMinutes;
                }
            }
        }

        /// <summary>
        /// Get the app.config setting to decide if we are going to poll the incoming dir,
        /// or actually respond to a new FAS XML file straight away by using a FileSystemWatcher.
        /// </summary>
        private PollOrRespondType PollOrRespond
        {
            get
            {
                PollOrRespondType porType = PollOrRespondType.Respond;
                if (!Enum.TryParse(Properties.Settings.Default.PollOrRespond, out porType))
                {
                    porType = PollOrRespondType.Respond;
                }
                return porType;
            }
        }

        /// <summary>
        /// Default contructor
        /// </summary>
        public FASXmlPoller()
        {
            InitializeComponent();

            if (PollOrRespond == PollOrRespondType.Poll)
            {
                // Timer is specified in milliseconds, so convert pollingMinutes from minutes to milliseconds
                _timer = new Timer(PollingIntervalMinutes.MinutesToMilliseconds());

                // Assign event delegate
                _timer.Elapsed += new ElapsedEventHandler(PollFASXML);
            }
        }

        /// <summary>
        /// On Start event handler
        /// </summary>
        /// <param name="args">
        /// Pass in what you like - I ain't gonna take no notice, crazy fool!
        /// </param>
        protected override void OnStart(string[] args)
        {
            if (PollOrRespond == PollOrRespondType.Poll)
            {
                //this.WriteEventLog(
                //    String.Format("Extend FAS XML Deployer Timer started, waiting for {0} minutes ...", PollingIntervalMinutes));
                _logger.InfoFormat("FASXmlPoller : Timer started, waiting for {0} minutes ...", PollingIntervalMinutes);
                _timer.Start();
            }
            else
            {
                _logger.Info("FASXmlPoller : Starting File System Watcher...");

                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = Properties.Settings.Default.IncomingDir;
                
                /* Watch for changes in LastAccess and LastWrite times, and
                   the renaming of files or directories. */
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                // Only watch xml files.
                watcher.Filter = "*.xml";

                // Add event handlers.
                watcher.Created += new FileSystemEventHandler(OnCreated);

                // Begin watching.
                watcher.EnableRaisingEvents = true;
            }

        }

        /// <summary>
        /// On Stop event handler
        /// </summary>
        protected override void OnStop()
        {
            if (PollOrRespond == PollOrRespondType.Poll)
            {
                //this.WriteEventLog("Extend FAS XML Deployer Timer stopped");
                _logger.Info("FASXmlPoller : Timer stopped");
                _timer.Stop();
            }
            else
            {
                _logger.Info("FASXmlPoller : Starting File System Watcher stopped");
            }
        }

        /// <summary>
        /// Define the File System Event Handler for file created. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file created.
            _logger.InfoFormat("FASXmlPoller : FAS XML Poller - found file {0}", e.FullPath);

            // Farm out the processing to another DLL to make for easier code changes
            FASXmlUtils fasXmlUtils = new FASXmlUtils(
                Properties.Settings.Default.IncomingDir,
                Properties.Settings.Default.ProcessedDir,
                Properties.Settings.Default.RejectedDir,
                _logger, 
                compiledTransform: _xslCompiledTransform);

            // Go do your stuff!
            FASXmlUtils.Status status = fasXmlUtils.ExtendAndMove(e.FullPath);

            fasXmlUtils = null;
            GC.Collect();
        }


        /// <summary>
        /// Timer elapsed event delegate
        /// </summary>
        /// <param name="sender">The timer object that has triggered the elapsed event</param>
        /// <param name="e">The time object event args</param>
        private void PollFASXML(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            //EventLog.WriteEntry("FASXMLPoller", "Extend FAS XML Deployer - checking ....");
            _logger.Info("FASXmlPoller : FAS XML Poller - checking ....");

            // Farm out the processing to another DLL to make for easier code changes
            FASXmlUtils fasXmlUtils = new FASXmlUtils(
                Properties.Settings.Default.IncomingDir,
                Properties.Settings.Default.ProcessedDir,
                Properties.Settings.Default.RejectedDir,
                _logger,
                compiledTransform: _xslCompiledTransform);

            // Go do your stuff!
            FASXmlUtils.Status status = fasXmlUtils.ExtendFASXml();

            fasXmlUtils = null;
            GC.Collect();

            //EventLog.WriteEntry("FASXMLPoller", string.Format("Extend FAS XML Deployer Timer restarted, waiting for {0} minutes ...", PollingIntervalMinutes));
            _logger.InfoFormat("FASXmlPoller : Timer restarted, waiting for {0} minutes ...", PollingIntervalMinutes);

            _timer.Start();
        }
    }

    /// <summary>
    /// Helper class to extend int to convert to milliseconds
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Extension method for int - to convert it's current value into milliseconds
        /// </summary>
        /// <param name="minutes">The current int - automatically bound</param>
        /// <returns>The converted milliseconds value</returns>
        public static int MinutesToMilliseconds(this int minutes)
        {
            return ((minutes * 60) * 1000);
        }
    }
}