using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using DuesManager.Helpers;
using System.Web;

namespace DuesManager.Pages.Payments
{
    public partial class Payments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SortExpression"] = null;
                ViewState["SortDirection"] = null;
                Rebind();

                txtPaymentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtDateFrom.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                txtDateTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void Rebind()
        {
            LoadPayments();
            LoadMembersForDropdown();
            LoadMembersForModalDropdown();
            LoadPaymentSummaries();
        }

        private void LoadPayments()
        {
            GridViewPayments.DataSource = GetPaymentsData();
            GridViewPayments.DataBind();
        }

        private DataTable GetPaymentsData()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT p.PaymentID, 
                           m.FirstName + ' ' + m.LastName AS MemberName, 
                           p.Amount, 
                           p.PaymentDate, 
                           p.Notes
                    FROM Payments p
                    INNER JOIN Members m ON p.MemberID = m.MemberID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        private void LoadPaymentSummaries()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Total Payments
                string totalQuery = "SELECT ISNULL(SUM(Amount), 0) FROM Payments";
                SqlCommand totalCmd = new SqlCommand(totalQuery, conn);
                decimal totalPayments = (decimal)totalCmd.ExecuteScalar();

                // Payments This Month
                string monthQuery = @"
                    SELECT ISNULL(SUM(Amount), 0) 
                    FROM Payments 
                    WHERE MONTH(PaymentDate) = @CurrentMonth AND YEAR(PaymentDate) = @CurrentYear";
                SqlCommand monthCmd = new SqlCommand(monthQuery, conn);
                monthCmd.Parameters.AddWithValue("@CurrentMonth", DateTime.Now.Month);
                monthCmd.Parameters.AddWithValue("@CurrentYear", DateTime.Now.Year);

                decimal paymentsThisMonth = (decimal)monthCmd.ExecuteScalar();

                litTotalPayments.Text = totalPayments.ToString("C");
                litPaymentsThisMonth.Text = paymentsThisMonth.ToString("C");
            }
        }

        private void LoadMembersForDropdown()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT DISTINCT m.MemberID, m.FirstName + ' ' + m.LastName AS FullName FROM Members m INNER JOIN Payments p ON p.MemberID = m.MemberID";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlMembers.DataSource = reader;
                ddlMembers.DataTextField = "FullName";
                ddlMembers.DataValueField = "MemberID";
                ddlMembers.DataBind();

                ddlMembers.Items.Insert(0, new ListItem("-- Select Member --", ""));
            }
        }

        private void LoadMembersForModalDropdown()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MemberID, FirstName + ' ' + LastName AS FullName FROM Members";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlModalMembers.DataSource = reader;
                ddlModalMembers.DataTextField = "FullName";
                ddlModalMembers.DataValueField = "MemberID";
                ddlModalMembers.DataBind();

                ddlModalMembers.Items.Insert(0, new ListItem("-- Select Member --", ""));
            }
        }

        protected void GridViewPayments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                decimal paymentAmount = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Amount"));
                var lblAmount = e.Row.Cells[2];

                lblAmount.Text = "$" + Math.Abs(paymentAmount).ToString("N2");

                if (paymentAmount > 1000)
                {
                    e.Row.CssClass = "table-warning";
                }
                else if (paymentAmount < 0)
                {
                    e.Row.CssClass = "table-danger";
                }
            }
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

        protected void GridViewPayments_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = GetPaymentsData();

            if (dt != null)
            {
                string sortDirection = GetSortDirection(e.SortExpression);

                DataView dv = new DataView(dt);
                dv.Sort = e.SortExpression + " " + sortDirection;
                AddSortArrows(e.SortExpression, sortDirection);

                GridViewPayments.DataSource = dv;
                GridViewPayments.DataBind();
            }
        }

        private void AddSortArrows(string sortExpression, string sortDirection)
        {
            foreach (DataControlField column in GridViewPayments.Columns)
            {
                switch (column.SortExpression)
                {
                    case "MemberName":
                        column.HeaderText = "Member";
                        break;
                    case "Amount":
                        column.HeaderText = "Amount";
                        break;
                    case "PaymentDate":
                        column.HeaderText = "Date";
                        break;
                    case "Notes":
                        column.HeaderText = "Notes";
                        break;
                }

                if (column.SortExpression == sortExpression)
                {
                    string arrow = (sortDirection == "ASC") ? " <i class='fas fa-arrow-up'></i>" : " <i class='fas fa-arrow-down'></i>";
                    column.HeaderText += arrow;
                }
            }
        }


        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string memberID = ddlMembers.SelectedValue;
            string dateFrom = txtDateFrom.Text;
            string dateTo = txtDateTo.Text;

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
            WHERE 1=1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(memberID))
                {
                    query += " AND m.MemberID = @MemberID";
                    cmd.Parameters.AddWithValue("@MemberID", memberID);
                }

                if (!string.IsNullOrEmpty(dateFrom))
                {
                    query += " AND p.PaymentDate >= @DateFrom";
                    cmd.Parameters.AddWithValue("@DateFrom", DateTime.Parse(dateFrom));
                }

                if (!string.IsNullOrEmpty(dateTo))
                {
                    query += " AND p.PaymentDate <= @DateTo";
                    cmd.Parameters.AddWithValue("@DateTo", DateTime.Parse(dateTo));
                }

                query += " ORDER BY p.PaymentDate DESC";

                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridViewPayments.DataSource = dt;
                GridViewPayments.DataBind();
            }

            LoadPaymentSummaries();

            btnClearFilters.Enabled =
                !string.IsNullOrEmpty(memberID) ||
                !string.IsNullOrEmpty(dateFrom) ||
                !string.IsNullOrEmpty(dateTo);
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            ddlMembers.SelectedIndex = 0;
            txtDateFrom.Text = "";
            txtDateTo.Text = "";

            LoadPayments();
            LoadPaymentSummaries();

            btnClearFilters.Enabled = false;
        }


        protected void btnSavePayment_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int memberID = int.Parse(ddlModalMembers.SelectedValue);
                decimal paymentAmount = decimal.Parse(txtPaymentAmount.Text.Trim());
                DateTime paymentDate = DateTime.Parse(txtPaymentDate.Text.Trim());
                string notes = txtPaymentNotes.Text.Trim();

                string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "INSERT INTO Payments (MemberID, Amount, PaymentDate, Notes) VALUES (@MemberID, @Amount, @PaymentDate, @Notes)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MemberID", memberID);
                    cmd.Parameters.AddWithValue("@Amount", paymentAmount);
                    cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@Notes", notes);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                LoadPayments();
                LoadPaymentSummaries();

                ddlModalMembers.SelectedIndex = 0;
                txtPaymentAmount.Text = "";
                txtPaymentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtPaymentNotes.Text = "";

                PageHelper.CompleteDataOperation(HttpContext.Current);
            }
        }

        protected void GridViewPayments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int paymentID = Convert.ToInt32(GridViewPayments.DataKeys[e.RowIndex].Value);

            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Payments WHERE PaymentID = @PaymentID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@PaymentID", paymentID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            PageHelper.CompleteDataOperation(HttpContext.Current);
        }

        protected void GridViewPayments_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewPayments.EditIndex = e.NewEditIndex;
            Rebind();
        }

        protected void GridViewPayments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewPayments.EditIndex = -1;
            Rebind();
        }

        protected void GridViewPayments_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int paymentID = Convert.ToInt32(GridViewPayments.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridViewPayments.Rows[e.RowIndex];

            TextBox txtAmount = (TextBox)row.FindControl("txtEditAmount");
            TextBox txtPaymentDate = (TextBox)row.FindControl("txtEditPaymentDate");
            TextBox txtNotes = (TextBox)row.FindControl("txtEditNotes");

            string amountText = txtAmount.Text.Trim();
            string paymentDateText = txtPaymentDate.Text.Trim();
            string notes = txtNotes.Text.Trim();

            decimal amount;
            DateTime paymentDate;

            if (!decimal.TryParse(amountText, out amount)) return;
            if (!DateTime.TryParse(paymentDateText, out paymentDate)) return;

            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    UPDATE Payments
                    SET Amount = @Amount,
                        PaymentDate = @PaymentDate,
                        Notes = @Notes
                    WHERE PaymentID = @PaymentID";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                cmd.Parameters.AddWithValue("@Notes", notes);
                cmd.Parameters.AddWithValue("@PaymentID", paymentID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            PageHelper.CompleteDataOperation(HttpContext.Current);
        }
    }
}