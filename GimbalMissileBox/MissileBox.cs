using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Colin.Gimbal.Parts;

namespace GimbalMissileBox
{
    [Ultra.CustomPart(PartsCategories.Magazines)]
    [Serializable]
    public class MissileBox : Component
    {
        public int ReloadCount = 10;

        public MissileBox()
        {
        }

        static ComponentStaticData Data;

        static Dictionary<LRMissileLauncher, TimeSpan> ReloadTime = new Dictionary<LRMissileLauncher, TimeSpan>();

        public override ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return Data;
            }
        }

        public override Component GetNewInstance()
        {
            return new MissileBox();
        }

        public override void LoadStaticData()
        {
            Data = new ComponentStaticData(this);

            ComponentLevelStaticData.DisplayName = "Missile Magazine";
            ComponentLevelStaticData.Description = "The Missile Magazine allows a vehicle to carry extra missile ammo.\n\n10 LR Missile Reloads";
            ComponentLevelStaticData.UnitPrice = 210;
            ComponentLevelStaticData.RequiredRank = PlayerRank.Commodore;
            ComponentLevelStaticData.OriginalPhysics = new Physics();
            ComponentLevelStaticData.MaxHitPoints = 11f;
            ComponentLevelStaticData.OriginalPhysics.Mass = 14f;
            ComponentLevelStaticData.OriginalPhysics.Moment = 40000f;
            ComponentLevelStaticData.RadarCrossSection = 0.08f;
            ComponentLevelStaticData.ForwardAeroFactor = 2f;
            ComponentLevelStaticData.SideAeroFactor = 2f;
            ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(32f, 32f);

            Ultra.TextureLoader.LoadTexture(Data, "MissileMag.png");

            base.LoadStaticData();
        }

        public override void ReloadAndRefuelRecursive(Component prototype)
        {
            ReloadCount = 10;
            base.ReloadAndRefuelRecursive(prototype);
        }

        TimeSpan reportTime = new TimeSpan();

        public override void Update(GameTime gameTime)
        {
            if (World != null)
            {
                Reload(this.RootParentAttachment, gameTime.TotalGameTime);
            }
            
            reportTime += gameTime.ElapsedGameTime;
            if(reportTime.TotalSeconds > 10)
            {
                reportTime -= new TimeSpan(0, 0, 10);
                GimbalGameManager.Console.Push(OwnerVehicle.AmmoManager.GetCount(AmmoType.LRMissile).ToString() + " Missiles");
            }
            base.Update(gameTime);
        }

        public void Reload(Component c, TimeSpan gameTime)
        {
            foreach(var subc in c.ChildAttachments)
            {
                Reload(subc, gameTime);
            }
            if(c is Colin.Gimbal.Parts.LRMissileLauncher && ReloadCount > 0)
            {
                var lr = c as Colin.Gimbal.Parts.LRMissileLauncher;
                var rails = lr.mRails;
                if (rails != null && rails.Count == 0)
                {
                    if(!ReloadTime.ContainsKey(lr))
                    {
                        ReloadTime.Add(lr, gameTime.Add(new TimeSpan(0, 0, 10)));
                    }
                    else
                    {
                        if (ReloadTime[lr] < gameTime)
                        {
                            ReloadTime.Remove(lr);
                            GimbalGameManager.Console.Push("Reloading LR Rack");
                            Console.WriteLine("Reloading LR Rack");
                            int num = 2;
                            float num2 = 6f;
                            float num3 = 4f;
                            float num4;
                            bool arg_4C_0 = base.IsMirror;
                            int num5 = 0;
                            int num6 = 0;
                            if (4 > num)
                            {
                                num4 = -((float)num * num2 / 2f);
                            }
                            else
                            {
                                num4 = -(4f * num2 / 2f);
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                float num7 = 0f;
                                if (num5 % 2 == 1)
                                {
                                    num7 = num2 / 2f;
                                }
                                else
                                {
                                    num7 += num2;
                                }
                                LRMissile lRMissile = new LRMissile();
                                lRMissile.Physics.Position = new Vector2(num4 + (float)num6 * num2 + num7, (float)((num5 + 1) % 2) * num3);
                                lr.AddAttachmentToThis(lRMissile, false, true);
                                lRMissile.OwnerVehicle = lr.OwnerVehicle;
                                lRMissile.IntroduceToWorld(World);
                                rails.Add(lRMissile);
                                num6++;
                                if ((num5 % 2 == 1 && num6 >= num) || (num5 % 2 == 0 && num6 >= num - 1))
                                {
                                    num6 = 0;
                                    num5++;
                                }
                            }
                            ReloadCount--;
                        }
                    }
                    //lr.ReloadAndRefuelRecursive(lr.OwnerVehicle.RepairPrototype.CoreComponent);
                    /*
                    int num = 2;
                    float num2 = 6f;
                    float num3 = 4f;
                    float num4;
                    bool arg_4C_0 = base.IsMirror;
                    int num5 = 0;
                    int num6 = 0;
                    if (4 > num)
                    {
                        num4 = -((float)num * num2 / 2f);
                    }
                    else
                    {
                        num4 = -(4f * num2 / 2f);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        float num7 = 0f;
                        if (num5 % 2 == 1)
                        {
                            num7 = num2 / 2f;
                        }
                        else
                        {
                            num7 += num2;
                        }
                        LRMissile lRMissile = new LRMissile();
                        lRMissile.Physics.Position = new Vector2(num4 + (float)num6 * num2 + num7, (float)((num5 + 1) % 2) * num3);
                        lr.AddAttachmentToThis(lRMissile, false, true);
                        lRMissile.OwnerVehicle = lr.OwnerVehicle;
                        lRMissile.GetType().GetMethod("IntroduceToWorld", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new Type[] { typeof(World) }, null).Invoke(lRMissile, new object[] { World });
                        rails.Add(lRMissile);
                        num6++;
                        if ((num5 % 2 == 1 && num6 >= num) || (num5 % 2 == 0 && num6 >= num - 1))
                        {
                            num6 = 0;
                            num5++;
                        }
                    }*/
                    

                }
            }
        }
    }
}
