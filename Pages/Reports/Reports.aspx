<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="DuesManager.Pages.Reports.Reports" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-3 d-flex justify-content-between align-items-center">
        Payment Reports
        <asp:Button ID="btnExportCSV" runat="server" Text="Export to CSV" CssClass="btn btn-success" OnClick="btnExportCSV_Click" />
    </h2>

    <!-- Filters -->
    <div class="filters mb-3 d-flex align-items-end">
        <!-- Member Filter -->
        <div class="me-2">
            <label for="ddlMembers">Member</label>
            <asp:DropDownList ID="ddlMembers" runat="server" CssClass="form-control" />
        </div>

        <!-- From Date Filter -->
        <div class="me-2">
            <label for="txtDateFrom">From Date</label>
            <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control" TextMode="Date" />
        </div>

        <!-- To Date Filter -->
        <div class="me-2">
            <label for="txtDateTo">To Date</label>
            <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control" TextMode="Date" />
        </div>

        <!-- Filter Button -->
        <div class="me-2">
            <label>&nbsp;</label>
            <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-secondary d-block" OnClick="btnFilter_Click" />
        </div>

        <!-- Clear Filters Button -->
        <div>
            <label>&nbsp;</label>
            <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" CssClass="btn btn-outline-secondary d-block" OnClick="btnClearFilters_Click" />
        </div>
    </div>

    <!-- Summary -->
    <div class="summary mb-3">
        <h4>Total Payments:
            <asp:Literal ID="litTotalPayments" runat="server" />
        </h4>
    </div>

    <!-- Payments Report Grid -->
    <asp:GridView
        ID="GridViewReport"
        runat="server"
        AutoGenerateColumns="false"
        CssClass="table table-hover table-bordered"
        DataKeyNames="PaymentID"
        AllowSorting="true"
        OnSorting="GridViewReport_Sorting"
        EmptyDataText="No payments found.">

        <Columns>
            <asp:BoundField DataField="PaymentID" HeaderText="Payment ID" SortExpression="PaymentID" />
            <asp:BoundField DataField="MemberName" HeaderText="Member" SortExpression="MemberName" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:C}" />
            <asp:BoundField DataField="PaymentDate" HeaderText="Payment Date" SortExpression="PaymentDate" DataFormatString="{0:MM/dd/yyyy}" />
            <asp:BoundField DataField="Notes" HeaderText="Notes" />
        </Columns>
    </asp:GridView>

</asp:Content>
