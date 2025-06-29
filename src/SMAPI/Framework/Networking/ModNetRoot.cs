using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;

namespace StardewModdingAPI.Framework.Networking
{
    public class ModNetRoot<T> : NetRoot<T> where T : class, INetObject<INetSerializable>
    {
        private string ModId;
        public ModNetRoot(string ModId)
        {
            this.ModId = ModId;
        }

        public ModNetRoot(string ModId, T value) : base(value)
        {
            this.ModId = ModId;
        }
    }
}
