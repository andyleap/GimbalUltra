using Colin.Gimbal;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HangarBoost
{
    public class Boost
    {
        public static List<Vehicle> Stock;

        public static Dictionary<string, Vehicle> Cache = new Dictionary<string, Vehicle>();

        [Ultra.AutoHook("Colin.Gimbal.VehicleCache", "Refresh", Skip = true)]
        public static void HangarRefresh(VehicleCache vc)
        {
            lock (vc.mInstanceLock)
            {
                List<Vehicle> list = new List<Vehicle>();
                if(Stock == null)
                {
                    Stock = BitmapVehicleSerializer.ReadAllStockVehicles();
                }
                list.AddRange(Stock);
                foreach (var ship in GetVehicles())
                {
                    if(Cache.ContainsKey(ship))
                    {
                        if (Cache[ship] != null)
                        {
                            list.Add(Cache[ship]);
                        }
                    } else
                    {
                        var vehicle = BitmapVehicleSerializer.LoadVehicleFromStorageHandleErrors(ship);
                        if(ship.StartsWith("Stolen") && vehicle != null)
                        {
                            vehicle.IsStolenDesign = true;
                        }
                        Cache.Add(ship, vehicle);
                        if (vehicle != null)
                        {
                            list.Add(vehicle);
                        }
                    }
                }
                foreach (Vehicle current in list)
                {
                    vc.mData[current.VehicleGuid] = current;
                    GimbalGameManager.StatsDatabase.MergeBestData(current);
                }
            }
        }

        public static List<String> GetVehicles()
        {
            Utility.DirExistsElseCreate("Hangar");
            List<string> list1 = new List<string>(Directory.GetFiles("Hangar", "*.png"));
            Utility.DirExistsElseCreate("Stolen");
            List<string> list2 = new List<string>(Directory.GetFiles("Stolen", "*.png"));
            list1.AddRange(list2);
            return list1;
        }

        [Ultra.AutoHook("Colin.Gimbal.BitmapVehicleSerializer", "SaveVehicle")]
        public static void SaveVehicle(Vehicle vehicle, string fullPath, bool forceNewGuid)
        {
            if(Cache.ContainsKey(fullPath))
            {
                Cache.Remove(fullPath);
            }
            if(HangarCache.ContainsKey(vehicle.VehicleGuid))
            {
                HangarCache.Remove(vehicle.VehicleGuid);
            }
        }
        [Ultra.AutoHook("Colin.Gimbal.BitmapVehicleSerializer", "RemoveVehicle")]
        public static void RemoveVehicle(Vehicle removalVehicle)
        {
            if(Cache.ContainsKey(removalVehicle.FilePath))
            {
                Cache.Remove(removalVehicle.FilePath);
            }
            if (HangarCache.ContainsKey(removalVehicle.VehicleGuid))
            {
                HangarCache.Remove(removalVehicle.VehicleGuid);
            }
        }

        static Dictionary<Guid, Vehicle> HangarCache = new Dictionary<Guid, Vehicle>();

        // Colin.Gimbal.HangarScreen
        public void RefreshVehicles(HangarScreen hs)
        {
            GimbalGameManager.VehicleCache.Refresh();
            List<Vehicle> allFlyableVehicles = GimbalGameManager.VehicleCache.GetAllFlyableVehicles();
            List<Vehicle> list = new List<Vehicle>();
            foreach (Vehicle current in allFlyableVehicles)
            {
                if (GimbalGameManager.BetaMode || !current.CoreComponent.IsBetaOnly())
                {
                    current.IsMastermindShip();
                    if ((!current.IsStolenDesign && !current.MastermindFlag) || (current.IsStolenDesign && hs.mShowStolen.State) || (current.MastermindFlag && hs.mShowMastermind.State))
                    {
                        if (HangarCache.ContainsKey(current.VehicleGuid))
                        {
                            list.Add(HangarCache[current.VehicleGuid]);
                        }
                        else
                        {
                            Vehicle vehicle = current.DuplicateMeFromBluePrints(true);
                            vehicle.OwnerPlayer = new Player(hs.mWorld.TeamA);
                            vehicle.IntroduceToWorld(hs.mWorld);
                            vehicle.CoreComponent.Physics.Position = Vector2.Zero;
                            vehicle.CoreComponent.Physics.Angle = 0f;
                            vehicle.SolidifyStats();
                            if (vehicle.CoreComponent != null)
                            {
                                vehicle.CoreComponent.Update(new GameTime());
                            }
                            vehicle.CoreComponent.CalculateOuterBoundsForEditor();
                            HangarCache.Add(current.VehicleGuid, vehicle);
                            list.Add(vehicle);
                        }
                    }
                }
            }
            hs.mVehicles = list;
            hs.AdjustTickSizeIfNeeded();
        }

    }
}
