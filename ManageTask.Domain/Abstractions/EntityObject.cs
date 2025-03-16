namespace ManageTask.Domain.Abstractions
{
    public abstract class EntityObject<T>(T id)
    {
        public T Id { get; protected set; } = id ?? throw new ArgumentNullException(nameof(id), "Id cannot be null");
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (EntityObject<T>)obj;
            return EqualityComparer<T>.Default.Equals(Id, other.Id);
        }
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}
