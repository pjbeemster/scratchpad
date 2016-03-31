using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Logging;
using Coats.Crafts.Data;
using Tridion.ContentDelivery.UGC.WebService;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Web.Mvc;
using Coats.Crafts.Gateway;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Coats.Crafts.Configuration;
using Coats.Crafts.Models;
using DD4T.ContentModel;
using DD4T.Factories;
using DD4T.ContentModel.Factories;
using DD4T.Providers.WCFServices.CoatsTridionComponentServiceRef;
using Castle.Windsor;
using Coats.Crafts.HtmlHelpers;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Coats.Crafts.Repositories.Tridion
{
    public class RegistrationRepository : IRegistrationRepository
    {
        public ILogger Logger { get; set; }
        private IAppSettings _settings;
        

        public RegistrationRepository(IAppSettings settings)
        {
            _settings = settings;
        }

        public bool checkDisplayNameExists(string displayname)
        {
            bool checkDisplayName = true;

            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        checkDisplayName = client.checkDisplayNameExists(displayname);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("{0} {1}", "checkDisplayNameExists", ex));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("{0} {1}", "checkDisplayNameExists", ex));
            }

            return checkDisplayName;
        }


        public bool checkEmailAddressExists(string RegisteredEmailAddress)
        {
            bool IsExist = false;
            Logger.Info("checkEmailAddressExists()");

            SqlCommand cmd = null;
            SqlConnection conn = null;

            string connection = ConfigurationManager.ConnectionStrings["CatsRegisterDb"].ToString();
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Connection: {0}", connection);

            try
            {
                conn = new SqlConnection(connection);
                conn.Open();
                cmd = new SqlCommand("select COUNT(*) from dbo.RegisterData where RegisteredEmailAddress like '%"+RegisteredEmailAddress+"%'");
                
                cmd.Connection = conn;
               
               
                int rowsCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (rowsCount > 0)
                {
                    IsExist = true;
                }
                
                
            }
            catch (Exception ex)
            {
                Logger.Error("RegistrationRepository: Could not connect to database , Error: " + ex);
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return IsExist;
        }

        public bool SaveRegisterData(string RegisteredEmailAddress, string IPAdressofRegister, string IPAdressofConfirmer)
        {

            Logger.Info("SaveRegisterData()");

            SqlCommand cmd = null;
            SqlConnection conn = null;

            string connection = ConfigurationManager.ConnectionStrings["CatsRegisterDb"].ToString();
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Connection: {0}", connection);

            try
            {
                conn = new SqlConnection(connection);
                conn.Open();
                cmd = new SqlCommand(WebConfiguration.Current.RegisterAddSP);
                cmd.CommandType=CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@RegisteredEmailAddress", RegisteredEmailAddress);
                cmd.Parameters.AddWithValue("@IPAdressofRegister", IPAdressofRegister);
                cmd.Parameters.AddWithValue("@IPAdressofConfirmer", IPAdressofConfirmer);
                 
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new Exception("Incorrect number of rows affected (should be 1) RowsAffected: " + rowsAffected);
                }

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("rowsAffected: {0}", rowsAffected);
            }
            catch (Exception ex)
            {
                Logger.Error("RegistrationRepository: Could not save contact data in database form, Error: " + ex);
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return true;
        }

    }
}