namespace PlanSysteem.Commands
{
    // Command pattern contract: elke command kan worden uitgevoerd en ongedaan gemaakt.
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}