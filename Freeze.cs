

using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace DevTools
{

    public class Freeze : UnturnedPlayerComponent
    {

        private readonly Vector3 fPos;
        private Vector3 lPos;

        private Freeze()
        {
            fPos = lPos = Player.Position;

            if (!Player.IsInVehicle) return;
            var veh = Player.CurrentVehicle;
            var passagers = veh.passengers;
            foreach (var steamplayer in Provider.clients)
            {
                if (steamplayer.playerID.characterName == Player.CharacterName)
                {
                    for (var i = 0; i < passagers.Length; i++)
                    {
                        if (passagers[i].player != steamplayer) continue;

                        var pos = Player.Position;
                        var seat = (byte)i;

                        Vector3 point;
                        byte angle;
                        veh.getExit(seat, out point, out angle);
                        VehicleManager.sendExitVehicle(veh, seat, (point + (point - pos)), angle, false);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (Player.Position == lPos) return;
            Player.Position.Set(fPos.x , fPos.y , fPos.z);
            lPos = Player.Position;
        }

    }

}