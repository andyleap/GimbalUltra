using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorTweaks
{
    public class LinkerTweaks
    {

        // Colin.Gimbal.ComponentLinker
        [Ultra.AutoHook("Colin.Gimbal.ComponentLinker", "DoInput", Skip = true)]
        public static bool DoInput(ComponentLinker cl, GameTime gameTime, KeyboardState keyboardState, ColinPadState colinPadState, MouseState mouseState)
        {
            keyboardState = cl.mInputReset.UpdateAndFilterState(keyboardState);
            mouseState = cl.mInputReset.UpdateAndFilterState(mouseState);
            if (!cl.Hide)
            {
                Vector2 mousePos = new Vector2((float)mouseState.X, (float)mouseState.Y);
                if (cl.mWeArePicking)
                {
                    if (keyboardState.IsKeyDown(Keys.Escape) || mouseState.RightButton == ButtonState.Pressed)
                    {
                        cl.CancelPick();
                        return false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && cl.mLastMouseState.LeftButton == ButtonState.Released)
                    {
                        if (cl.PotentialLink == null && !keyboardState.IsKeyDown(Keys.LeftShift) && !keyboardState.IsKeyDown(Keys.RightShift))
                        {
                            cl.CancelPick();
                            return false;
                        }
                        if (!cl.mLinkList.Contains(cl.PotentialLink))
                        {
                            cl.mLinkList.Add(cl.PotentialLink);
                        }
                        cl.PotentialLink.InputMappingA.Bindings.Clear();
                        cl.PotentialLink.InputMappingB.Bindings.Clear();
                        if (!keyboardState.IsKeyDown(Keys.LeftShift) && !keyboardState.IsKeyDown(Keys.RightShift))
                        {
                            ((BasicEventCallback)typeof(ComponentLinker).GetField("mOnDoneEvent").GetValue(cl))(cl, new EventArgs());
                            cl.mWeArePicking = false;
                            cl.mInputReset.Reset();
                        }
                        GimbalGameManager.SoundManager.Play("bink");
                    }
                }
                if (cl.mWeArePicking)
                {
                    Vector2 vector = new Vector2(256f, 256f);
                    if (!Utility.PointIsInRect(cl.PotentialLinkWorldPos, -vector / 2f, vector) && mouseState.LeftButton == ButtonState.Pressed && cl.mLastMouseState.LeftButton == ButtonState.Released)
                    {
                        cl.CancelPick();
                    }
                }
                cl.mClearHover = false;
                if (Utility.PointIsInRect(mousePos, cl.mOrigin + new Vector2(48f, 0f), new Vector2(26f, 24f)))
                {
                    cl.mClearHover = true;
                    if (mouseState.LeftButton == ButtonState.Pressed && cl.mLastMouseState.LeftButton == ButtonState.Released)
                    {
                        if (cl.mWeArePicking)
                        {
                            cl.CancelPick();
                        }
                        else
                        {
                            if (cl.mLinkList.Count > 0)
                            {
                                cl.mLinkList.RemoveAt(cl.mLinkList.Count - 1);
                                ((BasicEventCallback)typeof(ComponentLinker).GetField("mOnDoneEvent").GetValue(cl))(cl, new EventArgs());
                                cl.mInputReset.Reset();
                                GimbalGameManager.SoundManager.Play("bink_low");
                            }
                        }
                    }
                }
                cl.mAddHover = false;
                if (Utility.PointIsInRect(mousePos, cl.mOrigin + new Vector2(-6f, 0f), new Vector2(53f, 24f)))
                {
                    cl.mAddHover = true;
                    if (mouseState.LeftButton == ButtonState.Pressed && cl.mLastMouseState.LeftButton == ButtonState.Released)
                    {
                        if (cl.mWeArePicking)
                        {
                            cl.CancelPick();
                        }
                        else
                        {
                            cl.StartPick();
                        }
                    }
                }
                cl.mLastMouseState = mouseState;
                return cl.mWeArePicking;
            }
            return false;
        }

    }
}
