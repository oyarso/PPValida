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
      

[FieldOptional]
        public string titulo;
        [FieldOptional]
        public string tipoDoc;
        //[FieldOptional]
        //public string tipoPrint; //{ get; set; }
        //////[FieldConverter(ConverterKind.Date, "dd-MM-yyyy")]
        //[FieldOptional]
        //public string fecha; //{ get; set; }
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
        public static Boolean EsFecha(string tipoDoc)
        {
            try
            {
                DateTime.Parse(tipoDoc);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static Boolean EsPrint(string tipoDoc)
        {
            string impresora=tipoDoc.ToLower();
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

