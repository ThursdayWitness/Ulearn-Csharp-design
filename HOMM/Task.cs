using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.MapObjects
{
    public class Dwelling : IAssignable
    {
        public int Owner { get; set; }
    }

    public class Mine : IAssignable, IPickable, IWinnable
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IPickable, IWinnable
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves : IWinnable
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : IPickable
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IWinnable winnableObj)
            {
                if (!player.CanBeat(winnableObj.Army))
                {
                    player.Die();
                    return;
                }
            }
            if (mapObject is IAssignable assignableObj)
            {
                assignableObj.Owner = player.Id;
            }
            if (mapObject is IPickable pickableObj)
            {
                player.Consume(pickableObj.Treasure);
            }
        }
    }

    public interface IWinnable
    {
        Army Army { get; }
    }

    public interface IPickable
    {
        Treasure Treasure { get; }
    }

    public interface IAssignable
    {
        int Owner { set; }
    }
}