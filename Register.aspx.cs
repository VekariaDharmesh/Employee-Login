using System;
using System.Configuration;
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

            // make sure all required fields are filled out
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(employeeCode) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please fill in all fields.";
                return;
            }

            // fetch db connection string
            string connStr = ConfigurationManager.ConnectionStrings["EmployeeDBConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // check if employee code or email is already in use
                    string checkQuery = "SELECT COUNT(*) FROM Employees WHERE EmployeeCode = @EmployeeCode OR Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        checkCmd.Parameters.AddWithValue("@Email", email);

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            lblMessage.Text = "Employee code or email is already registered.";
                            return;
                        }
                    }

                    // insert the new employee record
                    string insertQuery = "INSERT INTO Employees (FullName, EmployeeCode, Email, Password) " +
                                          "VALUES (@FullName, @EmployeeCode, @Email, @Password)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FullName", fullName);
                        insertCmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Password", password);

                        insertCmd.ExecuteNonQuery();
                    }

                    // account created successfully, navigate to login
                    Response.Redirect("Login.aspx");
                }
                catch (Exception)
                {
                    // generic fallback error message
                    lblMessage.Text = "Unable to complete registration. Please try again.";
                }
            }
        }
    }
}
