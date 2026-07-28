namespace ClinicFlow.Domain.Primitives;

public abstract class Entity : IEquatable<Entity>
{
    public long Id { get; protected set; }

    public byte[]? ConcurrencyToken { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity other && Equals(other);
    }

    public bool Equals(Entity? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || GetType() != other.GetType() || Id == 0 || other.Id == 0)
        {
            return false;
        }

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
