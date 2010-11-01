using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    public class Beasts: Monster
    {
        public Beasts(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
        }
    }
}
