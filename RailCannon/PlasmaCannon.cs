using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlasmaCannon
{
    [Ultra.CustomPart(PartsCategories.Weapons, MastermindOnly = true)]
    [Serializable]
    public class PlasmaCannon : BaseGun
    {
        private const float ROF = 0.9f;

        private static ComponentStaticData Data;

        private static SoundEffect Firing;
        [NonSerialized]
        public bool SoundFired = false;
        private SoundEffectInstance FiringInstance;

        [NonSerialized]
        private ImprovedMuzzleFlash mMuzzleFlash;

        public override ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return Data;
            }
        }

        public PlasmaCannon()
        {
        }

        public PlasmaCannon(Vector2 inPos) : base(inPos)
		{
        }

        public PlasmaCannon(Vector2 inPos, float inAng) : base(inPos, inAng)
		{
        }

        public override Component GetNewInstance()
        {
            return new PlasmaCannon();
        }

        public override void InstantiateMembers()
        {
            base.InstantiateMembers();
            mMuzzlePoint = new Vector2(-0f, -110f);
            mFireControl = new ChargeUpFireControl(0.1f, 2);
        }

        public override void VirtualBuildDecals()
        {
            base.VirtualBuildDecals();
            mMuzzleFlash = new ImprovedMuzzleFlash(0f, -110f, 0.2f, 2f, true);
            if (base.OwnerPlayer != null)
            {
                mMuzzleFlash.SetColor(Utility.MultiplyColorBrightness(base.OwnerPlayer.GetColor(TeamColorType.TrailEndColor), 0.6f));
            }
            base.AddGlowyDecalToThis(mMuzzleFlash);
        }

        public override void LoadStaticData()
        {
            Data = new ComponentStaticData(this);
            ComponentLevelStaticData.DisplayName = "Plasma Cannon";
            ComponentLevelStaticData.Description = "Eat hot plasma death!";
            ComponentLevelStaticData.DescFunctionA = "Fire";
            ComponentLevelStaticData.UnitPrice = 2200;
            ComponentLevelStaticData.RequiredRank = PlayerRank.Admiral;
            ComponentLevelStaticData.EditorROF = 0.1f;
            ComponentLevelStaticData.EditorDamage = 2500;
            ComponentLevelStaticData.OriginalPhysics = new Physics();
            ComponentLevelStaticData.AmmoType = AmmoType.EnergyCharge;
            ComponentLevelStaticData.MaxAmmo = 22;
            ComponentLevelStaticData.ProjectileSpread = 0.001f;
            ComponentLevelStaticData.MaxHitPoints = 220f;
            ComponentLevelStaticData.OriginalPhysics.Mass = 180f;
            ComponentLevelStaticData.OriginalPhysics.Moment = 1000000f;
            ComponentLevelStaticData.RadarCrossSection = 1.5f;
            ComponentLevelStaticData.RCSBoostOnUsage = 3f;
            ComponentLevelStaticData.ForwardAeroFactor = 3f;
            ComponentLevelStaticData.SideAeroFactor = 5f;
            ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(49f, 115f);

            Ultra.TextureLoader.LoadTexture(Data, "HEPP_MK-I.png");

            Firing = Ultra.SoundLoader.Load("HEPP_MK-I.wav");

            base.LoadStaticData();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GimbalGameManager.ClientOptions.SoundOn)
            {
                if (FiringInstance == null)
                {
                    FiringInstance = Firing.CreateInstance();
                }
                if (((ChargeUpFireControl)mFireControl).Charging && !SoundFired)
                {
                    FiringInstance.Apply3D(World.Camera.Sound3DListener, SoundEmitter);
                    FiringInstance.Play();
                    SoundFired = true;
                }
            }
        }

        public override void DrawThis(GameTime gameTime, SpriteBatch spriteBatch, int iteration, bool isGlowyDraw)
        {
            base.DrawThis(gameTime, spriteBatch, iteration, isGlowyDraw);

            spriteBatch.DrawString(Utility.ColinsFont, SoundEmitter.Position.ToString(), AbsolutePosition + new Vector2(50, 50), Color.Black);

        }

        public override void FireA(GameTime gameTime)
        {
            base.FireA(gameTime);

            if (mMuzzleFlash != null)
            {
                mMuzzleFlash.Flash(gameTime);
            }
            float num = base.AbsoluteAngle;
            num += base.GetNetSyncedRandomishFloat() * base.StaticData.ProjectileSpread;
            Vector2 vector = Vector2.Transform(base.Physics.Forward, Matrix.CreateRotationZ(num));
            Vector2 inPos = base.AbsolutePosition + Vector2.Transform(mMuzzlePoint, Matrix.CreateRotationZ(num));
            BaseProjectile baseProjectile = new PlasmaCannonShot(base.World, inPos, base.RootParentAttachment.Physics.Velocity, vector, base.OwnerPlayer, this);
            baseProjectile.Physics.Angle = num;
            base.FireBullet(baseProjectile, gameTime);
            if (GimbalGameManager.ClientOptions.ShowParticles && base.World.Camera.GetIsInViewFrustum(base.AbsolutePosition, 100f))
            {
                int num2 = Utility.Random.Next_i(1, 3);
                for (int i = 0; i < num2; i++)
                {
                    float duration = Utility.Random.Next_f(0.2f, 1.2f);
                    Spark spark = new Spark(inPos, "particle", duration, Color.White, new Color(base.OwnerPlayer.GetColor(TeamColorType.TrailEndColor), 0));
                    spark.BlurOn = true;
                    spark.IsGlowy = true;
                    spark.WanderOn = true;
                    spark.WanderBoost = 1f;
                    spark.Physics.Velocity = base.RootParentAttachment.Physics.Velocity + vector * Utility.Random.Next_f(200f, 2000f);
                    spark.FudgeScaleWithZoomFactor = 0.1f;
                    spark.Scale = Utility.Random.Next_f(0.2f, 0.7f);
                    spark.Physics.Velocity += Utility.Random.Next_f(0f, 0.3f) * base.Physics.Velocity;
                    base.World.IntroduceNewParticle(spark);
                }
            }
            if (base.World.Camera.Target == base.RootParentAttachment)
            {
                base.World.Camera.Shake(1f);
            }
            baseProjectile.DoRecoil(this);
            GimbalGameManager.SoundManager.Stop(mCueFireA);
            SoundFired = false;
        }

        public override void FireB(GameTime gameTime)
        {
        }
    }

    [Serializable]
    public class ChargeUpFireControl : FireControl
    {
        public float ChargeUpTime;
        public float ChargeUpLeft;
        public bool Charging = false;

        public override bool FireImminent
        {
            get
            {
                return mFireACommanded;
            }
        }

        public ChargeUpFireControl(float shotsPerSecondA, float ChargeUp) : base(shotsPerSecondA)
        {
            this.ChargeUpTime = ChargeUp;
        }

        public override void CommandFireA()
        {
            mFireACommanded = true;
        }

        public override void CommandFireB()
        {
        }

        public override bool GetFireA(GameTime gameTime)
        {
            TimeSpan totalGameTime = gameTime.TotalGameTime;
            TimeSpan t = new TimeSpan(Utility.SecsToTicks((float)this.mPeriodA.TotalSeconds / Camera.TimeMultiplier));
            if (this.mFireACommanded && totalGameTime > this.mLastShotTime + t)
            {
                this.mLastShotTime = totalGameTime;
                ChargeUpLeft = ChargeUpTime;
                Charging = true;
            }
            if(Charging)
            {
                ChargeUpLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(ChargeUpLeft < 0)
                {
                    Charging = false;
                    return true;
                }
            }
            return false;
        }
        public override bool GetFireB(GameTime gameTime)
        {
            return false;
        }
        public override void Update(GameTime gameTime)
        {
            mFireACommanded = false;
        }
    }
}
