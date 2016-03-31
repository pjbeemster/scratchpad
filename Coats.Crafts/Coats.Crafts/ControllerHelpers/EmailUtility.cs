using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Logging;
using System.Xml.Linq;
using Coats.IndustrialPortal.Constants;
using Coats.IndustrialPortal.Gateway;
using System.IO;
using Coats.IndustrialPortal.Gateway.CoatsIntegrationService;
using Coats.Crafts.Utils;
using Castle.Windsor;
using Amaze.Net4.Framework.Services.Impl;

namespace Coats.Crafts.ControllerHelpers
{
    public class EmailUtility
    {
        public EmailUtility()
        {
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            var logger = accessor.Container.Resolve<ILogger>();

            logger.Debug("Calling EmailUtility constructor>>>>>>");

            accessor.Container.Release(logger);
        }

        public ILogger Logger { get; set; }

        public string GetEmailBody<T>(T model, string templateString)
        {
            var tps = new TemplateParserService();

            var emailString = tps.ParseTemplate(templateString, string.Empty, model, string.Empty);

            return emailString;
        }

        public static string GetEmailBody(string processName, string appDataPath)
        {
            string emailBody = string.Empty;
            var item = GetXmlDocFromAppData(processName, appDataPath);
            var xElement = item.Element(EmailKeys.EMAIL);
            if (xElement != null)
            {
                var element = xElement.Element(EmailKeys.EMAILBODY);
                if (element != null)
                {
                    emailBody = element.Value.Trim();

                }
            }

            return emailBody;

        }

        private static XDocument GetXmlDocFromAppData(string processName, string appDataPath)
        {
            var file = Path.Combine(appDataPath, processName + ".xml");
            var item = XDocument.Load(file);
            return item;
        }

        public Dictionary<string, string> GetEmailTemplateValues(string processName, string appDataPath)
        {
            var xdoc = GetXmlDocFromAppData(processName, appDataPath);

            return xdoc.Descendants(EmailKeys.EMAIL).Descendants().ToDictionary(item => item.Name.ToString(), item => item.Value);
        }



        /// <summary>
        /// Send an email using the specified address.  Uses the vales in the model object to populate the placeholders in the 
        /// specified email template
        /// </summary>
        /// <param name="model"></param>
        /// <param name="emailTemplate"></param>
        /// <param name="fromEmailAddress"></param>
        /// <param name="toEmailAddress"></param>
        /// <returns></returns>
        public string SendEmail(object model, string emailTemplate, string fromEmailAddress, string toEmailAddress)
        {

            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            var logger = accessor.Container.Resolve<ILogger>();

            logger.Debug("Calling EmailUtility constructor>>>>>>");



            logger.Debug("Calling EmailUtility >>>>>>");
            logger.DebugFormat("Calling EmailUtility emailTemplate {0}", emailTemplate);
            logger.DebugFormat("Calling EmailUtility fromEmailAddress {0}", fromEmailAddress);
            logger.DebugFormat("Calling EmailUtility toEmailAddress {0}", toEmailAddress);

            try
            {
                var appDataPath = HttpContext.Current.Server.MapPath(Common.APPDATA);

                var templateValues = GetEmailTemplateValues(emailTemplate, appDataPath);
                var tokenisedEmailBody = TokenAdapter.ReplaceToken(templateValues[EmailKeys.EMAILBODY]);

                var emailtemplate = new EmailTemplate
                {
                    BodyText = GetEmailBody(model, tokenisedEmailBody),
                    DisplayName = templateValues[EmailKeys.DISPLAYNAME],
                    FromMailAddress = fromEmailAddress,
                    SubjectText = templateValues[EmailKeys.SUBJECTTEXT],
                    ToMailAddress = toEmailAddress
                };

                logger.Debug("EmailUtility sending email");

                return EmailGateway.Instance.SendEmail(emailtemplate);
            }
            catch (Exception ex)
            {
                Logger.Error("EmailUtility exception", ex);
                throw;
            }
            finally
            {

                accessor.Container.Release(logger);
            }
        }
    }
}