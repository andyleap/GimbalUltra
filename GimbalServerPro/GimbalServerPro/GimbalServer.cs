using Colin.Gimbal;
using Colin.Gimbal.Parts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace GimbalServerPro
{
    internal class GimbalServer
    {
        private static GimbalGameManager gameManager;

        private static GimbalGameInstance gameInstance;

        private static Thread gameThread;

        private static object InputLock = new object();

        private static ManualResetEvent gameIsLoadedEvent;

        private static ServerProOptions serverProOptions;

        public static World LastWorld;

        public static int LastRound;

        public static bool HasUltra = false;

        public static Random rng = new Random();

        private static Dictionary<Guid, string> DesignValid = new Dictionary<Guid, string>();

        private static void Main(string[] args)
        {
            string value = "Real Time Commands: quit info ban kick scores changelevel players stats say drain undrain";
            if (args.Length > 2)
            {
                Console.WriteLine("Error: Too many arguments. You may use an optional ServerOptions filename as the first and only argument.");
                Console.WriteLine("Shutting down...");
                return;
            }
            GimbalServer.serverProOptions = new ServerProOptions();
            if (args.Length == 1)
            {
                ServerOptions.FileName = args[0];
                if (!Utility.FileExists(ServerOptions.FileName))
                {
                    Console.WriteLine("Could not find specified ServerOptions file.");
                    Console.WriteLine("Qutting...");
                    return;
                }
            }
            if (args.Length == 2)
            {
                ServerOptions.FileName = args[0];
                if (!Utility.FileExists(ServerOptions.FileName))
                {
                    Console.WriteLine("Could not find specified ServerOptions file.");
                    Console.WriteLine("Qutting...");
                    return;
                }
                GimbalServer.serverProOptions = ServerProOptions.Load(args[1]);
            }
            else
            {
                GimbalServer.serverProOptions = ServerProOptions.Load("ServerProOptions.xml");
            }
            Console.WriteLine("---- Gimbal Standalone Server ----");
            Console.WriteLine("Tip: You may use an alternate ServerOptions file by specifying the ServerOptions filename as the first command line argument.");
            Console.WriteLine(value);
            Console.WriteLine("Using \"" + ServerOptions.FileName + "\" for server options.");
            Console.WriteLine("Launching game thread...");
            GimbalServer.gameThread = new Thread(new ThreadStart(GimbalServer.StartRunGameLoop));
            GimbalServer.gameThread.IsBackground = false;
            GimbalServer.gameThread.Start();
            GimbalServer.gameIsLoadedEvent = new ManualResetEvent(false);
            GimbalServer.gameIsLoadedEvent.WaitOne();
            Console.WriteLine("Starting command line...");
            while (GimbalServer.gameInstance.IsRunning)
            {
                Console.Write(">");
                string text = Console.ReadLine();
                string[] array = text.Split(new char[]
                {
                    ' '
                });
                try
                {
                    lock (GimbalServer.InputLock)
                    {
                        if (string.Compare(array[0], "quit", true) == 0 || string.Compare(array[0], "stop", true) == 0 || string.Compare(array[0], "exit", true) == 0 || string.Compare(array[0], "shutdown", true) == 0)
                        {
                            GimbalServer.gameInstance.CloseGameInstance();
                        }
                        else if (string.Compare(array[0], "info", true) == 0)
                        {
                            GimbalServer.gameInstance.PrintServerOptions();
                        }
                        else if (string.Compare(array[0], "ban", true) == 0)
                        {
                            if (array.Length > 1)
                            {
                                GimbalServer.gameInstance.Ban(string.Join(" ", array, 1, array.Length - 1));
                            }
                        }
                        else if (string.Compare(array[0], "kick", true) == 0)
                        {
                            if (array.Length > 1)
                            {
                                GimbalServer.gameInstance.Kick(string.Join(" ", array, 1, array.Length - 1));
                            }
                        }
                        else if (string.Compare(array[0], "scores", true) == 0)
                        {
                            GimbalServer.gameInstance.PrintScores();
                        }
                        else if (string.Compare(array[0], "changelevel", true) == 0 || string.Compare(array[0], "cl", true) == 0)
                        {
                            GimbalServer.gameInstance.ForceChangeLevelFromExternal();
                        }
                        else if (string.Compare(array[0], "players", true) == 0 || string.Compare(array[0], "list", true) == 0 || string.Compare(array[0], "listplayers", true) == 0)
                        {
                            GimbalServer.gameInstance.PrintPlayers();
                        }
                        else if (string.Compare(array[0], "stats", true) == 0 || string.Compare(array[0], "fps", true) == 0)
                        {
                            GimbalServer.gameInstance.PrintStats();
                        }
                        else if (string.Compare(array[0], "drain", true) == 0)
                        {
                            GimbalServer.gameInstance.DrainServer();
                        }
                        else if (string.Compare(array[0], "undrain", true) == 0)
                        {
                            GimbalServer.gameInstance.UnDrainServer();
                        }
                        else
                        {
                            if (string.Compare(array[0], "say", true) == 0 || string.Compare(array[0], "y", true) == 0 || string.Compare(array[0], "admin", true) == 0 || string.Compare(array[0], "chat", true) == 0 || string.Compare(array[0], "announce", true) == 0)
                            {
                                if (array.Length <= 1)
                                {
                                    continue;
                                }
                                try
                                {
                                    GimbalServer.gameInstance.Announce(text.Substring(array[0].Length + 1));
                                    continue;
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            if (string.Compare(array[0].Substring(0, 1), "y") == 0)
                            {
                                if (array.Length < 1)
                                {
                                    continue;
                                }
                                try
                                {
                                    GimbalServer.gameInstance.Announce(text.Substring(1));
                                    continue;
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            Console.WriteLine("Unknown Command.");
                            Console.WriteLine(value);
                        }
                    }
                }
                catch (Exception e)
                {
                    GimbalServer.HandleError(e);
                    Console.WriteLine("Gimbal Standalone Server is now SHUT DOWN.");
                    return;
                }
            }
            Console.WriteLine("Gimbal Standalone Server is now SHUT DOWN.");
        }

        private static void HandleError(Exception e)
        {
            GimbalGameManager.Console.Push(e.ToString());
            GimbalGameManager.Console.Save();
            GimbalLauncher.HandleError(e, true);
        }

        private static void StartRunGameLoop()
        {
            Console.WriteLine("Game thread is live.");
            Console.WriteLine("Initializing game...");
            Type type = Type.GetType("Colin.Gimbal.Ultra, Gimbal");
            if (type != null)
            {
                GimbalServer.HasUltra = true;
                Console.WriteLine("Ultra detected, enabling advanced features!");
                FieldInfo field = type.GetField("AwardMoneyKill");
                field.SetValue(null, new Action<GameLogicHandler, Player, Player>(delegate (GameLogicHandler GMH, Player killer, Player victim)
                {
                    Console.WriteLine("Award Money Kill!");
                    if (killer.PrimaryVehicle != null)
                    {
                        float num3 = 1f;
                        if (victim.IsBot && !victim.PrimaryVehicle.IsMothership)
                        {
                            num3 = 0.2f;
                        }
                        int num4 = GameLogicHandler.CalculateWinAmount(killer.VehicleProgress, victim.VehicleProgress, num3);
                        MethodInfo method = killer.GetType().GetMethod("WinMoney", BindingFlags.Instance | BindingFlags.NonPublic);
                        method.Invoke(killer, new object[]
                        {
                            num4
                        });
                        if (!victim.IsBot)
                        {
                            MethodInfo method2 = killer.GetType().GetMethod("WinVehicle", BindingFlags.Instance | BindingFlags.NonPublic);
                            method2.Invoke(killer, new object[]
                            {
                                victim.PrimaryVehicle.VehicleGuid
                            });
                        }
                    }
                }));
                FieldInfo field2 = type.GetField("MakeRandomFighter");
                if (GimbalServer.serverProOptions.CustomBots)
                {
                    field2.SetValue(null, new Func<Team, Player>((Team team) => GimbalServer.MakeBotPlayer(team)));
                }
                FieldInfo field3 = type.GetField("HandleAuthRequest");
                StreamWriter log = new StreamWriter("server.log", true);
                field3.SetValue(null, new Action<GimbalGameInstance, AuthMessageRequestData>(delegate (GimbalGameInstance ggi, AuthMessageRequestData req)
                {
                    log.WriteLine("{0}: player {1} connected from {2} with SteamID {3} or serial {4}", new object[]
                    {
                        DateTime.Now,
                        req.PlayerName,
                        req.Connection.RemoteEndPoint.Address.ToString(),
                        req.SteamID,
                        req.SerialNumber
                    });
                    log.Flush();
                }));
            }
            try
            {
                GimbalGameManager.ClientOptions = ClientOptions.Load();
                GimbalGameManager.ServerOptions = ServerOptions.Load();
                GimbalGameManager.ClientOptions.SoundOn = false;
                GimbalGameManager.ClientOptions.FlamesOn = false;
                GimbalGameManager.ClientOptions.MotionBlur = false;
                GimbalGameManager.ClientOptions.Multithreading = false;
                GimbalGameManager.ClientOptions.BloomOn = false;
                GimbalGameManager.ClientOptions.BackgroundDustOn = false;
                GimbalGameManager.ClientOptions.ShowParticles = false;
                GimbalGameManager.ClientOptions.ShowTrails = false;
                string newValue = string.Join("\n", (from kvp in GimbalServer.serverProOptions.PartLimits
                                                     select kvp.Key + " - " + kvp.Value.ToString()).ToArray<string>());
                GimbalGameManager.ServerOptions.MOTD = GimbalGameManager.ServerOptions.MOTD.Replace("{BUDGET}", GimbalServer.serverProOptions.MaxBudget.ToString());
                GimbalGameManager.ServerOptions.MOTD = GimbalGameManager.ServerOptions.MOTD.Replace("{PARTLIMITS}", newValue);
                if (GimbalGameManager.ServerOptions.ProcessorAffinity != 0)
                {
                    try
                    {
                        Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(GimbalGameManager.ServerOptions.ProcessorAffinity);
                        Console.WriteLine("ProcessorAffinity = " + Process.GetCurrentProcess().ProcessorAffinity.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to set ProcessorAffinity to " + GimbalGameManager.ServerOptions.ProcessorAffinity);
                        Console.WriteLine(ex.ToString());
                    }
                }
                GimbalGameManager.Headless = true;
                GimbalServer.gameManager = new GimbalGameManager();
                GimbalGameManager.VehicleCache.Refresh();
                GimbalServer.gameInstance = GimbalGameInstance.LaunchGameServer();
                GimbalServer.gameInstance.ScreenLoadContent();
                GimbalGameManager.ScreenManager.PushScreen(GimbalServer.gameInstance);
            }
            catch (Exception e)
            {
                GimbalServer.HandleError(e);
                if (GimbalServer.gameInstance != null)
                {
                    GimbalServer.gameInstance.CloseGameInstance();
                }
                Console.WriteLine("Gimbal Standalone Server is now SHUT DOWN.");
                return;
            }
            Console.WriteLine("Load complete.");
            if (GimbalServer.gameInstance.IsRunning)
            {
                Console.WriteLine("Game loop is running.");
            }
            TimeSpan t = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan t2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan timeSpan = new TimeSpan(0L);
            TimeSpan timeSpan2 = new TimeSpan(0L);
            new botfix();
            try
            {
                GimbalServer.gameIsLoadedEvent.Set();
                while (GimbalServer.gameInstance.IsRunning)
                {
                    lock (GimbalServer.InputLock)
                    {
                        if (GimbalGameManager.GimbalInstance.World != GimbalServer.LastWorld || GimbalGameManager.GimbalInstance.GameLogicHandler.RoundNumber != GimbalServer.LastRound)
                        {
                            GimbalServer.LastWorld = GimbalGameManager.GimbalInstance.World;
                            GimbalServer.LastRound = GimbalGameManager.GimbalInstance.GameLogicHandler.RoundNumber;
                            string newValue2 = string.Join("\n", (from kvp in GimbalServer.serverProOptions.PartLimits
                                                                  select kvp.Key + " - " + kvp.Value.ToString()).ToArray<string>());
                            GimbalGameManager.GimbalInstance.ServerOptions.MOTD = GimbalGameManager.GimbalInstance.ServerOptions.MOTD.Replace("{BUDGET}", GimbalServer.serverProOptions.MaxBudget.ToString());
                            GimbalGameManager.GimbalInstance.ServerOptions.MOTD = GimbalGameManager.GimbalInstance.ServerOptions.MOTD.Replace("{PARTLIMITS}", newValue2);
                        }
                        foreach (Player current in GimbalGameManager.GimbalInstance.World.GetAllPlayers())
                        {
                            if (!current.IsOnSpectatorTeam && !current.WaitingForRespawn && !current.IsEliminated && current.PrimaryVehicle.Alive && !current.IsBot && !current.PrimaryVehicle.IsMothership)
                            {
                                string text = GimbalServer.CheckVehicle(current.PrimaryVehicle);
                                if (text != "")
                                {
                                    current.PrimaryVehicle.CoreComponent.TryDoDamage(3.40282347E+38f, current, current.PrimaryVehicle.CoreComponent.AbsolutePosition, null, false, false);
                                    NotificationData notificationData = new NotificationData();
                                    notificationData.Message = text;
                                    notificationData.Duration = 5f;
                                    notificationData.AddBehavior = 0;
                                    GimbalGameManager.GimbalInstance.NetServer.SendMessage(new NotificationMessage(notificationData), current.Soul.Connection);
                                }
                            }
                        }
                        t2 = new TimeSpan(DateTime.Now.Ticks);
                        timeSpan2 = t2 - t - timeSpan;
                        double num = 0.0;
                        if (GimbalGameManager.ClientOptions.VSync)
                        {
                            num = 0.016666666666666666;
                        }
                        if (timeSpan2.TotalSeconds > num)
                        {
                            timeSpan = t2 - t;
                            GameTime gameTime = new GameTime(timeSpan, timeSpan2, timeSpan, timeSpan2);
                            Utility.TempGameTime = gameTime;
                            GimbalServer.gameInstance.ScreenUpdate(gameTime);
                            GimbalGameManager.Console.Update();
                        }
                        else if (GimbalGameManager.ClientOptions.VSync)
                        {
                            double num2 = 0.016666666666666666 - timeSpan2.TotalSeconds;
                            Thread.Sleep(Utility.Clamp((int)(num2 * 1000.0), 0, 1000));
                        }
                        else
                        {
                            Thread.Sleep(0);
                        }
                    }
                    Thread.Sleep(0);
                }
            }
            catch (Exception e2)
            {
                GimbalGameManager.Console.Push("Error in GimbalServer.StartRunGameLoop()");
                GimbalServer.HandleError(e2);
                GimbalServer.gameInstance.CloseGameInstance();
            }
        }

        public static void AddBots()
        {
            int num = 3;
            int num2 = 0;
            Team teamA = GimbalGameManager.GimbalInstance.World.TeamA;
            Team team = GimbalGameManager.GimbalInstance.World.TeamB;
            if (GimbalGameManager.ServerOptions.GameMode == GameMode.Deathmatch || GimbalGameManager.ServerOptions.GameMode == GameMode.EliminationDM || GimbalGameManager.ServerOptions.GameMode == GameMode.Race)
            {
                team = GimbalGameManager.GimbalInstance.World.TeamA;
            }
            for (int i = 0; i < num; i++)
            {
                Player player = GimbalServer.MakeBotPlayer(teamA);
                GimbalGameManager.GimbalInstance.World.IntroduceNewPlayer(player, PlayerType.Contestant, false);
                GimbalGameManager.GimbalInstance.World.VanishVehicle(player.PrimaryVehicle);
                player.PlayerID = GimbalGameManager.GimbalInstance.World.GetNewPlayerID();
                GimbalGameManager.GimbalInstance.Players.Add(player);
                GimbalServer.CheckAddVehicleToCache(player.PrimaryVehicle);
                GimbalGameManager.Console.Push("Added Bot Player " + player.ToString() + " to world.");
            }
            for (int j = 0; j < num2; j++)
            {
                Player player2 = BotConfigs.MakeRandomFighter(team);
                GimbalGameManager.GimbalInstance.World.IntroduceNewPlayer(player2, PlayerType.Contestant, true);
                GimbalGameManager.GimbalInstance.World.VanishVehicle(player2.PrimaryVehicle);
                player2.PlayerID = GimbalGameManager.GimbalInstance.World.GetNewPlayerID();
                GimbalGameManager.GimbalInstance.Players.Add(player2);
                GimbalServer.CheckAddVehicleToCache(player2.PrimaryVehicle);
                GimbalGameManager.Console.Push("Added Bot Player " + player2.ToString() + " to world.");
            }
        }

        public static Player MakeBotPlayer(Team team)
        {
            string[] files;
            if (Directory.Exists(GimbalServer.serverProOptions.CustomBotsFolder + Path.DirectorySeparatorChar + team.DisplayName))
            {
                files = Directory.GetFiles(GimbalServer.serverProOptions.CustomBotsFolder + Path.DirectorySeparatorChar + team.DisplayName, "*.png");
            }
            else
            {
                files = Directory.GetFiles(GimbalServer.serverProOptions.CustomBotsFolder, "*.png");
            }
            if (files.Length == 0)
            {
                files = Directory.GetFiles(GimbalServer.serverProOptions.CustomBotsFolder, "*.png");
            }
            string text = files[GimbalServer.rng.Next(files.Length)];
            Player player = new Player(team);
            player.IsBot = true;
            player.IsMastermind = true;
            player.Money = GimbalServer.serverProOptions.MaxBudget;
            Vehicle vehicle = BitmapVehicleSerializer.LoadVehicleFromStorageHandleErrors(text);
            FieldInfo field = player.GetType().GetField("mPrimaryVehicle", BindingFlags.Instance | BindingFlags.NonPublic);
            vehicle.IsStockDesign = true;
            vehicle = GimbalServer.DuplicateMeFromBluePrints(vehicle, true);
            AIGuidance aIGuidance = new AIGuidance();
            MethodInfo method = aIGuidance.GetType().GetMethod("AddSteering", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method2 = aIGuidance.GetType().GetMethod("AddStabilizer", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method3 = aIGuidance.GetType().GetMethod("AddForwardThruster", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (Component c in vehicle.FlatComponentList)
            {
                if (c is SpinController)
                {
                    method.Invoke(aIGuidance, new object[]
                    {
                        c
                    });
                    Console.WriteLine("SpinController Found");
                }
                if (c is MouseStabilizer)
                {
                    method2.Invoke(aIGuidance, new object[]
                    {
                        c
                    });
                    Console.WriteLine("MouseStabilizer Found");
                }
                if (c is BaseThruster)
                {
                    BaseThruster baseThruster = (BaseThruster)c;
                    if (baseThruster.InputMappingA.Bindings.Count > 0 && baseThruster.InputMappingA.Bindings[0].BindingType == BindingType.Key && baseThruster.InputMappingA.Bindings[0].KeyBind == Keys.W)
                    {
                        method3.Invoke(aIGuidance, new object[]
                        {
                            c
                        });
                    }
                }
                if (c is SequentialFire)
                {
                    SequentialFire sequentialFire = (SequentialFire)c;
                    if (sequentialFire.Guns.Count > 0)
                    {
                        AmmoType ammoType = (sequentialFire.Guns[0] as BaseGun).StaticData.AmmoType;
                        GimbalTargetController gimbalTargetController = new GimbalTargetController(ammoType, 0.04f, true, true, true);
                        MethodInfo method4 = gimbalTargetController.GetType().GetMethod("AddControlledComponent", BindingFlags.Instance | BindingFlags.NonPublic);
                        method4.Invoke(gimbalTargetController, new object[]
                        {
                            c
                        });
                        sequentialFire.Guns[0].AddAttachmentToThis(gimbalTargetController);
                    }
                }
                if (c is InputSlaver)
                {
                    InputSlaver inputSlaver = (InputSlaver)c;
                    if (inputSlaver.Slaves.Count > 0 && inputSlaver.Slaves[0] is BaseGun)
                    {
                        AmmoType ammoType2 = (inputSlaver.Slaves[0] as BaseGun).StaticData.AmmoType;
                        GimbalTargetController gimbalTargetController2 = new GimbalTargetController(ammoType2, 0.04f, true, true, true);
                        MethodInfo method5 = gimbalTargetController2.GetType().GetMethod("AddControlledComponent", BindingFlags.Instance | BindingFlags.NonPublic);
                        method5.Invoke(gimbalTargetController2, new object[]
                        {
                            c
                        });
                        inputSlaver.Slaves[0].AddAttachmentToThis(gimbalTargetController2);
                    }
                }
                if (c is BaseGun)
                {
                    if (!vehicle.FlatComponentList.OfType<BaseSoftwareComponent>().Any((BaseSoftwareComponent bc) => bc.GetControlledComponents().Contains(c)))
                    {
                        if (c is FlareLauncher)
                        {
                            PointDefenseController pointDefenseController = new PointDefenseController();
                            MethodInfo method6 = pointDefenseController.GetType().GetMethod("AddControlledComponent", BindingFlags.Instance | BindingFlags.NonPublic);
                            method6.Invoke(pointDefenseController, new object[]
                            {
                                c
                            });
                            c.AddAttachmentToThis(pointDefenseController);
                        }
                        else
                        {
                            GimbalTargetController gimbalTargetController3 = new GimbalTargetController(c.StaticData.AmmoType, 0.04f, true, true, true);
                            MethodInfo method7 = gimbalTargetController3.GetType().GetMethod("AddControlledComponent", BindingFlags.Instance | BindingFlags.NonPublic);
                            method7.Invoke(gimbalTargetController3, new object[]
                            {
                                c
                            });
                            c.AddAttachmentToThis(gimbalTargetController3);
                        }
                    }
                }
                c.InputMappingA.UnBind();
                c.InputMappingB.UnBind();
            }
            vehicle = GimbalServer.DuplicateMeFromBluePrints(vehicle, true);
            vehicle.OwnerPlayer = player;
            field.SetValue(player, vehicle);
            vehicle.CoreComponent.AddAttachmentToThis(aIGuidance);
            return player;
        }

        public static string CheckVehicle(Vehicle v)
        {
            if (GimbalServer.DesignValid.ContainsKey(v.VehicleGuid))
            {
                return GimbalServer.DesignValid[v.VehicleGuid];
            }
            if (v.Price > GimbalServer.serverProOptions.MaxBudget)
            {
                GimbalServer.DesignValid.Add(v.VehicleGuid, "Your vehicle is too expensive, please choose a vehicle less then $" + GimbalServer.serverProOptions.MaxBudget.ToString() + " trillion");
                return GimbalServer.DesignValid[v.VehicleGuid];
            }
            foreach (KeyValuePair<string, int> ItemLimit in GimbalServer.serverProOptions.PartLimits)
            {
                int num = v.FlatComponentList.Count(delegate (Component c)
                {
                    string arg_19_0 = c.StaticData.DisplayName;
                    KeyValuePair<string, int> itemLimit4 = ItemLimit;
                    return arg_19_0 == itemLimit4.Key;
                });
                int arg_CC_0 = num;
                KeyValuePair<string, int> itemLimit = ItemLimit;
                if (arg_CC_0 > itemLimit.Value)
                {
                    Dictionary<Guid, string> arg_10F_0 = GimbalServer.DesignValid;
                    Guid arg_10F_1 = v.VehicleGuid;
                    string arg_10A_0 = "Your vehicle has too many ";
                    KeyValuePair<string, int> itemLimit2 = ItemLimit;
                    string arg_10A_1 = itemLimit2.Key;
                    string arg_10A_2 = "s, Max allowed is ";
                    KeyValuePair<string, int> itemLimit3 = ItemLimit;
                    arg_10F_0.Add(arg_10F_1, arg_10A_0 + arg_10A_1 + arg_10A_2 + itemLimit3.Value.ToString());
                    return GimbalServer.DesignValid[v.VehicleGuid];
                }
            }
            GimbalServer.DesignValid.Add(v.VehicleGuid, "");
            return "";
        }

        public static void CheckAddVehicleToCache(Vehicle vehicle)
        {
            if (vehicle != null && !GimbalGameManager.VehicleCache.Exists(vehicle.VehicleGuid))
            {
                vehicle.IsCachedOnly = true;
                vehicle.IsStolenDesign = false;
                vehicle.IsStockDesign = vehicle.IsStockDesign;
                GimbalGameManager.VehicleCache.AddVehicle(GimbalServer.DuplicateMeFromBluePrints(vehicle, false));
                GimbalGameManager.Console.Push("Vehicle added to cache - " + vehicle.VehicleGuid);
            }
        }

        public static Vehicle DuplicateMeFromBluePrints(Vehicle me, bool skipVirtualBuilds)
        {
            Vehicle vehicle = new Vehicle(me.VehicleName, null, 0);
            vehicle.VehicleAuthorID = me.VehicleAuthorID;
            vehicle.VehicleAuthorName = me.VehicleAuthorName;
            vehicle.VehicleDescription = me.VehicleDescription;
            vehicle.VehicleGuid = me.VehicleGuid;
            vehicle.DateCreated = me.DateCreated;
            vehicle.DateModified = me.DateModified;
            vehicle.HUDOuterBounds = me.HUDOuterBounds;
            vehicle.FilePath = me.FilePath;
            vehicle.IsStockDesign = me.IsStockDesign;
            vehicle.IsStolenDesign = me.IsStolenDesign;
            vehicle.IsCachedOnly = me.IsCachedOnly;
            vehicle.MastermindFlag = me.MastermindFlag;
            vehicle.MaxRecordedSpeed = me.MaxRecordedSpeed;
            if (me.CoreComponent != null)
            {
                me.CoreComponent.DuplicateMeFromBluePrints(vehicle, skipVirtualBuilds);
            }
            vehicle.SolidifyStats();
            if (me.ThumbnailTexture != null)
            {
                vehicle.SetThumbnailTexture(me.ThumbnailTexture);
            }
            return vehicle;
        }
    }
}
