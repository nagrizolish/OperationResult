using System;

namespace OperationResult.Errors
{
    /// <summary>
    /// Represent Error what appear when for example : you try to change entity status on db? but status can be chenged to specified 
    /// </summary>
    public class InvalidStateError<TState> : Error
        where TState : Enum
    {
        public readonly TState _previousState;
        public readonly TState _newState;

        private InvalidStateError(TState previousState, TState newState)
            : base($"Entity can't change state from :{previousState} to {newState}")
        {
            _previousState = previousState;
            _newState = newState;
        }

        public static InvalidStateError<TState> Create(TState previousState, TState newState) =>
            new InvalidStateError<TState>(previousState, newState);
    }
}
