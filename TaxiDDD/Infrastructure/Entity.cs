using System.Collections.Generic;

namespace Ddd.Infrastructure
{
	/// <summary>
	/// Базовый класс всех DDD сущностей.
	/// </summary>
	/// <typeparam name="TId">Тип идентификатора</typeparam>
	public class Entity<TId>
	{
		public Entity(TId orderId)
		{
			OrderId = orderId;
		}

		public TId OrderId { get; }

		protected bool Equals(Entity<TId> other)
		{
			return EqualityComparer<TId>.Default.Equals(OrderId, other.OrderId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Entity<TId>)obj);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TId>.Default.GetHashCode(OrderId);
		}

		public override string ToString()
		{
			return $"{GetType().Name}({nameof(OrderId)}: {OrderId})";
		}
	}
}