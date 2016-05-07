<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="export.aspx.cs" Inherits="Service.Data.Excel.export" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="js/jquery-2.2.2.min.js"></script>
    <script type="text/javascript">
        function RunExport(pData) {
            console.log(pData);
            //debugger;
           // var pData = JSON.stringify(data);
            //alert(f);
            $('#listId').val(pData.listId);
            $('#languageId').val(pData.languageId);// listId, languageId, exportType, sysViewId
            $('#exportType').val(pData.exportType);
            $('#sysViewId').val(pData.sysViewId);
            $('form').submit();
        }
      
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <input type="hidden" id="listId" name="listId" />
         <input type="hidden" id="languageId" name="languageId" />
         <input type="hidden" id="exportType" name="exportType" />
         <input type="hidden" id="sysViewId" name="sysViewId" />
    </div>
    </form>
</body>
</html>

