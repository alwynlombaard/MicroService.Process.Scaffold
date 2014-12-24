using System;
using log4net;
using MicroService.Process.Nancy.Models;
using Nancy;

namespace MicroService.Process.Nancy.Modules
{
    public class HomeModule : NancyModule
    {

        public HomeModule(ILog log, IServiceProcess serviceProcess) : base("/")
        {
           
            Get["/"] = x => { 
                log.Info("Loading home page");
                                return new HomeModel
                                {
                                    ProcessStatus = serviceProcess.Status,
                                    Message = "Hello world",
                                    Timestamp = DateTime.UtcNow.ToLongTimeString()
                                };
            };

            Get["/dynamic"] = x =>
                new DynamicPageModel
                {
                    Message = "Hello world",
                    Timestamp = DateTime.UtcNow.ToLongTimeString()
                };
        }
    }
}
