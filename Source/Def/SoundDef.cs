using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MusicExpanded
{
    public class SoundDef : Verse.SoundDef
    {
        public List<Verse.SoundDef> replaces = new List<Verse.SoundDef>();
    }
}
