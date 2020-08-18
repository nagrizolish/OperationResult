using System;
using System.Threading.Tasks;
using OperationResult.Errors;

namespace OperationResult.OperationResult.Extensions
{
    /// <summary>
    /// Legend:
    /// Container - OperationResult is container that can contains value or error
    /// </summary>
    public static class OperationResultsExtensions
    {
        /// <summary>
        /// Pattern matching. Allows resolve value by container innner state
        /// </summary>
        public static T Match<TResult, T>(this OperationResult<TResult> result,
                                               Func<TResult, T> success,
                                               Func<Error, T> fail) =>
            result.HasError ?
            fail.Invoke(result.Error) :
            success.Invoke(result.Result);


        /// <summary>
        /// Pattern matching. Allow resolve value by container inner state and not return any thing 
        /// </summary>
        public static Unit Match<TResult, T>(this OperationResult<TResult> result,
                                                  Action<TResult> success,
                                                  Action<Error> fail)
        {
            if (result.HasError)
                fail.Invoke(result.Error);
            else
                success.Invoke(result.Result);

            return Unit.Value;
        }

        /// <summary>
        /// Set callback 
        /// </summary>
        public static OperationResult<Unit> IfSucces<TResult>(this OperationResult<TResult> result,
                                                                   Action<TResult> action)
        {
            try
            {
                return result.Match(
                     _success =>
                     {
                         action(result.Result);
                         return Unit.Value;
                     },
                     _fail => Unit.Value);
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Set fallback
        /// </summary>
        public static OperationResult<Unit> IfFail<TResult>(this OperationResult<TResult> result,
                                                                 Action<Error> action)
        {
            try
            {
                return result.Match(
                     _success => Unit.Value,
                     _fail =>
                     {
                         action(_fail);
                         return Unit.Value;
                     });
            }
            catch (Exception e) { return e; }
        }

        public static OperationResult<Unit> IfFail<TResult>(this OperationResult<TResult> result,
                                                                 Action<TResult> action)
        {
            try
            {
                return result.Match(
                     _success => Unit.Value,
                     _fail =>
                     {
                         action(result.Result);
                         return Unit.Value;
                     });
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container value if it exists async
        /// </summary>
        public static async Task<OperationResult<T>> MapAsync<TResult, T>(this Task<OperationResult<TResult>> result,
                                                                               Func<TResult, Task<T>> func)
        {
            try
            {
                var res = (await result).Map(func);
                return await res;
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container value if it exists async
        /// </summary>
        public static async Task<OperationResult<T>> MapAsync<TResult, T>(this Task<OperationResult<TResult>> result,
                                                                               Func<TResult, T> func)
        {
            try
            {
                var res = (await result);
                return res.Map(func);
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container value if it exists 
        /// </summary>
        public static OperationResult<T> Map<TResult, T>(this OperationResult<TResult> result,
                                                              Func<TResult, T> func)
        {
            try
            {
                return result.Match(_success => func.Invoke(result.Result),
                                    _fail => new OperationResult<T>(result.Error));
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container value if it exists 
        /// </summary>
        public static Task<OperationResult<T>> Map<TResult, T>(this OperationResult<TResult> result,
                                                                    Func<TResult, Task<T>> func)
        {
            try
            {
                return result.Match(async _success => new OperationResult<T>(await func.Invoke(result.Result)),
                                    _fail => Task.FromResult(OperationResult<T>.Fail(result.Error)));
            }
            catch (Exception e) { return Task.FromResult(OperationResult<T>.Fail(Error.Create(e))); }
        }

        /// <summary>
        /// Map container async
        /// </summary>
        public static async Task<OperationResult<T>> BindAsync<TResult, T>(this Task<OperationResult<TResult>> result,
                                                                                Func<OperationResult<TResult>, OperationResult<T>> func)
        {
            try
            {
                var res = await result;
                return res.Bind(func);
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container async
        /// </summary>
        public static async Task<OperationResult<T>> BindAsync<TResult, T>(this Task<OperationResult<TResult>> result,
                                                                                Func<OperationResult<TResult>, Task<OperationResult<T>>> func)
        {
            try
            {
                return await ((await result).Bind(res => func(res)));
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container
        /// </summary>
        public static OperationResult<T> Bind<TResult, T>(this OperationResult<TResult> result,
                                                               Func<OperationResult<TResult>, OperationResult<T>> func)
        {
            try
            {
                return result.Match(_success => func.Invoke(result.Result),
                                    _fail => OperationResult<T>.Fail(result.Error));
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Map container
        /// </summary>
        public static Task<OperationResult<T>> Bind<TResult, T>(this OperationResult<TResult> result,
                                                                     Func<OperationResult<TResult>, Task<OperationResult<T>>> func)
        {
            try
            {
                return result.Match(_success => func.Invoke(result.Result),
                                    _fail => Task.FromResult(new OperationResult<T>(result.Error)));
            }
            catch (Exception e) { return Task.FromResult(OperationResult<T>.Fail(Error.Create(e))); }
        }

        /// <summary>
        /// Async Map
        /// </summary>
        public static async Task<OperationResult<T>> MapT<TResult, T>(this OperationResult<Task<TResult>> result,
                                                                           Func<TResult, T> func)
        {
            try
            {
                return result.HasError ?
                        new OperationResult<T>(result.Error) :
                        func.Invoke(await result.Result);
            }
            catch (Exception e) { return e; }
        }

        /// <summary>
        /// Async Bind
        /// </summary>
        public static async Task<OperationResult<T>> BindT<TResult, T>(this OperationResult<Task<TResult>> result,
                                                                            Func<Task<OperationResult<TResult>>, Task<OperationResult<T>>> func)
        {
            try
            {
                return result.HasError ?
                        new OperationResult<T>(result.Error) :
                        (await func.Invoke(result.MapT(x => x)));
            }
            catch (Exception e) { return e; }
        }
    }
}
