namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;
    using System.Web.Script.Serialization;

    public class CatsRepository : ICatsRepository
    {
        private IAppSettings _settings;

        public CatsRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        public bool SaveCatsFormData(CatsContactForm formData)
        {
            this.Logger.Info("SaveCatsFormData()");
            SqlCommand command = null;
            SqlConnection connection = null;
            string connectionString = ConfigurationManager.ConnectionStrings["CatsDb"].ToString();
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("Connection: {0}", new object[] { connectionString });
            }
            string str2 = this.SerializeFormForLogging(formData);
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                command = new SqlCommand(WebConfiguration.Current.CatsInsertSQL) {
                    Connection = connection
                };
                command.Parameters.AddWithValue("@datesent", formData.dateSubmitted);
                command.Parameters.AddWithValue("@title", formData.Title);
                command.Parameters.AddWithValue("@fname", formData.FirstName);
                command.Parameters.AddWithValue("@lname", formData.LastName);
                command.Parameters.AddWithValue("@address1", formData.Address1);
                command.Parameters.AddWithValue("@address2", formData.Address2);
                command.Parameters.AddWithValue("@city", formData.City);
                command.Parameters.AddWithValue("@state", formData.State);
                command.Parameters.AddWithValue("@zipcode", formData.ZipCode);
                command.Parameters.AddWithValue("@country", formData.Country);
                command.Parameters.AddWithValue("@phone", formData.TelephoneNumber);
                command.Parameters.AddWithValue("@email", formData.EmailAddress);
                command.Parameters.AddWithValue("@prodtype", formData.ProductType);
                command.Parameters.AddWithValue("@proddescription", formData.ProductDescription);
                command.Parameters.AddWithValue("@questiontype", formData.QuestionType);
                command.Parameters.AddWithValue("@comments", formData.Comments);
                int num = command.ExecuteNonQuery();
                if (num != 1)
                {
                    throw new Exception("Incorrect number of rows affected (should be 1) RowsAffected: " + num);
                }
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.DebugFormat("rowsAffected: {0}", new object[] { num });
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(string.Concat(new object[] { "CatsRepository: Could not save contact data in database form, Error: ", exception, " FormData: ", str2 }));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return true;
        }

        private string SerializeFormForLogging(CatsContactForm formData)
        {
            formData.cp = null;
            return new JavaScriptSerializer().Serialize(formData);
        }

        public ILogger Logger { get; set; }
    }
}

