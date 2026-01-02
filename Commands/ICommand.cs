namespace PlanSysteem.Commands
{
    //Dit toegevoegd voor datastructuren
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}