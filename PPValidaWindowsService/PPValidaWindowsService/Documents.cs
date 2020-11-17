using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using System.IO;

namespace PPValidaWindowsService
{
    [DelimitedRecord("A:|")]
    [IgnoreEmptyLines]
    public class Documents
    {

        // En base a las variables capturadas por filehelper, se valida en los siguientes metodos
        [FieldOptional]
        public string titulo;
        [FieldOptional]
        public string tipoDoc;
        public static Boolean EsDoc(string tipoDoc)
        {
            if (tipoDoc == "33" || tipoDoc == "39" || tipoDoc == "52")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Boolean EsBole(string tipoDoc)
        {
            if (tipoDoc == "39")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Boolean EsFactu(string tipoDoc)
        {
            if (tipoDoc == "33")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Boolean EsGuia(string tipoDoc)
        {
            if (tipoDoc == "52")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string Docu(string tipoDoc)
        {
            return tipoDoc;
        }
        public static Boolean EsFecha(string tipoDoc)
        {
            DateTime dt;
            DateTime.TryParseExact(tipoDoc, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dt);
            if (dt == DateTime.MinValue)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static Boolean EsPrint(string tipoDoc)
        {
            string impresora = tipoDoc.ToLower();
            bool impre;
            impre = impresora.Contains("printer");
            if (impre == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}