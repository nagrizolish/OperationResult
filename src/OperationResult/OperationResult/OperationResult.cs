using System;
using OperationResult.Errors;
using OperationResult.OperationResult.Extensions;
using OperationResult.OperationResult.ValueMatching;

namespace OperationResult.OperationResult
{
    /// <summary>
    /// Object what can manage own state 
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    public struct OperationResult<TResult>
    {
        public OperationResult(Error error)
        {
            Error = error;
            Result = default;
        }
        public OperationResult(TResult result = default)
        {
            Result = result;
            Error = default;
        }
        public OperationResult(Exception error)
        {
            Error = Error.Create(error);
            Result = default;
        }

        public TResult Result { get; }
        public Error Error { get; }
        public bool HasError => Error != null;

        public static OperationResult<TResult> Success(TResult result) =>
            new OperationResult<TResult>(result);
        public static OperationResult<TResult> Fail(Error error) =>
            new OperationResult<TResult>(error);
        public static OperationResult<TResult> FromResult(Func<OperationResult<TResult>> fromFunc)
        {
            try { return fromFunc(); }
            catch (Exception e) { return e; }
        }

        public static implicit operator OperationResult<TResult>(TResult resultObject) =>
            new OperationResult<TResult>(resultObject);

        public static implicit operator OperationResult<TResult>(Exception e) =>
            new OperationResult<TResult>(e);

        public static implicit operator OperationResult<TResult>(Error error) =>
            new OperationResult<TResult>(error);

        /// <summary>
        /// Provide logic for build resolving container value and execute them.
        /// See also :
        /// <seealso cref="ContainerValueMatchBuilder{TContainerValue, TMatchingResult}"/>,
        /// <seealso cref="ContainerValueMatchResolver{TContainerValue, TMatchingResult}"/>
        /// </summary>
        /// <typeparam name="TMatchingResult">return this</typeparam>
        /// <example>
        ///     <code>
        ///     private static OperationResult<Task<string>> Aol(int test) =>
        ///           test switch
        ///               {
        ///                 1 => CustormError.Create("error 1"),
        ///                 2 => CustormError2.Create("error 2"),
        ///                 _ => Task.FromResult("success")
        ///               };
        ///      
        ///      string validation = (await Aol(3).MapT(x => x))
        ///                          .ResolveContainerValue<string>(builder => 
        ///                              builder.OnError<CustormError>(x => x.Message) // retrun "error 1"
        ///                                     .OnError<CustormError2>(x => x.Message) // return "error 2"
        ///                                     .OnSuccess(x => "Successasdasdasdasd")); //return "success"
        ///     </code>
        /// </example>
        public TMatchingResult ResolveContainerValue<TMatchingResult>(Func<ContainerValueMatchBuilder<TResult, TMatchingResult>,
                                                                           ContainerValueMatchResolver<TResult, TMatchingResult>> matchFunc)
        {
            if (matchFunc == null)
                throw new ArgumentNullException(nameof(matchFunc));

            var result = Result;
            var error = Error;

            var resolver = this.Match(_success => new ContainerValueMatchBuilder<TResult, TMatchingResult>(result),
                                      _fail => new ContainerValueMatchBuilder<TResult, TMatchingResult>(error));
            return matchFunc(resolver).ResolveValue();
        }

        public TMatchingResult ResolveContainer<TMatchingResult>(Func<ContainerValueMatchBuilder<OperationResult<TResult>, TMatchingResult>,
                                                                      ContainerValueMatchResolver<OperationResult<TResult>, TMatchingResult>> matchFunc)
        {
            if (matchFunc == null)
                throw new ArgumentNullException(nameof(matchFunc));

            var result = Result;
            var error = Error;

            var resolver = this.Match(_success => new ContainerValueMatchBuilder<OperationResult<TResult>, TMatchingResult>(result),
                                      _fail => new ContainerValueMatchBuilder<OperationResult<TResult>, TMatchingResult>(error));
            return matchFunc(resolver).ResolveValue();
        }
    }
}