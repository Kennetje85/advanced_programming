namespace PlanSysteem.Services
{
    // Voor console prompts; voorkomt dat domeinklassen direct Console.* gebruiken.
    public interface IUserPrompter
    {
        void WriteLine(string s);
        void Write(string s);
        string? ReadLine();
    }
}