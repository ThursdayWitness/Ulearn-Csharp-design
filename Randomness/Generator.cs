using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly List<FromDistribution> _classProperties = new List<FromDistribution>();

        public Generator()
        {
            foreach (var property in typeof(TClass).GetProperties())
            {
                _classProperties.Add(property.GetCustomAttributes(false)
                    .OfType<FromDistribution>().FirstOrDefault());
            }
        }

        public TClass Generate(Random seed)
        {
            var classInstance = new TClass();
            var counter = 0;
            foreach (var property in typeof(TClass).GetProperties())
            {
                if (_classProperties[counter] == null)
                {
                    counter++;
                    continue;
                }
                property.SetValue(classInstance, _classProperties[counter].Distribution.Generate(seed));
                counter++;
            }
            return classInstance;
        }
    }
}
