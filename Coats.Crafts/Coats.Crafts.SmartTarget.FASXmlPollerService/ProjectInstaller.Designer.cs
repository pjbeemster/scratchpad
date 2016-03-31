namespace Coats.Crafts.SmartTarget.FASXmlPollerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FASXmlPollerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FASXmlPollerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FASXmlPollerProcessInstaller
            // 
            this.FASXmlPollerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FASXmlPollerProcessInstaller.Password = null;
            this.FASXmlPollerProcessInstaller.Username = null;
            // 
            // FASXmlPollerInstaller
            // 
            this.FASXmlPollerInstaller.DisplayName = "FAS Xml Poller";
            this.FASXmlPollerInstaller.ServiceName = "FASXmlPoller";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FASXmlPollerProcessInstaller,
            this.FASXmlPollerInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FASXmlPollerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FASXmlPollerInstaller;
    }
}