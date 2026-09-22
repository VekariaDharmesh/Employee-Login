using System;
using System.Web.UI;

namespace EmployeeApp
{
    public partial class Welcome : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // check if employee session exists, otherwise redirect to login
            if (Session["EmployeeId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // populate employee details on first load
            if (!IsPostBack)
            {
                litFullName.Text = Session["FullName"] != null ? Session["FullName"].ToString() : "Employee";
                litEmployeeCode.Text = Session["EmployeeCode"] != null ? Session["EmployeeCode"].ToString() : "-";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // clear session data and end the current session
            Session.Clear();
            Session.Abandon();

            // redirect back to sign in page
            Response.Redirect("Login.aspx");
        }
    }
}
