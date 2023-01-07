using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Ddd.Infrastructure
{
	/// <summary>
	/// Базовый класс для всех Value типов.
	/// </summary>
	public class ValueType<T>
	{
		public override string ToString()
		{
			//PersonName(FirstName: A; LastName: B)
			var type = GetType();
			var result = new StringBuilder($"{type.Name}(");
			var propeties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().OrderBy(x => x.Name);
			
			foreach (var property in propeties)
			{
				result.Append($"{property.Name}: {property.GetValue(this)}; ");
			}

			result.Remove(result.Length-2, 2);
			result.Append(")");
			return result.ToString();
		}
		
		public override bool Equals(object obj)
		{
			if (obj is null)
				return false;
			var objType = obj.GetType();
			if (GetType() != objType) return false;
			return Equals((T)obj);
		}
		
		public bool Equals(T obj)
		{
			if (obj == null)
				return false;
			var objType = obj.GetType();
			foreach (var property in objType.GetProperties())
			{
				var thisValue = property.GetValue(this, null);
				var objValue = property.GetValue(obj,null);
				if(thisValue == null && objValue == null)
					continue;
				if(!thisValue.Equals(objValue))
					return false;
			}
			return true;
		}


		public override int GetHashCode()
		{
			var properties = GetType().GetProperties();
			unchecked
			{
				var hash = 17;
				foreach (var t in properties)
				{
					if (t.GetValue(this, null) != null)
						hash = hash * 31 + t.GetValue(this, null).GetHashCode();
				}
				return hash;
			}		
		}
	}
}