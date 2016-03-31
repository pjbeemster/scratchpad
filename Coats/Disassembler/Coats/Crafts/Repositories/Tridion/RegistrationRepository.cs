namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;

    public class RegistrationRepository : IRegistrationRepository
    {
        private IAppSettings _settings;

        public RegistrationRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        public bool checkDisplayNameExists(string displayname)
        {
            Exception exception;
            bool flag = true;
            try
            {
                using (CoatsCraftsIntegrationServiceContractClient client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        flag = client.checkDisplayNameExists(displayname);
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        this.Logger.Error(string.Format("{0} {1}", "checkDisplayNameExists", exception));
                    }
                    return flag;
                }
            }
            catch (Exception exception2)
            {
                exception = exception2;
                this.Logger.Error(string.Format("{0} {1}", "checkDisplayNameExists", exception));
            }
            return flag;
        }

        public bool checkEmailAddressExists(string RegisteredEmailAddress)
        {
            bool flag = false;
            this.Logger.Info("checkEmailAddressExists()");
            SqlCommand command = null;
            SqlConnection connection = null;
            string connectionString = ConfigurationManager.ConnectionStrings["CatsRegisterDb"].ToString();
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("Connection: {0}", new object[] { connectionString });
            }
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                command = new SqlCommand("select COUNT(*) from dbo.RegisterData where RegisteredEmailAddress like '%" + RegisteredEmailAddress + "%'") {
                    Connection = connection
                };
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                {
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error("RegistrationRepository: Could not connect to database , Error: " + exception);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return flag;
        }

        public bool SaveRegisterData(string RegisteredEmailAddress, string IPAdressofRegister, string IPAdressofConfirmer)
        {
            this.Logger.Info("SaveRegisterData()");
            SqlCommand command = null;
            SqlConnection connection = null;
            string connectionString = ConfigurationManager.ConnectionStrings["CatsRegisterDb"].ToString();
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("Connection: {0}", new object[] { connectionString });
            }
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                command = new SqlCommand(WebConfiguration.Current.RegisterAddSP) {
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@RegisteredEmailAddress", RegisteredEmailAddress);
                command.Parameters.AddWithValue("@IPAdressofRegister", IPAdressofRegister);
                command.Parameters.AddWithValue("@IPAdressofConfirmer", IPAdressofConfirmer);
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
                this.Logger.Error("RegistrationRepository: Could not save contact data in database form, Error: " + exception);
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

        public ILogger Logger { get; set; }
    }
}

