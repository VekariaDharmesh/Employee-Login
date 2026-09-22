using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace EmployeeApp
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if already logged in, skip login page and go straight to welcome dashboard
            if (!IsPostBack && Session["EmployeeId"] != null)
            {
                Response.Redirect("Welcome.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string employeeCode = txtEmployeeCode.Text.Trim();
            string password = txtPassword.Text.Trim();

            // validate inputs before calling database
            if (string.IsNullOrEmpty(employeeCode) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please enter both employee code and password.";
                return;
            }

            // read database connection string from web.config
            string connStr = ConfigurationManager.ConnectionStrings["EmployeeDBConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // query employee details with matched credentials
                    string query = "SELECT EmployeeId, FullName, EmployeeCode " +
                                   "FROM Employees " +
                                   "WHERE EmployeeCode = @EmployeeCode AND Password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // credentials verified, store user data in session
                                Session["EmployeeId"] = reader["EmployeeId"].ToString();
                                Session["FullName"] = reader["FullName"].ToString();
                                Session["EmployeeCode"] = reader["EmployeeCode"].ToString();

                                Response.Redirect("Welcome.aspx");
                            }
                            else
                            {
                                // credentials do not match
                                lblMessage.Text = "Invalid employee code or password.";
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // show user friendly error message if database connection fails
                    lblMessage.Text = "Unable to connect to the server. Please try again later.";
                }
            }
        }
    }
}
