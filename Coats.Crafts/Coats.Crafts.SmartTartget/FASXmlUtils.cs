using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Coats.Crafts.SmartTartget.SmartTargetDeploymentWebService;
using System.Xml.Xsl;
using System.Diagnostics;
using Coats.SQLite;
using log4net;

namespace Coats.Crafts.SmartTartget
{
    public class FASXmlUtils
    {
        private string _incomingDir;
        private string _processedDir;
        private string _rejectedDir;
        private ILog _logger = null;
        private XslCompiledTransform _xslCompiledTransform;

        public enum Status
        {
            NothingToDo,
            Success,
            ErrorsEncountered
        }

        /// <summary>
        /// Specifies the location of the folder to search for incoming FAS xml files
        /// </summary>
        public string IncomingDir
        {
            get { return _incomingDir; }
            set
            {
                _incomingDir = value.Replace('\\', '/');
                if (_incomingDir.EndsWith("/")) { _incomingDir = _incomingDir.TrimEnd('/'); }
            }
        }

        /// <summary>
        /// Specifies the location of the folder to move successfully processed FAS xml files
        /// </summary>
        public string ProcessedDir
        {
            get { return _processedDir; }
            set
            {
                _processedDir = value.Replace('\\', '/');
                if (_processedDir.EndsWith("/")) { _processedDir = _processedDir.TrimEnd('/'); }
            }
        }

        public XslCompiledTransform CompiledTransform
        {
            get 
            {
                if (_xslCompiledTransform == null)
                {
                    _xslCompiledTransform = new XslCompiledTransform(false);
                }
                return _xslCompiledTransform; 
            }
        }

        /// <summary>
        /// Specifies the location of the folder to move erroneous FAS xml files for later inspection
        /// </summary>
        public string RejectedDir
        {
            get { return _rejectedDir; }
            set
            {
                _rejectedDir = value.Replace('\\', '/');
                if (_rejectedDir.EndsWith("/")) { _rejectedDir = _rejectedDir.TrimEnd('/'); }
            }
        }

        /// <summary>
        /// Gets all FAS xml files from the specified dir (actually, it only looks for *.xml files!)
        /// </summary>
        /// <param name="idir">
        /// Specifies the location of the folder to search for FAS xml files
        /// </param>
        /// <returns></returns>
        public static string[] GetFASXmlFileList(string dir)
        {
            return Directory.GetFiles(dir, "*.xml");
        }

        /// <summary>
        /// Default empty contructor.
        /// DON'T FORGET TO SET IncomingDir, ProcessedDir and RejectDir properties!
        /// </summary>
        public FASXmlUtils() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incomingDir">Specifies the location of the folder to search for incoming FAS xml files</param>
        /// <param name="processedDir">Specifies the location of the folder to move successfully processed FAS xml files</param>
        /// <param name="rejectedDir">Specifies the location of the folder to move erroneous xml files for later inspection</param>
        /// <param name="logger">Specifies a log4net interface</param>
        public FASXmlUtils(string incomingDir, string processedDir, string rejectedDir, ILog logger, XslCompiledTransform compiledTransform = null)
        {
            IncomingDir = incomingDir;
            ProcessedDir = processedDir;
            RejectedDir = rejectedDir;
            _logger = logger;
            _xslCompiledTransform = compiledTransform;
        }

        /// <summary>
        /// Kick off the FAS XML file search, extend each found FAS XML file (XSLT transformation), call the SmartTarget Deployment Web Service,
        /// move to the specified processed or rejected location.
        /// The IncomingDir, ProcessedDir and RejectDir properties must be set prior to this call.
        /// </summary>
        /// <returns></returns>
        public Status ExtendFASXml()
        {
            // Quick sanity check...
            if (string.IsNullOrEmpty(IncomingDir)) 
            {
                LogMessage("FASXMLUtils.ExtendFASXML : IncomingDir property not set", LoggerMessageLevel.Error);
                throw new Exception("FASXMLUtils.ExtendFASXML: IncomingDir property not set"); 
            }
            if (string.IsNullOrEmpty(ProcessedDir)) 
            {
                LogMessage("FASXMLUtils.ExtendFASXML : ProcessedDir property not set", LoggerMessageLevel.Error);
                throw new Exception("FASXMLUtils.ExtendFASXML: ProcessedDir property not set"); 
            }
            if (string.IsNullOrEmpty(RejectedDir)) 
            {
                LogMessage("FASXMLUtils.ExtendFASXML : RejectedDir property not set", LoggerMessageLevel.Error);
                throw new Exception("FASXMLUtils.ExtendFASXML: RejectedDir property not set"); 
            }

            Status retStatus = Status.Success;

            // Get all FASXML files
            LogMessage(string.Format("FASXMLUtils.ExtendFASXML : searching {0} for new FASXML files", IncomingDir), LoggerMessageLevel.Info);
            string[] fasXmlFileList = GetFASXmlFileList(IncomingDir);

            // Return if there aren't any
            if (fasXmlFileList.Count() == 0)
            {
                LogMessage("FASXMLUtils.ExtendFASXML : none found", LoggerMessageLevel.Info);
                return Status.NothingToDo;
            }

            // Iterate through the FAS XML Filelist, and process one-by-one
            foreach (string fasXmlFile in fasXmlFileList)
            {
                Status tmpStatus = ExtendAndMove(fasXmlFile);
                if (tmpStatus == Status.ErrorsEncountered)
                {
                    retStatus = tmpStatus;
                }
            }

            fasXmlFileList = null;
            GC.Collect();

            return retStatus;
        }

        /// <summary>
        /// Applies an xslt transformation to the incoming FAS XML, 
        /// deploys the resulting FAS XML to FredHopper via the SmartTarget Deployment Web Service,
        /// and moves the incoming FAS XML to either the specified processed, or rejected directory.
        /// </summary>
        /// <param name="fasXmlFile">The file path of the incoming FAS XML</param>
        /// <returns>Status</returns>
        public Status ExtendAndMove(string fasXmlFile)
        {
            Status retStatus = Status.Success;

            LogMessage(string.Format("FASXMLUtils.ExtendAndMove : loading FASXML file {0}", fasXmlFile), LoggerMessageLevel.Info);

            // Try and load specified xml file
            XmlDocument fasXmlIn = null;
            try
            {
                fasXmlIn = new XmlDocument();

                //// Ash: 21/05/2013 -
                //// Changing the code to replace "known" LEGAL UTF-8 chars which cause FredHopper to reject.
                //// Currently, the list stands at:
                //// ’ - fancy Windows Word apostrophe.
                //// A0 [hex] - line feed or possibly &nbsp; (either way, it's invalid in FredHopper's eyes).
                ////fasXmlIn.Load(fasXmlFile);
                //string xmlSource = File.ReadAllText(fasXmlFile, System.Text.Encoding.UTF8);

                ////string xmlReplace = RemoveInvalidXmlChars(xmlSource);
                ////xmlSource = string.Empty;

                ////fasXmlIn.LoadXml(xmlReplace);
                ////xmlReplace = string.Empty;

                //fasXmlIn.LoadXml(xmlSource);
                //xmlSource = string.Empty;
                //GC.Collect();

                fasXmlIn.Load(fasXmlFile);
            }
            catch (Exception)
            {
                //EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.ExtendAndMove: FAILED to load FASXML file {0}", fasXmlFile));
                LogMessage(string.Format("FASXMLUtils.ExtendAndMove: FAILED to load FASXML file {0}", fasXmlFile), LoggerMessageLevel.Info);
                return Status.ErrorsEncountered;
            }

            // Transform (XSLT)
            XmlDocument fasXmlOut = null;
            if (Transform(fasXmlIn, out fasXmlOut) == Status.Success)
            {
                // Send the XmlDocument object to FredHopper
                if (SendToFredHopper(fasXmlOut, Path.GetFileNameWithoutExtension(fasXmlFile), _logger) == Status.ErrorsEncountered)
                {
                    // Only over wright if there was an error, otherwise leave it as previously set
                    retStatus = Status.ErrorsEncountered;
                }
                else
                {
                    // Everything went fine, so let's update the SQLite table
                    if (UpdateSQLite(fasXmlIn) == Status.ErrorsEncountered)
                    {
                        // Oops, encountered an error
                        retStatus = Status.ErrorsEncountered;
                    }
                }
            }
            else
            {
                // Only over wright if there was an error, otherwise leave it as previously set
                //EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.ExtendAndMove: FAILED to transform FASXML file {0}", fasXmlFile));
                LogMessage(string.Format("FASXMLUtils.ExtendAndMove: FAILED to transform FASXML file {0}", fasXmlFile), LoggerMessageLevel.Warn);
                retStatus = Status.ErrorsEncountered;
            }

            // Move the incoming FAS XML 
            string destinationFile = string.Format("{0}/{1}", (retStatus == Status.Success ? ProcessedDir : RejectedDir), Path.GetFileName(fasXmlFile));
            if (MoveFile(fasXmlFile, destinationFile) == Status.ErrorsEncountered)
            {
                // Only over wright if there was an error, otherwise leave it as previously set
                return Status.ErrorsEncountered;
            }

            fasXmlIn.RemoveAll();
            fasXmlIn = null;

            fasXmlOut.RemoveAll();
            fasXmlOut = null;

            GC.Collect();

            return retStatus;
        }

        // NOT NEEDED???
        //private string RemoveInvalidXmlChars(string input)
        //{
        //    //return new string(input.Where(value =>
        //    //    (value >= 0x0020 && value <= 0xD7FF) ||
        //    //    (value >= 0xE000 && value <= 0xFFFD) ||
        //    //    value == 0x0009 ||
        //    //    value == 0x000A ||
        //    //    value == 0x000D).ToArray());

        //    var isValid = new Predicate<char>(value =>
        //            (value >= 0x0020 && value <= 0xD7FF) ||
        //            (value >= 0xE000 && value <= 0xFFFD) ||
        //            value == 0x0009 ||
        //            value == 0x000A ||
        //            value == 0x000D);

        //    return new string(Array.FindAll(input.ToCharArray(), isValid));
        //}

        /// <summary>
        /// Should only be called after a successful FredHopper transfer.
        /// Works out what the operation should really be by checking if the supplied tcm id exists in the database.
        /// SmartTarget always provides "add" and "update" data as simply "add". We need to change this so we
        /// don't overwrite any fields such as "rating" or "commentcount", etc.
        /// If the tcm id doesn't exist, then it will be added to the SQLite database. 
        /// The only provisio is if the supplied operation is "delete" - in which case the tcm id is removed 
        /// from the database
        /// </summary>
        /// <param name="fasXml"></param>
        private Status UpdateSQLite(XmlDocument fasXml)
        {
            SQLiteDatabase sqlite = new SQLiteDatabase();
            Status retStatus = Status.Success;

            // Loop through each item in the FAS XML file
            XmlNodeList items = fasXml.SelectNodes("/items/item");

            foreach (XmlNode item in items)
            {
                // Get the item's Tcm Id 
                string tcmId = item.Attributes["identifier"].Value;
                // Get the item's operation (will be add or delete)
                string operation = item.Attributes["operation"].Value;
                // Go... do your stuff...
                if (!sqlite.UpdateOpertaion(tcmId, operation))
                {
                    //EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.UpdateSQLite: Couldn't update SQLite status for {0} (original operation {1})", tcmId, operation));
                    LogMessage(string.Format("FASXMLUtils.UpdateSQLite: Couldn't update SQLite status for {0} (original operation {1})", tcmId, operation), LoggerMessageLevel.Error);

                    // If the operation is "delete", don't worry too much.
                    if (operation.ToLower() != "delete")
                    {
                        retStatus = Status.ErrorsEncountered;
                    }
                }
            }

            items = null;
            sqlite = null;
            GC.Collect();
            return retStatus;
        }

        public static Status xSendToFredHopper(XmlDocument fasXml, string targetFileName)
        {
            return SendToFredHopper(fasXml, targetFileName, null);
        }
        
        /// <summary>
        /// Sends an XmlDocument object to FredHopper via a call to the 
        /// SmartTarget Deployment Web Service.
        /// </summary>
        /// <param name="fasXml">The XmlDocument object to send</param>
        /// <param name="targetFileName">The name to be given to the generated xml file on the FredHopper server</param>
        /// <returns>Status</returns>
        public static Status SendToFredHopper(XmlDocument fasXml, string targetFileName, ILog logger)
        {
            // ### ASH: REMOVE ###
            //return Status.Success;
            // ### ASH: REMOVE ###

            string fhTargetFileName = string.Format("{0}-{1}", DateTime.Now.ToString("yyyyMMdd-HHmmss"), targetFileName);
            if (logger != null)
            {
                logger.InfoFormat("FASXMLUtils.SendToFredHopper : Sending FASXML to FredHopper as {0}", fhTargetFileName);
            }

            // Send to FredHopper via WCF
            using (var client = new SmartTargetDeploymentClient())
            {
                try
                {
                    // No documentation for this WCF service, so not sure if Open is required?
                    client.Open();

                    // Deploy!
                    //bool deploySuccess = client.deploy(Encoding.Default.GetBytes(fasXml.OuterXml), fhTargetFileName);
                    
                    // Check for da BOM (Byte Order Marker)
                    // Actually, we are ommitting it here.
                    UTF8Encoding utf8EmitBOM = new UTF8Encoding(true);
                    
                    
                    bool deploySuccess = client.deploy(utf8EmitBOM.GetBytes(fasXml.OuterXml), fhTargetFileName);

                    // No documentation for this WCF service, so not sure if Close is required?
                    client.Close();

                    if (!deploySuccess)
                    {
                        //EventLog.WriteEntry("FASXMLUtils", "FASXMLUtils.SendToFredHopper: FAILED");
                        if (logger != null)
                        {
                            logger.Error("FASXMLUtils.SendToFredHopper : FAILED");
                        }
                        return Status.ErrorsEncountered;
                    }

                    // WORKS!!! WHY???
                    //XmlDocument xml = new XmlDocument();
                    //xml.Load("C:/Projects/FredHopperPOC/FredHopperPOC/Content/ST_Test.xml");
                    //client.Open();
                    //bool status2 = client.deploy(Encoding.Default.GetBytes(xml.OuterXml), "Brian_Epstein");
                    //client.Close();
                }
                catch (Exception ex)
                {
                    //EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.SendToFredHopper: Exception {0}", ex.ToString()));
                    if (logger != null)
                    {
                        logger.ErrorFormat("FASXMLUtils.SendToFredHopper: Exception {0}", ex.ToString());
                    }
                    return Status.ErrorsEncountered;
                }
            }
            
            GC.Collect();

            return Status.Success;
        }

        /// <summary>
        /// Simple file move, wrapped with try/catch 
        /// </summary>
        /// <param name="sourceFile">Source file path</param>
        /// <param name="destinationFile">Destination file path</param>
        /// <returns>Status</returns>
        private Status MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                // To move a file or folder to a new location:
                //EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.MoveFile: Moving {0} to {1}", sourceFile, destinationFile));
                LogMessage(string.Format("FASXMLUtils.MoveFile: Moving {0} to {1}", sourceFile, destinationFile), LoggerMessageLevel.Info);
                System.IO.File.Move(sourceFile, destinationFile);
                return Status.Success;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.MoveFile: Exception {0}", ex.ToString()));
                LogMessage(string.Format("FASXMLUtils.MoveFile: Exception {0}", ex.ToString()), LoggerMessageLevel.Error);
                return Status.ErrorsEncountered;
            }
        }

        /// <summary>
        /// Applies XSLT to tranform and extend the incoming FAS XML.
        /// The XSLT file path is set in the app.config in the FASXmlTransformXsltPath setting
        /// </summary>
        /// <param name="fasXml">XmlDocument containing the incoming FAS XML</param>
        /// <param name="transformedFASXml">Output XmlDocument containing the transformed FAS XML</param>
        /// <returns>Resulting Status of the stransformation</returns>
        private Status Transform(XmlDocument fasXml, out XmlDocument transformedFASXml)
        {
            transformedFASXml = null;
            transformedFASXml = new XmlDocument();

            try
            {
                #region Current

                LogMessage("FASXMLUtils.Transform : start", LoggerMessageLevel.Debug);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XsltSettings settings = new XsltSettings(false, true);

                    //System.Xml.Xsl.XslCompiledTransform xsl = new System.Xml.Xsl.XslCompiledTransform(true);
                    //System.Xml.Xsl.XslCompiledTransform xsl = new System.Xml.Xsl.XslCompiledTransform(false);

                    XmlUrlResolver xmlUrlResolver = new XmlUrlResolver();
                    //xsl.Load(Properties.Settings.Default.FASXmlTransformXsltPath, settings, xmlUrlResolver);
                    CompiledTransform.Load(Properties.Settings.Default.FASXmlTransformXsltPath, settings, xmlUrlResolver);


                    // Inject the SQLite extension object, ready for use by the xslt
                    XsltArgumentList extensionObj = new XsltArgumentList();
                    
                    SQLiteDatabase sqliteDB = new SQLiteDatabase();
                    extensionObj.AddExtensionObject("urn:sqlite-database", sqliteDB);

                    // Inject the XsltExtensionUtils extension object, ready for use by the xslt
                    XsltExtensionUtils extUtils = new XsltExtensionUtils();
                    extensionObj.AddExtensionObject("urn:extension-utils", extUtils);

                    //MemoryStream memoryStream = new MemoryStream();
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Encoding = new UTF8Encoding(false, true);
                    xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                    xmlWriterSettings.Indent = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                    {
                        //xsl.Transform(fasXml, extensionObj, xmlWriter);
                        CompiledTransform.Transform(fasXml, extensionObj, xmlWriter);

                        string xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());

                        transformedFASXml.LoadXml(xmlString);

                        // Free up the memory stream and force Garbage Collection
                        settings = null;
                        //xsl = null;
                        xmlUrlResolver = null;
                        //memoryStream.Dispose();
                        extensionObj.Clear();
                        extensionObj = null;
                        sqliteDB = null;
                        extUtils = null;
                        //xmlWriterSettings = null;
                        xmlString = string.Empty;
                    }
                }
                GC.Collect();

                LogMessage("FASXMLUtils.Transform : end", LoggerMessageLevel.Debug);

                #endregion
            }
            catch(Exception ex)
            {
                //EventLog.WriteEntry("FASXMLUtils", string.Format("FASXMLUtils.Transform: Exception {0}", ex.ToString()));
                LogMessage(string.Format("FASXMLUtils.Transform : Exception {0}", ex.ToString()), LoggerMessageLevel.Error);
                transformedFASXml = null;
                return Status.ErrorsEncountered;
            }

            return Status.Success;
        }

        enum LoggerMessageLevel
        {
            Debug,
            Error,
            Fatal,
            Info,
            Warn
        }

        private void LogMessage(string message, LoggerMessageLevel level)
        {
            if (_logger != null)
            {
                switch (level)
                {
                    case LoggerMessageLevel.Error:
                        _logger.Error(message);
                        break;
                    case LoggerMessageLevel.Fatal:
                        _logger.Fatal(message);
                        break;
                    case LoggerMessageLevel.Info:
                        _logger.Info(message);
                        break;
                    case LoggerMessageLevel.Warn:
                        _logger.Warn(message);
                        break;
                    case LoggerMessageLevel.Debug:
                    default:
                        _logger.Debug(message);
                        break;
                }
            }
        }


    }
}
