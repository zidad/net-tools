using Net.DependencyInjection;

[assembly:RegisterAssemblyInContainer]
namespace SampleConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var taskService = new SampleTaskRunner())
                taskService.Run();
        }
    }
}
