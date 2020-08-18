using System;
using System.Collections.Generic;
using System.Reflection;
using OperationResult.Errors;

namespace OperationResult.OperationResult.ValueMatching
{
    /// <summary>
    /// Build descriptor whats allow resolve value in container using in <see cref="OperationResult{TResult}.ResolveContainerValue"/>
    /// </summary>
    /// <typeparam name="TContainerValue">Conteiner value type</typeparam>
    /// <typeparam name="TMatchingResult">Expected type of resolving result</typeparam>
    public class ContainerValueMatchBuilder<TContainerValue, TMatchingResult>
    {
        private Dictionary<Type, Func<Error, TMatchingResult>> _errorsCases = new Dictionary<Type, Func<Error, TMatchingResult>>();
        private Func<TContainerValue, TMatchingResult> _successCase;

        private readonly Error _error;
        private readonly TContainerValue _result;

        public ContainerValueMatchBuilder(Error error)
        {
            _error = error;
        }

        public ContainerValueMatchBuilder(TContainerValue result)
        {
            _result = result;
        }

        /// <summary>
        /// Add resolver for specified error type
        /// </summary>
        /// <typeparam name="TError">error type</typeparam>
        public ContainerValueMatchBuilder<TContainerValue, TMatchingResult> OnError<TError>(Func<TError, TMatchingResult> resolver)
            where TError : Error
        {
            if (resolver == null)
                throw new ArgumentNullException($"Matching result resolver can not be null");

            if (typeof(Error).IsAssignableFrom(typeof(TError)))
            {
                _errorsCases.Add(typeof(TError), error => resolver((TError)error));
                return this;
            }

            throw new TargetException($"{typeof(TError)} not inherited from Error");
        }

        /// <summary>
        ///  Add resolver for success case (always must be last invocation in builder it's like a build)
        /// </summary>
        public ContainerValueMatchResolver<TContainerValue, TMatchingResult> OnSuccess(Func<TContainerValue, TMatchingResult> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException($"Matching result resolver can not be null");

            if (!_errorsCases.ContainsKey(typeof(Error)))
                _errorsCases.Add(typeof(Error), error => default);

            _successCase = resolver;

            return BuildResolver();
        }

        /// <summary>
        ///  Build resolver by descriptor thats provide method <see cref="ContainerValueMatchResolver{TContainerValue, TMatchingResult}.ResolveValue"/> what allow resolve container value 
        /// </summary>
        private ContainerValueMatchResolver<TContainerValue, TMatchingResult> BuildResolver()
        {
            if (_error != null)
            {
                if (_errorsCases.ContainsKey(_error.GetType()))
                    return new ContainerValueMatchResolver<TContainerValue, TMatchingResult>(
                        ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult>
                            .Create(_errorsCases[_error.GetType()], _error));

                throw new KeyNotFoundException($"{_error.GetType()} error have not appropriate handler");
            }

            return new ContainerValueMatchResolver<TContainerValue, TMatchingResult>(
                ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult>
                    .Create(_successCase, _result));
        }
    }
}
