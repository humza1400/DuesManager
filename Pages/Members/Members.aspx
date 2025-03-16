<%@ Page Title="Members" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Members.aspx.cs" Inherits="DuesManager.Pages.Members.Members" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Members</h2>
    
    <a href="#" class="btn btn-success mb-3 float-end" data-bs-toggle="modal" data-bs-target="#addMemberModal">
        <i class="fas fa-plus"></i> Add Member
    </a>

    <asp:GridView
        ID="GridViewMembers"
        runat="server"
        AutoGenerateColumns="False"
        DataKeyNames="MemberID"
        OnRowEditing="GridViewMembers_RowEditing"
        OnRowUpdating="GridViewMembers_RowUpdating"
        OnRowCancelingEdit="GridViewMembers_RowCancelingEdit"
        OnRowDeleting="GridViewMembers_RowDeleting"
        CssClass="table table-striped table-bordered">
        <Columns>
            <asp:BoundField DataField="MemberID" HeaderText="ID" ReadOnly="True" />
            <asp:BoundField DataField="FirstName" HeaderText="First Name" />
            <asp:BoundField DataField="LastName" HeaderText="Last Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="JoinDate" HeaderText="Join Date" ReadOnly="True" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-primary" CausesValidation="false">
                            <i class="fas fa-edit"></i> Edit
                    </asp:LinkButton>
                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this member?');" CausesValidation="false">
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

    <div class="modal fade" id="addMemberModal" tabindex="-1" aria-labelledby="addMemberModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="addMemberModalLabel">Add New Member</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required" CssClass="text-danger" Display="Dynamic" />
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control mb-2" placeholder="First Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required" CssClass="text-danger" Display="Dynamic" />
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control mb-2" placeholder="Last Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic" />
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control mb-2" placeholder="Email"></asp:TextBox>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSaveMember" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSaveMember_Click" />
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>


</asp:Content>