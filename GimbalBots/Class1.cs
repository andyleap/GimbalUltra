using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GimbalLogs
{
    public class Log
    {
        static StreamWriter log;
        static Log()
        {
            log = new StreamWriter("server.log", true);
        }
        
        [Ultra.AutoHook("Colin.Gimbal.GimbalGameInstance", "HandleAuthRequest")]
        public static void HandleAuthRequest(Colin.Gimbal.GimbalGameInstance ggi, Colin.Gimbal.AuthMessageRequestData req, Lidgren.Network.NetConnection sender)
        {
            Console.WriteLine("GimbalLogs");
            log.WriteLine("{0}: player {1} connected from {2} with SteamID {3} or serial {4}", new object[]
                    {
                        DateTime.Now,
                        req.PlayerName,
                        req.Connection.RemoteEndPoint.Address.ToString(),
                        req.SteamID,
                        req.SerialNumber
                    });
            log.Flush();
        }
    }
}
