using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ServiceProcess;
using FileHelpers;
using System.Configuration;



namespace PPValidaWindowsService
{
    public class filecreate
    {
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
               return("no existe path");
            }
        }
        public static void AlCambiar(object source, FileSystemEventArgs e)
        {
            string fileToCopy = e.FullPath.ToString();
            string pegar = ReadSetting("PathDestino");
            string copiar = ReadSetting("PathOrigen");
            //string path = @"C:\home\global\log.txt";
            //using (var tw = new StreamWriter(path, true))
            //{
            //    tw.WriteLine("pegar: "); tw.WriteLine(pegar);
            //    tw.WriteLine("copiar: "); tw.WriteLine(copiar);
            //}
            string destinationDirectory = pegar;
                try
                {
                    var engine = new FileHelperEngine<Documents>();
                    var records = engine.ReadFile(e.FullPath);

                    if (Documents.EsDoc(records[0].tipoDoc) && Documents.EsPrint(records[2].tipoDoc) && Documents.EsFecha(records[3].tipoDoc))
                    {
                    
                    //using (var tw = new StreamWriter(path, true))
                    //{
                    //    tw.WriteLine("entra al document");
                    //}
                    try
                        {
                            File.Copy(fileToCopy, destinationDirectory + Path.GetFileName(fileToCopy));
                        }
                        catch
                        {
                            ServiceController sc = new ServiceController();
                            sc.ServiceName = "PPVWindowsService";

                            if (sc.Status == ServiceControllerStatus.Stopped)
                            {
                                // Start the service if the current status is stopped.
                                try
                                {
                                    // Start the service, and wait until its status is "Running".
                                    sc.Start();
                                    sc.WaitForStatus(ServiceControllerStatus.Running);
                                    // Display the current service status.
                                }
                                catch (InvalidOperationException)
                                {
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ServiceController sc = new ServiceController();
                    sc.ServiceName = "PPVWindowsService";

                    if (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        // Start the service if the current status is stopped.
                        try
                        {
                            // Start the service, and wait until its status is "Running".
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            // Display the current service status.
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    }
                }
            }
    }
}