# cAce-full-proj
Unfinished 3d shooter project i worked on in 2020, it has a few interesting ideas although the code itself isn't great, and a refactor might actually make some of the project a decent asset
- Custom cursor implementation with an offset, using the new input system this works well although it may or may not respect screen boundaries (usually does but not always)
This implementation was mainly to "copy" a few anticheats, where the cursor is offset in such a way that it acts as a virtual cursor, this would prevent aim bots from working by simply snapping the OS's cursors
It also allows people to create their own cursors by putting textures in a folder, and creating a CustomCursor data container with that texture, color (RGBA), name
- Implements a camera system that rotates based on the player's camera, and the actual character
It's similar to cinemachine, in fact it was supposed to be like that until I realized that cinemachine is much easier to use than I though
