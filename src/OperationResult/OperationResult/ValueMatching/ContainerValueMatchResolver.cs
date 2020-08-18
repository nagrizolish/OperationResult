namespace OperationResult.OperationResult.ValueMatching
{
    /// <summary>
    ///  Provide value resolving logic in and provide some type restriction on 
    ///  <see cref="OperationResult{TResult}.ResolveContainerValue{TMatchingResult}(System.Func{ContainerValueMatchBuilder{TResult, TMatchingResult}, ContainerValueMatchResolver{TResult, TMatchingResult}})"/>
    /// </summary>
    public class ContainerValueMatchResolver<TContainerValue, TMatchingResult>
    {
        private ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult> _descriptor;

        internal ContainerValueMatchResolver(ContainerValueMatchingDescriptor<TContainerValue, TMatchingResult> descriptor) =>
            _descriptor = descriptor;

        internal TMatchingResult ResolveValue() =>
             _descriptor.HasError ? _descriptor.ErrorCase(_descriptor.Error) : _descriptor.SuccessCase(_descriptor.ResultContainerValue);
    }
}
