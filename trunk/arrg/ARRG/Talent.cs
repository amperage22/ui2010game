using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class Talent
    {
        private int amount, amount2, points, maxPoints;
        Modifier modifier, modifier2;
        CreatureType opponent;
        private String description;

        /**
         * amount The numerical modification (+5 of the "+5 hp")
         * mod What type of modification will be done to the player/creature
         * opponent The type of creature that this mod affects (opponent-wise)
         * maxPoints The maximum amount of points a player may spend on this talent
         * mult The multiplier on amount per point of this talent
         */
        public Talent(int amount, Modifier mod, CreatureType opponent, int maxPoints, int mult, String description)
        {
            this.amount = amount;
            modifier = mod;
            this.opponent = opponent;
            points = 0;
            this.maxPoints = maxPoints;
            if (description == null)
                throw new Exception("You must pass a valid description to this talent!");
            this.description = description;
        }

        /**
         * If a talent has two attributes, use this constructor
         */
        public Talent(int amount, Modifier mod, CreatureType opponent, int maxPoints, int mult, String description, int amount2, Modifier mod2)
            : this(amount, mod, opponent, maxPoints, mult, description)
        {
            this.amount2 = amount2;
            modifier2 = mod2;
        }

        public bool increment()
        {
            if (points == maxPoints)
                return false;
            points++;
            return true;
        }

        public bool decrement()
        {
            if (points == 0)
                return false;
            points--;
            return true;
        }

        public void reset()
        {
            points = 0;
        }

        public override String ToString()
        {
            return String.Format("{0}/{1}", points, maxPoints);
        }

        public String getDescription()
        {
            return description;
        }
    }
}
