using Microsoft.Azure.WebJobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageBatchIgnore
{
    public class Functions
    {
        public async Task TriggerAsync(
            [QueueTrigger("test")] string triggerMessage,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"{ DateTime.Now } - Going to Wait 10 Seconds - {triggerMessage}");
            const int tenSeconds = 10 * 1000;
            await Task.Delay(tenSeconds);
            Console.WriteLine($"{ DateTime.Now } - Waited 10 Seconds - {triggerMessage}");
        }
    }
}
