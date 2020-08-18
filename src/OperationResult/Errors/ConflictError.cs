
namespace OperationResult.Errors
{
    /// <summary>
    /// Error represent HTTP 409 conflict, for example creation on db entity with already existing Id
    /// </summary>
    public class ConflictError : Error
    {
        protected ConflictError(string message) : base(message) { }

        public new static ConflictError Create(string message) => new ConflictError(message);
    }
}
