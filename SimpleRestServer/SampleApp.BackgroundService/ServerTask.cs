using SimpleRestServer;
using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SampleApp.BackgroundService
{
    public sealed class ServerTask : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;

        private AppServiceConnection appServiceConnection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += OnCanceled;

            serviceDeferral = taskInstance.GetDeferral();

            var appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (appService != null &&
                appService.Name == "RestServerService")
            {
                appServiceConnection = appService.AppServiceConnection;
                appServiceConnection.RequestReceived += OnRequestReceived;
            }
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string command = message["Command"] as string;

            switch (command)
            {
                case "Initialize":
                    {
                        var messageDeferral = args.GetDeferral();

                        var config = new ServerConfig() { Port = 8000 };
                        var server = new RestServer(config);
                        server.Route("/", RootAction);

                        IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                            async workItem =>
                            {
                                await server.RunAsync();
                            });

                        var returnMessage = new ValueSet();
                        returnMessage.Add("Status", "Success");

                        var responseStatus = await args.Request.SendResponseAsync(returnMessage);

                        messageDeferral.Complete();

                        break;
                    }

                case "Quit":
                    {
                        serviceDeferral.Complete();
                        break;
                    }
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        { }

        private HttpResponse RootAction(HttpRequest request)
        {
            string body = @"
<html>
    <header><title>SampleApp</title></header>
    <body>SampleApp</body>
</html>";

            var response = new HttpResponse(HttpVersion.Version1_1, HttpStatus.Ok);
            response.SetContent(body);
            response.SetConnection(HttpHeaderField.Connection.KeepAlive);

            return response;
        }
    }
}
