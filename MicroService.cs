using System;
using System.Configuration;
using log4net;
using Microsoft.Owin.Hosting;

namespace MicroService.Process
{
    public class MicroService
    {
        private readonly IServiceProcess _serviceProcess;
        private readonly ILog _log;
        private IDisposable _app;

        public MicroService(IServiceProcess serviceProcess,  ILog log)
        {
            _serviceProcess = serviceProcess;
            _log = log;
        }

        public void Start()
        {
            _log.Info("Starting Web pipeline...");
            var port = ConfigurationManager.AppSettings["Port"];
            _app = WebApp.Start<WebPipeline>("http://+:" + port);
            _log.InfoFormat("Web pipeline started. Listening on port {0}", port);

            _log.Info("Starting Service process...");
            _serviceProcess.Start();
            _log.Info("Service process started...");

        }

        public void Stop()
        {
            _log.Info("Stopping Web pipeline...");
            _app.Dispose();
            _log.Info("Web pipeline stopped.");

            _log.Info("Stopping Service process...");
            _serviceProcess.Stop();
            _log.Info("Service process stopped.");
        }
    }
}
