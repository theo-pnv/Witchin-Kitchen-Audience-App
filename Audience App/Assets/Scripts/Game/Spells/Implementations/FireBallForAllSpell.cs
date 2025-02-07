﻿using System.Collections;
using System.Collections.Generic;
using audience.game;
using audience.messages;
using UnityEngine;

namespace audience.game
{

    public class FireBallForAllSpell : ASpell
    {
        public override string GetSpritePath()
        {
            return "Spells/FireballTurret";
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.fireball_for_all;
        }

        public override string GetTitle()
        {
            return "Fireballs Turret";
        }

        public override string GetDescription()
        {
            return "More fireballs!!";
        }

    }

}
