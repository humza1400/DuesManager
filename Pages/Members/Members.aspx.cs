using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using DuesManager.Helpers;
using System.Web;

namespace DuesManager.Pages.Members
{
	public partial class Members : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            // LoadMembers only on first load
			if (!IsPostBack)
            {
				LoadMembers();
            }
        }

		private void LoadMembers()
		{
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Members";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridViewMembers.DataSource = dt;
                GridViewMembers.DataBind();
            }
        }

        protected void GridViewMembers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewMembers.EditIndex = e.NewEditIndex;
            LoadMembers();
        }

        protected void GridViewMembers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewMembers.EditIndex = -1;
            LoadMembers();
        }

        protected void GridViewMembers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int memberID = Convert.ToInt32(GridViewMembers.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridViewMembers.Rows[e.RowIndex];

            string firstName = ((TextBox)row.Cells[1].Controls[0]).Text;
            string lastName = ((TextBox)row.Cells[2].Controls[0]).Text;
            string email = ((TextBox)row.Cells[3].Controls[0]).Text;

            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE Members SET FirstName = @FirstName, LastName = @LastName, Email = @Email WHERE MemberID = @MemberID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@MemberID", memberID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            GridViewMembers.EditIndex = -1;
            LoadMembers();

            PageHelper.CompleteDataOperation(HttpContext.Current);
        }

        protected void GridViewMembers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int memberID = Convert.ToInt32(GridViewMembers.DataKeys[e.RowIndex].Value);

            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Members WHERE MemberID = @MemberID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@MemberID", memberID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            PageHelper.CompleteDataOperation(HttpContext.Current);
        }

        protected void btnSaveMember_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();

            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO Members (FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            PageHelper.CompleteDataOperation(HttpContext.Current);
        }
    }
}