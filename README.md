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
- Extra chat commands [Some commands are from [Corecii](https://github.com/Corecii) and [Nico's](https://github.com/larnin) [Server Mod](https://github.com/Corecii/ServerMod)]
- The max player server limit can be adjusted [Credit to [vddCore](https://github.com/vddCore) & [Reherc's](https://github.com/REHERC) port]

<details>
  <summary>List of Commands</summary>

  * /date Displays the host's time and date. If used while not the host, displays the client's time and date.
  * /description Displays the description of the current level.
  * /help Explains how commands work.
  * /level Displays the name of the current level.
  * /private [HOST ONLY] Sets the server to private as well as the password.
  * /public [HOST ONLY] Sets the server to public.
  * /server Displays the server title. [HOST ONLY] The title can also be changed.
  * /shuffle [HOST ONLY] Shuffles the current level playlist.
  * /timeout [HOST ONLY] Starts the countdown timer. You can also set how long the countdown is (in seconds).
  * /canceltimeout [HOST ONLY] Cancels the countdown timer.
</details>


KNOWN ISSUES:
- When collisions are active, the tires on the Network cars act strangely. This is likely because of the car body becoming kinematic. This can be avoided by switching between simulated and kinematic when necessary, but I have not implemented that. (May be fixed)

THINGS I WANT TO ADD BUT I DON'T HAVE TIME:
- A /restartme command that allows a player to restart themselves without leaving and rejoining.
- Server Host Events that allow the host to change the gameplay like turn on Monster Truck for everyone who has the mod in the server. This wouldn't be compatible to base game unfortunately, but it could make something like Monster Truck servers real or even let the host change car physics for players. (Would disable leaderboards of course)
