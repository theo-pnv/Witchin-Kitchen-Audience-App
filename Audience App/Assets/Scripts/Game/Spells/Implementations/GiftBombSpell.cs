﻿using System.Collections;
using System.Collections.Generic;
using audience.game;
using audience.messages;
using UnityEngine;

namespace audience.game
{

    public class GiftBombSpell : ASpell
    {
        public override string GetSpritePath()
        {
            return "Spells/GiftBomb";
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.gift_bomb;
        }

        public override string GetTitle()
        {
            return "Gift Bomb";
        }

        public override string GetDescription()
        {
            return "The player may think it's a gift, but...";
        }

    }

}
