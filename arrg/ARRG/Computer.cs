using System;
using System.Collections.Generic;
using GoblinXNA.SceneGraph;

namespace ARRG_Game
{
    class Computer: Player
    {
        public Computer(int playerNum, MarkerNode ground)
            : base(playerNum, ground)
        {
        }

        public override void updateAttack(Die[] die2)
        {
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null)
                {
                    d.setWeakestEnemy(die2);
                }
            }
        }
    }
}
