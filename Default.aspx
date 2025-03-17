<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DuesManager.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid py-4">
        <h2 class="text-center mb-4">📊 DuesManager Dashboard</h2>

        <div class="row g-4">
            <div class="col-md-4">
                <div class="card shadow-lg rounded-4">
                    <div class="card-body text-center">
                        <h5 class="card-title">Total Payments</h5>
                        <h3 class="text-success">
                            <asp:Literal ID="litTotalPayments" runat="server" />
                        </h3>
                        <i class="fas fa-dollar-sign fa-2x text-muted"></i>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card shadow-lg rounded-4">
                    <div class="card-body text-center">
                        <h5 class="card-title">Payments This Month</h5>
                        <h3 class="text-primary">
                            <asp:Literal ID="litPaymentsThisMonth" runat="server" />
                        </h3>
                        <i class="fas fa-calendar-alt fa-2x text-muted"></i>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card shadow-lg rounded-4">
                    <div class="card-body text-center">
                        <h5 class="card-title">Active Members</h5>
                        <h3 class="text-info">
                            <asp:Literal ID="litActiveMembers" runat="server" />
                        </h3>
                        <i class="fas fa-users fa-2x text-muted"></i>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-4 mt-4">
            <div class="col-md-6">
                <div class="card shadow-lg rounded-4">
                    <div class="card-body">
                        <h5 class="card-title">Monthly Payments (Last 6 Months)</h5>
                        <canvas id="paymentsChart"></canvas>
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card shadow-lg rounded-4">
                    <div class="card-body">
                        <h5 class="card-title">Member Distribution</h5>
                        <canvas id="membersChart"></canvas>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        const paymentsCtx = document.getElementById('paymentsChart').getContext('2d');
        const paymentsChart = new Chart(paymentsCtx, {
            type: 'line',
            data: {
                labels: <%= MonthlyLabels %>,
                datasets: [{
                    label: 'Payments',
                    data: <%= MonthlyPayments %>,
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 2,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        const membersCtx = document.getElementById('membersChart').getContext('2d');
        const membersChart = new Chart(membersCtx, {
            type: 'doughnut',
            data: {
                labels: <%= MemberTypesLabels %>,
                datasets: [{
                    label: 'Members',
                    data: <%= MemberTypesData %>,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.7)',
                        'rgba(54, 162, 235, 0.7)',
                        'rgba(255, 206, 86, 0.7)'
                    ],
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true
            }
        });
    </script>

</asp:Content>