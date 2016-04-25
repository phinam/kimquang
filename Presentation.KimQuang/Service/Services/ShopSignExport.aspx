<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopSignExport.aspx.cs" Inherits="Presentation.WebApp2.Services.ShopSignExport"  ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/plugins/jQuery/jQuery-2.1.4.min.js"></script>
<script type="text/javascript">
    function _preview(html) {
        $('#preview-content').html(html);
        $('#preview-content table.htCore colgroup').remove();
    };
    function _print() {
        window.setTimeout(function () {
            window.print();
        }, 500);
    };
    function _clear() {
        $('#data,#filetype').val('');
    };
    function _export(type, data, callback) {
        var _data = data;
        if (typeof _data == 'undefined' || _data == null) {
            var table_div = document.getElementById('preview-content');
            _data = table_div.innerHTML;
        }
        $('#data').val('\uFEFF' + _data);
        if (type) { $('#filetype').val(type); }
        $('#frmPreview').submit();
        window.setTimeout(function () {
            if (callback) callback();
        }, 7000);
    };
    </script>
    <style type="text/css">
        body
        {
            background: #fff;
        }
        #preview-content, #preview-content table
        {
            margin: 0 auto;
        }
        #preview-content div.title
        {
            font-size: 40px;
            text-align: center;
            padding: 100px;
        }
    </style>
</head>
<body>
    <form id="frmPreview" runat="server" action="PrintPreview" method="post">
    <div id="preview-content">
        <div class="title">
            Waiting for preview...</div>
    </div>
    <input type="hidden" id="data" name="data" />
    <input type="hidden" id="filetype" name="filetype" />
    </form>
</body>
</html>
