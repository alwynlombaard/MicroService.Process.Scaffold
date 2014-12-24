namespace MicroService.Process
{
    public interface IServiceProcess
    {
        void Start();
        void Stop();
        string Status { get; }
    }
}