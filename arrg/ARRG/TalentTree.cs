using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class TalentTree
    {
        private int[] pointsAllocd = new int[3];
        private bool[] alreadyUnlockedT = new bool[2];
        private Talent[] tier1 = new Talent[3];
        private Talent[] tier2 = new Talent[3];
        private Talent tier3;

        public TalentTree(CreatureType c)
        {
            switch (c)
            {
                case CreatureType.BEASTS:
                    tier1[0] = new Talent(1, ModifierType.POWER, CreatureType.ALL, 2, "DAMAGE dealt.");
                    tier1[1] = new Talent(5, ModifierType.CRIT, CreatureType.ALL, 1, "chance to CRITICALLY hit the enemy.");
                    tier1[2] = new Talent(1, ModifierType.HP, CreatureType.ALL, 2, "HP per monster.");
                    tier2[0] = new Talent(1, ModifierType.DODGE, CreatureType.BEASTS, 2, "chance for your BEASTS to\nDODGE an enemy attack.");
                    tier2[1] = new Talent(2, ModifierType.ADDITIONAL_ATTACK_CHANCE, CreatureType.BEASTS, 1, "chance for your BEASTS to attack\nTWICE in one turn.");
                    tier2[2] = new Talent(5, ModifierType.PARRY, CreatureType.BEASTS, 2, "chance for your BEASTS to\nPARRY an attack.");
                    tier3 = new Talent(50, ModifierType.ADDITIONAL_ATTACK_CHANCE, CreatureType.BEASTS, 1, "chance for your BEASTS to attack\nTWICE in one turn!");
                    break;
                case CreatureType.DRAGONKIN:
                    tier1[0] = new Talent(1, ModifierType.HP, CreatureType.ALL, 2, "HP per monster.");
                    tier1[1] = new Talent(5, ModifierType.HIT, CreatureType.ALL, 1, "chance to HIT the enemy.");
                    tier1[2] = new Talent(1, ModifierType.POWER, CreatureType.ALL, 2, "DAMAGE dealt.");
                    tier2[0] = new Talent(2, ModifierType.CRIT, CreatureType.DRAGONKIN, 2, "chance for DRAGONKIN to\nCRITICALLY hit the enemy.");
                    tier2[1] = new Talent(2, ModifierType.ADDITIONAL_ATTACK_CHANCE, CreatureType.DRAGONKIN, 1, "chance for your DRAGONKIN to attack\nTWICE in one turn.");
                    tier2[2] = new Talent(5, ModifierType.PARRY, CreatureType.DRAGONKIN, 2, "chance for your DRAGONKIN to\nPARRY an attack.");
                    tier3 = new Talent(30, ModifierType.FIREBREATH_ATTACK_CHANCE, CreatureType.DRAGONKIN, 1, "chance for your DRAGONKIN to cast a\nFIRE BREATH attack!");
                    break;
                case CreatureType.ROBOTS:
                    tier1[0] = new Talent(2, ModifierType.CRIT, CreatureType.ALL, 2, "chance to CRITICALLY hit the enemy.");
                    tier1[1] = new Talent(4, ModifierType.DODGE, CreatureType.ALL, 1, "chance to DODGE an enemy attack.");
                    tier1[2] = new Talent(2, ModifierType.HP, CreatureType.ALL, 2, "HP per monster.", -2, ModifierType.POWER, "DAMAGE dealt.");
                    tier2[0] = new Talent(2, ModifierType.HIT, CreatureType.ROBOTS, 2, "chance for ROBOTS to\nHIT the enemy.");
                    tier2[1] = new Talent(2, ModifierType.ADDITIONAL_ATTACK_CHANCE, CreatureType.ROBOTS, 1, "chance for your ROBOTS to attack\nTWICE in one turn.");
                    tier2[2] = new Talent(5, ModifierType.PARRY, CreatureType.ROBOTS, 2, "chance for your ROBOTS to\nPARRY an attack.");
                    tier3 = new Talent(30, ModifierType.LIGHTNING_ATTACK_CHANCE, CreatureType.ROBOTS, 1, "chance for your ROBOTS to cast a\nLIGHTNING BOLT attack.");
                    break;
                default:
                    throw new Exception(String.Format("Bad CreatureType passed to TalentTree constructor {0}", c));
            }
        }

        public bool increment(int row, int col)
        {
            if (!canAllocTier(row) || (row == 2 && col != 1)) return false;
            bool returnVal = false;
            switch (row)
            {
                case 0: returnVal = tier1[col].increment(); break;
                case 1: returnVal = tier2[col].increment(); break;
                case 2: returnVal = tier3.increment(); break;
            }
            if (returnVal)
                pointsAllocd[row]++;
            return returnVal;
        }

        public bool decrement(int row, int col)
        {
            if (row == 2 && col != 1) return false;
            bool returnVal = false;
            switch (row)
            {
                case 0: returnVal = tier1[col].decrement(); break;
                case 1: returnVal = tier2[col].decrement(); break;
                case 2: returnVal = tier3.decrement(); break;
            }
            if (returnVal)
                pointsAllocd[row]--;
            return returnVal;
        }

        public void reset()
        {
            for (int i = 0; i < tier1.Length; i++)
                tier1[i].reset();
            for (int i = 0; i < tier2.Length; i++)
                tier2[i].reset();
            tier3.reset();
            for (int i = 0; i < pointsAllocd.Length; i++)
                pointsAllocd[i] = 0;
        }

        public String getPointStr(int row, int col)
        {
            //We are going to have a 3, 2, 1 tree
            if (row == 2 && col != 1)
                return "";
            switch (row)
            {
                case 0: return tier1[col].ToString();
                case 1: return tier2[col].ToString();
                case 2: return tier3.ToString();
            }
            //Unreachable return
            return "";
        }

        public String getDescription(int row, int col)
        {
            //We are going to have a 3, 2, 1 tree
            if (row == 2 && col != 1)
                return "";
            String toReturn;
            switch (row)
            {
                case 0: toReturn = tier1[col].getDescription(); break;
                case 1: toReturn = tier2[col].getDescription(); break;
                case 2: toReturn = tier3.getDescription(); break;
                default: throw new Exception("Bad row passed!");
            }
            if (!canAllocTier(row))
                return String.Format("You need to spend {0} point{3} in TIER {1} talents to get this.\n\n{2}", 3 - pointsAllocd[row - 1], row == 1 ? "ONE" : "TWO", toReturn, 3 - pointsAllocd[row - 1] == 1 ? "" : "s");
            return toReturn;
        }

        public bool isMaxed(int row, int col)
        {
            //We are going to have a 3, 2, 1 tree
            if (row == 2 && col != 1)
                return false;
            switch (row)
            {
                case 0: return tier1[col].isMaxed();
                case 1: return tier2[col].isMaxed();
                case 2: return tier3.isMaxed();
            }
            //Unreachable return
            return false;
        }
        /*
        public bool justUnlockedTier(int tier)
        {
            switch (tier)
            {
                case 1: return true;
                case 2:
                case 3:
                    if (pointsAllocd[tier - 2] >= 3)
                        if (alreadyUnlockedT[tier - 2]) return false;
                        else return (alreadyUnlockedT[tier - 2] = true);
                    else return false;
            }
            return false;
        }
        */
        public bool canAllocTier(int tier)
        {
            switch (tier)
            {
                case 0: return true;
                case 1:
                case 2:
                    return pointsAllocd[tier - 1] >= 3;
            }
            return false;
        }

        public int getPointsInTier(int tier)
        {
            return pointsAllocd[tier];
        }

        public void resetTier(int tier)
        {
            pointsAllocd[tier] = 0;
            switch (tier)
            {
                case 0:
                    for (int i = 0; i < 3; i++)
                        tier1[i].reset();
                        break;
                case 1:
                    for (int i = 0; i < 3; i++)
                        tier2[i].reset();
                        break;
                case 2: tier3.reset(); break;
            }
        }

        public int getPointsAllocd()
        {
            int sum = 0;
            for (int i = 0; i < 3; i++)
                sum += pointsAllocd[i];
            return sum;
        }

        public List<Talent> getTalents()
        {
            List<Talent> talents = new List<Talent>();
            for (int i = 0; i < 3; i++)
            {
                if (tier1[i].hasPoints())
                    talents.Add(tier1[i]);
                if (tier2[i].hasPoints())
                    talents.Add(tier2[i]);
            }
            if (tier3.hasPoints())
                talents.Add(tier3);
            return talents;
        }
    }
}
