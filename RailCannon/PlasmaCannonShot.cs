using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ultra;

namespace PlasmaCannon
{
    class PlasmaCannonShot : BaseProjectile
    {
        public const float MuzzleVelocity = 17600f;

        public const float Mass = 0.05f;

        public const float BaseDamage = 750f;

        public const float ExplosiveDamage = 250f;

        public const float ExplosiveRadius = 100f;

        public const float LifeSpan = 0.35f;

        public const float RicochetModifier = 1f;

        public PlasmaCannonShot() : base(null, Vector2.Zero, Vector2.Zero, Vector2.Zero, null, null)
		{
        }

        public PlasmaCannonShot(World containingWorld, Vector2 inPos, Vector2 inVel, Vector2 inDir, Player inOwner, Component inMurderWeapon) : base(containingWorld, inPos, inVel, inDir, inOwner, inMurderWeapon)
		{
            base.Alive = true;
            this.mDamage = 2000f;
            this.mMuzzleVelocity = 2000f;
            base.Physics.Mass = 10f;
            this.mRicochetModifierValue = 0f;
            this.mBaseTexture = TextureLoader.Load("PlasmaShot.png");
            this.mTextureHash = "PlasmaShot.png".GetHashCode();
            base.Scale = 2f;
            base.BlurOn = false;
            this.mColor = inOwner.GetColor(TeamColorType.LightColor);
            base.Physics.ImageCenterOfMass = new Vector2(9f, 11f);
            this.mAppearTime = 0.01f;
            this.mLifeTime = 8f;
            this.IsGlowy = true;
            
            inDir = Utility.SafeNormalize(inDir);
            base.Physics.Velocity += inDir * this.mMuzzleVelocity;
        }

        public override void DoImpactSound()
        {
            GimbalGameManager.SoundManager.Play3D("rail_hit", base.World.Camera.Sound3DListener, base.World.Camera.GetAdjustedSoundPosition(base.Physics.Position));
        }

        public override void DoImpact(Component affectedComponent, Vector2 worldImpactPosition, Vector2 targetImpactPosition, Vector2 normal)
        {
            base.DoImpact(affectedComponent, worldImpactPosition, targetImpactPosition, normal);
            if (affectedComponent != null)
            {
                Vector2 vector = -targetImpactPosition;
                vector = Utility.SafeNormalize(vector);
                vector *= 4f;
                targetImpactPosition += vector;
            }
            Vector2 value = Vector2.Zero;
            if (affectedComponent != null)
            {
                value = Utility.SafeNormalize(worldImpactPosition - affectedComponent.RootParentAttachment.AbsolutePosition) * 10f;
            }
            base.World.Explosions.SafeAdd(new Explosion(base.World, base.Owner, base.MurderWeapon, worldImpactPosition + value, 100f * this.mAblatedFactor, 500f * this.mAblatedFactor, 1f, false));
            if (affectedComponent != null)
            {
                FireEmitter decal = new FireEmitter(new TimeSpan(0, 0, 1), 0.2f, targetImpactPosition.X, targetImpactPosition.Y, false);
                affectedComponent.AddShortLivedDecalToThis(decal);
                VaporEmitter vaporEmitter = new VaporEmitter(1f, 3f, targetImpactPosition.X, targetImpactPosition.Y);
                vaporEmitter.Vapate(new TimeSpan(0, 0, 2));
                affectedComponent.AddShortLivedDecalToThis(vaporEmitter);
                affectedComponent.AddGlowyShortLivedDecalToThis(new Spark(targetImpactPosition, "burninghole", 9f, Color.White, new Color(255, 180, 0, 0))
                {
                    Physics =
                    {
                        Angle = Utility.GetRandomAngle()
                    },
                    Scale = 1.5f,
                    WanderOn = false
                });
            }
            if (base.World.Camera.GetIsInViewFrustum(worldImpactPosition, 1000f))
            {
                byte a = 10;
                HorizontalLensFlare horizontalLensFlare = new HorizontalLensFlare(worldImpactPosition, "lensflare", 0.1f, new Color(255, 255, 255, a), new Color(50, 50, 255, 0));
                horizontalLensFlare.IsGlowy = true;
                base.World.IntroduceNewParticle(horizontalLensFlare);
                int num = Utility.Random.Next_i(0, 2);
                for (int i = 0; i < num; i++)
                {
                    FallingSprite chunk = base.World.GetChunk(worldImpactPosition);
                    float x = Utility.Random.Next_f() * 150f + 10f;
                    float randomAngle = Utility.GetRandomAngle();
                    chunk.Scale = 2f;
                    chunk.Physics.Angle = randomAngle;
                    chunk.Physics.AngularVelocity = Utility.Random.Next_f() * 20f - 10f;
                    chunk.Physics.Velocity = Vector2.Transform(new Vector2(x, 0f), Matrix.CreateRotationZ(randomAngle));
                    base.World.IntroduceNewParticle(chunk);
                }
                if (GimbalGameManager.ClientOptions.ShowParticles)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        float duration = Utility.Random.Next_f(0.3f, 2f);
                        Spark spark = new Spark(worldImpactPosition, "particle", duration, new Color(255, 210, 71, 255), new Color(128, 0, 0, 0));
                        spark.BlurOn = true;
                        spark.IsGlowy = true;
                        spark.Physics.Velocity = Utility.GetRandomRadialVelocityInArc(500f, Utility.GetAngle(normal), 1.57079637f);
                        spark.FudgeScaleWithZoomFactor = 0.05f;
                        spark.Scale = Utility.Random.Next_f(0.2f, 1f);
                        base.World.IntroduceNewParticle(spark);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        float duration2 = Utility.Random.Next_f() * 0.8f + 3f;
                        TransitionSprite transitionSprite = new TransitionSprite(worldImpactPosition, "particle", duration2, new SpriteProperties
                        {
                            Color = new Color(255, 240, 210, 255),
                            Scale = new Vector2(0.7f, 0.7f)
                        }, new SpriteProperties
                        {
                            Color = new Color(255, 240, 210, 0),
                            Scale = new Vector2(0.3f, 0.3f)
                        });
                        transitionSprite.IsGlowy = true;
                        transitionSprite.BlurOn = true;
                        transitionSprite.Physics.Velocity = Utility.GetRandomRadialVelocityInArc(30f, Utility.GetAngle(normal), 1.57079637f);
                        transitionSprite.FudgeScaleWithZoomFactor = 0.3f;
                        transitionSprite.Scale = Utility.Random.Next_f(0.4f, 1.1f);
                        base.World.IntroduceNewParticle(transitionSprite);
                    }
                    for (int l = 0; l < 7; l++)
                    {
                        float duration3 = Utility.Random.Next_f(0.3f, 2f);
                        Spark spark2 = new Spark(worldImpactPosition, "particle", duration3, new Color(255, 238, 185, 255), new Color(230, 97, 0, 0));
                        spark2.BlurOn = true;
                        spark2.IsGlowy = true;
                        float x2 = Utility.Random.Next_f() * 200f;
                        float randomAngle2 = Utility.GetRandomAngle();
                        spark2.Physics.Velocity = Vector2.Transform(new Vector2(x2, 0f), Matrix.CreateRotationZ(randomAngle2));
                        spark2.FudgeScaleWithZoomFactor = 0.1f;
                        spark2.Scale = Utility.Random.Next_f(0.4f, 1.1f);
                        spark2.Physics.Velocity += Utility.Random.Next_f(0f, 0.1f) * base.Physics.Velocity;
                        base.World.IntroduceNewParticle(spark2);
                    }
                    for (int m = 0; m < 5; m++)
                    {
                        float duration4 = Utility.Random.Next_f() * 0.8f + 2f;
                        TransitionSprite transitionSprite2 = new TransitionSprite(worldImpactPosition, "particle", duration4, new SpriteProperties
                        {
                            Color = Color.White,
                            Scale = new Vector2(0.8f, 0.8f)
                        }, new SpriteProperties
                        {
                            Color = new Color(base.Owner.GetColor(TeamColorType.TrailStartColor), 0f),
                            Scale = new Vector2(0.3f, 0.3f)
                        });
                        transitionSprite2.BlurOn = true;
                        transitionSprite2.IsGlowy = true;
                        float x3 = Utility.Random.Next_f() * 20f;
                        float randomAngle3 = Utility.GetRandomAngle();
                        transitionSprite2.Physics.Velocity = Vector2.Transform(new Vector2(x3, 0f), Matrix.CreateRotationZ(randomAngle3));
                        transitionSprite2.Physics.Velocity += Utility.SafeNormalize(base.Physics.Velocity) * Utility.Random.Next_f(0f, 300f);
                        transitionSprite2.FudgeScaleWithZoomFactor = 0.3f;
                        transitionSprite2.Scale = Utility.Random.Next_f(0.5f, 1.3f);
                        base.World.IntroduceNewParticle(transitionSprite2);
                    }
                }
                BaseDecal baseDecal = new TransitionSprite(worldImpactPosition, "shockwave", 0.7f, new SpriteProperties
                {
                    Color = new Color(255, 255, 255, 20),
                    Scale = new Vector2(0.4f, 0.4f)
                }, new SpriteProperties
                {
                    Color = new Color(255, 255, 255, 0),
                    Scale = new Vector2(1f, 1f)
                });
                baseDecal.FudgeScaleWithZoomFactor = 0.05f;
                baseDecal.IsGlowy = true;
                base.World.IntroduceNewParticle(baseDecal);
                AnimatedDecal animatedDecal = new AnimatedDecal(worldImpactPosition, "splode5", 1f, 64, 64, 5, 25f, 1);
                animatedDecal.IsGlowy = true;
                animatedDecal.Physics.Angle = Utility.GetRandomAngle();
                animatedDecal.Scale = Utility.Random.Next_f() * 0.7f + 1f;
                animatedDecal.FudgeScaleWithZoomFactor = 0.05f;
                base.World.IntroduceNewParticle(animatedDecal);
                float num2 = Utility.Random.Next_f(1f, 1.3f);
                Spark spark3 = new Spark(worldImpactPosition, "flash", 0.05f, new Color(255, 255, 255, 255), new Color(255, 255, 255, 0));
                spark3.IsGlowy = true;
                spark3.Scale = 1.5f * num2;
                base.World.IntroduceNewParticle(spark3);
                SpriteProperties spriteProperties = new SpriteProperties();
                spriteProperties.Color = new Color(255, 255, 200, 60);
                spriteProperties.Scale = new Vector2(1f, 1f) * 5f * num2;
                SpriteProperties spriteProperties2 = new SpriteProperties();
                spriteProperties2.Color = new Color(255, 255, 200, 0);
                spriteProperties.Scale = new Vector2(1f, 1f) * 3f * num2;
                TransitionSprite transitionSprite3 = new TransitionSprite(worldImpactPosition, "engine_flare", 0.2f, spriteProperties, spriteProperties2);
                transitionSprite3.IsGlowy = true;
                base.World.IntroduceNewParticle(transitionSprite3);
            }
        }
    }
}
