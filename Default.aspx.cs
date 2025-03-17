using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using Newtonsoft.Json;

namespace DuesManager
{
    public partial class Default : Page
    {
        protected string MonthlyLabels;
        protected string MonthlyPayments;
        protected string MemberTypesLabels;
        protected string MemberTypesData;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DuesManagerDB"].ConnectionString;

            decimal totalPayments = 0;
            decimal paymentsThisMonth = 0;
            int activeMembers = 0;

            var months = new List<string>();
            var payments = new List<decimal>();

            var memberTypesCounts = new Dictionary<string, int>
    {
        { "Active", 0 },
        { "Pending", 0 },
        { "Inactive", 0 }
    };

            Random rand = new Random();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Total payments
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Amount), 0) FROM Payments", conn))
                {
                    totalPayments = (decimal)cmd.ExecuteScalar();
                }

                // Payments this month
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT ISNULL(SUM(Amount), 0)
            FROM Payments
            WHERE MONTH(PaymentDate) = @Month AND YEAR(PaymentDate) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", DateTime.Now.Month);
                    cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);

                    paymentsThisMonth = (decimal)cmd.ExecuteScalar();
                }

                // Get all members
                using (SqlCommand cmd = new SqlCommand("SELECT MemberID FROM Members", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int roll = rand.Next(100); // Random number 0-99

                            string status;
                            if (roll < 60)
                            {
                                status = "Active";
                                activeMembers++;
                            }
                            else if (roll < 90)
                            {
                                status = "Pending";
                            }
                            else
                            {
                                status = "Inactive";
                            }

                            memberTypesCounts[status]++;
                        }
                    }
                }

                // Payments by month (last 6 months)
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT 
                FORMAT(PaymentDate, 'yyyy-MM') AS Month,
                SUM(Amount) AS TotalAmount
            FROM Payments
            WHERE PaymentDate >= DATEADD(MONTH, -5, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
            GROUP BY FORMAT(PaymentDate, 'yyyy-MM')
            ORDER BY Month", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string month = reader.GetString(0);
                            decimal amount = reader.GetDecimal(1);

                            DateTime dt = DateTime.ParseExact(month + "-01", "yyyy-MM-dd", null);
                            months.Add($"\"{dt.ToString("MMM yyyy")}\"");
                            payments.Add(amount);
                        }
                    }
                }

                conn.Close();
            }

            // Assign values to the UI components
            litTotalPayments.Text = totalPayments.ToString("C");
            litPaymentsThisMonth.Text = paymentsThisMonth.ToString("C");
            litActiveMembers.Text = activeMembers.ToString();

            // Convert dictionaries to lists for chart JS data
            MemberTypesLabels = "[" + string.Join(", ", memberTypesCounts.Keys.Select(k => $"\"{k}\"")) + "]";
            MemberTypesData = "[" + string.Join(", ", memberTypesCounts.Values) + "]";

            MonthlyLabels = "[" + string.Join(", ", months) + "]";
            MonthlyPayments = "[" + string.Join(", ", payments) + "]";
        }

    }
}
