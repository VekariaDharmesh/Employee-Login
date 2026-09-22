using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace EmployeeApp
{
    public partial class Welcome : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // protect employee page: check if user session is active
            if (Session["EmployeeId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadEmployeeProfile();
            }
        }

        private void LoadEmployeeProfile()
        {
            string employeeId = Session["EmployeeId"].ToString();
            string connStr = ConfigurationManager.ConnectionStrings["EmployeeDBConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    // retrieve employee details using disconnected ADO.NET architecture
                    string query = "SELECT EmployeeId, FullName, EmployeeCode, Email, CreatedDate " +
                                   "FROM Employees " +
                                   "WHERE EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = Convert.ToInt32(employeeId) });

                        // use SqlDataAdapter to fill DataSet and DataTable
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds, "Employees");

                            DataTable dt = ds.Tables["Employees"];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                litFullName.Text = row["FullName"].ToString();
                                litEmployeeCode.Text = row["EmployeeCode"].ToString();
                                litEmail.Text = row["Email"].ToString();

                                if (row["CreatedDate"] != DBNull.Value)
                                {
                                    litJoinDate.Text = Convert.ToDateTime(row["CreatedDate"]).ToString("dd MMM yyyy");
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // show friendly message without exposing database details
                    lblMessage.Text = "Something went wrong. Please try again.";
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // clear session data and end the user session
            Session.Clear();
            Session.Abandon();

            Response.Redirect("Login.aspx");
        }
    }
}
