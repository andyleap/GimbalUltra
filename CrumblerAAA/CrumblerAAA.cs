using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crumbler
{
    [Ultra.CustomPart(PartsCategories.Weapons, MastermindOnly = true)]
    [Serializable]
    public sealed class CrumblerAAA : Colin.Gimbal.BaseGun
    {
        private const float ROF = 6f;

        private static Colin.Gimbal.ComponentStaticData Data;

        [NonSerialized]
        public Colin.Gimbal.ImprovedMuzzleFlash mMuzzleFlash;

        public override Colin.Gimbal.ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return CrumblerAAA.Data;
            }
        }

        public CrumblerAAA()
        {
        }

        public CrumblerAAA(Vector2 inPos) : base(inPos)
        {
        }

        public CrumblerAAA(Vector2 inPos, float inAng) : base(inPos, inAng)
        {
        }

        public override Colin.Gimbal.Component GetNewInstance()
        {
            return new CrumblerAAA();
        }

        public override void InstantiateMembers()
        {
            base.InstantiateMembers();
            this.mMuzzlePoint = new Vector2(1f, -20f);
            this.mFireControl = new Colin.Gimbal.FireControl(6f);
        }

        public override void VirtualBuildDecals()
        {
            base.VirtualBuildDecals();
            this.mMuzzleFlash = new Colin.Gimbal.ImprovedMuzzleFlash(2f, -18f, 0.1f, 0.7f, true);
            base.AddGlowyDecalToThis(this.mMuzzleFlash);
            this.mSmoke = new Colin.Gimbal.VaporEmitter(0.2f, 1f, this.mMuzzlePoint.X, this.mMuzzlePoint.Y);
            base.AddDecalToThis(this.mSmoke);
        }

        public override void LoadStaticData()
        {
            CrumblerAAA.Data = new Colin.Gimbal.ComponentStaticData(this);
            this.ComponentLevelStaticData.DisplayName = "AB9 \"Crumbler\"";
            this.ComponentLevelStaticData.ShortDisplayName = "Crumbler";
            this.ComponentLevelStaticData.Description = "The AB9 autoloading cannon fires fast air-burst rounds quickly and accurately. The rounds carry a slightly bigger payload than in it's precedessor, AB4, thus dealing higher damage. This weapon is perfectly suited for a point defence turret, though it can also be applied as an primary weapon.";
            this.ComponentLevelStaticData.DescFunctionA = "Fire";
            this.ComponentLevelStaticData.UnitPrice = 290;
            this.ComponentLevelStaticData.EditorROF = 6f;
            this.ComponentLevelStaticData.EditorDamage = 20;
            this.ComponentLevelStaticData.OriginalPhysics = new Colin.Gimbal.Physics();
            this.ComponentLevelStaticData.AmmoType = Colin.Gimbal.AmmoType.AirBurst;
            this.ComponentLevelStaticData.MaxAmmo = 300;
            this.ComponentLevelStaticData.ProjectileSpread = 0.04f;
            this.ComponentLevelStaticData.MaxHitPoints = 25f;
            this.ComponentLevelStaticData.OriginalPhysics.Mass = 7f;
            this.ComponentLevelStaticData.OriginalPhysics.Moment = 20000f;
            this.ComponentLevelStaticData.RadarCrossSection = 0.14f;
            this.ComponentLevelStaticData.RCSBoostOnUsage = 0.8f;
            this.ComponentLevelStaticData.ForwardAeroFactor = 1.3f;
            this.ComponentLevelStaticData.SideAeroFactor = 1.2f;
            this.ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(25.5f, 23.5f);
            Ultra.TextureLoader.LoadTexture(Data, "CrumblerAAA.png");
            base.LoadStaticData();
        }

        public override void FireA(GameTime gameTime)
        {
            base.FireA(gameTime);
            Colin.Gimbal.GimbalGameManager.SoundManager.Play3D("shredderaaa", base.World.Camera.Sound3DListener, base.SoundEmitter);
            if (this.mMuzzleFlash != null)
            {
                this.mMuzzleFlash.Flash(gameTime);
            }
            float num = base.AbsoluteAngle;
            num += base.GetNetSyncedRandomishFloat() * base.StaticData.ProjectileSpread;
            Vector2 vector = Vector2.Transform(base.Physics.Forward, Matrix.CreateRotationZ(num));
            Vector2 inPos = base.AbsolutePosition + Vector2.Transform(this.mMuzzlePoint, Matrix.CreateRotationZ(num));
            Colin.Gimbal.BaseProjectile baseProjectile = new Colin.Gimbal.Airburst(base.World, inPos, base.RootParentAttachment.Physics.Velocity, vector, base.OwnerPlayer, this);
            baseProjectile.Physics.Angle = num;
            base.FireBullet(baseProjectile, gameTime);
            if (Colin.Gimbal.GimbalGameManager.ClientOptions.ShowParticles)
            {
                float duration = Colin.Gimbal.Utility.Random.Next_f(0.5f, 5f);
                float scaleFactor = 30f;
                float num2 = Colin.Gimbal.Utility.Random.Next_f(3f, 10f);
                Colin.Gimbal.Spark spark = new Colin.Gimbal.Spark(inPos, "smoke", duration, new Color(255, 255, 255, 100), new Color(255, 255, 255, 0), num2 * 0.2f, num2);
                spark.On = true;
                spark.WanderBoost *= 0.5f;
                spark.Physics.Velocity = base.RootParentAttachment.Physics.Velocity * 0.3f;
                spark.Physics.Velocity += vector * scaleFactor;
                base.World.IntroduceNewParticle(spark, true);
            }
            if (base.World.Camera.GetIsInViewFrustum(base.AbsolutePosition, 10f) && base.World.Camera.Z > -3f)
            {
                Colin.Gimbal.FallingSprite fallingSprite = new Colin.Gimbal.FallingSprite(base.AbsolutePosition, "spent_shell");
                fallingSprite.WanderOn = false;
                fallingSprite.Physics.Position += Vector2.Transform(Vector2.UnitX * 3f, Matrix.CreateRotationZ(base.AbsoluteAngle));
                fallingSprite.Scale = 0.9f;
                fallingSprite.Physics.Angle = base.AbsoluteAngle;
                fallingSprite.Physics.AngularVelocity = Colin.Gimbal.Utility.Random.Next_f(4f, 14f);
                fallingSprite.Physics.Velocity = base.AbsoluteVelocity;
                fallingSprite.Physics.Velocity += Vector2.Transform(-Vector2.UnitY * Colin.Gimbal.Utility.Random.Next_f(50f, 110f), Matrix.CreateRotationZ(base.AbsoluteAngle + 1.3f));
                fallingSprite.Physics.Velocity += Colin.Gimbal.Utility.GetRandomRadialVelocity(20f);
                base.World.IntroduceNewParticle(fallingSprite);
            }
            if (base.World.Camera.Target == base.RootParentAttachment)
            {
                base.World.Camera.Shake(0.5f);
            }
            baseProjectile.DoRecoil(this);
        }
    }
}