using System;
using OperationResult.Errors;

namespace OperationResult.OperationResult.ValueMatching
{
    /// <summary>
    /// Descriptor wht provide information how to resolve <see cref="OperationResult{TResult}"/> value used by <see cref="ContainerValueMatchResolver{TContainerValue, TMatchingResult}"/>.
    /// See also <seealso cref="OperationResult{TResult}.ResolveContainerValue{TMatchingResult}(Func{ContainerValueMatchBuilder{TResult, TMatchingResult}, ContainerValueMatchResolver{TResult, TMatchingResult}})"/>
    /// </summary>
    internal class ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult>
    {
        public bool HasError { get; }
        public Error Error { get; }
        public TContainerValue ResultContainerValue { get; }

        public Func<TContainerValue, TMatchingResult> SuccessCase { get; }
        public Func<Error, TMatchingResult> ErrorCase { get; }

        /// <summary>
        /// Buiild descriptor for success case 
        /// </summary>
        private ContainerValueMatchingDescriptor(Func<TContainerValue, TMatchingResult> successCase,
                                                TContainerValue value)
        {
            HasError = false;
            SuccessCase = successCase;
            ResultContainerValue = value;
        }

        /// <summary>
        /// Buiild descriptor for error case 
        /// </summary>
        private ContainerValueMatchingDescriptor(Func<Error, TMatchingResult> errroCase,
                                                Error error)
        {
            HasError = true;
            ErrorCase = errroCase;
            Error = error;
        }

        #region Initialization
        public static ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult> Create(
            Func<Error, TMatchingResult> errroCase,
            Error error)
        {
            if (error == null || errroCase == null)
                throw new InvalidOperationException($"Resolver descriptor have unreachble state");

            return new ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult>(errroCase, error);
        }

        public static ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult> Create(
            Func<TContainerValue, TMatchingResult> successCase,
            TContainerValue value)
        {
            if (value == null || successCase == null)
                throw new InvalidOperationException($"Resolver descriptor have unreachble state");

            return new ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult>(successCase, value);
        }
        #endregion
    }
}
