# V 1.0.7 Update 2

- Added compatability with Advanced Company's night vision goggles to reduce sanity loss. If any other mods implement something similar to this, please do let me know of them.
- Fake items now have a scrap value.
- Added insanity loss scaling with bigger player lobbies. This is the first iteration of it right now, and will most likely be changed. You can change the amount of players to take into account when scaling in the config file. Setting it to 1 disables it.
- Reworked how Skinwalker mod is integrated. (Before it literally just copied the way skinwalker loads sound files, basically running skinwalker mod twice.)
- Added more configurable settings in the config.
- Added a tooltip to the pills to indicate that they can be used now. (I somehow forgot to add this.)
- Added the ability for player hallucinations to play skinwalker sound files.

- Fixed panic attack audio/visual effects happening instantly rather than gradually over time.
- Fixed Skinwalker mod clips "playing" when playing solo.

I'm sorry for the lack of major content updates and the frequency of updates in general. I'm not the best at coding, and don't have a lof of time to work on this mod in my free time.

# V1.0.5 Update 1
Before I get into the changes/fixes, I want to say I am very sorry for the wait on this patch.

- Added integration with Skinwalker mod. (Clips recorded by the Skinwalker mod have a chance of playing randomly).
- Added functionality to pills.
- Added more settings to the config file.

- Fixed Flashlights not actually lowering sanity loss.
- Fixed panic attack debuffs being permanent.
- Fixed panic visual/audio effects happening instantly instead of over time.

## Planned Features for next update:
- Insanity gain/loss scaling for amount of players in lobby.
- More panic attack debuffs.
- Insanity gain or loss from certain events.

As always, please let me know of any suggestions you have for the mod and do not be afraid to report any bugs on the github.

# V.1.0.0 Official Release

- Increased max insanity cap to 200.

- Added 3 new insanity levels:
    - Low Insanity: Less than 40 insanity.
    - Medium Insanity: More than 40 insanity.
    - High Insanity: More than 100 insanity.

- Changed what causes sanity loss and how much sanity is lost:
  - Being alone inside the facility.
  - No enabled flashlight in inventory.
  - Power shutoff hallucination.
  - Panic Attacks

- Added a panic attack system that happens when you reach max sanity.
    - Can be helped by being around others or leaving the facility.
    - Causes slowness, stamina loss, vision loss, and death if enabled via config.
    
- Added 4 Hallucinations:
  
  - Fake player hallucination
  - Auditory Hallucination
  - Fake Items
  - Power shutoff
