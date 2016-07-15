using Colin.Gimbal;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MastermindArmor
{
    [Ultra.CustomPart(PartsCategories.Armor, MastermindOnly = true)]
    [Serializable]
    public class HeavyArmorPlateRightHook : Component
    {
        private static ComponentStaticData Data;

        public override ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return Data;
            }
        }

        public HeavyArmorPlateRightHook()
        {
        }

        public HeavyArmorPlateRightHook(Vector2 inPos) : base(inPos)
		{
        }

        public HeavyArmorPlateRightHook(Vector2 inPos, float inAng) : base(inPos, inAng)
		{
        }

        public override Component GetNewInstance()
        {
            return new HeavyArmorPlateRightHook();
        }

        public override void LoadStaticData()
        {
            Data = new ComponentStaticData(this);
            ComponentLevelStaticData.DisplayName = "Armor Plate Right Hook";
            ComponentLevelStaticData.Description = "Mount armor to the exterior of your vehicle for enhanced survivability, at cost of maneuverability. Armor reduces the effects of explosions and beam weapons.";
            ComponentLevelStaticData.UnitPrice = 1200;
            ComponentLevelStaticData.RequiredRank = PlayerRank.Admiral;
            ComponentLevelStaticData.OriginalPhysics = new Physics();
            ComponentLevelStaticData.MaxHitPoints = 2000f;
            ComponentLevelStaticData.OriginalPhysics.Mass = 120f;
            ComponentLevelStaticData.OriginalPhysics.Moment = 880000f;
            ComponentLevelStaticData.RadarCrossSection = 2.4f;
            ComponentLevelStaticData.ForwardAeroFactor = 24f;
            ComponentLevelStaticData.SideAeroFactor = 24f;
            ComponentLevelStaticData.AblationFactor = 0.5f;
            ComponentLevelStaticData.CanRicochet = true;
            ComponentLevelStaticData.MaxRicochetAngle = 0.5235988f;
            ComponentLevelStaticData.CollisionDamageThreshold = 120f;
            ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(23f, 64f);

            Ultra.TextureLoader.LoadTexture(Data, "HAP2R.png");

            base.LoadStaticData();
        }
    }
}
