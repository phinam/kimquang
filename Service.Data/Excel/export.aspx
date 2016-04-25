<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="export.aspx.cs" Inherits="Service.Data.Excel.export" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="js/jquery-2.2.2.min.js"></script>
    <script type="text/javascript">
        function RunExport(data) {
           
            //data = JSON.stringify(data);
            //alert(f);
            $('#data').val(data);
            $('form').submit();
        }
      
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <input type="hidden" id="data" name="data" />
    </div>
    </form>
</body>
</html>

