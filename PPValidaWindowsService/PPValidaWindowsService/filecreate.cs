using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ServiceProcess;
using FileHelpers;
using System.Configuration;
using FileHelpers.Events;
using System.Collections;

namespace PPValidaWindowsService
{
    public class filecreate
    {




        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static void RestartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);


                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch
            {
                // ...
            }
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }






        //lee carpeta en app.config
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                return ("no existe path");
            }
        }
        public static void AlCambiar(object source, FileSystemEventArgs e)
        {
            string fileToCopy = e.FullPath.ToString();
            string pegar = ReadSetting("PathDestino");
            string copiar = ReadSetting("PathOrigen");
            string crearlog = ReadSetting("PathLog");
            string Impresora = ReadSetting("PathImpresora");
            string destinationDirectory = pegar;
            string pegar2 = ReadSetting("PathDestinoCopia");
            string destinationDirectory2 = pegar2;

            try
            {
                //lee el documento por fila y si cumple las validaciones copia el archivo en la carpeta destinada.
                
                var motor = new FileHelperEngine<Documents>();
                var grabacion = motor.ReadFile(e.FullPath);
                try
                {
                    RestartService("PPVWindowsService");

                    if (Documents.EsDoc(grabacion[0].tipoDoc) && Documents.EsPrint(grabacion[2].tipoDoc) && Documents.EsFecha(grabacion[3].tipoDoc))
                    {
                        try
                        {
                            File.Copy(fileToCopy, destinationDirectory2 + Path.GetFileName(fileToCopy));







                            //>>>>>>>>>>>>>>>>>>>>>>>>factura>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><
                            int xyz = 0;
                            if (Documents.EsFactu(grabacion[0].tipoDoc))
                            {

                                xyz = 26;
                            }
                            if (Documents.EsBole(grabacion[0].tipoDoc))
                            {

                                xyz = 15;
                            }
                            if (Documents.EsGuia(grabacion[0].tipoDoc))
                            {

                                xyz = 26;
                            }

                                //>>>>>>>>>>>>>>>>>>>>>>>>factura>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><
                                if (Documents.EsFactu(grabacion[0].tipoDoc))
                                {

                                string[] arreglo = new string[xyz];

                                for (int i = 0; i < arreglo.Length; i++)
                                {
                                    arreglo[i] = "";
                                }
                                for (int i = 0; i < arreglo.Length; i++)
                                {
                                    arreglo[i] = Documents.Docu(grabacion[i].tipoDoc);
                                }


                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><Leer linea fin de linea <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


                                string sLine = ""; // Creamos un string donde se guardaran las lineas del archivo
                                int line = 0; // Leemos el numero de linea comenzando por 0
                                ArrayList arrText = new ArrayList(); // Creamos una matriz para guardar linea por linea

                                StreamReader objLeer = new StreamReader(e.FullPath);  // Creamos el objeto "objLeer" desde una funcion de la libreria IO.
                                while (sLine != null)
                                {
                                    sLine = objLeer.ReadLine();
                                    if (sLine != null)
                                        arrText.Add(sLine);
                                }
                                objLeer.Close();
                                RestartService("PPVWindowsService");

                                foreach (string sOutput in arrText)
                                {
                                    line++;
                                }
                                int lineas = line;
                                RestartService("PPVWindowsService");

                                string[] arregloItem = new string[line];
                                for (int i = 0; i < lineas - 1; i++)
                                {
                                    arregloItem[i] = null;
                                }
                                for (int i = 0; i < lineas - 1; i++)
                                {
                                    arregloItem[i] = arrText[i].ToString();
                                }
                                RestartService("PPVWindowsService");


                                try
                                {
                                    var filipath = destinationDirectory + Path.GetFileName(fileToCopy);
                                    string tidoc = null;
                                    if (arreglo[0] == "39")
                                    {
                                        tidoc = "Boleta";
                                    }
                                    if (arreglo[0] == "33")
                                    {
                                        tidoc = "Factura";
                                    }
                                    if (arreglo[0] == "52")
                                    {
                                        tidoc = "Guia";
                                    }
                                    using (var tw2 = new StreamWriter(filipath, true))

                                {
                                    tw2.WriteLine("Tipo Documento: " + tidoc);
                                    tw2.WriteLine("Num. Documento: " + arreglo[1]);
                                    tw2.WriteLine("Fecha Boleta: " + arreglo[3]);
                                    tw2.WriteLine("Fecha Pedido: " + arreglo[4]);
                                    tw2.WriteLine("F. de Pago: " + arreglo[5]);
                                    tw2.WriteLine("Rut: " + arreglo[6]);
                                    tw2.WriteLine("Cliente: " + arreglo[7]);
                                    tw2.WriteLine("Direccion: " + arreglo[9].Substring(0, 21));
                                    tw2.WriteLine("Comuna: " + arreglo[10]);
                                    tw2.WriteLine("Ciudad: " + arreglo[11]);
                                    int mneto = Int32.Parse(arreglo[13]);
                                    int miva = Int32.Parse(arreglo[16]);
                                    int mtotal = Int32.Parse(arreglo[17]);
                                    tw2.WriteLine("Num. Pedido: " + arreglo[18]);
                                    tw2.WriteLine("Vendedor: " + arreglo[19]);
                                    tw2.WriteLine("Hora: " + arreglo[20] + "\n");
                                    int[] arr = new int[lineas - 1];
                                    string[] arr2 = new string[lineas - 1];
                                    int[] subtotal = new int[lineas - 1];
                                    string[] cantidad = new string[lineas - 1];
                                    string[] descripcion = new string[lineas - 1];
                                    
                                    
                                    for (int i = 26; i < lineas - 1; i++)
                                    {
                                        string test = arregloItem[i];
                                        if (test.Contains("B:") == true)
                                        {
                                            string[] result = test.Split(new string[] { "B:", "|" }, StringSplitOptions.RemoveEmptyEntries);

                                            for (int qc = 0; qc < result.Length; qc++)
                                            {
                                                qc++;
                                                descripcion[qc] = result[qc].Substring(0, 26);
                                                tw2.WriteLine("Desc: " + descripcion[qc]);
                                                qc++;
                                                cantidad[qc] = result[qc];
                                                cantidad[qc] = cantidad[qc].TrimStart(new Char[] { '0' });
                                                cantidad[qc] = cantidad[qc].TrimEnd(new Char[] { '0' });
                                                cantidad[qc] = cantidad[qc].TrimEnd(new Char[] { '.' });
                                                tw2.WriteLine("Cantidad: " + cantidad[qc]);
                                                qc++;
                                                qc++;
                                                qc++;
                                                Int32.TryParse(result[qc], out subtotal[qc]);
                                                tw2.WriteLine("Subtotal: " + subtotal[qc] + "\n");
                                                qc++;
                                                qc++;

                                            }
                                        }



                                    }
                                    tw2.WriteLine("Monto Neto: " + mneto);
                                    tw2.WriteLine("Monto IVA: " + miva);
                                    tw2.WriteLine("Monto Total: " + mtotal + "\n");
                                   
                                    tw2.Flush();
                                }
                                }                            



                            catch
                        {
                            RestartService("PPVWindowsService");
                        }
                            }


                            //>>>>>>>>>>>>>>>>>>>>>>>boleta>>>>>>>>>>>>>>>>>>>>>>><

                            if (Documents.EsBole(grabacion[0].tipoDoc))
                                {

                                    string[] arreglo = new string[xyz];

                                    for (int i = 0; i < arreglo.Length; i++)
                                    {
                                        arreglo[i] = "";
                                    }
                                    for (int i = 0; i < arreglo.Length; i++)
                                    {
                                        arreglo[i] = Documents.Docu(grabacion[i].tipoDoc);
                                    }


                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><Leer linea fin de linea <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


                                    string sLine = ""; // Creamos un string donde se guardaran las lineas del archivo
                                    int line = 0; // Leemos el numero de linea comenzando por 0
                                    ArrayList arrText = new ArrayList(); // Creamos una matriz para guardar linea por linea

                                    StreamReader objLeer = new StreamReader(e.FullPath);  // Creamos el objeto "objLeer" desde una funcion de la libreria IO.
                                    while (sLine != null)
                                    {
                                        sLine = objLeer.ReadLine();
                                        if (sLine != null)
                                            arrText.Add(sLine);
                                    }
                                    objLeer.Close();
                                    RestartService("PPVWindowsService");

                                    foreach (string sOutput in arrText)
                                    {
                                        line++;
                                    }
                                    int lineas = line;
                                    RestartService("PPVWindowsService");

                                    string[] arregloItem = new string[line];
                                    for (int i = 0; i < lineas - 1; i++)
                                    {
                                        arregloItem[i] = null;
                                    }
                                    for (int i = 0; i < lineas - 1; i++)
                                    {
                                        arregloItem[i] = arrText[i].ToString();
                                    }
                                    RestartService("PPVWindowsService");


                                    try
                                    {
                                        var filipath = destinationDirectory + Path.GetFileName(fileToCopy);
                                        string tidoc = null;
                                        if (arreglo[0] == "39")
                                        {
                                            tidoc = "Boleta";
                                        }
                                        if (arreglo[0] == "33")
                                        {
                                            tidoc = "Factura";
                                        }
                                        if (arreglo[0] == "52")
                                        {
                                            tidoc = "Guia";
                                        }
                                        using (var tw2 = new StreamWriter(filipath, true))

                                    {
                                        tw2.WriteLine("Tipo Documento: " + tidoc);
                                        tw2.WriteLine("Num. Documento: " + arreglo[1]);
                                        tw2.WriteLine("Fecha Boleta: " + arreglo[3]);
                                        tw2.WriteLine("Fecha Pedido: " + arreglo[4]);
                                        tw2.WriteLine("F. de Pago: " + arreglo[5]);
                                        tw2.WriteLine("Rut: " + arreglo[6]);
                                        tw2.WriteLine("Cliente: " + arreglo[7]);
                                        tw2.WriteLine("Direccion: " + arreglo[8]);
                                        tw2.WriteLine("Comuna: " + arreglo[9]);
                                        tw2.WriteLine("Ciudad: " + arreglo[10]);
                                        int mtotal = Int32.Parse(arreglo[11]);
                                        tw2.WriteLine("Num. Pedido: " + arreglo[12]);
                                        tw2.WriteLine("Vendedor: " + arreglo[13]);
                                        tw2.WriteLine("Hora: " + arreglo[14] + "\n");
                                        int[] arr = new int[lineas - 1];
                                        string[] arr2 = new string[lineas - 1];
                                        int[] subtotal = new int[lineas - 1];
                                        string[] cantidad = new string[lineas - 1];
                                        string[] descripcion = new string[lineas - 1];


                                        for (int i = 15; i < lineas-1; i++)
                                        {
                                            string test = arregloItem[i];
                                            if (test.Contains("B:") == true)
                                            {
                                                string[] result = test.Split(new string[] { "B:", "|" }, StringSplitOptions.RemoveEmptyEntries);

                                                for (int qc = 0; qc < result.Length; qc++)
                                                {
                                                    qc++;
                                                    descripcion[qc] = result[qc];
                                                    tw2.WriteLine("Desc: " + descripcion[qc].Substring(0, 26));
                                                    qc++;
                                                    cantidad[qc] = result[qc];
                                                    cantidad[qc] = cantidad[qc].TrimStart(new Char[] { '0' });
                                                    cantidad[qc] = cantidad[qc].TrimEnd(new Char[] { '0' });
                                                    cantidad[qc] = cantidad[qc].TrimEnd(new Char[] { '.' });
                                                    tw2.WriteLine("Cantidad: " + cantidad[qc]);
                                                    qc++; qc++; qc++;
                                                    Int32.TryParse(result[qc], out subtotal[qc]);
                                                    tw2.WriteLine("Subtotal: " + subtotal[qc] + "\n");
                                                    qc++; qc++;

                                                }
                                            }



                                        }
                                        tw2.WriteLine("Monto Total: " + mtotal + "\n");
                               
                                        tw2.Flush();
                                    }
                                    try
                                    {
                                        File.Copy(filipath, Impresora + Path.GetFileName(fileToCopy), true);
                                    // File.Copy(filipath, Impresora, true);
                                }


                               
                                
                                catch
                                {
                                    RestartService("PPVWindowsService");
                                }

                            }
                                 



                                    catch
                                    {
                                        RestartService("PPVWindowsService");
                                    }
                            }


                            //factura>>>>>>>>>>>>>>

                            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><guia>>>>>>>>>>>>>>>>>>>>>>>>><
                            if (Documents.EsGuia(grabacion[0].tipoDoc))
                                {

                                        string[] arreglo = new string[xyz];

                                        for (int i = 0; i < arreglo.Length; i++)
                                        {
                                            arreglo[i] = "";
                                        }
                                        for (int i = 0; i < arreglo.Length; i++)
                                        {
                                            arreglo[i] = Documents.Docu(grabacion[i].tipoDoc);
                                        }


                                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><Leer linea fin de linea <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


                                        string sLine = ""; // Creamos un string donde se guardaran las lineas del archivo
                                        int line = 0; // Leemos el numero de linea comenzando por 0
                                        ArrayList arrText = new ArrayList(); // Creamos una matriz para guardar linea por linea

                                        StreamReader objLeer = new StreamReader(e.FullPath);  // Creamos el objeto "objLeer" desde una funcion de la libreria IO.
                                        while (sLine != null)
                                        {
                                            sLine = objLeer.ReadLine();
                                            if (sLine != null)
                                                arrText.Add(sLine);
                                        }
                                        objLeer.Close();
                                        RestartService("PPVWindowsService");

                                        foreach (string sOutput in arrText)
                                        {
                                            line++;
                                        }
                                        int lineas = line;
                                        RestartService("PPVWindowsService");

                                        string[] arregloItem = new string[line];
                                        for (int i = 0; i < lineas - 1; i++)
                                        {
                                            arregloItem[i] = null;
                                        }
                                        for (int i = 0; i < lineas - 1; i++)
                                        {
                                            arregloItem[i] = arrText[i].ToString();
                                        }
                                        RestartService("PPVWindowsService");


                                        try
                                        {
                                            var filipath = destinationDirectory + Path.GetFileName(fileToCopy);
                                            string tidoc = null;
                                            if (arreglo[0] == "39")
                                            {
                                                tidoc = "Boleta";
                                            }
                                            if (arreglo[0] == "33")
                                            {
                                                tidoc = "Factura";
                                            }
                                            if (arreglo[0] == "52")
                                            {
                                                tidoc = "Guia";
                                            }
                                            using (var tw2 = new StreamWriter(filipath, true))

                                    {
                                        tw2.WriteLine("Tipo Documento: " + tidoc);
                                        tw2.WriteLine("Num. Documento: " + arreglo[1]);
                                        tw2.WriteLine("Fecha Boleta: " + arreglo[3]);
                                        tw2.WriteLine("Fecha Pedido: " + arreglo[4]);
                                        tw2.WriteLine("F. de Pago: " + arreglo[5]);
                                        tw2.WriteLine("Rut: " + arreglo[6]);
                                        tw2.WriteLine("Cliente: " + arreglo[7]);
                                        tw2.WriteLine("Direccion: " + arreglo[9].Substring(0, 21));
                                        tw2.WriteLine("Comuna: " + arreglo[10]);
                                        tw2.WriteLine("Ciudad: " + arreglo[11]);
                                        int mneto = Int32.Parse(arreglo[13]);
                                        int miva = Int32.Parse(arreglo[16]);
                                        int mtotal = Int32.Parse(arreglo[17]);
                                        tw2.WriteLine("Num. Pedido: " + arreglo[18]);
                                        tw2.WriteLine("Vendedor: " + arreglo[19]);
                                        tw2.WriteLine("Hora: " + arreglo[20] + "\n");
                                        int[] arr = new int[lineas - 1];
                                        string[] arr2 = new string[lineas - 1];
                                        int[] subtotal = new int[lineas - 1];
                                        string[] cantidad = new string[lineas - 1];
                                        string[] descripcion = new string[lineas - 1];


                                        for (int i = 26; i < lineas - 1; i++)
                                        {
                                            string test = arregloItem[i];
                                            if (test.Contains("B:") == true)
                                            {
                                                string[] result = test.Split(new string[] { "B:", "|" }, StringSplitOptions.RemoveEmptyEntries);

                                                for (int qc = 0; qc < result.Length; qc++)
                                                {
                                                    qc++;
                                                    descripcion[qc] = result[qc].Substring(0, 26);
                                                    tw2.WriteLine("Desc: " + descripcion[qc]);

                                                    cantidad[qc] = result[qc];
                                                    cantidad[qc] = cantidad[qc].TrimStart(new Char[] { '0' });
                                                    cantidad[qc] = cantidad[qc].TrimEnd(new Char[] { '0' });
                                                    cantidad[qc] = cantidad[qc].TrimEnd(new Char[] { '.' });
                                                    tw2.WriteLine("Cantidad: " + cantidad[qc]);
                                                    qc++;
                                                    qc++;
                                                    qc++;
                                                    Int32.TryParse(result[qc], out subtotal[qc]);
                                                    tw2.WriteLine("Subtotal: " + subtotal[qc] + "\n");
                                                    qc++;
                                                    qc++;

                                                }
                                            }



                                        }
                                        tw2.WriteLine("Monto Neto: " + mneto);
                                        tw2.WriteLine("Monto IVA: " + miva);
                                        tw2.WriteLine("Monto Total: " + mtotal + "\n");
                  
                                        tw2.Flush();
                                    }
                                        }



                                        catch
                                        {
                                            RestartService("PPVWindowsService");
                                        }
                            }

                            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<guia>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<



                        }
                        catch
                        {
                            RestartService("PPVWindowsService");
                        }
                    }
                    else
                    {
                        //sino es valido apendizar
                        try
                        {
                            var pathlog = crearlog + "log.txt";
                            using (StreamWriter w = File.AppendText(pathlog))
                            {
                                Log("Archivo apendizado", w);
                                w.WriteLine("Nombre del archivo: " + Path.GetFileName(e.FullPath));
                                var lines = File.ReadLines(e.FullPath);
                                foreach (var line in lines)
                                {
                                    w.WriteLine(line);
                                }
                                w.Close();
                            }

                            using (StreamReader r = File.OpenText(pathlog))
                            {
                                DumpLog(r);
                            }
                        }
                        //captura errores al apendizar
                        catch
                        {
                            RestartService("PPVWindowsService");
                        }
                    }
                }

                catch
                {
                    RestartService("PPVWindowsService");
                }
            }
            catch
            {
                RestartService("PPVWindowsService");
            }

        }

    }
}