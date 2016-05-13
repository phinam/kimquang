using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace CustomSQLFunction
{
    public partial class CustomFunction
    {
        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlString UnsignString(SqlString str)
        {
            if (str == null || str == "")
                return str;
            string s = str.ToString();
            /* str = str.replace(/à|á|ạ|?|ã|â|?|?|?|?|?|?|?|?|?|?|?/g, "a");
             str = str.replace(/è|é|?|?|?|ê|?|?|?|?|?.+/g, "e");
             str = str.replace(/ì|í|?|?|?/g, "i");
             str = str.replace(/ò|ó|?|?|õ|ô|?|?|?|?|?|?|?|?|?|?|?.+/g, "o");
             str = str.replace(/ù|ú|?|?|?|?|?|?|?|?|?/g, "u");
             str = str.replace(/?|ý|?|?|?/g, "y");
             str = str.replace(/?/g, "d");
             */
            s = locdau(s);

            //s = s.replace(/ -+-/ g, "-"); //thay th? 2- thành 1-
            //s = s.replace(/^\-+|\-+$/ g, "");
            str = s;
            return str;
        }
        public static string locdau(string slug)
        {
   //          slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, "a");
			//slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, "e");
			//slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, "i");
			//slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, "o");
			//slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, "u");
			//slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, "y");
			//slug = slug.replace(/đ/gi, "d");
            //??i ký t? có d?u thành không d?u
            slug = ReplaceArray(slug,new string[] { "á","à","ả","ạ","ã","ă","ắ","ằ","ẳ","ẵ","ặ","â","ấ","ầ","ẩ","ẫ","ậ" }, "a");
            //slug = slug.Replace(/ á | à | ? | ? | ã | ? | ? | ? | ? | ? | ? | â | ? | ? | ? | ? | ? / gi, "a");
            slug = ReplaceArray(slug, new string[] { "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ" }, "e");
            //slug = slug.replace(/ é | è | ? | ? | ? | ê | ? | ? | ? | ? | ? / gi, "e");
            slug = ReplaceArray(slug, new string[] { "i","í","ì","ỉ","ĩ","ị" }, "i");
            //slug = slug.replace(/ i | í | ì | ? | ? | ? / gi, "i");
            slug = ReplaceArray(slug, new string[] { "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ" }, "o");
            //slug = slug.replace(/ ó | ò | ? | õ | ? | ô | ? | ? | ? | ? | ? | ? | ? | ? | ? | ? | ? / gi, "o");
            slug = ReplaceArray(slug, new string[] { "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự" }, "u");
            //slug = slug.replace(/ ú | ù | ? | ? | ? | ? | ? | ? | ? | ? | ? / gi, "u");
            slug = ReplaceArray(slug, new string[] { "ý","ỳ","ỷ","ỹ","ỵ" }, "y");
            //slug = slug.replace(/ ý | ? | ? | ? | ? / gi, "y");
            slug = ReplaceArray(slug, new string[] { "đ" }, "d");
            //slug = slug.replace(/ ? / gi, "d");
            //Xóa các ký t? ??t bi?t
            slug = ReplaceArray(slug, new string[] {"`","~","!","@","#","|","$","%","^","&","*","(","\\","+","=",",",".","/","?",">","<","\"","\"",":",";","_" }, "");
            //slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\"|\"|\:|\;|_/gi, "");
            //??i kho?ng tr?ng thành ký t? g?ch ngang
              
            //slug = slug.replace(/ / gi, " ");
            //??i nhi?u ký t? g?ch ngang liên ti?p thành 1 ký t? g?ch ngang
            //Phòng tr??ng h?p ng??i nh?p vào quá nhi?u ký t? tr?ng
            slug = slug.Replace("-----", "-");
            slug = slug.Replace("----", "-");
            slug = slug.Replace("---", "-");
            slug = slug.Replace("--", "-");
            //Xóa các ký t? g?ch ngang ? ??u và cu?i
            slug = "@" + slug + "@";
            slug = slug.Replace("@-", "");
            slug = slug.Replace("-@", "");
            slug = slug.Replace("@", "");
            return slug;
            //return new SqlString( value.Value.ToString(formatstring.Value));      
        }

        public static string ReplaceArray(string str,string[] arr, string newvalue)
        {
            for(int i=0;i<arr.Length;i++)
            {
                str = str.Replace(arr[i], newvalue);
            }
            return str;
        }
    }
}
