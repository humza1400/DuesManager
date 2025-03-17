using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;

namespace DuesManager.Pages.Reports
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateFrom.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                txtDateTo.Text = DateTime.Now.ToString("yyyy-MM-dd");

                LoadMembers();
                LoadReport();
                LoadSummary();
            }
        }

        private void LoadMembers()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MemberID, FirstName + ' ' + LastName AS FullName FROM Members";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlMembers.DataSource = reader;
                ddlMembers.DataTextField = "FullName";
                ddlMembers.DataValueField = "MemberID";
                ddlMembers.DataBind();

                ddlMembers.Items.Insert(0, new ListItem("-- All Members --", ""));
            }
        }

        private void LoadReport(string sortExpression = null, string sortDirection = "ASC")
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = @"
                    SELECT p.PaymentID,
                           m.FirstName + ' ' + m.LastName AS MemberName,
                           p.Amount,
                           p.PaymentDate,
                           p.Notes
                    FROM Payments p
                    INNER JOIN Members m ON p.MemberID = m.MemberID
                    WHERE 1 = 1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(ddlMembers.SelectedValue))
                {
                    query += " AND p.MemberID = @MemberID";
                    cmd.Parameters.AddWithValue("@MemberID", ddlMembers.SelectedValue);
                }

                if (!string.IsNullOrEmpty(txtDateFrom.Text))
                {
                    query += " AND p.PaymentDate >= @DateFrom";
                    cmd.Parameters.AddWithValue("@DateFrom", DateTime.Parse(txtDateFrom.Text));
                }

                if (!string.IsNullOrEmpty(txtDateTo.Text))
                {
                    query += " AND p.PaymentDate <= @DateTo";
                    cmd.Parameters.AddWithValue("@DateTo", DateTime.Parse(txtDateTo.Text));
                }

                if (!string.IsNullOrEmpty(sortExpression))
                {
                    query += $" ORDER BY {sortExpression} {sortDirection}";
                }
                else
                {
                    query += " ORDER BY p.PaymentDate DESC";
                }

                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridViewReport.DataSource = dt;
                GridViewReport.DataBind();

                ViewState["ExportData"] = dt;
            }
        }

        private void LoadSummary()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE 1 = 1";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(ddlMembers.SelectedValue))
                {
                    query += " AND MemberID = @MemberID";
                    cmd.Parameters.AddWithValue("@MemberID", ddlMembers.SelectedValue);
                }

                if (!string.IsNullOrEmpty(txtDateFrom.Text))
                {
                    query += " AND PaymentDate >= @DateFrom";
                    cmd.Parameters.AddWithValue("@DateFrom", DateTime.Parse(txtDateFrom.Text));
                }

                if (!string.IsNullOrEmpty(txtDateTo.Text))
                {
                    query += " AND PaymentDate <= @DateTo";
                    cmd.Parameters.AddWithValue("@DateTo", DateTime.Parse(txtDateTo.Text));
                }

                cmd.CommandText = query;

                decimal totalPayments = (decimal)cmd.ExecuteScalar();
                litTotalPayments.Text = totalPayments.ToString("C");
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadReport();
            LoadSummary();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            ddlMembers.SelectedIndex = 0;
            txtDateFrom.Text = "";
            txtDateTo.Text = "";

            LoadReport();
            LoadSummary();
        }

        protected void GridViewReport_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;
            string sortDirection = GetSortDirection(sortExpression);

            LoadReport(sortExpression, sortDirection);
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null && sortExpression == column)
            {
                string lastDirection = ViewState["SortDirection"] as string;
                if (lastDirection != null && lastDirection == "ASC")
                {
                    sortDirection = "DESC";
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["ExportData"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                LoadReport();
                dt = ViewState["ExportData"] as DataTable;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i]);
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = row[i].ToString().Replace(",", " ");
                    sb.Append(value);
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            string fileName = "PaymentsReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
    }
}
