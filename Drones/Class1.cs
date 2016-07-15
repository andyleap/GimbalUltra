using Colin.Gimbal;
using Colin.Gimbal.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drones
{
    [Ultra.CustomPart(PartsCategories.Machines)]
    [Serializable]
    public class StaticDrone : Component
    {
        private static ComponentStaticData Data;

        public override ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return Data;
            }
        }

        public StaticDrone()
        {
        }

        public StaticDrone(Vector2 inPos) : base(inPos)
        {
        }

        public override Component GetNewInstance()
        {
            return new StaticDrone();
        }

        public override void InstantiateMembers()
        {
            base.InstantiateMembers();
        }

        public override void LoadStaticData()
        {
            Data = new ComponentStaticData(this);
            this.ComponentLevelStaticData.DisplayName = "Drone Platform";
            this.ComponentLevelStaticData.Description = "The Drone platform is a small base platform that, upon activation, detaches from the vessel and floats.";
            this.ComponentLevelStaticData.UnitPrice = 420;
            this.ComponentLevelStaticData.RequiredRank = PlayerRank.Admiral;
            this.ComponentLevelStaticData.OriginalPhysics = new Physics();
            this.ComponentLevelStaticData.MaxHitPoints = 120f;
            this.ComponentLevelStaticData.OriginalPhysics.Mass = 32f;
            this.ComponentLevelStaticData.OriginalPhysics.Moment = 48000f;
            this.ComponentLevelStaticData.RadarCrossSection = 1.0f;
            this.ComponentLevelStaticData.ForwardAeroFactor = 20f;
            this.ComponentLevelStaticData.SideAeroFactor = 20f;
            this.ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(31.5f, 31.5f);
            this.ComponentLevelStaticData.AmmoType = Colin.Gimbal.AmmoType.None;
            this.ComponentLevelStaticData.MaxAmmo = 1;
            this.ComponentLevelStaticData.DescFunctionA = "Deploy";

            this.LoadBaseTexture("platform_six");

            base.LoadStaticData();
        }

        static bool mLaunched = false;

        AIGuidance ai;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(FunctionA && World.IsGameHost)
            {
                SafeDetach();
                World.GimbalGameInstance.Broadcast(new MissileLaunchMessage(new MissileLaunchData(OwnerVehicle.VehicleWorldID, ComponentID, false)));
                FunctionA = false;
                if (!mLaunched)
                {
                    Physics.AngularVelocity -= base.AbsoluteAngularVelocity * 0.85f;
                    Physics.Velocity += base.Physics.Forward * 180f;
                    mLaunched = true;
                    ai = new AIGuidance();
                    this.AddAttachmentToThis(ai, false, true);
                    List<Component> fcl = new List<Component>();
                    this.MakeFlatComponentList(fcl);
                    foreach(var c in fcl)
                    {
                        if(c is BaseGun)
                        {
                            var tc = new GimbalTargetController(c.StaticData.AmmoType, 0.04f, true, true, true);
                            tc.AddControlledComponent(c);
                            this.AddAttachmentToThis(tc, false, true);
                        }
                    }
                    
                }
            }

            if(mLaunched)
            {
                
            }
        }
    }
}
