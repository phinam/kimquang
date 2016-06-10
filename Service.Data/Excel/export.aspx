<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="export.aspx.cs" Inherits="Service.Data.Excel.export" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="js/jquery-2.2.2.min.js"></script>
    <script type="text/javascript">
        function RunExport(pData) {
           // var pData = JSON.stringify(data);
            //alert(f);
            $('#listId').val(pData.listId);
            $('#languageId').val(pData.languageId);// listId, languageId, exportType, sysViewId
            $('#exportType').val(pData.exportType);
            $('#sysViewId').val(pData.sysViewId);

            $('#addressTo').val(pData.addressTo);
            $('#fullName').val(pData.fullName);
            $('#telePhone').val(pData.telePhone);
            $('#cellPhone').val(pData.cellPhone);
            $('#email').val(pData.email);
            $('#position').val(pData.position);
            $('#fileName').val(pData.fileName);
            
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

        <input type="hidden" id="fullName" name="fullName" />
         <input type="hidden" id="addressTo" name="addressTo" />
        <input type="hidden" id="telePhone" name="telePhone" />
         <input type="hidden" id="cellPhone" name="cellPhone" />
         <input type="hidden" id="email" name="email" />
         <input type="hidden" id="position" name="position" />
         <input type="hidden" id="fileName" name="fileName" />
    </div>
    </form>
</body>
</html>

