using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Timers;
using System.Configuration;


namespace PPValidaWindowsService
{
    public partial class PPVWindowsService : ServiceBase
    {
        public PPVWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("PPValida Windows Service se esta iniciando", EventLogEntryType.Information);
            string copiar = filecreate.ReadSetting("PathOrigen");
            EventLog.WriteEntry(copiar, EventLogEntryType.Information);
            FileSystemWatcher observador = new FileSystemWatcher(copiar);
            observador.NotifyFilter = (
            NotifyFilters.LastAccess |
            NotifyFilters.LastWrite |
            NotifyFilters.FileName |
            NotifyFilters.DirectoryName);

            observador.Filter = "*.txt";
            try
            {
                observador.Created += filecreate.AlCambiar;
                observador.EnableRaisingEvents = true;
            }
            catch
            {
                ServiceController sc = new ServiceController();
                sc.ServiceName = "PPVWindowsService";

                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    try
                    {
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }


        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("PPValida Windows Service se esta deteniendo", EventLogEntryType.Information);
        }
    }


}

