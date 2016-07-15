using Colin.Gimbal;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Wasp
{
    [Ultra.CustomPart(PartsCategories.Airframes)]
    [Serializable]
    public sealed class Wasp : Component
    {
        private static ComponentStaticData Data;

        public override ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return Data;
            }
        }

        public Wasp()
        {
        }

        public Wasp(Vector2 inPos) : base(inPos)
        {
        }

        public override Component GetNewInstance()
        {
            return new Wasp();
        }

        public override void VirtualBuildDecals()
        {
            base.VirtualBuildDecals();
            base.AddGlowyDecalToThis(new Strobe(base.ImagePos(109, 81), 0.6f, 1.4f, 0.7f, 0.1f));
            base.AddGlowyDecalToThis(new Strobe(base.ImagePos(146, 81), 0.6f, 1.4f, 0.7f, 0.1f));
            base.EasyAddBlinky(130, 52);
            MachWave decal = new MachWave(new Vector2(0f, 0f));
            base.AddDecalToThis(decal);
        }

        public override void LoadStaticData()
        {
            Data = new ComponentStaticData(this);
            this.ComponentLevelStaticData.DisplayName = "Wasp Platform";
            this.ComponentLevelStaticData.Description = "The Wasp showcases large mounting area, excellent moment of inertia for maneuverability, and durable armor.";
            this.ComponentLevelStaticData.UnitPrice = 420;
            this.ComponentLevelStaticData.RequiredRank = PlayerRank.Admiral;
            this.ComponentLevelStaticData.OriginalPhysics = new Physics();
            this.ComponentLevelStaticData.MaxHitPoints = 1030f;
            this.ComponentLevelStaticData.OriginalPhysics.Mass = 32f;
            this.ComponentLevelStaticData.OriginalPhysics.Moment = 20000f;
            this.ComponentLevelStaticData.RadarCrossSection = 1.8f;
            this.ComponentLevelStaticData.ForwardAeroFactor = 20f;
            this.ComponentLevelStaticData.SideAeroFactor = 40f;
            this.ComponentLevelStaticData.DroneSound = "ship_hum_k";
            this.ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(127f, 106f);
            this.ComponentLevelStaticData.CameraFocalOffset = new Vector2(127f, 106f) - this.ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass;

            Ultra.TextureLoader.LoadTexture(Data, "Wasp.png");

            base.LoadStaticData();
        }
    }

}
