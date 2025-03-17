<%@ Page Title="Payments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payments.aspx.cs" Inherits="DuesManager.Pages.Payments.Payments" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Payment Overview</h2>
    <div class="d-flex justify-content-between align-items-center mb-3">
        <div class="summary">
            <h4>Total Payments Received:
                <asp:Literal ID="litTotalPayments" runat="server" />
            </h4>
            <h5>Payments This Month:
                <asp:Literal ID="litPaymentsThisMonth" runat="server" />
            </h5>
        </div>
        <a href="#" class="btn btn-success mb-3 float-end" data-bs-toggle="modal" data-bs-target="#addPaymentModal">
            <i class="fas fa-plus"></i> Add Payment
        </a>
    </div>

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
        <!-- Clear Filter Button -->
        <div class="me-2">
            <label>&nbsp;</label>
            <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" CssClass="btn btn-secondary d-block" OnClick="btnClearFilters_Click" Enabled="false" />
        </div>
    </div>



    <asp:GridView
        ID="GridViewPayments"
        runat="server"
        AutoGenerateColumns="false"
        CssClass="table table-hover table-bordered"
        DataKeyNames="PaymentID"
        AllowSorting="true"
        OnSorting="GridViewPayments_Sorting"
        OnRowDataBound="GridViewPayments_RowDataBound"
        OnRowEditing="GridViewPayments_RowEditing"
        OnRowUpdating="GridViewPayments_RowUpdating"
        OnRowCancelingEdit="GridViewPayments_RowCancelingEdit"
        OnRowDeleting="GridViewPayments_RowDeleting"
        EmptyDataText="No payments found.">
        <Columns>
            <asp:BoundField DataField="PaymentID" HeaderText="ID" ReadOnly="True" />
            <asp:TemplateField HeaderText="Member" SortExpression="MemberName">
                <ItemTemplate>
                    <%# Eval("MemberName") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                <ItemTemplate>
                    <%# String.Format("{0:C}", Eval("Amount")) %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditAmount" runat="server" Text='<%# Bind("Amount") %>' CssClass="form-control" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Payment Date" SortExpression="PaymentDate">
                <ItemTemplate>
                    <%# String.Format("{0:MM/dd/yyyy}", Eval("PaymentDate")) %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditPaymentDate" runat="server" Text='<%# Bind("PaymentDate", "{0:yyyy-MM-dd}") %>' TextMode="Date" CssClass="form-control" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Notes">
                <ItemTemplate>
                    <%# Eval("Notes") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditNotes" runat="server" Text='<%# Bind("Notes") %>' CssClass="form-control" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-primary" CausesValidation="false">
                    <i class="fas fa-edit"></i> Edit
                    </asp:LinkButton>
                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-danger" CausesValidation="false"
                        OnClientClick="return confirm('Are you sure you want to delete this payment?');">
                    <i class="fas fa-trash-alt"></i> Delete
                    </asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success" CausesValidation="false">
                            <i class="fas fa-save"></i> Save
                    </asp:LinkButton>
                    <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary" CausesValidation="false">
                            <i class="fas fa-times"></i> Cancel
                    </asp:LinkButton>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div class="modal fade" id="addPaymentModal" tabindex="-1" aria-labelledby="addPaymentModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addPaymentModalLabel">Add New Payment</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">

                    <!-- Member Dropdown -->
                    <asp:RequiredFieldValidator ID="rfvModalMembers" runat="server" ControlToValidate="ddlModalMembers"
                        InitialValue="" ErrorMessage="Please select a member."
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="AddPaymentGroup" />
                    <label for="ddlModalMembers">Select Member</label>
                    <asp:DropDownList ID="ddlModalMembers" runat="server" CssClass="form-control mb-3" ValidationGroup="AddPaymentGroup" />

                    <!-- Payment Amount -->
                    <asp:RequiredFieldValidator ID="rfvPaymentAmount" runat="server" ControlToValidate="txtPaymentAmount"
                        ErrorMessage="Payment amount is required." CssClass="text-danger" Display="Dynamic" ValidationGroup="AddPaymentGroup" />
                    <asp:RegularExpressionValidator ID="revPaymentAmount" runat="server" ControlToValidate="txtPaymentAmount"
                        ValidationExpression="^-?\d+(\.\d{1,2})?$" ErrorMessage="Enter a valid amount." CssClass="text-danger" Display="Dynamic" ValidationGroup="AddPaymentGroup" />
                    <label for="txtPaymentAmount">Payment Amount</label>
                    <asp:TextBox ID="txtPaymentAmount" runat="server" CssClass="form-control mb-3" placeholder="Enter payment amount" />

                    <!-- Payment Date -->
                    <asp:CompareValidator ID="cvPaymentDate" runat="server" ControlToValidate="txtPaymentDate"
                        Operator="DataTypeCheck" Type="Date" ErrorMessage="Enter a valid date." CssClass="text-danger" Display="Dynamic" ValidationGroup="AddPaymentGroup" />
                    <label for="txtPaymentDate">Payment Date</label>
                    <asp:TextBox ID="txtPaymentDate" runat="server" CssClass="form-control mb-3" TextMode="Date" />

                    <!-- Notes -->
                    <label for="txtPaymentNotes">Notes (Optional)</label>
                    <asp:TextBox ID="txtPaymentNotes" runat="server" CssClass="form-control mb-3" placeholder="Optional notes" TextMode="MultiLine" Rows="3" />

                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSavePayment" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSavePayment_Click" ValidationGroup="AddPaymentGroup" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
