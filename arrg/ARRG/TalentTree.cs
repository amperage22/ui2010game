using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class TalentTree
    {

        private int spentPoints;
        private Talent[] row1 = new Talent[3];
        private Talent[] row2 = new Talent[2];
        private Talent row3;

        public TalentTree(CreatureType c)
        {
            switch (c)
            {
                case CreatureType.BEASTS:
                    row1[0] = new Talent(1, Modifier.DAMAGE, CreatureType.ALL, 3, "0, 0");
                    row1[1] = new Talent(2, Modifier.HIT, CreatureType.ALL, 3, "0, 1");
                    row1[2] = new Talent(1, Modifier.HP, CreatureType.ALL, 3, "0, 2");
                    row2[0] = new Talent(1, Modifier.DODGE, CreatureType.ALL, 3, "1, 0");
                    row2[1] = new Talent(1, Modifier.HP, CreatureType.DRAGONKIN, 3, "1, 2");
                    row3 = new Talent(2, Modifier.ADDITIONAL_ATTACK, CreatureType.BEASTS, 1, "3, 1");
                    break;
                case CreatureType.DRAGONKIN:
                    row1[0] = new Talent(1, Modifier.HP, CreatureType.ALL, 3, "0, 0");
                    //TODO: When defending?
                    row1[1] = new Talent(-2, Modifier.DAMAGE, CreatureType.ALL, 3, "0, 1");
                    row1[2] = new Talent(1, Modifier.DAMAGE, CreatureType.ALL, 3, "0, 2");
                    row2[0] = new Talent(2, Modifier.CRIT, CreatureType.ALL, 3, "1, 0");
                    row2[1] = new Talent(1, Modifier.DAMAGE, CreatureType.ROBOT, 3, "1, 2");
                    row3 = new Talent(2, Modifier.ADDITIONAL_ATTACK, CreatureType.DRAGONKIN, 1, "3, 1");
                    break;
                case CreatureType.ROBOT:
                    row1[0] = new Talent(2, Modifier.CRIT, CreatureType.ALL, 3, "0, 0");
                    //TODO: When defending?
                    row1[1] = new Talent(2, Modifier.HP, CreatureType.ALL, 3, "0, 1");
                    row1[2] = new Talent(2, Modifier.HP, CreatureType.ALL, 3, "0, 2", -2, Modifier.DAMAGE);
                    row2[0] = new Talent(2, Modifier.HIT, CreatureType.ALL, 2, "1, 0");
                    row2[1] = new Talent(1, Modifier.DAMAGE, CreatureType.BEASTS, 2, "1, 2");
                    row3 = new Talent(2, Modifier.ADDITIONAL_ATTACK, CreatureType.ROBOT, 1, "3, 1");
                    break;
                default:
                    throw new Exception(String.Format("Bad CreatureType passed to TalentTree constructor {0}", c));
            }

            spentPoints = 0;
        }

        public bool increment(int row, int col)
        {
            if ((row == 1 && col == 1) || (row == 2 && col != 1)) return false;
            switch (row)
            {
                case 0: return row1[col].increment();
                case 1: return row2[col == 2 ? 1 : 0].increment();
                case 2: return row3.increment();
            }
            //Unreachable return
            return false;
        }

        public bool decrement(int row, int col)
        {
            if ((row == 1 && col == 1) || (row == 2 && col != 1)) return false;
            switch (row)
            {
                case 0: return row1[col].decrement();
                case 1: return row2[col == 2 ? 1 : 0].decrement();
                case 2: return row3.decrement();
            }
            //Unreachable return
            return false;
        }

        public void reset()
        {
            for (int i = 0; i < row1.Length; i++)
                row1[i].reset();
            for (int i = 0; i < row2.Length; i++)
                row2[i].reset();
            row3.reset();
        }

        public String getPointStr(int row, int col)
        {
            //We are going to have a 3, 2, 1 tree
            if ((row == 1 && col == 1) || (row == 2 && col != 1))
                return "";
            switch (row)
            {
                case 0: return row1[col].ToString();
                case 1: return row2[col == 2 ? 1 : 0].ToString();
                case 2: return row3.ToString();
            }
            //Unreachable return
            return "";
        }

        public String getDescription(int row, int col)
        {
            //We are going to have a 3, 2, 1 tree
            if ((row == 1 && col == 1) || (row == 2 && col != 1))
                return "";
            switch (row)
            {
                case 0: return row1[col].getDescription();
                case 1: return row2[col == 2 ? 1 : 0].getDescription();
                case 2: return row3.getDescription();
            }
            //Unreachable return
            return "";
        }
    }
}
