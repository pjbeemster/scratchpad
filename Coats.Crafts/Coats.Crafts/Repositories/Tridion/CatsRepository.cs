using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text;

using Castle.Core.Logging;

using Tridion.ContentDelivery.UGC.WebService;
using Tridion.ContentDelivery.Web.Utilities;
using System.Web.Script.Serialization;

using Coats.Crafts.Configuration;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Data;
using System.Net;
using System.Collections.Specialized;
using System.Data.Services.Client;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Models;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Xml;


namespace Coats.Crafts.Repositories.Tridion
{
    public class CatsRepository : ICatsRepository
    {
        private IAppSettings _settings;

        public CatsRepository(IAppSettings settings)
        {
            _settings = settings;
        }

        public ILogger Logger { get; set; }

        public bool SaveCatsFormData(CatsContactForm formData) {

            Logger.Info("SaveCatsFormData()");

            SqlCommand cmd = null;
            SqlConnection conn = null;

            string connection = ConfigurationManager.ConnectionStrings["CatsDb"].ToString();
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Connection: {0}", connection);

            string formDataString = this.SerializeFormForLogging(formData);

            try
            {
                conn = new SqlConnection(connection);
                conn.Open();
                cmd = new SqlCommand(WebConfiguration.Current.CatsInsertSQL);
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@datesent", formData.dateSubmitted);
                cmd.Parameters.AddWithValue("@title", formData.Title);
                cmd.Parameters.AddWithValue("@fname", formData.FirstName);
                cmd.Parameters.AddWithValue("@lname", formData.LastName);
                cmd.Parameters.AddWithValue("@address1", formData.Address1);
                cmd.Parameters.AddWithValue("@address2", formData.Address2);
                cmd.Parameters.AddWithValue("@city", formData.City);
                cmd.Parameters.AddWithValue("@state", formData.State);
                cmd.Parameters.AddWithValue("@zipcode", formData.ZipCode);
                cmd.Parameters.AddWithValue("@country", formData.Country);
                cmd.Parameters.AddWithValue("@phone", formData.TelephoneNumber);
                cmd.Parameters.AddWithValue("@email", formData.EmailAddress);
                cmd.Parameters.AddWithValue("@prodtype", formData.ProductType);
                cmd.Parameters.AddWithValue("@proddescription", formData.ProductDescription);
                cmd.Parameters.AddWithValue("@questiontype", formData.QuestionType);
                cmd.Parameters.AddWithValue("@comments", formData.Comments);
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
                Logger.Error("CatsRepository: Could not save contact data in database form, Error: " + ex + " FormData: " + formDataString);
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

        private string SerializeFormForLogging(CatsContactForm formData)
        {
            //get rid of cp to stop it polluting formDataString
            formData.cp = null;

            return new JavaScriptSerializer().Serialize(formData);
        }
    }
}