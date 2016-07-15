using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TitanEngine
{
    [Ultra.CustomPart(PartsCategories.Propulsion, MastermindOnly = true)]
    [Serializable]
    public class TitanThruster : Colin.Gimbal.BaseThruster, IDeserializationCallback
    {
        private const float MaxThrust = 900000f;

        private static Colin.Gimbal.ComponentStaticData Data;

        [NonSerialized]
        public Colin.Gimbal.BaseDecal mForThrust;

        [NonSerialized]
        public Colin.Gimbal.BaseDecal mBackThrust;

        [NonSerialized]
        public Colin.Gimbal.BaseDecal mFlareForward;

        [NonSerialized]
        public Colin.Gimbal.BaseDecal mFlareBack;

        [NonSerialized]
        public Colin.Gimbal.BrailEmitter mIonTrail;

        [NonSerialized]
        public Colin.Gimbal.BrailEmitter mIonTrailBack;

        public override Colin.Gimbal.ComponentStaticData ComponentLevelStaticData
        {
            get
            {
                return TitanThruster.Data;
            }
        }

        public TitanThruster()
        {
        }

        public TitanThruster(Vector2 inPos) : base(inPos)
        {
        }

        public TitanThruster(Vector2 inPos, float inAng) : base(inPos, inAng)
        {
        }

        public override Colin.Gimbal.Component GetNewInstance()
        {
            return new TitanThruster();
        }

        public override void InstantiateMembers()
        {
            base.InstantiateMembers();
            this.mSpooler = new Colin.Gimbal.CubicSpooler(1f, 0.8f, true);
        }

        public override void VirtualBuildDecals()
        {
            base.VirtualBuildDecals();
            this.mForThrust = new Colin.Gimbal.BaseDecal(0f, 36f, 0f, "thrust5");
            this.mForThrust.On = false;
            this.mForThrust.FudgeScaleWithZoomFactor = 0.1f;
            base.AddGlowyDecalToThis(this.mForThrust);
            this.mBackThrust = new Colin.Gimbal.BaseDecal(0f, -41f, 3.1f, "thrust5");
            this.mBackThrust.On = false;
            this.mBackThrust.FudgeScaleWithZoomFactor = 0.1f;
            base.AddGlowyDecalToThis(this.mBackThrust);
            this.mFlareBack = new Colin.Gimbal.LensFlareDecal(new Vector2(0f, -51f), "engine_flare");
            this.mFlareBack.Color = new Color(219, 253, 255);
            this.mFlareBack.Alpha = 0.03f;
            this.mFlareBack.Scale = 0.5f;
            base.AddGlowyDecalToThis(this.mFlareBack);
            this.mFlareForward = new Colin.Gimbal.LensFlareDecal(new Vector2(0f, 46f), "engine_flare");
            this.mFlareForward.Color = new Color(219, 253, 255);
            this.mFlareForward.Alpha = 0.03f;
            this.mFlareForward.Scale = 0.5f;
            base.AddGlowyDecalToThis(this.mFlareForward);
            Colin.Gimbal.SpriteProperties start = new Colin.Gimbal.SpriteProperties(new Color(90, 210, 255, 50), 0.5f);
            Colin.Gimbal.SpriteProperties end = new Colin.Gimbal.SpriteProperties(new Color(0, 20, 255, 0), 0.35f);
            this.mIonTrail = new Colin.Gimbal.BrailEmitter(new Vector2(0f, 36f), "ion_trail_line", start, end, 0.9f, 1f);
            this.mIonTrail.Physics.Angle = 3.14159274f;
            this.mIonTrail.EjectVelocity = 400f;
            base.AddDecalToThis(this.mIonTrail);
            this.mIonTrailBack = new Colin.Gimbal.BrailEmitter(new Vector2(0f, -41f), "ion_trail_line", start, end, 0.9f, 1f);
            this.mIonTrailBack.EjectVelocity = 400f;
            base.AddDecalToThis(this.mIonTrailBack);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.mThrust = 900000f;
        }

        public override void LoadStaticData()
        {
            Data = new Colin.Gimbal.ComponentStaticData(this);
            this.ComponentLevelStaticData.OriginalPhysics = new Colin.Gimbal.Physics();
            this.ComponentLevelStaticData.DisplayName = "Titan Thruster";
            this.ComponentLevelStaticData.Description = "The Titan Thruster provides massive propulsion best suited for mastermind platforms. It boasts insane thrust, but is affected by high mass and slow throttle response.";
            this.ComponentLevelStaticData.DescFunctionA = "Forward";
            this.ComponentLevelStaticData.DescFunctionB = "Reverse";
            this.ComponentLevelStaticData.UnitPrice = 600;
            this.ComponentLevelStaticData.RequiredRank = Colin.Gimbal.PlayerRank.Commodore;
            this.ComponentLevelStaticData.EditorThrust = (int)Math.Round(14400.0000683963299);
            this.ComponentLevelStaticData.MaxHitPoints = 400f;
            this.ComponentLevelStaticData.OriginalPhysics.Mass = 400f;
            this.ComponentLevelStaticData.OriginalPhysics.Moment = 2400000f;
            this.ComponentLevelStaticData.MaxFuel = 0f;
            this.ComponentLevelStaticData.RadarCrossSection = 0.8f;

            this.ComponentLevelStaticData.ForwardAeroFactor = 43f;
            this.ComponentLevelStaticData.SideAeroFactor = 23f;
            this.ComponentLevelStaticData.OriginalPhysics.ImageCenterOfMass = new Vector2(39f, 59f);

            Ultra.TextureLoader.LoadTexture(Data, "TitanThruster.png");

            base.LoadStaticData();
        }

        public override void Update(GameTime gameTime)
        {
            if (!base.Alive)
            {
                base.FunctionA = false;
                base.FunctionB = false;
                base.Update(gameTime);
                return;
            }
            this.mForThrust.On = (base.Active && base.ShowThrust && this.mSpooler.SpoolOutput > 0f);
            this.mBackThrust.On = (base.Active && base.ShowThrust && this.mSpooler.SpoolOutput < 0f);
            this.mFlareForward.On = this.mForThrust.On;
            this.mFlareBack.On = this.mBackThrust.On;
            Vector2 vector = base.TotalVelocity;
            Vector2 value = -this.mPhysics.Forward * this.mIonTrail.EjectVelocity;
            float num = Colin.Gimbal.Utility.GetAngle(vector - value);
            num *= MathHelper.Lerp(1f, 0f, Math.Abs(num) / 3.14159274f);
            this.mForThrust.Physics.Angle = num * 0.6f;
            vector *= -1f;
            value = -this.mPhysics.Forward * this.mIonTrail.EjectVelocity;
            num = Colin.Gimbal.Utility.GetAngle(vector - value);
            num *= MathHelper.Lerp(1f, 0f, Math.Abs(num) / 3.14159274f);
            this.mBackThrust.Physics.Angle = num * 0.6f + 3.14159274f;
            this.mForThrust.ScaleV = new Vector2(Colin.Gimbal.Utility.Random.Next_f(0.72f, 0.8f) * Math.Abs(this.mSpooler.SpoolOutput), Colin.Gimbal.Utility.Random.Next_f(0.9f, 1.1f) * Math.Abs(this.mSpooler.SpoolOutput));
            this.mBackThrust.ScaleV = new Vector2(Colin.Gimbal.Utility.Random.Next_f(0.72f, 0.8f) * Math.Abs(this.mSpooler.SpoolOutput), Colin.Gimbal.Utility.Random.Next_f(0.9f, 1.1f) * Math.Abs(this.mSpooler.SpoolOutput));
            this.mIonTrail.Alpha = MathHelper.Clamp(0.6f * this.mSpooler.SpoolOutput, 0f, 1f);
            this.mIonTrailBack.Alpha = MathHelper.Clamp(-0.6f * this.mSpooler.SpoolOutput, 0f, 1f);
            Colin.Gimbal.SpriteProperties startProperties = new Colin.Gimbal.SpriteProperties(new Color(base.OwnerPlayer.GetColor(Colin.Gimbal.TeamColorType.TrailStartColor), 1f), 0.55f);
            Colin.Gimbal.SpriteProperties endProperties = new Colin.Gimbal.SpriteProperties(new Color(base.OwnerPlayer.GetColor(Colin.Gimbal.TeamColorType.TrailEndColor), 0f), 0.39f);
            this.mIonTrail.StartProperties = startProperties;
            this.mIonTrail.EndProperties = endProperties;
            this.mIonTrailBack.StartProperties = startProperties;
            this.mIonTrailBack.EndProperties = endProperties;
            Color color = Colin.Gimbal.Utility.MultiplyColorBrightness(base.OwnerPlayer.GetColor(Colin.Gimbal.TeamColorType.BaseColor), 0.6f);
            this.mForThrust.Color = color;
            this.mBackThrust.Color = color;
            this.mForThrust.Alpha = Colin.Gimbal.Utility.Random.Next_f(0.6f, 0.7f);
            this.mBackThrust.Alpha = Colin.Gimbal.Utility.Random.Next_f(0.6f, 0.7f);
            this.mFlareForward.Color = color;
            this.mFlareBack.Color = color;
            this.mFlareForward.Alpha = Math.Abs(this.mSpooler.SpoolOutput * 0.06f);
            this.mFlareBack.Alpha = Math.Abs(this.mSpooler.SpoolOutput * 0.06f);
            if (this.mForThrust.On && this.mSpooler.SpoolPosition > 0.1f)
            {
                this.mIonTrail.Vapate(Colin.Gimbal.Utility.GetTimeSpanInSeconds(0.9f));
            }
            if (this.mBackThrust.On && this.mSpooler.SpoolPosition < -0.1f)
            {
                this.mIonTrailBack.Vapate(Colin.Gimbal.Utility.GetTimeSpanInSeconds(0.9f));
            }
            if (this.mSpooler.AbsSpoolOutput > 0.01f && Colin.Gimbal.GimbalGameManager.ClientOptions.SoundOn)
            {
                if (this.mCueThrust == null || this.mCueThrust.IsStopped)
                {
                    this.mCueThrust = Colin.Gimbal.GimbalGameManager.SoundManager.Play3D("hthruster", base.World.Camera.Sound3DListener, base.SoundEmitter);
                }
                else
                {
                    this.mCueThrust.Apply3D(base.World.Camera.Sound3DListener, base.SoundEmitter);
                }
                float value2 = -this.mAirVelocity.Y;
                float num2 = Colin.Gimbal.Utility.ReRange(value2, 0f, 1200f, 0.3f, 1f);
                this.mCueThrust.SetVariable("pitch", this.mSpooler.AbsSpoolOutput * num2);
                this.mCueThrust.SetVariable("engine_ramp", this.mSpooler.AbsSpoolOutput);
            }
            else
            {
                this.StopSounds();
            }
            base.Update(gameTime);
        }
    }
}
