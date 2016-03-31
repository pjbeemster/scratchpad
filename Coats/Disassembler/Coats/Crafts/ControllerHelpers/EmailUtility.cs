namespace Coats.Crafts.ControllerHelpers
{
    using Amaze.Net4.Framework.Services.Impl;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Utils;
    using Coats.IndustrialPortal.Gateway;
    using Coats.IndustrialPortal.Gateway.CoatsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Xml.Linq;

    public class EmailUtility
    {
        public EmailUtility()
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger instance = applicationInstance.Container.Resolve<ILogger>();
            instance.Debug("Calling EmailUtility constructor>>>>>>");
            applicationInstance.Container.Release(instance);
        }

        public static string GetEmailBody(string processName, string appDataPath)
        {
            string str = string.Empty;
            XElement element = GetXmlDocFromAppData(processName, appDataPath).Element("email");
            if (element != null)
            {
                XElement element2 = element.Element("emailbody");
                if (element2 != null)
                {
                    str = element2.Value.Trim();
                }
            }
            return str;
        }

        public string GetEmailBody<T>(T model, string templateString)
        {
            TemplateParserService service = new TemplateParserService();
            return service.ParseTemplate<T>(templateString, string.Empty, model, string.Empty);
        }

        public Dictionary<string, string> GetEmailTemplateValues(string processName, string appDataPath)
        {
            return GetXmlDocFromAppData(processName, appDataPath).Descendants("email").Descendants<XElement>().ToDictionary<XElement, string, string>(item => item.Name.ToString(), item => item.Value);
        }

        private static XDocument GetXmlDocFromAppData(string processName, string appDataPath)
        {
            return XDocument.Load(Path.Combine(appDataPath, processName + ".xml"));
        }

        public string SendEmail(object model, string emailTemplate, string fromEmailAddress, string toEmailAddress)
        {
            string str3;
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger instance = applicationInstance.Container.Resolve<ILogger>();
            instance.Debug("Calling EmailUtility constructor>>>>>>");
            instance.Debug("Calling EmailUtility >>>>>>");
            instance.DebugFormat("Calling EmailUtility emailTemplate {0}", new object[] { emailTemplate });
            instance.DebugFormat("Calling EmailUtility fromEmailAddress {0}", new object[] { fromEmailAddress });
            instance.DebugFormat("Calling EmailUtility toEmailAddress {0}", new object[] { toEmailAddress });
            try
            {
                string appDataPath = HttpContext.Current.Server.MapPath("~/app_data");
                Dictionary<string, string> emailTemplateValues = this.GetEmailTemplateValues(emailTemplate, appDataPath);
                string templateString = TokenAdapter.ReplaceToken(emailTemplateValues["emailbody"]);
                EmailTemplate template = new EmailTemplate {
                    BodyText = this.GetEmailBody<object>(model, templateString),
                    DisplayName = emailTemplateValues["displayname"],
                    FromMailAddress = fromEmailAddress,
                    SubjectText = emailTemplateValues["subjecttext"],
                    ToMailAddress = toEmailAddress
                };
                instance.Debug("EmailUtility sending email");
                str3 = EmailGateway.Instance.SendEmail(template);
            }
            catch (Exception exception)
            {
                this.Logger.Error("EmailUtility exception", exception);
                throw;
            }
            finally
            {
                applicationInstance.Container.Release(instance);
            }
            return str3;
        }

        public ILogger Logger { get; set; }
    }
}

