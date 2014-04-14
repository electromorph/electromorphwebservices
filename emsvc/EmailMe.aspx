<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailMe.aspx.cs" Inherits="emsvc.EmailMe" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Contact Electromorph</title>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js" ></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" style="width: 409px">
                <tr>
                    <td>
                        <asp:Label ID="lblName" runat="server" Text="Name*"></asp:Label><br />
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" ValidationGroup="contact" Width="457px"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Subject*"></asp:Label><br />
                    </td>
                    <td>
                        <asp:TextBox ID="txtSubject" runat="server" Width="459px"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="txtSubject"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEmail" runat="server" Text="Email*"></asp:Label><br />
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" Width="460px"></asp:TextBox><br />
                        <asp:RegularExpressionValidator ID="valRegEx" runat="server" ControlToValidate="txtEmail" ValidationExpression=".*@.*\..*" ErrorMessage="*Invalid Email address." Display="dynamic">
                        </asp:RegularExpressionValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="Label4" runat="server" Text="Body*"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Height="143px" Width="461px"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" ControlToValidate="txtBody"></asp:RequiredFieldValidator>
                        <asp:Label ID="lblDisplay" runat="server" Text="" Visible = "false" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="464px" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>

                        <asp:Button ID="Button1" runat="server" OnClick="btnSend_Click" Text="Send" Width="100px" />

                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
