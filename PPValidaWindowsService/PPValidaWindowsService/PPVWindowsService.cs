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
using System.Windows.Threading;

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
        
            Dispatcher changeDispatcher = null;
            ManualResetEvent changeDispatcherStarted = new ManualResetEvent(false);
            Action changeThreadHandler = () =>
            {
                changeDispatcher = Dispatcher.CurrentDispatcher;
                changeDispatcherStarted.Set();
                Dispatcher.Run();
            };
            new Thread(() => changeThreadHandler()) { IsBackground = true }.Start();
            changeDispatcherStarted.WaitOne();

            EventLog.WriteEntry("PPValida Windows Service se esta iniciando", EventLogEntryType.Information);
            string copiar = filecreate.ReadSetting("PathOrigen");
            EventLog.WriteEntry(copiar, EventLogEntryType.Information);
            FileSystemWatcher observador = new FileSystemWatcher(copiar);

            observador.NotifyFilter = ( NotifyFilters.FileName | NotifyFilters.Size );
            observador.Changed += (sender, e) => changeDispatcher.BeginInvoke(new Action(() => filecreate.AlCambiar(sender, e)));
            observador.EnableRaisingEvents = true;


            string crearlog = filecreate.ReadSetting("PathLog");
            var logpath=crearlog + "log.txt";
            observador.InternalBufferSize = 64 * 1024;
            observador.IncludeSubdirectories = false;
            try
            {
                if (!File.Exists(logpath))
            {
                    using (var tw = new StreamWriter(logpath, true))
                    {
                        tw.WriteLine("Archivo log:");
                        tw.Close();
                    }
                }
           
            }
            catch
            {
                filecreate.RestartService("PPVWindowsService");
            }
            observador.Filter = "*.txt";
            try
            {
                try
                {
                    observador.EnableRaisingEvents = false;
                    observador.Changed += filecreate.AlCambiar;   /* do my stuff once asynchronously */
                }

                finally
                {
                    observador.EnableRaisingEvents = true;
                }
            }
            catch
            {
              filecreate.RestartService("PPVWindowsService");
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("PPValida Windows Service se esta deteniendo", EventLogEntryType.Information);
        }
    }


}

