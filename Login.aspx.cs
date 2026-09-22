using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace EmployeeApp
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if already logged in, redirect straight to dashboard
            if (!IsPostBack && Session["EmployeeId"] != null)
            {
                Response.Redirect("Welcome.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string employeeCode = txtEmployeeCode.Text.Trim();
            string password = txtPassword.Text.Trim();

            // validate user inputs
            if (string.IsNullOrEmpty(employeeCode) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please enter both employee code and password.";
                return;
            }

            // retrieve connection string from Web.config
            string connStr = ConfigurationManager.ConnectionStrings["EmployeeDBConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // parameterized query to prevent sql injection
                    string query = "SELECT EmployeeId, FullName, EmployeeCode " +
                                   "FROM Employees " +
                                   "WHERE EmployeeCode = @EmployeeCode AND Password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 50) { Value = employeeCode });
                        cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = password });

                        // execute reader for connected data retrieval
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // credentials verified, store user details in session
                                Session["EmployeeId"] = reader["EmployeeId"].ToString();
                                Session["FullName"] = reader["FullName"].ToString();
                                Session["EmployeeCode"] = reader["EmployeeCode"].ToString();

                                Response.Redirect("Welcome.aspx");
                            }
                            else
                            {
                                lblMessage.Text = "Invalid employee code or password.";
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // generic user friendly message without exposing database details
                    lblMessage.Text = "Something went wrong. Please try again.";
                }
            }
        }
    }
}
