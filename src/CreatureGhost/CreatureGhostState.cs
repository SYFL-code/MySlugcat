using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SlughostMod;

public class CreatureGhostState : CreatureState
{

    public CreatureGhostState(AbstractCreature abstractCreature, SlugcatStats.Name slugcatCharacter, bool isGhost) : base(abstractCreature)
    {

    }
}
