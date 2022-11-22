# Music Expanded Framework
This mod overhauls how music functions in RimWorld! It ensures there's always a track playing and introduces cues for specific tracks to play.

# Defs

# Logic
## General Information
Cues are used to play specific songs at specific times. There's a set list in the mod that are used for specific meanings, as well as a custom one you can use to call in your own mod.

## Cues
### None
Not all tracks require a cue, and by default a TrackDef will use `None`. This is intended for tracks that don't require a specific cue to play. 

### Battle Cues
There are four battle cues, `BattleSmall`, `BattleMedium`, `BattleLarge`, and `BattleLegendary`.

Various events call these cues, such as `RaidEnemy`, `Infestation`, or `Ambush`. The gist is that these are the intense events and an intense song should immediately play for it. The "size" of the battle track to play is based on the points for those events. You can find the logic for that in [Source/Utilities.cs](/Source/Utilities.cs) from the `BattleCue` function.

### MainMenu
This cue is to identify which track to play on the Menu. Multiple tracks can have this cue, and the mod will choose one at random based on the track's commonality.

### Credits
Similar to the MainMenu cue, this is used to identify the credits song to play. Again, multiple tracks can have this cue and a random one will be chosen based on the track's commonality.

### LoneColonist
When you start a new colony with only one colonist, this cue will play.

### HasNamedColonist
This cue will play when you start a game with a specifically named colonist, or randomly in the theme when you still have that colonist. It must be paired with `cueData` with the name of the pawn.

#### Example
```xml
<MusicExpanded.TrackDef>
    <label>Daniel's song</label>
    <defName>Example_Daniel</defName>
    <clipPath>Daniel.mp3</clipPath>
    <cue>HasColonistNamed</cue>
    <cueData>Odeum</cueData>
</MusicExpanded.TrackDef>
```

### Custom
This cue isn't called in the framework itself, but is used so that modders can extend the cue feature for their own events. For example, the ManInBlack event could be patched with the `PlayCue` modExtension. Meant to be paired with `cueData` to figure out what this cue is used for.

Specifically, MusicExpanded will first match that a `cue` matches, and then that the `cueData` matches. So, given a custom cue, and we call `MusicExpanded.Utilities.PlayTrack(Cue.Custom, "Potato")` we'll play a track that matches the cue data and cue exactly.

#### Example
```xml
<MusicExpanded.TrackDef>
    <label>Daniel's song</label>
    <defName>Example_Daniel</defName>
    <clipPath>Daniel.mp3</clipPath>
    <cue>HasColonistNamed</cue>
    <cueData>Odeum</cueData>
</MusicExpanded.TrackDef>
```
