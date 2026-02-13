using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace labo.signalr.api.Hubs
{
    public class UnHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public UnHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public override async Task OnConnectedAsync()
        {
            base.OnConnectedAsync();
            // TODO: Ajouter votre logique
            IEnumerable<UselessTask> value = await _context.UselessTasks.ToListAsync();
            await Clients.Caller.SendAsync("UpdateTasks", value);
        }

        private async Task TaskList()
        {
            IEnumerable<UselessTask> value = await _context.UselessTasks.ToListAsync();
            await Clients.All.SendAsync("UpdateTasks", value);
        }

        public async Task Add(string taskText)
        {
            UselessTask uselessTask = new UselessTask()
            {
                Completed = false,
                Text = taskText
            };
            _context.UselessTasks.Add(uselessTask);
            await _context.SaveChangesAsync();
            TaskList();
        }

        public async Task Complete(int id)
        {
            UselessTask? task = await _context.FindAsync<UselessTask>(id);
            if (task != null)
            {
                task.Completed = true;
                await _context.SaveChangesAsync();
                TaskList();
            }
            
        }
    }
}
