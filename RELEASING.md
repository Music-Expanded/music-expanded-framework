# Releasing

0. Bump the version field in the csproj, so that the version is set correctly on the assembly.
1. Go the [latest workflow on the main branch](https://github.com/Music-Expanded/music-expanded-framework/actions?query=branch%3Amain)
2. Download the artifact on that workflow titled `MusicExpandedFramework`.
3. Unzip that as a folder into your Mod folder.
    - You can find the mod folder in steam by clicking on the gear icon of Steam, under `Manage > Browse Local Files` The mod folder will be titled `Mods`
    - The final file structure to this file should be something like `RimWorld/Mods/MusicExpandedFramework/RELEASING.md`
4. Start RimWorld
5. In the mod list, find `ModExpandedFramework` and hit the `Upload to Steam Workshop` button.
    - If this is the first time the mod is released, it'll add a file, `About/PublishedFileId.txt`. We're going to want that in the repo so pass it to Caaz or upload that to the repo.
6. Update Github Release. (Optional, but nice to have.)
    - Create a new release. Give it a tag of whatever the version currently is.
    - Get the zip from the download before, upload it to the attachments selection
    - Optionally add some change notes and a title
    - Save
