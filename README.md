# This is the alpha version of Update V1.1.0
## I am fully aware there is nothing here yet. There will be soon.
## Everything you see here is subject to change. There are more things to be added to this update.

## Do not download this with the intent to use in normal gameplay. This is only for bug reporting / testing / suggestions.

### Changes:

- Fully reworked the fake player AI.

   - It can now spawn at any sanity level.
   - It can now select a random player's suit. (DOES NOT COPY MORECOMPANY COSMETICS YET)
   - Like before, it has three possible stages: staring, wandering, and chasing.
   - It will always stare at you if your insanity level is less than a quarter.
   - If your insanity level is around 75% of the max amount, it will wander around its spawn point.
   - It finally plays footstep sounds
     
-  ### Wandering
    - When the fake player spawns, it will generate 3-5 random spots around the spawn point, and make its way to all of them and will despawn when finished.
    - If the player is seen by or gets too close to them, it will proceed to stare at the player before entering chase mode.
    - (Not added yet) While it makes its way to their destination, it will look around the environment.

- ### Chasing
     - **IT IS NO LONGER STOPPED BY DOORS. YOU NEED TO RUN FOR YOUR LIFE OR LEAVE THE FACILITY**
     - This is mostly the same. It will chase the player until it despawns.
     - (Not added yet) If the player loses LOS, the hallucination will walk to the last seen position and look around. If it doesn't spot you, it will despawn,

- Reworked how hallucinations are selected and stored.
   - Hallucinations are now stored in a dictionary with their ID and the sanity level they should occur at.
   - Allows for chaining hallucinations.
   - Allows for easier additions of hallucinations.

- Walkie Talkies can play Auditory Hallucination sound effects and Skinwalker clips if installed.

- Your sanity level will not be at max until you experience a panic attack. (For those wondering, this is refering to an enum that stores the player's sanity level).

### Additions
- Added a few more sound effects.
  
- Added 2 new hallucinations ( more are coming with this update. This is just the alpha version like stated before.)
  
- Added Light Proximity Detection:
     - You will not lose sanity when in proximity of a light source **(THIS EXCLUDES FLASHLIGHTS)**.
  
- Added a message to notify the player when they're about to or are already experiencing a panic attack.

### Integrations

- Updated skinwalker integration:
     - Please let me know if you experience any performance issues with this new integration method.
     - If you are, I am planning on having a setting in the config the allows you to switch to the older integration method
     - **THIS MEANS YOU WOULD HAVE TO DOWNLOAD A OLDER VERSION OF SKINWALKER (BEFORE 3.0.0)**
- Updated AdvancedCompany compatibility with the Helmet Lamp.
