# How To Guides

## How do I make a Music Expanded theme?

### Examples
- [Music Expanded Core](https://github.com/Music-Expanded/music-expanded-core)
  - A collection of three themes, each in their own folder for ease of use.

### Guide
At the bare minimum a theme requires at least two tracks. Music Expanded will not play the same track twice (which apparently was vanilla behavior as well).

#### Step 1: Adding TrackDefs
`ExampleMod/Defs/Tracks.xml`
```xml
<Defs>
    <MusicExpanded.TrackDef>
        <label>Caaz - The Coolest Song on Earth</label>
        <defName>ExampleMod_TheCoolestSongOnEarth</defName>
        <clipPath>ExampleMod/CoolSong.mp3</clipPath>
    </MusicExpanded.TrackDef>
    <MusicExpanded.TrackDef>
        <label>Caaz - The Worst Song on Earth</label>
        <defName>ExampleMod_TheWorstSongOnEarth</defName>
        <clipPath>ExampleMod/WorstSong.mp3</clipPath>
    </MusicExpanded.TrackDef>
</Defs>
```
The `label` is used when the "now playing" feature is enabled, so let's ensure that's pretty. `clipPath` is used to locate the file for the audio. As with all RimWorld sounds, this should be located within the `Sounds` folder of the mod, so `ExampleMod/Sounds/ExampleMod/CoolSong.mp3` in this case.

#### Step 2: Adding the ThemeDef
Next, we can create the Theme and associate it with those tracks.

`ExampleMod/Defs/Theme.xml`
```xml
<Defs>
    <MusicExpanded.ThemeDef>
        <defName>ExampleMod_Theme</defName>
        <label>Music Expanded: Example</label>
		<iconPath>UI/HeroArt/RimWorldLogo</iconPath>
        <description>An example theme to show modders how to make a cool thing!</description>
        <tracks>
            <li>ExampleMod_TheCoolestSongOnEarth</li>
            <li>ExampleMod_TheWorstSongOnEarth</li>
        </tracks>
    </MusicExpanded.ThemeDef>
</Defs>
```

#### Step 3: You're Done! Mostly.
Badabing bada done. Load up your mod, with Music Expanded Framework as well, of course. In the mod settings for Music Expanded, you should find your theme! 

That's the bare minimum though. Let's get fancy. Want to replace the starting guitar riff sound? Let's add a sound def that'll replace it.

`ExampleMod/Defs/Sounds.xml`
```xml
<Defs>
    <MusicExpanded.SoundDef>
        <defName>ExampleMod_StartSting</defName>
        <replaces><li>GameStartSting</li></replaces>
        <subSounds>
            <li>
                <onCamera>True</onCamera>      
                <grains>
                    <li Class="AudioGrain_Clip">
                        <clipPath>ExampleMod/StartSting</clipPath>
                    </li>
                </grains>      
                <volumeRange>100~100</volumeRange>
            </li>
        </subSounds>
    </MusicExpanded.SoundDef>
</Defs>
```
The `replaces` field is what drives this feature, this single SoundDef can replace multiple vanilla sounds if you really wanted. Anyway, we need to add it to the Themedef as well. Back in that file, let's update it to the following:

`ExampleMod/Defs/Theme.xml`
```xml
<Defs>
    <MusicExpanded.ThemeDef>
        <defName>ExampleMod_Theme</defName>
        <label>Music Expanded: Example</label>
		<iconPath>UI/HeroArt/RimWorldLogo</iconPath>
        <description>An example theme to show modders how to make a cool thing!</description>
        <sounds>
            <li>ExampleMod_StartSting</li>
        </sounds>
        <tracks>
            <li>ExampleMod_TheCoolestSongOnEarth</li>
            <li>ExampleMod_TheWorstSongOnEarth</li>
        </tracks>
    </MusicExpanded.ThemeDef>
</Defs>
```

And there you are! When you start a new scenario, the guitar riff should be replaced by whatever your sounddef has.

#### Step 4: Adding a Cue to a Track
Music Expanded adds the feature to play specific tracks during specific situations. Is your name Daniel? Good news so is mine, here, let's add a cool theme song when you start a colonist named `Daniel`. In the `ExampleMod/Defs/Tracks.xml` file, let's add this def.
```xml
<MusicExpanded.TrackDef>
    <label>Caaz - Daniel's Song</label>
    <defName>ExampleMod_DanielsSong</defName>
    <clipPath>ExampleMod/Daniel's Song.mp3</clipPath>
    <cue>StartWithNamedColonist</cue>
    <namedPawn>Daniel</namedPawn>
</MusicExpanded.TrackDef>
```
Then, we can add that to the list of tracks in the theme.