using System;
using System.Text;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        private readonly string _product;
        private readonly MessageType _type;
        private readonly MessageTopic _topic;
        public Category(string product, MessageType type, MessageTopic topic)
        {
            _product = product;
            _type = type;
            _topic = topic;
        }
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is Category category)
                {
                    return _product == category._product && 
                           _type == category._type && 
                           _topic == category._topic;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (_product, _type, _topic).GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var category = (Category) obj;
            var byProduct = _product.CompareTo(category._product);
            var byType = _type.CompareTo(category._type);
            var byTopic = _topic.CompareTo(category._topic);
            if (byProduct != 0) return byProduct;
            else if (byType != 0) return byType;
            else if (byTopic != 0) return byTopic;
            return 0;
        }

        public static bool operator >(Category first, Category second)
        {
            return first.CompareTo(second) > 0;
        }
        
        public static bool operator <(Category first, Category second)
        {
            return first.CompareTo(second) < 0;
        }
        
        public static bool operator >=(Category first, Category second)
        {
            if (first.Equals(second)) return true;
            return first.CompareTo(second) > 0;
        }
        
        public static bool operator <=(Category first, Category second)
        {
            if (first.Equals(second)) return true;
            return first.CompareTo(second) < 0;
        }

        public string ToString()
        {
            var result = new StringBuilder();
            result.Append(_product+".");
            result.Append(_type+".");
            result.Append(_topic);
            return result.ToString();
        }
        
    }
}
