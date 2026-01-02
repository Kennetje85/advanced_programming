using System;
using System.Collections.Generic;
using PlanSysteem.Commands;

namespace PlanSysteem.Services
{
    /// <summary>
    /// Manages executed commands using two LIFO stacks: _undo and _redo.
    /// Gebruik: in plaats van direct domeinacties uit te voeren, maak een ICommand,
    /// roep <see cref="ExecuteCommand"/> aan. Voor undo/redo gebruik <see cref="Undo"/> / <see cref="Redo"/>.
    /// </summary>
    public sealed class CommandManager
    {
        private readonly Stack<ICommand> _undo = new();
        private readonly Stack<ICommand> _redo = new();

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;

        /// <summary>
        /// Execute a command, push it onto the undo stack and clear the redo stack.
        /// </summary>
        public void ExecuteCommand(ICommand command)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            command.Execute();
            _undo.Push(command);
            _redo.Clear();
        }

        /// <summary>
        /// Undo last executed command (LIFO). The undone command is pushed to the redo stack.
        /// </summary>
        public void Undo()
        {
            if (!CanUndo) return;
            var cmd = _undo.Pop();
            cmd.Undo();
            _redo.Push(cmd);
        }

        /// <summary>
        /// Redo last undone command (LIFO). The redone command is pushed back to the undo stack.
        /// </summary>
        public void Redo()
        {
            if (!CanRedo) return;
            var cmd = _redo.Pop();
            cmd.Execute();
            _undo.Push(cmd);
        }

        /// <summary>
        /// Clear both stacks (useful for resetting state, e.g. on new session).
        /// </summary>
        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
        }
    }
}