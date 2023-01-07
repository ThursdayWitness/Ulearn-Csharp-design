using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflection.Randomness
{
    public class FromDistribution : Attribute
    {
        public readonly IContinuousDistribution Distribution;
        
        public FromDistribution(Type distribution, params object[] parameters)
        {
            try
            {
                Distribution = (IContinuousDistribution)Activator.CreateInstance(distribution, parameters);
            }
            catch (Exception)
            {
                throw new ArgumentException("Argument exception: List containing arguments for NormalDistribution constructor is incorrect");
            }
        }
    }
    
    public class Generator<TClass> where TClass : new()
    {
        private readonly IEnumerable<PropertyInfo> _classProperties = typeof(TClass).GetProperties()
            .Where(propertyInfo=> propertyInfo.GetCustomAttributes(false)
                .OfType<FromDistribution>().FirstOrDefault() != null);

        public TClass Generate(Random seed)
        {
            var classInstance = new TClass();
            foreach (var property in _classProperties)
            {
                var propertyDistribution = property.GetCustomAttributes(false)
                                                .OfType<FromDistribution>().First().Distribution;
                property.SetValue(classInstance, propertyDistribution.Generate(seed));
            }

            return classInstance;
        }
    }
}
