using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    public enum CardType { NONE,STAT_MOD, DMG_DONE, DMG_PREVENT};
    class Card
    {
        private int marker;
        private int healthMod, dmgMod, dmgDone, dmgPrevent;
        private CardType type;

        public Card(CardType type,int marker, int dmg, int health)
        {
            this.type = type;
            this.marker = marker;

            switch (type)
            {
                case CardType.STAT_MOD: dmgMod = dmg; dmgDone = 0; dmgPrevent = 0; healthMod = health; break;
                case CardType.DMG_DONE: dmgMod = 0; dmgDone = dmg; dmgPrevent = 0; healthMod = 0; break;
                case CardType.DMG_PREVENT: dmgMod = 0; dmgDone = 0; dmgPrevent = health; healthMod = 0; break;
            }
        }
        public Card(CardType type,int marker, int mod)
        {
            this.type = type;
            this.marker = marker;

            switch (type)
            {
                case CardType.DMG_DONE: dmgMod = 0; dmgDone = mod; dmgPrevent = 0; healthMod = 0; break;
                case CardType.DMG_PREVENT: dmgMod = 0; dmgDone = 0; dmgPrevent = mod ; healthMod = 0; break;
            }
        }
        public void castSpell(Monster m)
        {
            switch (type)
            {
                case CardType.STAT_MOD: m.addMod(dmgMod,healthMod); break;
                case CardType.DMG_DONE: m.dealDirectDmg(dmgDone); break;
                case CardType.DMG_PREVENT: m.preventDmg(dmgPrevent); break;
            }
        }
    }
}
