using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace MicroService.Process
{
    public sealed class ServiceProcess : IServiceProcess
    {
        private readonly ILog _log;
        private readonly object _lock = new object();

        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get; private set; }
        public bool IsStopping { get; private set; }

        public string Status
        {
            get
            {
                if (IsStopping)
                {
                    return "Stopping";
                }
                return IsRunning ? "Running" : "Stopped";
            }
        }
    
        public ServiceProcess(ILog log)
        {
            _log = log;
        }

        public void Start()
        {
            _log.Info("Starting Service Process");

            lock (_lock)
            {
                if (IsRunning)
                {
                    _log.Info("Cannot start Service Process because it is running");
                    return;
                }
                IsRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                var task = new Task(Run, _cancellationTokenSource.Token);
                task.Start();
                _log.Info("Service Process Started");
            }
        }

        public void Stop()
        {
            _log.Info("Stopping Service Process");
            lock (_lock)
            {
                if (!IsRunning)
                {
                    _log.Info("Cannot stop Service Process because it is not running");
                    return;
                }
                IsStopping = true;
                _cancellationTokenSource.Cancel();
            }
        }

        private void Run()
        {
            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    if (e is TaskCanceledException)
                    {
                        lock (_lock)
                        {
                            IsRunning = false;
                            IsStopping = false;
                            _log.Info("Service Process Stopped");
                        }
                    }
                    return e is TaskCanceledException;
                });
            }
        }

        private async Task RunAsync(CancellationToken token)
        {
            const int intervalMinutes = 1;
            while (true)
            {
                _log.Info("Processing...");
                //perform some task here
                _log.InfoFormat("Finished...processing again in {0} minute(s)", intervalMinutes);
                
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), token);
            }
        }
    }
}