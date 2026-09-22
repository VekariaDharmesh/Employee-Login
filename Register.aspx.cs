using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace EmployeeApp
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string employeeCode = txtEmployeeCode.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // validate required form fields
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(employeeCode) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please fill in all fields.";
                return;
            }

            // read database connection string from Web.config
            string connStr = ConfigurationManager.ConnectionStrings["EmployeeDBConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // verify employee code and email uniqueness
                    string checkQuery = "SELECT COUNT(*) FROM Employees WHERE EmployeeCode = @EmployeeCode OR Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.Add(new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 50) { Value = employeeCode });
                        checkCmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            lblMessage.Text = "Employee code or email is already registered.";
                            return;
                        }
                    }

                    // insert new employee using parameterized command
                    string insertQuery = "INSERT INTO Employees (FullName, EmployeeCode, Email, Password) " +
                                          "VALUES (@FullName, @EmployeeCode, @Email, @Password)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullName });
                        insertCmd.Parameters.Add(new SqlParameter("@EmployeeCode", SqlDbType.NVarChar, 50) { Value = employeeCode });
                        insertCmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
                        insertCmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = password });

                        insertCmd.ExecuteNonQuery();
                    }

                    // redirect to login after registration
                    Response.Redirect("Login.aspx");
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
