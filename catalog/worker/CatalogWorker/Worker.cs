using CatalogWorker.Data;
using RabbitMQLib;
using System.Text.Json;

namespace CatalogWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CatalogService _catalogService;
        private readonly RabbitMqService _rabbitMq;

        public Worker(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Worker>();
            _catalogService = new CatalogService(loggerFactory);
            _rabbitMq = new RabbitMqService();
            _logger.LogInformation("Initialization completed");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMq.StartConnection("items", "categories");
            _rabbitMq.Consume("items", ProcessItemMessage);
            _rabbitMq.Consume("categories", ProcessCategoryMessage);

            _logger.LogInformation("Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000, stoppingToken);
            }
        }

        private void ProcessItemMessage(string json)
        {
            _logger.LogInformation("From RabiitMQ (queue = 'items') recieved: " + json);

            var message = JsonSerializer.Deserialize<ItemMessage>(json);

            if (message == null)
                _logger.LogError("Message error. Message is null");
            else if (message.Item == null)
                _logger.LogError("Message error. Message is empty, data is null: " + message.Action);
            else if (message.Action == "AddItem")
                _catalogService.AddItem(message.Item);
            else if (message.Action == "UpdateItem")
                _catalogService.UpdateItem(message.Item);
            else if (message.Action == "DeleteItem")
                _catalogService.DeleteItem(message.Item);
            else
                _logger.LogError("Message error. Unknown action in item message: " + message.Action);
        }

        private void ProcessCategoryMessage(string json)
        {
            _logger.LogInformation("From RabiitMQ (queue = 'categories') recieved: " + json);

            var message = JsonSerializer.Deserialize<CategoryMessage>(json);

            if (message == null)
                _logger.LogError("Message error. Message is null");
            else if(message.Category == null)
                _logger.LogError("Message error. Message is empty, data is null: " + message.Action);
            else if (message.Action == "AddCategory")
                _catalogService.AddCategory(message.Category);
            else if (message.Action == "UpdateCategory")
                _catalogService.UpdateCategory(message.Category);
            else if (message.Action == "DeleteCategory")
                _catalogService.DeleteCategory(message.Category);
            else
                _logger.LogError("Message error. Unknown action in item message: " + message.Action);
        }
    }
}