using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace OrderApi.SignalR
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly PeriodicTimer _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        public MyBackgroundService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            //{
            //    //var data = FetchData();
            //    Console.WriteLine("5 client");
            //    //await _hubContext.Clients.All.SendAsync("Receive", 5);
            //    //await _hubContext.Clients.Group("mygroup").SendAsync("Receive", 5);
                
            //}
            while (true)
            {

            }

        }
      
    }
}
