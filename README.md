# Distance Online Additions

What this adds:

- Ability to hide player names in multiplayer
- Ability to enable cheats in multiplayer (Disables leaderboard uploads) [Credit to [JohnCorby](https://github.com/JohnCorby)]
- Ability to enable collisions in multiplayer (Disables leaderboard uploads)
- Ability to allow multiplayer event triggers (Disables leaderboard uploads, also only works with collisions on)
- Ability to hide chat in multiplayer
- Ability to disable the 60 seconds timeout in multiplayer
- Ability to change the length of time for the timeout in multiplayer
- Ability to disable the audio coming from other cars in multiplayer
- Ability to adjust the maximum detail for cars online
- Ability to disable the killgrid rendering for every player online
- Ability to adjust the outline glow brightness for online cars
- The max player server limit can be adjusted [Credit to [vddCore](https://github.com/vddCore) & [Reherc's](https://github.com/REHERC) port]


KNOWN ISSUES:
- When collisions are active, the tires on the Network cars act strangely. This is likely because of the car body becoming kinematic. This can be avoided by switching between simulated and kinematic when necessary, but I have not implemented that.
- When collisions are active, sometimes cars appear laggy at the start of a level. This is due to the Network car waiting for collisions to be active, but positioning the car as if collisions are already on. (It takes 10 seconds for collisions to activate after a car spawns)

THINGS I WANT TO ADD BUT I DON'T HAVE TIME:
- A /restartme command that allows a player to restart themselves without leaving and rejoining.
- Server Host Events that allow the host to change the gameplay like turn on Monster Truck for everyone who has the mod in the server. This wouldn't be compatible to base game unfortunately, but it could make something like Monster Truck servers real or even let the host change car physics for players. (Would disable leaderboards of course)
- Prevent the issue where players get sent to lobby when a level is loading, or at least make the game notify the player what went wrong.
- Porting server commands from the very old Spectrum server mod.
