<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="EconomicWebApp.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crops</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="title">
            <h1>Crops Available</h1>
        </div>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT * FROM [CropTable]"></asp:SqlDataSource>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="CropName" HeaderText="CropName" SortExpression="CropName" />
                <asp:BoundField DataField="CropTypes" HeaderText="CropTypes" SortExpression="CropTypes" />
                <asp:BoundField DataField="UsePerAcre" HeaderText="UsePerAcre" SortExpression="UsePerAcre" />
                <asp:BoundField DataField="GrossProduction" HeaderText="GrossProduction" SortExpression="GrossProduction" />
                <asp:BoundField DataField="Season" HeaderText="PlantingSeason" SortExpression="Season" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
