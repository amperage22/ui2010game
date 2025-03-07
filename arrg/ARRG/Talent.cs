﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class Talent
    {
        private int amount, amount2, points, maxPoints;
        private ModifierType modifier, modifier2;
        private CreatureType opponent;
        private String description, description2;
        private bool dual_effect = false;

        /**
         * amount The numerical modification (+5 of the "+5 hp")
         * mod What type of modification will be done to the player/creature
         * opponent The type of creature that this mod affects (opponent-wise)
         * maxPoints The maximum amount of points a player may spend on this talent
         * mult The multiplier on amount per point of this talent
         */
        public Talent(int amount, ModifierType mod, CreatureType opponent, int maxPoints, String description)
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
        public Talent(int amount, ModifierType mod, CreatureType opponent, int maxPoints, String description, int amount2, ModifierType mod2, String description2)
            : this(amount, mod, opponent, maxPoints, description)
        {
            this.amount2 = amount2;
            modifier2 = mod2;
            this.description2 = description2;
            dual_effect = true;
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

        public bool isMaxed()
        {
            return points == maxPoints;
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
            bool maxedOut = points == maxPoints;
            String result = String.Format(
                "{0}{1}{2}{3} {4}",
                maxedOut ? "" : "Next Point: \n\n",
                amount >= 0 ? "+" : "",
                maxedOut ? amount * points : amount * (points + 1),
                isPercentModifier(modifier) ? "%" : "",
                description
                );
            if (dual_effect)
                return String.Format(
                    "{0}\n{1}{2}{3} {4}",
                    result,
                    amount2 >= 0 ? "+" : "",
                    maxedOut ? amount2 * points : amount2 * (points + 1),
                    isPercentModifier(modifier2) ? "%" : "",
                    description2);
            return result;
        }

        private bool isPercentModifier(ModifierType m)
        {
            switch (m)
            {
                case ModifierType.NONE:
                case ModifierType.POWER:
                case ModifierType.HP:
                    return false;
                default:
                    return true;
            }
        }
        
        public bool hasPoints() {
            return points != 0;
        }

        public bool hasDualEffect()
        {
            return dual_effect;
        }

        public Buff getAsBuff()
        {
            return new Buff(modifier, opponent, amount);
        }

        //Only works if there is a second effect
        public Buff getAsBuff2()
        {
            if (!dual_effect)
                throw new Exception("This talent only has 1 effect!");
            return new Buff(modifier2, opponent, amount2);
        }
    }
}
