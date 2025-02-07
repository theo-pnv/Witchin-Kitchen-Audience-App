﻿using System.Collections;
using System.Collections.Generic;
using audience.game;
using audience.messages;
using UnityEngine;

namespace audience.game
{

    public class DiscoManiaSpell : ASpell
    {
        public override string GetSpritePath()
        {
            return "Spells/Discomania";
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.disco_mania;
        }

        public override string GetTitle()
        {
            return "Disco Mania";
        }

        public override string GetDescription()
        {
            return "Reverse a player's controls and make them dance!";
        }

    }

}
