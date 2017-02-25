using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevTools
{
    public class DevTools : RocketPlugin
    {
        public FriendsGroupID_t id;
        public SteamPlayer player1;
        public Freenex.FeexRanks.DatabaseManager db;
        public List<string> Data;
        public int res;
        public List<UnturnedPlayer> List;
        static List<GameObject> plugins = new List<GameObject>();
        internal static List<IRocketPlugin> Plugins { get { return plugins.Select(g => g.GetComponent<IRocketPlugin>()).Where(p => p != null).ToList(); } }
        public static DevTools Instance;

        protected override void Load()
        {
            LoggerColor("DevTools Loaded!", ConsoleColor.Cyan);
            LoggerColor("Visit our shop at https://plugin4u.cf/", ConsoleColor.Magenta);
            Instance = this;
        }

        public decimal GetPlayerBalance(string SteamID)
        {
            if (PluginLoaded("Uconomy"))
            {
                return fr34kyn01535.Uconomy.Uconomy.Instance.Database.GetBalance(SteamID);
            }
            else return 0;

        }

        public bool HavePermission(string perm, IRocketPlayer player)
        {
            return R.Permissions.HasPermission(player, perm);
        }

        public bool VacBanned(UnturnedPlayer player)
        {
            return (bool)player.SteamProfile.IsVacBanned;
        }

        private bool PluginLoaded(string name)
        {
            foreach (var p in Plugins)
            {
                if (p.Name == name)
                {
                    res = res + 1;
                }
            }
            if (res > 0) return true;
            else return false;
        }

        private void Freeze(UnturnedPlayer player)
        {
            player.Player.gameObject.AddComponent<Freeze>();
        }

        public void UnFreeze(UnturnedPlayer player)
        {
            Destroy(GetComponent<Freeze>());
        }

        public List<string> GetUconomySQLData()
        {
            if (PluginLoaded("Uconomy"))
            {
                Data.Add(fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseAddress);
                Data.Add(fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabasePort.ToString());
                Data.Add(fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseUsername);
                Data.Add(fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabasePassword);
                Data.Add(fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseName);
                Data.Add(fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.DatabaseTableName);
                return Data;
            }
            else return null;
        }

        public static string PlayerIP(CSteamID cSteamID)
        {
            P2PSessionState_t State;
            SteamGameServerNetworking.GetP2PSessionState(cSteamID, out State);
            return Parser.getIPFromUInt32(State.m_nRemoteIP);
        }

        public void LoggerColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text, color);
        }

        public void SpyPlayer(UnturnedPlayer player)
        {
            player.Player.askScreenshot(player.CSteamID);
        }

        public void Dequip(UnturnedPlayer player)
        {
            player.Player.equipment.dequip();
        }

        public bool IsGold(UnturnedPlayer player)
        {
            return player.IsPro;
        }

        public string InVehicleID(UnturnedPlayer player)
        {
            return Convert.ToString(player.CurrentVehicle.id);
        }

        public List<UnturnedPlayer> GetAllPlayers()
        {
            foreach (var SteamPlayer in Provider.clients)
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(SteamPlayer);
                List.Add(player);
            }
            return List;
        }

        public SteamPlayer GetSteamPlayerFromUnturnedPlayer(UnturnedPlayer player)
        {
            foreach (var SteamPlayer in Provider.clients)
            {
                if (player.CSteamID == SteamPlayer.playerID.steamID)
                {
                    return SteamPlayer;
                }
            }
            return null;
        }

        public void MaxSkills(UnturnedPlayer player)
        {
            foreach (Skill skill in (player.Player.skills.skills.SelectMany((Skill[] skills) => skills)))
            {
                skill.level = skill.max;
            }
        }
       
        public string GetSteamGroupName(CSteamID CSteamID)
        {
            id.m_FriendsGroupID = (short)CSteamID.m_SteamID;
            return SteamFriends.GetFriendsGroupName(id);
        }

        public int GetSteamGroupMembersCount(CSteamID CSteamID)
        {
            id.m_FriendsGroupID = (short)CSteamID.m_SteamID;
            return SteamFriends.GetFriendsGroupMembersCount(id);
        }

        public void SetRCONSettings(string password, ushort port)
        {
            R.Settings.Instance.RCON.Password = password;
            R.Settings.Instance.RCON.Port = port;
        }

        public void BroadcastRCONServer(string message)
        {
            Rocket.Core.RCON.RCONServer.Broadcast(message);
        }

        public void SetWelcome(string text, Color color)
        {
            ChatManager.welcomeText = text;
            ChatManager.welcomeColor = color;
        }

        public int GetPlayerRank(UnturnedPlayer player)
        {
            if (PluginLoaded("FeexRanks"))
            {
                return db.GetRankBySteamID(player.CSteamID.m_SteamID.ToString());
            }
            else return 0;
        }
    }
}