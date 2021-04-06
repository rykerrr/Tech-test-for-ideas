# cAce-full-proj
Unfinished 3d shooter project i worked on in 2020, it has a few interesting ideas although the code itself isn't great (most of the ideas were based on anticheat system, although they're not fully implemented), and a refactor might actually make some of the project a decent asset.
It also has some features which may seem like they were based on multiplayer systems, as multiplayer was planned to be implemented here, but the project grinded to a halt as I realised the code became difficult to maintain due to readability issues, especially when having to take breaks due to studies
- Custom cursor implementation with an offset, using the new input system this works well although it may or may not respect screen boundaries (usually does but not always)
This implementation was mainly to "copy" a few anticheats, where the cursor is offset in such a way that it acts as a virtual cursor, this would prevent aim bots from working by simply snapping the OS's cursors
It also allows people to create their own cursors by putting textures in a folder, and creating a CustomCursor data container with that texture, color (RGBA), name
The cursor also has an IFF, allowing you to have an OnEnemy, OnAlly and OnNeutral cursor (can just be recolors of the same texture, red, green/blue, grey)
UI support for said cursor by using UI raycasts
- Save/Load/Delete file I/O based system with cursors, and the IFF-equipped cursors
- Implements a camera system that rotates based on the player's camera, and the actual character
It's similar to cinemachine, in fact it was supposed to be like that until I realized that cinemachine is much easier to use than I though
- Texture merging certain textures to create "complex"/composite textures for use within cursors
- Uses iTween to create an expandable default cursor
- Different ray effect visuals, instant-hit ray, travel-over-time (translation) ray, while the damage is applied beforehand
