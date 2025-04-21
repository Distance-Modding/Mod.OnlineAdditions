using Events;
using Events.RaceMode;
using Events.ClientToAllClients;
using Events.ChatLog;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OnlineAdditions.Patches
{
    //I can read commands in this patch.

    //level and description command untested.
    [HarmonyPatch(typeof(ChatInputV2), "CheckForServerCode", new System.Type[] { typeof(string) })]
    internal class ChatInputV2__CheckForServerCode
    {

        [HarmonyPrefix]
        internal static bool CheckForCommands(ChatInputV2 __instance, string text)
        {
            Mod.Log.LogInfo(text);
            if (string.IsNullOrEmpty(text) || text.Length > 0 && text[0] != '/')
            {
                Mod.Log.LogInfo("Not a command!");
                return true;
            }
            int num1 = text.IndexOf(" ");
            string str = string.Empty;
            string key;
            bool flag = false; //This is used when dealing with specific commands
            int result1 = 0;

            if (num1 != -1)
            {
                key = text.Substring(1, num1 - 1);
                Mod.Log.LogInfo("Command Name: " + key);
                str = text.Substring(num1 + 1, text.Length - num1 - 1);
                Mod.Log.LogInfo("Subtext: " + str);
                flag = int.TryParse(str, out result1);
            }
            else
            {
                key = text.Substring(1, text.Length - 1);
            }

            if (key != null)
            {
                Mod.Log.LogInfo("Command is: " + key);
                if (key == "date")
                {
                    StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Current date: " + DateTime.Now.ToString()));
                    return false;
                }

                if (key == "help")
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        string helpDescription = HelpDescription(str);
                        if (Mod.Instance.amIHost)
                        {
                            StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data(helpDescription));
                        }
                        else
                        {
                            __instance.AddChat(helpDescription);
                        }
                    }
                    else
                    {
                        if (Mod.Instance.amIHost)
                        {
                            StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data(HelpCommandList));
                        }
                        else
                        {
                            __instance.AddChat(HelpCommandList);
                        }
                    }
                    return false;
                }

                if (key == "level")
                {
                    StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Current Level: " + G.Sys.GameManager_.LevelName_));
                    return false;
                }

                if (key == "description")
                {
                    StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Level Description: " + G.Sys.LevelSets_.GetLevelInfo(G.Sys.GameManager_.LevelPath_).levelDescription_));
                    return false;
                }

                if (key == "playercount" && Mod.Instance.amIHost)
                {
                    if(string.IsNullOrEmpty(str))
                    {
                        StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Player Count: " + (UnityEngine.Network.maxConnections + 1))); 
                    }
                    else if (!string.IsNullOrEmpty(str) && flag && result1 > 1 && result1 < 100)
                    {
                        StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Player count has been set to " + result1));
                        G.Sys.NetworkingManager_.maxPlayerCount_ = result1;
                    }
                    else
                    {
                        __instance.AddChat("Please enter a valid number between 2-99 (e.g. /playercount 20)".Colorize("[CE3333]"));
                    }
                    return true;
                }

                if (key == "private" && Mod.Instance.amIHost)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        G.Sys.NetworkingManager_.password_ = str;
                        G.Sys.NetworkingManager_.privateServer_ = true;
                        UnityEngine.Network.incomingPassword = str;
                        G.Sys.NetworkingManager_.ReportToMasterServer();
                        StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("The server is now private! \nPassword: " + str));
                    }
                    else
                    {
                        G.Sys.NetworkingManager_.password_ = "123";
                        G.Sys.NetworkingManager_.privateServer_ = true;
                        UnityEngine.Network.incomingPassword = "123";
                        G.Sys.NetworkingManager_.ReportToMasterServer();
                        StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("The server is now private! \nPassword: 123"));
                    }
                    return false;
                }

                if (key == "public" && Mod.Instance.amIHost)
                {
                    G.Sys.NetworkingManager_.password_ = "";
                    G.Sys.NetworkingManager_.privateServer_ = false;
                    UnityEngine.Network.incomingPassword = "";
                    G.Sys.NetworkingManager_.ReportToMasterServer();
                    StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("The server is now public!"));
                    return false;
                }

                if (key == "server")
                {
                    if (!string.IsNullOrEmpty(str) && !flag)
                    {
                        G.Sys.NetworkingManager_.serverTitle_ = str;
                        G.Sys.NetworkingManager_.ReportToMasterServer();
                        StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Server title changed to: " + str));
                    }
                    else
                    {
                        StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data("Server Title: " + G.Sys.NetworkingManager_.serverTitle_));
                    }
                    return false;
                }

                if (key == "shuffle" && Mod.Instance.amIHost)
                {
                    if (G.Sys.GameManager_.ModeID_ == GameModeID.Trackmogrify)
                    {
                        __instance.AddChat(("You can't manage the playlist in trackmogrify").Colorize("[CE3333]"));
                        return false;
                    }

                    LevelPlaylist playlist = new LevelPlaylist();
                    playlist.CopyFrom(G.Sys.GameManager_.LevelPlaylist_);

                    if (playlist.Count_ == 0)
                    {
                        __instance.AddChat(("The playlist is empty!").Colorize("[CE3333]"));
                        return false;
                    }

                    if (GameManager.SceneName_.Equals("GameMode"))
                    {
                        ShuffleOnGame(playlist);
                    }
                    else
                    {
                        ShuffleOnLobby(playlist);
                    }

                    G.Sys.GameManager_.LevelPlaylist_.SetIndex(0);
                    G.Sys.GameManager_.NextLevelName_ = G.Sys.GameManager_.LevelPlaylist_.Playlist_[0].levelNameAndPath_.levelName_;
                    G.Sys.GameManager_.NextLevelPath_ = G.Sys.GameManager_.LevelPlaylist_.Playlist_[0].levelNameAndPath_.levelPath_;

                    __instance.AddChat("Playlist shuffled!");

                    return false;
                }

                if (key == "timeout" && Mod.Instance.amIHost && !Mod.Instance.allPlayersFinished)
                {
                    int time;
                    if (flag)
                    {
                        time = result1;
                    }
                    else
                    {
                        time = Mod.TimeLimitAmount.Value;
                    }

                    StaticTargetedEvent<FinalCountdownActivate.Data>.Broadcast(UnityEngine.RPCMode.All, new FinalCountdownActivate.Data(Timex.ModeTime_ + time, UnityEngine.Mathf.RoundToInt(time)));
                    Mod.Instance.countdownActive = true;
                    return false;
                }

                if (key == "canceltimeout" && Mod.Instance.amIHost && !Mod.Instance.allPlayersFinished)
                {
                    StaticTransceivedEvent<FinalCountdownCancel.Data>.Broadcast(new FinalCountdownCancel.Data());
                    Mod.Instance.countdownActive = false;
                    return false;
                }

                /*if (key == "restartme" && !Mod.Instance.playerFinished && !Mod.Instance.countdownActive)
                {
                    G.Sys.GameManager_.Mode_.FinishAllLocalPlayers(FinishType.DNF);
                    
                    
                    Mod.Instance.StartCoroutine(Mod.Instance.ActivateRestartAfterSeconds(2f));
                    return false;
                }*/

                //__instance.AddChat(("The command '" + key + "' doesn't exist.").Colorize("[CE3333]"));
            }
            return true;
        }

        internal static string HelpDescription(string command)
        {
            switch(command)
            {
                case "canceltimeout":
                    return "\"/canceltimeout\"\n[HOST ONLY] Cancels the countdown timer.";

                case "date":
                    return "\"/date\"\nDisplays the host's time and date.";

                case "description":
                    return "\"/description\"\nDisplays the description of the current level.";

                case "endvote":
                    return "\"/endvote\"\n[HOST ONLY] Ends the vote for the current level and displays a score based on submitted votes.";

                case "finishall":
                    return "\"/finishall\"\nForces a DNF on all players in a lobby.";

                case "help":
                    return "\"/help [command]\"\nYou're using that command right now! What more help do you need?";

                case "kick":
                    return "\"/kick [player]\"\n[HOST ONLY] kicks the player specified from the lobby. You can also use a number representing a player. You can get this number with \"/list\".";

                case "level":
                    return "\"/level\"\nDisplays the name of the current level.";

                case "list":
                    return "\"/list\"\n[HOST ONLY] Displays a list of all players that are in the lobby.";

                case "playercount":
                    return "\"/playercount [number]\"\n[HOST ONLY] Displays the player count for the server. You can also the player count (up to 99).";

                case "private":
                    return "\"/private [password]\"\n[HOST ONLY] Sets the server to private as well as the password.";

                case "public":
                    return "\"/public\"\n[HOST ONLY] Sets the server to public.";

                case "server":
                    return "\"/server [title]\"\nDisplays the server title. [HOST ONLY] The title can also be changed.";

                case "shuffle":
                    return "\"/shuffle\"\n[HOST ONLY] Shuffles the current level playlist.";

                case "startvote":
                    return "\"startvote\"\n[HOST ONLY] Starts a vote for the current level. Players can submit a positive or negative vote by sending \"+\" or \"-\" into the chat";

                case "timelimit":
                    return "\"/timelimit [time]\"\n[HOST ONLY] Changes the timelimit of the Reverse Tag time (in seconds).";

                case "timeout":
                    return "\"/timeout [time]\"\n[HOST ONLY] Starts the countdown timer. You can also set how long the countdown is (in seconds).";

                default:
                    return ("Server command '" + command + "' not recognized").Colorize("[CE3333]");
            }
        }

        internal static void ShuffleOnLobby(LevelPlaylist playlist)
        {
           List<LevelPlaylist.ModeAndLevelInfo> shuffledList = playlist.playlist_;
            CommandUtilities.Shuffle(playlist.Playlist_, new Random());
            G.Sys.GameManager_.LevelPlaylist_.Clear();
            foreach (LevelPlaylist.ModeAndLevelInfo lvl in shuffledList)
                G.Sys.GameManager_.LevelPlaylist_.Add(lvl);
        }

        internal static void ShuffleOnGame(LevelPlaylist playlist)
        {
            int index = G.Sys.GameManager_.LevelPlaylist_.Index_;
            LevelPlaylist.ModeAndLevelInfo item = playlist.Playlist_[index];
            playlist.Playlist_.RemoveAt(index);

            List <LevelPlaylist.ModeAndLevelInfo> shuffledList = playlist.Playlist_;
            CommandUtilities.Shuffle(playlist.Playlist_, new Random());
            G.Sys.GameManager_.LevelPlaylist_.Clear();
            G.Sys.GameManager_.LevelPlaylist_.Add(item);
            foreach (LevelPlaylist.ModeAndLevelInfo lvl in shuffledList)
                G.Sys.GameManager_.LevelPlaylist_.Add(lvl);
        }

        internal const string HelpCommandList = "Use \"/help [command]\" for a quick description of how a command works." +
                            "\nList of commands:" +
                            "\ndate" +
                            "\ndescription" +
                            "\nfinishall" +
                            "\nhelp" +
                            "\nkick" +
                            "\nlevel" +
                            "\nplayercount" +
                            "\nprivate" +
                            "\npublic" +
                            "\nserver" +
                            "\nshuffle" +
                            "\nstartvote" +
                            "\nendvote" +
                            "\ntimelimit" +
                            "\ntimeout" +
                            "\ncanceltimeout";
    }

    internal static class CommandUtilities
    {
        internal static void Shuffle<T>(this IList<T> list, Random rnd)
        {

            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }

        internal static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
