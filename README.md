# Music Expanded Framework
This mod overhauls how music functions in RimWorld! It ensures there's always a track playing and introduces cues for specific tracks to play.

# Defs

## MusicExpanded.ThemeDef

| Type | Field | Description of Use
| - | - | -
| `string` | label | The theme's title in mod settings. 
| `string` | description | The description in mod settings.
| `string` | iconPath | The image icon in the mod settings
| `List<MusicExpanded.TrackDef>` | tracks | Associates the theme with several tracks.
| `List<MusicExpanded.SoundDef>` | sounds | A list of sounds to replace existing vanilla sounddefs with the theme's sounddef.

## MusicExpanded.TrackDef

| Type | Field | Description of Use
| - | - | -
| `string` | label | Used as the track's title when Now Playing is enabled.
| ``


# Logic
## Cues
There are a number of cues built into this mod, each with their own function. You can find the full list in [Source/Cue.cs](/Source/Cue.cs). Notable cues meant for the community will be described in detail here.

### None
Not all tracks require a cue, and by default a TrackDef will use `None`. This is intended for tracks that don't require a specific cue to play. 

### Battle Cues
There are four battle cues, `BattleSmall`, `BattleMedium`, `BattleLarge`, and `BattleLegendary`.

Various events call these cues, such as `RaidEnemy`, `Infestation`, or `Ambush`. The gist is that these are the intense events and an intense song should immediately play for it. The "size" of the battle track to play is based on the points for those events. You can find the logic for that in [Source/Utilities.cs](/Source/Utilities.cs) from the `BattleCue` function.

### MainMenu
This cue is to identify which track to play on the Menu. Multiple tracks can have this cue, and the mod will choose one at random based on the track's commonality.

### Credits
Similar to the MainMenu cue, this is used to identify the credits song to play. Again, multiple tracks can have this cue and a random one will be chosen based on the track's commonality.

### ManInBlack
This cue is called for the ManInBlack event, to play a dramatic song when you get saved for being a bad RimWorld player

### LoneColonist
When you start a new colony with only one colonist, this cue is used to find a specific track to play.

### StartWithNamedColonist
This cue must be paired with the TrackDef field `namedPawn` to associate the cue with a specific name. 

### AnimalInsanity
This cue is called exclusively for animal insanity events.