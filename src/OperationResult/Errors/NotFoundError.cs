
namespace OperationResult.Errors
{
    //Represent not found HTTP 404. Well here clearly, contain an Id that was not found
    public class NotFoundError<TId> : Error
    {
        public TId Id { get; set; }

        private NotFoundError(TId id) : base($"Entity with id:{id} was not found")
        {
            Id = id;
        }

        public static NotFoundError<TId> Create(TId id) =>
            new NotFoundError<TId>(id);
    }
}
