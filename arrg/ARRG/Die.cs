using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.SceneGraph;

namespace ARRG_Game
{
    class Die
    {
        private MarkerNode[] sides;

        public Die(MarkerNode[] sides)
        {
            if (sides.Length != 6) throw new Exception("A die must have exactly 6 sides.");
            for (int i = 0; i < sides.Length; i++)
                if (sides[i] == null)
                    throw new Exception("One or more of the MarkerNodes are null.");
            this.sides = sides;
        }
    }
}
