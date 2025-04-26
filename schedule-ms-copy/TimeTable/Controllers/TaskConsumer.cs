using Confluent.Kafka;
using TimeTable.Models.Repository;


public class TaskConsumer : ConsumerBase
{
    private IServiceScopeFactory _scopeFactory;
    public TaskConsumer(IConsumer<Ignore, string> consumer, IServiceScopeFactory scopeFactory) : base(consumer, scopeFactory)
    {
        _topic = "events.task";
        _scopeFactory = scopeFactory;
    }
    public override async void ProcessingMessage(string Message)
    {
        Console.WriteLine("УЙ");
        try
        {
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(Message);
            Guid taskId = Guid.Parse(jsonObject["task_id"]?.ToString());
            Guid lessonId = Guid.Parse(jsonObject["lesson_id"]?.ToString());
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var lessonRepository = scope.ServiceProvider.GetRequiredService<ILessonRepository>();
                if (lessonRepository is LessonRepository)
                {
                    var _lessonRepository = lessonRepository as LessonRepository;
                    await _lessonRepository.Update(lessonId, taskId: taskId);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }
}




