namespace ManageTask.Infrastructure.Data.Entities
{
    public class BaseEntity<T>
    {
        public T Id { get; set; } = default!;
    }
}
