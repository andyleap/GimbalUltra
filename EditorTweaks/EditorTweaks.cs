using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colin.Gimbal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EditorTweaks
{
    public class EditorTweaks
    {
        public static Texture2D White;

        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "ScreenLoadContent", Skip = true)]
        public static void ScreenLoadContent(DesignerScreen ds)
        {
            if(White == null)
            {
                White = new Texture2D(GimbalGameManager.GraphicsDevice, 1, 1);
                White.SetData(new Color[] { Color.White });
            }
            ds.mDimensions = new Vector2(Utility.ScreenW, Utility.ScreenH);
            ds.mPosition = new Vector2(0, 0);
            ds.mGridCenter = new Vector2((int)(ds.mDimensions.X - 500)/2 + 498, (int)(ds.mDimensions.Y - 270)/2 + 110);
            ds.mFont = Utility.LoadFont("colinsfont");
            ds.mBlackTexture = Utility.LoadTexture("white");
            ds.mHeadingTexture = Utility.LoadTexture("heading_designer");
            ds.mBinBackdropTexture = Utility.LoadTexture("designer_bin_backdrop");
            ds.mDescBackdropTexture = Utility.LoadTexture("designer_desc_backdrop");
            ds.mGridBackdropTexture = Utility.LoadTexture("designer_grid_backdrop");
            ds.mCGTexture = Utility.LoadTexture("designer_cg_left");
            ds.mCGTextTexture = Utility.LoadTexture("designer_cg_text");
            ds.mSelectedOverlayPosTexture = Utility.LoadTexture("designer_selected_pos");
            ds.mSelectedOverlayDirTexture = Utility.LoadTexture("designer_selected_dir");
            ds.mSoftwareStandInTexture = Utility.LoadTexture("designer_software_standin");
            ds.mSoftwareStandInSelectedTexture = Utility.LoadTexture("designer_software_standin_selected");
            ds.mMastermindLogo = Utility.LoadTexture("designer_mastermind_logo");
            if (!ds.mMastermindMode && ds.mVehicle != null && ds.mVehicle.IsMastermindShip())
            {
                ds.mMastermindMode = true;
                ds.mBudgetCap = GimbalGameManager.ClientOptions.Money;
            }
            if (!ds.mMastermindMode)
            {
                ds.mBudgetCap = Utility.Min(ds.mBudgetCap, GameLogicHandler.GetMoneyForRank(PlayerRank.Admiral));
            }
            ds.mScreenDescLabel = new Label(new Vector2(327f, 24f), ds.mDimensions.X - 340f, WrapMode.MultiLine, "colinsfont", Color.White);
            ds.mControls.Add(ds.mScreenDescLabel);
            ds.mFlightTestButton = new SimpleImageButton(new Vector2(ds.mDimensions.X - 304f, ds.mDimensions.Y - 59f), false, "designer_test", new Color(255, 255, 0));
            ds.mFlightTestButton.OnClick += new BasicEventCallback(ds.TestFlightButtonCallback);
            ds.mFlightTestButton.FireOnDownClick = false;
            ds.mControls.Add(ds.mFlightTestButton);
            SimpleImageButton simpleImageButton = new SimpleImageButton(new Vector2(ds.mDimensions.X - 140f, ds.mDimensions.Y - 59f), false, "designer_done", new Color(255, 255, 0));
            simpleImageButton.OnClick += new BasicEventCallback(ds.DoneButtonCallback);
            simpleImageButton.FireOnDownClick = false;
            ds.mControls.Add(simpleImageButton);
            Vector2 value = new Vector2(515, ds.mDimensions.Y - 268);
            ds.mPanUpButton = new SimpleImageButton(value + new Vector2(28f, 0f), false, "designer_pan_up", new Color(255, 255, 0));
            ds.mPanUpButton.OnClick += new BasicEventCallback(ds.PanUpButtonCallback);
            ds.mPanUpButton.FireOnDownClick = true;
            ds.mPanUpButton.TurboRepeat = true;
            ds.mControls.Add(ds.mPanUpButton);
            ds.mPanDownButton = new SimpleImageButton(value + new Vector2(28f, 66f), false, "designer_pan_down", new Color(255, 255, 0));
            ds.mPanDownButton.OnClick += new BasicEventCallback(ds.PanDownButtonCallback);
            ds.mPanDownButton.FireOnDownClick = true;
            ds.mPanDownButton.TurboRepeat = true;
            ds.mControls.Add(ds.mPanDownButton);
            ds.mPanLeftButton = new SimpleImageButton(value + new Vector2(0f, 27f), false, "designer_pan_left", new Color(255, 255, 0));
            ds.mPanLeftButton.OnClick += new BasicEventCallback(ds.PanLeftButtonCallback);
            ds.mPanLeftButton.FireOnDownClick = true;
            ds.mPanLeftButton.TurboRepeat = true;
            ds.mControls.Add(ds.mPanLeftButton);
            ds.mPanRightButton = new SimpleImageButton(value + new Vector2(65f, 27f), false, "designer_pan_right", new Color(255, 255, 0));
            ds.mPanRightButton.OnClick += new BasicEventCallback(ds.PanRightButtonCallback);
            ds.mPanRightButton.FireOnDownClick = true;
            ds.mPanRightButton.TurboRepeat = true;
            ds.mControls.Add(ds.mPanRightButton);
            ds.mPanResetButton = new SimpleImageButton(value + new Vector2(34f, 34f), false, "designer_pan_reset", new Color(255, 255, 0));
            ds.mPanResetButton.OnClick += new BasicEventCallback(ds.PanResetButtonCallback);
            ds.mPanResetButton.FireOnDownClick = true;
            ds.mPanResetButton.TurboRepeat = true;
            ds.mControls.Add(ds.mPanResetButton);
            ds.mZoomInButton = new SimpleImageButton(value + new Vector2(95f, 16f), false, "designer_zoom_in", new Color(255, 255, 0));
            ds.mZoomInButton.OnClick += new BasicEventCallback(ds.ZoomInButtonCallback);
            ds.mZoomInButton.FireOnDownClick = true;
            ds.mZoomInButton.TurboRepeat = true;
            ds.mControls.Add(ds.mZoomInButton);
            ds.mZoomOutButton = new SimpleImageButton(value + new Vector2(95f, 47f), false, "designer_zoom_out", new Color(255, 255, 0));
            ds.mZoomOutButton.OnClick += new BasicEventCallback(ds.ZoomOutButtonCallback);
            ds.mZoomOutButton.FireOnDownClick = true;
            ds.mZoomOutButton.TurboRepeat = true;
            ds.mControls.Add(ds.mZoomOutButton);
            Vector2 value2 = new Vector2(ds.mDimensions.X - 522, ds.mDimensions.Y - 151);
            ds.mRemoveButton = new SimpleImageButton(value2 + new Vector2(0f, 0f), false, "designer_remove", new Color(255, 255, 0));
            ds.mRemoveButton.OnClick += new BasicEventCallback(ds.RemoveButtonCallback);
            ds.mRemoveButton.SoundName = "bink_low";
            ds.mRemoveButton.FireOnDownClick = true;
            ds.mControls.Add(ds.mRemoveButton);
            ds.mCcwButton = new SimpleImageButton(value2 + new Vector2(74f, 0f), false, "designer_ccw", new Color(255, 255, 0));
            ds.mCcwButton.OnClick += new BasicEventCallback(ds.CcwButtonCallback);
            ds.mCcwButton.FireOnDownClick = true;
            ds.mCcwButton.TurboRepeat = true;
            ds.mControls.Add(ds.mCcwButton);
            ds.mCwButton = new SimpleImageButton(value2 + new Vector2(141f, 0f), false, "designer_cw", new Color(255, 255, 0));
            ds.mCwButton.OnClick += new BasicEventCallback(ds.CwButtonCallback);
            ds.mCwButton.FireOnDownClick = true;
            ds.mCwButton.TurboRepeat = true;
            ds.mControls.Add(ds.mCwButton);
            SimpleImageButton simpleImageButton2 = new SimpleImageButton(value2 + new Vector2(3f, 74f), false, "designer_new", new Color(255, 255, 0));
            simpleImageButton2.OnClick += new BasicEventCallback(ds.NewButtonCallback);
            simpleImageButton2.FireOnDownClick = true;
            ds.mControls.Add(simpleImageButton2);
            ds.mSaveButton = new SimpleImageButton(value2 + new Vector2(63f, 74f), false, "designer_save", new Color(255, 255, 0));
            ds.mSaveButton.OnClick += new BasicEventCallback(ds.SaveButtonCallback);
            ds.mSaveButton.FireOnDownClick = true;
            ds.mControls.Add(ds.mSaveButton);
            ds.mMastermindModeButton = new SimpleImageButton(value2 + new Vector2(147f, 74f), false, "designer_atom_button", new Color(255, 255, 0));
            ds.mMastermindModeButton.OnClick += new BasicEventCallback(ds.MastermindModeButtonCallback);
            ds.mMastermindModeButton.FireOnDownClick = true;
            ds.mMastermindModeButton.UserHoverImageForGreyout = true;
            ds.mControls.Add(ds.mMastermindModeButton);
            ds.mNameLabel = new Label(new Vector2(23f, ds.mDimensions.Y - 471), 320f, WrapMode.WindowBound, "moirebold", new Color(103, 230, 241));
            ds.mNameLabel.Text = "HG-22 Airstabber";
            ds.mControls.Add(ds.mNameLabel);
            ds.mEditableName = new TextBox(new Vector2(17f, ds.mDimensions.Y - 496), 320f, 16, 0, WrapMode.WindowBound, false, Utility.LoadFont("moirebold"), new Color(103, 230, 241));
            ds.mEditableName.Text = "HG-22 Stabby";
            ds.mEditableName.OnEnter += new BasicEventCallback(ds.EditNameCallBack);
            ds.mControls.Add(ds.mEditableName);
            ds.mPriceLabel = new Label(new Vector2(352f, ds.mDimensions.Y - 471), 500f, WrapMode.WindowBound, "moirebold", new Color(255, 255, 255));
            ds.mPriceLabel.Text = "$350T";
            ds.mControls.Add(ds.mPriceLabel);
            Vector2 vector = new Vector2(38f, ds.mDimensions.Y - 433);
            Vector2 value3 = vector;
            int num = 23;
            ds.mHitPointsLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mHitPointsLabel.Text = "Durability: 1";
            ds.mControls.Add(ds.mHitPointsLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mMassLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mMassLabel.Text = "Mass: 164 Tons";
            ds.mControls.Add(ds.mMassLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mMomentLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mMomentLabel.Text = "Moment: 180 m4";
            ds.mControls.Add(ds.mMomentLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mDragLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mDragLabel.Text = "Drag: 164";
            ds.mControls.Add(ds.mDragLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mRCSLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mRCSLabel.Text = "Visibility: 164";
            ds.mControls.Add(ds.mRCSLabel);
            value3 += new Vector2(0f, (float)num);
            value3 += new Vector2(0f, 13f);
            ds.mDescLabel = new Label(value3 + new Vector2(0f, 0f), 440f, WrapMode.MultiLine, "colinsfont", new Color(255, 255, 255));
            ds.mDescLabel.Text = "Designed for light weight, speed, and maneuverability, the SF-26 is an excellent platform for scouting and light attack.";
            ds.mControls.Add(ds.mDescLabel);
            ds.mEditableDesc = new TextBox(value3 + new Vector2(-6f, -25f), 440f, 260, 0, WrapMode.MultiLine, false, Utility.ColinsFont, Color.White);
            ds.mEditableDesc.Text = "blah";
            ds.mEditableDesc.OnEnter += new BasicEventCallback(ds.EditDescCallBack);
            ds.mControls.Add(ds.mEditableDesc);
            value3 += new Vector2(0f, (float)num);
            value3 = vector + new Vector2(200f, 0f);
            ds.mThrustLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mThrustLabel.Text = "Thrust: 0";
            ds.mControls.Add(ds.mThrustLabel);
            ds.mDamageLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mDamageLabel.Text = "Damage: 0";
            ds.mControls.Add(ds.mDamageLabel);
            ds.mTorqueLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mTorqueLabel.Text = "Torque: 0";
            ds.mControls.Add(ds.mTorqueLabel);
            ds.mPowerLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mPowerLabel.Text = "Power: 0";
            ds.mControls.Add(ds.mPowerLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mROFLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mROFLabel.Text = "Rate: 0";
            ds.mControls.Add(ds.mROFLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mAmmoTypeLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mAmmoTypeLabel.Text = "Ammo: 0";
            ds.mControls.Add(ds.mAmmoTypeLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mMagazineSizeLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mMagazineSizeLabel.Text = "Magazine: 0";
            ds.mControls.Add(ds.mMagazineSizeLabel);
            value3 += new Vector2(0f, (float)num);
            ds.mMaxSpeedLabel = new Label(value3 + new Vector2(0f, 0f), 500f, WrapMode.WindowBound, "colinsfont_bold", new Color(255, 255, 255));
            ds.mMaxSpeedLabel.Text = "Top Speed: ";
            ds.mControls.Add(ds.mMaxSpeedLabel);
            value3 += new Vector2(0f, (float)num);
            value3 = new Vector2(vector.X, value3.Y);
            ds.mCustomArcSlider = new SliderHor(value3 + new Vector2(70f, 131f), 339);
            ds.mCustomArcSlider.OnScroll += new BasicEventCallback(ds.CustomArcSliderScrollCallback);
            ds.mCustomArcSlider.NumSteps = 361;
            ds.mCustomArcSlider.Snap = true;
            ds.mControls.Add(ds.mCustomArcSlider);
            ds.mCustomArcSliderLabel = new Label(value3 + new Vector2(164f, 156f), 500f, WrapMode.WindowBound);
            ds.mCustomArcSliderLabel.Text = "Swept Arc: 190 degrees";
            ds.mControls.Add(ds.mCustomArcSliderLabel);
            ds.mCustomAutoReturnCheckBox = new CheckBox(value3 + new Vector2(254f, 187f));
            ds.mCustomAutoReturnCheckBox.Label = "Auto-Return";
            ds.mCustomAutoReturnCheckBox.OnClick += new BasicEventCallback(ds.CustomAutoReturnCheckBoxCallback);
            ds.mControls.Add(ds.mCustomAutoReturnCheckBox);
            ds.mCustomDeflectionSlider = new SliderHor(value3 + new Vector2(70f, 109f), 339);
            ds.mCustomDeflectionSlider.OnScroll += new BasicEventCallback(ds.CustomDeflectionSliderScrollCallback);
            ds.mCustomDeflectionSlider.NumSteps = 46;
            ds.mCustomDeflectionSlider.Snap = true;
            ds.mControls.Add(ds.mCustomDeflectionSlider);
            ds.mCustomDeflectionSliderLabel = new Label(value3 + new Vector2(164f, 134f), 500f, WrapMode.WindowBound);
            ds.mCustomDeflectionSliderLabel.Text = "Deflection: 45 degrees";
            ds.mControls.Add(ds.mCustomDeflectionSliderLabel);
            ds.mCustomMaxExtensionSlider = new SliderHor(value3 + new Vector2(70f, 125f), 339);
            ds.mCustomMaxExtensionSlider.OnScroll += new BasicEventCallback(ds.CustomMaxExtensionSliderScrollCallback);
            ds.mCustomMaxExtensionSlider.NumSteps = 101;
            ds.mCustomMaxExtensionSlider.Snap = true;
            ds.mControls.Add(ds.mCustomMaxExtensionSlider);
            ds.mCustomMaxExtensionSliderLabel = new Label(value3 + new Vector2(202f, 150f), 500f, WrapMode.WindowBound);
            ds.mCustomMaxExtensionSliderLabel.Text = "Extension Limit:";
            ds.mControls.Add(ds.mCustomMaxExtensionSliderLabel);
            ds.mCustomRudderSensitivitySlider = new SliderHor(value3 + new Vector2(70f, 163f), 339);
            ds.mCustomRudderSensitivitySlider.OnScroll += new BasicEventCallback(ds.CustomRudderSensitivitySliderScrollCallback);
            ds.mCustomRudderSensitivitySlider.NumSteps = 101;
            ds.mCustomRudderSensitivitySlider.Snap = true;
            ds.mControls.Add(ds.mCustomRudderSensitivitySlider);
            ds.mCustomRudderSensitivitySliderLabel = new Label(value3 + new Vector2(164f, 188f), 500f, WrapMode.WindowBound);
            ds.mCustomRudderSensitivitySliderLabel.Text = "Sensitivity: 100%";
            ds.mControls.Add(ds.mCustomRudderSensitivitySliderLabel);
            ds.mCustomSteerersLinker = new ComponentLinker(value3 + new Vector2(300f, 122f));
            ds.mCustomSteerersLinker.Label = "Steering Thrusters";
            ds.mCustomSteerersLinker.OnStartAdd += new BasicEventCallback(ds.LinkerStartAddCallback);
            ds.mCustomSteerersLinker.OnDone += new BasicEventCallback(ds.LinkerDoneCallback);
            ds.mControls.Add(ds.mCustomSteerersLinker);
            ds.mCustomTurnSpeedSlider = new SliderHor(value3 + new Vector2(70f, 158f), 339);
            ds.mCustomTurnSpeedSlider.OnScroll += new BasicEventCallback(ds.CustomTurnSpeedSliderScrollCallback);
            ds.mCustomTurnSpeedSlider.NumSteps = 101;
            ds.mCustomTurnSpeedSlider.Snap = true;
            ds.mControls.Add(ds.mCustomTurnSpeedSlider);
            ds.mCustomTurnSpeedSliderLabel = new Label(value3 + new Vector2(164f, 183f), 500f, WrapMode.WindowBound);
            ds.mCustomTurnSpeedSliderLabel.Text = "Yaw Rate: 1 rot/sec";
            ds.mControls.Add(ds.mCustomTurnSpeedSliderLabel);
            ds.mCustomSensitivitySlider = new SliderHor(value3 + new Vector2(70f, 158f), 339);
            ds.mCustomSensitivitySlider.OnScroll += new BasicEventCallback(ds.CustomSensitivitySliderScrollCallback);
            ds.mCustomSensitivitySlider.NumSteps = 201;
            ds.mCustomSensitivitySlider.Snap = true;
            ds.mControls.Add(ds.mCustomSensitivitySlider);
            ds.mCustomSensitivitySliderLabel = new Label(value3 + new Vector2(164f, 183f), 500f, WrapMode.WindowBound);
            ds.mCustomSensitivitySliderLabel.Text = "Sensitivity: 4";
            ds.mControls.Add(ds.mCustomSensitivitySliderLabel);
            ds.mCustomGunsLinker = new ComponentLinker(value3 + new Vector2(300f, 147f));
            ds.mCustomGunsLinker.Label = "Controlled Guns";
            ds.mCustomGunsLinker.OnStartAdd += new BasicEventCallback(ds.LinkerStartAddCallback);
            ds.mCustomGunsLinker.OnDone += new BasicEventCallback(ds.LinkerDoneCallback);
            ds.mControls.Add(ds.mCustomGunsLinker);
            ds.mCustomSlavesLinker = new ComponentLinker(value3 + new Vector2(300f, 147f));
            ds.mCustomSlavesLinker.Label = "Slaved Turrets";
            ds.mCustomSlavesLinker.OnStartAdd += new BasicEventCallback(ds.LinkerStartAddCallback);
            ds.mCustomSlavesLinker.OnDone += new BasicEventCallback(ds.LinkerDoneCallback);
            ds.mControls.Add(ds.mCustomSlavesLinker);
            ds.mCustomProjectilePicker = new Selector(value3 + new Vector2(194f, 268f), 231);
            ds.mCustomProjectilePicker.OnChange += new BasicEventCallback(ds.CustomProjectilePickerCallback);
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry("None (Direct Aim)", AmmoType.None));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.AirBurst), AmmoType.AirBurst));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.Bullet38mm), AmmoType.Bullet38mm));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.Bullet41mm), AmmoType.Bullet41mm));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.SmallBurst), AmmoType.SmallBurst));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.Shell50mm), AmmoType.Shell50mm));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.Shell120mm), AmmoType.Shell120mm));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.ShellBig), AmmoType.ShellBig));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.Flak), AmmoType.Flak));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.RailShot), AmmoType.RailShot));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry("Packet", AmmoType.PacketCharge));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry("Beam", AmmoType.EnergyCharge));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.LightMissile), AmmoType.LightMissile));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.LRMissile), AmmoType.LRMissile));
            ds.mCustomProjectilePicker.AddSelectionEntry(new SelectionEntry(AmmoManager.GetAmmoDisplayName(AmmoType.Tether), AmmoType.Tether));
            ds.mControls.Add(ds.mCustomProjectilePicker);
            ds.mCustomProjectileTypeLabel = new Label(value3 + new Vector2(76f, 268f), 500f, WrapMode.WindowBound);
            ds.mCustomProjectileTypeLabel.Text = "Trajectory";
            ds.mControls.Add(ds.mCustomProjectileTypeLabel);
            ds.mCustomSightPicker = new Selector(value3 + new Vector2(224f, 176f), 200);
            ds.mCustomSightPicker.OnChange += new BasicEventCallback(ds.CustomSightPickerCallback);
            ds.mCustomSightPicker.AddSelectionEntry(new SelectionEntry("None", GunSightType.None));
            ds.mCustomSightPicker.AddSelectionEntry(new SelectionEntry("Dots", GunSightType.Dots));
            ds.mCustomSightPicker.AddSelectionEntry(new SelectionEntry("Gnats", GunSightType.Gnats));
            ds.mCustomSightPicker.AddSelectionEntry(new SelectionEntry("Diamonds", GunSightType.Diamonds));
            ds.mCustomSightPicker.AddSelectionEntry(new SelectionEntry("Circles", GunSightType.Circles));
            ds.mControls.Add(ds.mCustomSightPicker);
            ds.mCustomSightTypeLabel = new Label(value3 + new Vector2(105f, 176f), 500f, WrapMode.WindowBound);
            ds.mCustomSightTypeLabel.Text = "Sight Style";
            ds.mControls.Add(ds.mCustomSightTypeLabel);
            ds.mCustomInvertCheckbox = new CheckBox(value3 + new Vector2(294f, 200f));
            ds.mCustomInvertCheckbox.Label = "Invert";
            ds.mCustomInvertCheckbox.OnClick += new BasicEventCallback(ds.CustomMirrorCheckboxCallback);
            ds.mControls.Add(ds.mCustomInvertCheckbox);
            ds.mCustomInvertACheckbox = new CheckBox(value3 + new Vector2(294f, 180f));
            ds.mCustomInvertACheckbox.Label = "Invert A";
            ds.mCustomInvertACheckbox.OnClick += new BasicEventCallback(ds.CustomInvertACheckboxCallback);
            ds.mControls.Add(ds.mCustomInvertACheckbox);
            ds.mCustomInvertBCheckbox = new CheckBox(value3 + new Vector2(294f, 200f));
            ds.mCustomInvertBCheckbox.Label = "Invert B";
            ds.mCustomInvertBCheckbox.OnClick += new BasicEventCallback(ds.CustomInvertBCheckboxCallback);
            ds.mControls.Add(ds.mCustomInvertBCheckbox);
            ds.mCustomFiresACheckbox = new CheckBox(value3 + new Vector2(294f, 180f));
            ds.mCustomFiresACheckbox.Label = "Fires A";
            ds.mCustomFiresACheckbox.OnClick += new BasicEventCallback(ds.CustomFiresACheckboxCallback);
            ds.mControls.Add(ds.mCustomFiresACheckbox);
            ds.mCustomFiresBCheckbox = new CheckBox(value3 + new Vector2(294f, 200f));
            ds.mCustomFiresBCheckbox.Label = "Fires B";
            ds.mCustomFiresBCheckbox.OnClick += new BasicEventCallback(ds.CustomFiresBCheckboxCallback);
            ds.mControls.Add(ds.mCustomFiresBCheckbox);
            ds.mCustomDurationSlider = new SliderHor(value3 + new Vector2(70f, 89f), 339);
            ds.mCustomDurationSlider.OnScroll += new BasicEventCallback(ds.CustomDurationSliderScrollCallback);
            ds.mCustomDurationSlider.NumSteps = 201;
            ds.mCustomDurationSlider.Snap = true;
            ds.mControls.Add(ds.mCustomDurationSlider);
            ds.mCustomDurationSliderLabel = new Label(value3 + new Vector2(66f, 114f), 500f, WrapMode.WindowBound);
            ds.mCustomDurationSliderLabel.Text = "Duration: 1 seconds";
            ds.mControls.Add(ds.mCustomDurationSliderLabel);
            ds.mCustomProximitySlider = new SliderHor(value3 + new Vector2(70f, 229f), 339);
            ds.mCustomProximitySlider.OnScroll += new BasicEventCallback(ds.CustomProximitySliderScrollCallback);
            ds.mCustomProximitySlider.NumSteps = 951;
            ds.mCustomProximitySlider.Snap = true;
            ds.mControls.Add(ds.mCustomProximitySlider);
            ds.mCustomProximitySliderLabel = new Label(value3 + new Vector2(66f, 254f), 500f, WrapMode.WindowBound);
            ds.mCustomProximitySliderLabel.Text = "Proximity: 1 seconds";
            ds.mControls.Add(ds.mCustomProximitySliderLabel);
            ds.mControlledByLabel = new Label(new Vector2(value3.X - 13f, ds.mDimensions.Y - 115), 100f, WrapMode.Unbounded);
            ds.mControlledByLabel.Text = "Controlled by:";
            ds.mControlledByLabel.Hide = false;
            ds.mControls.Add(ds.mControlledByLabel);
            ds.mControlledByNameLabel = new SimpleTextButton(new Vector2(value3.X + 140f, ds.mDimensions.Y - 115), 100f, WrapMode.Unbounded, "colinsfont", Color.White, Color.Yellow);
            ds.mControlledByNameLabel.Text = "Stabilizer Module";
            ds.mControlledByNameLabel.Hide = false;
            ds.mControlledByNameLabel.OnClick += new BasicEventCallback(ds.OnControlledByClick);
            ds.mControls.Add(ds.mControlledByNameLabel);
            ds.mPickerLabel = new Label(new Vector2(value3.X - 13f, ds.mDimensions.Y - 115), 100f, WrapMode.Unbounded);
            ds.mPickerLabel.Text = "Controls";
            ds.mPickerLabel.Hide = true;
            ds.mControls.Add(ds.mPickerLabel);
            ds.mAPicker = new InputPicker(new Vector2(204f, ds.mDimensions.Y - 113), true);
            ds.mAPicker.InputMapping = new InputMapping("Function A");
            ds.mAPicker.OnDone += new BasicEventCallback(ds.InputPickerCallback);
            ds.mControls.Add(ds.mAPicker);
            ds.mBPicker = new InputPicker(new Vector2(204f, ds.mDimensions.Y - 77), true);
            ds.mBPicker.InputMapping = new InputMapping("Function B");
            ds.mBPicker.OnDone += new BasicEventCallback(ds.InputPickerCallback);
            ds.mControls.Add(ds.mBPicker);
            ds.mBudgetLabel = new Label(new Vector2(ds.mDimensions.X -  274, ds.mDimensions.Y - 111), 500f, WrapMode.WindowBound, "moirebold", new Color(255, 255, 255));
            ds.mControls.Add(ds.mBudgetLabel);
            Vector2 vector2 = new Vector2(306f, 74f);
            Label label = new Label(vector2, 100f, WrapMode.Unbounded);
            label.Text = "Convert to";
            ds.mControls.Add(label);
            SimpleTextButton simpleTextButton = new SimpleTextButton(vector2 + new Vector2(122f, 0f), 642f, WrapMode.Unbounded, "colinsfont", Color.White, Color.Yellow);
            simpleTextButton.OnClick += new BasicEventCallback(ds.WASDConfirmCallback);
            simpleTextButton.Text = "WASD";
            simpleTextButton.Underline = true;
            ds.mControls.Add(simpleTextButton);
            Label label2 = new Label(vector2 + new Vector2(185f, 0f), 100f, WrapMode.Unbounded);
            label2.Text = "or";
            ds.mControls.Add(label2);
            SimpleTextButton simpleTextButton2 = new SimpleTextButton(vector2 + new Vector2(210f, 0f), 642f, WrapMode.Unbounded, "colinsfont", Color.White, Color.Yellow);
            simpleTextButton2.OnClick += new BasicEventCallback(ds.ArrowsConfirmCallback);
            simpleTextButton2.Text = "Arrows";
            simpleTextButton2.Underline = true;
            ds.mControls.Add(simpleTextButton2);
            ds.mSnapCheckbox = new CheckBox(new Vector2(626f, 74f));
            ds.mSnapCheckbox.State = GimbalGameManager.ClientOptions.DesignerSnap;
            ds.mSnapCheckbox.Label = "Snap to Grid";
            ds.mSnapCheckbox.OnClick += new BasicEventCallback(ds.SnapOptionCallback);
            ds.mControls.Add(ds.mSnapCheckbox);
            ds.mMirrorCheckbox = new CheckBox(new Vector2(813f, 74f));
            ds.mMirrorCheckbox.State = GimbalGameManager.ClientOptions.DesignerMirror;
            ds.mMirrorCheckbox.Label = "Mirror Placement";
            ds.mMirrorCheckbox.OnClick += new BasicEventCallback(ds.MirrorOptionCallback);
            ds.mControls.Add(ds.mMirrorCheckbox);
            ds.mPartsBin = new PartsBin(new Vector2(12f, 70f));
            int rows = (int)Math.Floor((ds.mDimensions.Y - 584) / 84);
            FinishPartsBin(ds.mPartsBin, rows);
            ds.mPartsBin.OnSelectedEvent += new BasicEventCallback(ds.PartsBinSelectedCallback);
            ds.mPartsBin.OnUnselectedEvent += new BasicEventCallback(ds.UnselectedCallback);
            ds.mPartsBin.OnDragGrabEvent += new BasicEventCallback(ds.GrabCallback);
            ds.mPartsBin.MastermindMode = ds.mMastermindMode;
            ds.mPartsBin.RefreshCategories();
            ds.mPartsBin.RefreshListing();
            ds.mControls.Add(ds.mPartsBin);
            ds.mPartSelector = new Selector(new Vector2(ds.mDimensions.X - 308, ds.mDimensions.Y - 154), 292);
            ds.mPartSelector.OnChange += new BasicEventCallback(ds.PartSelectorCallback);
            ds.mControls.Add(ds.mPartSelector);
            ds.RefreshPartSelector();
            ds.mStatusLabel = new Label(new Vector2(515, ds.mDimensions.Y - 154), 500f, WrapMode.WindowBound);
            ds.mStatusLabel.Text = "";
            ds.mControls.Add(ds.mStatusLabel);
            ds.mUndoButton = new SimpleTextButton(ds.mGridCenter + new Vector2(ds.mDimensions.X - 308, ds.mDimensions.Y - 200), 642f, WrapMode.Unbounded, "colinsfont", Color.White, Color.Yellow);
            ds.mUndoButton.OnClick += new BasicEventCallback(ds.UndoButtonCallback);
            ds.mUndoButton.Text = "Undo";
            ds.mUndoButton.Underline = true;
            ds.mControls.Add(ds.mUndoButton);
            ds.RefreshVehicleNameAndDesc();
            ds.mSelectedGridComponent = null;
            ds.mInspectedGridComponent = null;
            ds.mSelectedMirroredGridComponent = null;
            ds.mViewportRenderTarget = new RenderTarget2D(GimbalGameManager.GraphicsDevice, (int)(ds.mDimensions.X - 500), (int)(ds.mDimensions.Y - 270), 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);

            ds.mWorld.Camera.ScreenCenter = new Vector2((int)(ds.mDimensions.X - 500) / 2, (int)(ds.mDimensions.Y - 270) / 2);

            ds.mLoadIsComplete = true;
        }

        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "GetWorldToScreenTransform", Skip = true)]
        public static Matrix GetWorldToScreenTransform(DesignerScreen ds)
        {
            Vector2 value = new Vector2(0f, 0f);
            Vector2 value2 = new Vector2(ds.mGridBounds.Width/2, ds.mGridBounds.Height / 2) - ds.mGridCenter - ds.mPosition + value;
            return ds.mWorld.Camera.GetCameraTransform(0f) * Matrix.CreateTranslation(new Vector3(-value2, 0f));
        }

        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "TransformFromScreenToWorldCoords", Skip = true)]
        public static Vector2 TransformFromScreenToWorldCoords(DesignerScreen ds, Vector2 screenPos)
        {
            Vector2 value = new Vector2(0.5f, 0.5f);
            Vector2 value2 = new Vector2(ds.mGridBounds.Width / 2, ds.mGridBounds.Height / 2) - ds.mGridCenter - ds.mPosition + value;
            return Vector2.Transform(screenPos + value2, ds.mWorld.Camera.GetInvertedCameraTransform(0f));
        }

        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "ScreenUpdate", Skip = true)]
        public static void ScreenUpdate(DesignerScreen ds, GameTime gameTime)
        {
            ds.mRemoveButton.GreyOut = true;
            ds.mCcwButton.GreyOut = true;
            ds.mCwButton.GreyOut = true;
            ds.ShowHideCustomLabels();
            Component component = null;
            ds.mGridBounds = new Rectangle(498, 110, (int)(ds.mDimensions.X - 500), (int)(ds.mDimensions.Y - 270));

            if (ds.mState == DesignerState.InspectingPartsBinPart)
            {
                ds.mNameLabel.Text = ds.mPartsBinData.DisplayName;
                ds.mPriceLabel.Text = "$" + ds.mPartsBinData.UnitPrice + "T";
                ds.mHitPointsLabel.Text = "Durability: " + ds.mPartsBinData.MaxHitPoints;
                ds.mMassLabel.Text = "Mass: " + ds.mPartsBinData.OriginalPhysics.Mass + " Tons";
                ds.mMomentLabel.Text = "Moment: " + (ds.mPartsBinData.OriginalPhysics.Moment / 4000f).ToString("0") + " m4";
                ds.mDragLabel.Text = "Drag: " + ds.mPartsBinData.ForwardAeroFactor;
                ds.mRCSLabel.Text = "Visibility: " + (ds.mPartsBinData.RadarCrossSection * 10f).ToString("0.0");
                ds.mThrustLabel.Text = "Thrust: " + ds.mPartsBinData.EditorThrust + " tons";
                ds.mTorqueLabel.Text = "Torque: " + ds.mPartsBinData.EditorTorque + " ft/lbs";
                ds.mPowerLabel.Text = "Power: " + ds.mPartsBinData.EditorPower + " kW";
                ds.mDamageLabel.Text = "Damage: " + ds.mPartsBinData.EditorDamage;
                ds.mROFLabel.Text = "Rate: " + ds.mPartsBinData.EditorROF.ToString("0.##") + " shots/sec";
                ds.mAmmoTypeLabel.Text = "Ammo: " + AmmoManager.GetAmmoDisplayName(ds.mPartsBinData.AmmoType);
                ds.mMagazineSizeLabel.Text = "Magazine: " + ds.mPartsBinData.MaxAmmo;
                ds.mDescLabel.Text = ds.mPartsBinData.Description;
                ds.TweakLabelsForRestrictedPart(ds.mPartsBinData);
            }
            else if (ds.mState == DesignerState.InspectingVehiclePart)
            {
                ds.mNameLabel.Text = ds.mInspectedGridComponent.StaticData.DisplayName;
                ds.mPriceLabel.Text = "$" + ds.mInspectedGridComponent.StaticData.UnitPrice + "T";
                ds.mHitPointsLabel.Text = "Durability: " + ds.mInspectedGridComponent.StaticData.MaxHitPoints;
                ds.mMassLabel.Text = "Mass: " + ds.mInspectedGridComponent.StaticData.OriginalPhysics.Mass + " Tons";
                ds.mMomentLabel.Text = "Moment: " + (ds.mInspectedGridComponent.StaticData.OriginalPhysics.Moment / 4000f).ToString("0") + " m4";
                ds.mDragLabel.Text = "Drag: " + ds.mInspectedGridComponent.StaticData.ForwardAeroFactor;
                ds.mRCSLabel.Text = "Visibility: " + (ds.mInspectedGridComponent.StaticData.RadarCrossSection * 10f).ToString("0.0");
                ds.mThrustLabel.Text = "Thrust: " + ds.mInspectedGridComponent.StaticData.EditorThrust + " tons";
                ds.mTorqueLabel.Text = "Torque: " + ds.mInspectedGridComponent.StaticData.EditorTorque + " ft/lbs";
                ds.mPowerLabel.Text = "Power: " + ds.mInspectedGridComponent.StaticData.EditorPower + " kW";
                ds.mDamageLabel.Text = "Damage: " + ds.mInspectedGridComponent.StaticData.EditorDamage;
                ds.mROFLabel.Text = "Rate: " + ds.mInspectedGridComponent.StaticData.EditorROF.ToString("0.##") + " shots/sec";
                ds.mAmmoTypeLabel.Text = "Ammo: " + AmmoManager.GetAmmoDisplayName(ds.mInspectedGridComponent.StaticData.AmmoType);
                ds.mMagazineSizeLabel.Text = "Magazine: " + ds.mInspectedGridComponent.StaticData.MaxAmmo;
                ds.mDescLabel.Text = ds.mInspectedGridComponent.StaticData.Description;
                if (ds.mInspectedGridComponent != null && (!ds.mInspectedGridComponent.IsCoreComponent || ds.mInspectedGridComponent.ChildAttachments.Count == 0))
                {
                    ds.mRemoveButton.GreyOut = false;
                    ds.mCcwButton.GreyOut = ds.mInspectedGridComponent.IsCoreComponent;
                    ds.mCwButton.GreyOut = ds.mInspectedGridComponent.IsCoreComponent;
                }
                component = ds.mInspectedGridComponent;
                ds.TweakLabelsForRestrictedPart(ds.mInspectedGridComponent.StaticData);
            }
            else if (ds.mState == DesignerState.DraggingPart)
            {
                ds.mNameLabel.Text = ds.mSelectedGridComponent.StaticData.DisplayName;
                ds.mPriceLabel.Text = "$" + ds.mSelectedGridComponent.StaticData.UnitPrice + "T";
                ds.mHitPointsLabel.Text = "Durability: " + ds.mSelectedGridComponent.StaticData.MaxHitPoints;
                ds.mMassLabel.Text = "Mass: " + ds.mSelectedGridComponent.StaticData.OriginalPhysics.Mass + " Tons";
                ds.mMomentLabel.Text = "Moment: " + (ds.mSelectedGridComponent.StaticData.OriginalPhysics.Moment / 4000f).ToString("0") + " m4";
                ds.mDragLabel.Text = "Drag: " + ds.mSelectedGridComponent.StaticData.ForwardAeroFactor;
                ds.mRCSLabel.Text = "Visibility: " + (ds.mSelectedGridComponent.StaticData.RadarCrossSection * 10f).ToString("0.0");
                ds.mThrustLabel.Text = "Thrust: " + ds.mSelectedGridComponent.StaticData.EditorThrust + " tons";
                ds.mTorqueLabel.Text = "Torque: " + ds.mSelectedGridComponent.StaticData.EditorTorque + " ft/lbs";
                ds.mPowerLabel.Text = "Power: " + ds.mSelectedGridComponent.StaticData.EditorPower + " kW";
                ds.mDamageLabel.Text = "Damage: " + ds.mSelectedGridComponent.StaticData.EditorDamage;
                ds.mROFLabel.Text = "Rate: " + ds.mSelectedGridComponent.StaticData.EditorROF.ToString("0.##") + " shots/sec";
                ds.mAmmoTypeLabel.Text = "Ammo: " + AmmoManager.GetAmmoDisplayName(ds.mSelectedGridComponent.StaticData.AmmoType);
                ds.mMagazineSizeLabel.Text = "Magazine: " + ds.mSelectedGridComponent.StaticData.MaxAmmo;
                ds.mDescLabel.Text = ds.mSelectedGridComponent.StaticData.Description;
                ds.mRemoveButton.GreyOut = false;
                ds.mCcwButton.GreyOut = false;
                ds.mCwButton.GreyOut = false;
                component = ds.mSelectedGridComponent;
                ds.TweakLabelsForRestrictedPart(ds.mSelectedGridComponent.StaticData);
            }
            else if (ds.mState == DesignerState.Chilling)
            {
                if (ds.mSelectedGridComponent == null)
                {
                    ds.mNameLabel.Text = ds.mVehicle.VehicleName;
                    ds.mPriceLabel.Text = "$" + ds.mVehicle.Price + "T";
                    ds.mMaxSpeedLabel.Text = "Top Speed: " + ((ds.mVehicle.MaxRecordedSpeed > 0f) ? ((ds.mVehicle.MaxRecordedSpeed * 0.6f).ToString("0") + " mph") : "Unknown");
                    ds.mMaxSpeedLabel.Hide = false;
                    if (ds.mVehicle.CoreComponent != null)
                    {
                        ds.mHitPointsLabel.Text = "Durability: " + ds.mVehicle.CoreComponent.StaticData.MaxHitPoints;
                        ds.mMassLabel.Text = "Mass: " + ds.mVehicle.CoreComponent.Physics.Mass + " Tons";
                        ds.mMomentLabel.Text = "Moment: " + (ds.mVehicle.CoreComponent.Physics.Moment / 4000f).ToString("0") + " m4";
                        ds.mDragLabel.Text = "Drag: " + ds.mVehicle.ForwardDrag.ToString("0");
                        ds.mRCSLabel.Text = "Visibility: " + (ds.mVehicle.CoreComponent.RadarCrossSection * 10f).ToString("0.0");
                        ds.TweakLabelsForRestrictedPart(ds.mVehicle.CoreComponent.StaticData);
                    }
                    else
                    {
                        ds.mHitPointsLabel.Text = "Durability: 0";
                        ds.mMassLabel.Text = "Mass: 0 Tons";
                        ds.mMomentLabel.Text = "Moment: 0 m4";
                        ds.mDragLabel.Text = "Drag: 0";
                        ds.mRCSLabel.Text = "Visibility: " + 0f.ToString("0.0");
                    }
                    if (ds.mVehicle.VehicleDescription != null)
                    {
                        ds.mDescLabel.Text = ds.mVehicle.VehicleDescription;
                    }
                    else
                    {
                        ds.mDescLabel.Text = "";
                    }
                }
                else
                {
                    ds.mNameLabel.Text = ds.mSelectedGridComponent.StaticData.DisplayName;
                    ds.mPriceLabel.Text = "$" + ds.mSelectedGridComponent.StaticData.UnitPrice + "T";
                    ds.mHitPointsLabel.Text = "Durability: " + ds.mSelectedGridComponent.StaticData.MaxHitPoints;
                    ds.mMassLabel.Text = "Mass: " + ds.mSelectedGridComponent.StaticData.OriginalPhysics.Mass + " Tons";
                    ds.mMomentLabel.Text = "Moment: " + (ds.mSelectedGridComponent.StaticData.OriginalPhysics.Moment / 4000f).ToString("0") + " m4";
                    ds.mDragLabel.Text = "Drag: " + ds.mSelectedGridComponent.StaticData.ForwardAeroFactor;
                    ds.mRCSLabel.Text = "Visibility: " + (ds.mSelectedGridComponent.StaticData.RadarCrossSection * 10f).ToString("0.0");
                    ds.mThrustLabel.Text = "Thrust: " + ds.mSelectedGridComponent.StaticData.EditorThrust + " tons";
                    ds.mTorqueLabel.Text = "Torque: " + ds.mSelectedGridComponent.StaticData.EditorTorque + " ft/lbs";
                    ds.mPowerLabel.Text = "Power: " + ds.mSelectedGridComponent.StaticData.EditorPower + " kW";
                    ds.mDamageLabel.Text = "Damage: " + ds.mSelectedGridComponent.StaticData.EditorDamage;
                    ds.mROFLabel.Text = "Rate: " + ds.mSelectedGridComponent.StaticData.EditorROF.ToString("0.##") + " shots/sec";
                    ds.mAmmoTypeLabel.Text = "Ammo: " + AmmoManager.GetAmmoDisplayName(ds.mSelectedGridComponent.StaticData.AmmoType);
                    ds.mMagazineSizeLabel.Text = "Magazine: " + ds.mSelectedGridComponent.StaticData.MaxAmmo;
                    ds.mDescLabel.Text = ds.mSelectedGridComponent.StaticData.Description;
                    component = ds.mSelectedGridComponent;
                    ds.TweakLabelsForRestrictedPart(ds.mSelectedGridComponent.StaticData);
                }
                if (ds.mSelectedGridComponent != null && (!ds.mSelectedGridComponent.IsCoreComponent || ds.mSelectedGridComponent.ChildAttachments.Count == 0))
                {
                    ds.mRemoveButton.GreyOut = false;
                    ds.mCcwButton.GreyOut = ds.mSelectedGridComponent.IsCoreComponent;
                    ds.mCwButton.GreyOut = ds.mSelectedGridComponent.IsCoreComponent;
                }
            }
            else if (ds.mState == DesignerState.PickingLink)
            {
                component = ds.mSelectedGridComponent;
            }
            if (!string.IsNullOrEmpty(ds.mStatusPumpedMessage))
            {
                ds.mStatusLabel.Text = ds.mStatusPumpedMessage;
            }
            else if (ds.mState == DesignerState.PickingLink)
            {
                if (ds.mProspectiveMountParent != null)
                {
                    ds.mStatusLabel.Text = "Add " + ds.mProspectiveMountParent.StaticData.DisplayName;
                }
                else
                {
                    ds.mStatusLabel.Text = "Click a part to add it";
                }
            }
            else if (ds.mState == DesignerState.DraggingPart)
            {
                if (ds.mProspectiveMountParent != null)
                {
                    ds.mStatusLabel.Text = "Mount to " + ds.mProspectiveMountParent.StaticData.DisplayName;
                }
                else if (ds.mProspectiveTouchParent != null)
                {
                    ds.mStatusLabel.Text = "Cannot mount here";
                }
                else
                {
                    ds.mStatusLabel.Text = "";
                }
            }
            else if (ds.mState == DesignerState.InspectingVehiclePart)
            {
                if (ds.mInspectedGridComponent != null && !ds.mInspectedGridComponent.IsCoreComponent)
                {
                    ds.mStatusLabel.Text = "Mounted to " + ds.mInspectedGridComponent.ParentAttachment.StaticData.DisplayName;
                }
                else
                {
                    ds.mStatusLabel.Text = "";
                }
            }
            else if (ds.mState == DesignerState.Chilling)
            {
                if (ds.mSelectedGridComponent != null && !ds.mSelectedGridComponent.IsCoreComponent)
                {
                    ds.mStatusLabel.Text = "Mounted to " + ds.mSelectedGridComponent.ParentAttachment.StaticData.DisplayName;
                }
                else
                {
                    ds.mStatusLabel.Text = "";
                }
            }
            ds.mStatusPumpedMessage = null;
            ds.mPickerLabel.Hide = true;
            ds.mAPicker.Hide = true;
            ds.mBPicker.Hide = true;
            ds.mControlledByLabel.Hide = true;
            ds.mControlledByNameLabel.Hide = true;
            ds.mControlledByComponent = null;
            if (component != null)
            {
                ds.mAPicker.InputMapping = component.InputMappingA;
                ds.mBPicker.InputMapping = component.InputMappingB;
                List<Component> list = component.FindControllingComponents();
                if (list.Count > 0)
                {
                    ds.mControlledByLabel.Hide = false;
                    ds.mControlledByNameLabel.Hide = false;
                    ds.mControlledByComponent = list[0];
                    ds.mControlledByNameLabel.Text = ds.mControlledByComponent.StaticData.DisplayName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(component.StaticData.DescFunctionA))
                    {
                        ds.mAPicker.InputMapping.Name = component.StaticData.DescFunctionA;
                        ds.mAPicker.Hide = false;
                        ds.mPickerLabel.Hide = false;
                    }
                    if (!string.IsNullOrEmpty(component.StaticData.DescFunctionB))
                    {
                        ds.mBPicker.InputMapping.Name = component.StaticData.DescFunctionB;
                        ds.mBPicker.Hide = false;
                        ds.mPickerLabel.Hide = false;
                    }
                }
            }
            if (ds.mState == DesignerState.Chilling && ds.mInspectedGridComponent == null && ds.mSelectedGridComponent == null)
            {
                ds.mNameLabel.Hide = true;
                ds.mDescLabel.Hide = true;
                ds.mEditableName.Hide = false;
                ds.mEditableDesc.Hide = false;
            }
            else
            {
                ds.mNameLabel.Hide = false;
                ds.mDescLabel.Hide = false;
                ds.mEditableName.Hide = true;
                ds.mEditableDesc.Hide = true;
            }
            ds.ShowHideCustomControls(component);
            ds.RefreshCustomControls(component);
            ds.mSaveButton.GreyOut = (!ds.mSaveIsDirty || ds.mVehicle.CoreComponent == null);
            ds.mZoomInButton.Hide = !ds.mMastermindMode;
            ds.mPanUpButton.Hide = !ds.mMastermindMode;
            ds.mPanDownButton.Hide = !ds.mMastermindMode;
            ds.mPanLeftButton.Hide = !ds.mMastermindMode;
            ds.mPanRightButton.Hide = !ds.mMastermindMode;
            ds.mPanResetButton.Hide = !ds.mMastermindMode;
            ds.mZoomInButton.Hide = !ds.mMastermindMode;
            ds.mZoomOutButton.Hide = !ds.mMastermindMode;
            ds.mMastermindModeButton.GreyOut = ds.mMastermindMode;
            ds.mMastermindModeButton.Hide = !GimbalGameManager.ClientOptions.MastermindStatus;
            ds.mUndoButton.Hide = (ds.mUndoBuffer.Count <= 0 || ds.mUndoCursor <= 0);
            if (ds.mVehicle != null)
            {
                ds.mSoftwareList = ds.mVehicle.SoftwareList;
            }
            else
            {
                ds.mSoftwareList = new List<Component>();
            }
            ds.mScreenDescLabel.Text = (ds.mMastermindMode ? "" : (ds.mScreenDescLabel.Text = "Drag and drop parts to build your design. Remember to assign controls and test fly your design."));
            ds.mBudgetLabel.Text = "Free Budget: $" + (ds.WorkingBudget - ds.mVehicle.Price) + "T";
            if (ds.mBudgetLabel.Text.Length > 19)
            {
                ds.mBudgetLabel.Text = "Free: $" + (ds.WorkingBudget - ds.mVehicle.Price) + "T";
            }
            if (ds.mVehicle.Price <= ds.WorkingBudget)
            {
                ds.mBudgetLabel.Color = Color.White;
            }
            else
            {
                ds.mBudgetLabel.Color = new Color(255, 45, 29);
            }
            Vector2 position = ds.mMouseWorldCoords + ds.mMouseGrabWorldOffset;
            Vector2 mouseScreenPos = Vector2.Transform(position, ds.GetWorldToScreenTransform());
            ds.mPartIsOnTrashIconOrPartsBin = (ds.mRemoveButton.Hovering || ds.mPartsBin.TestHovering(ds.mPosition, mouseScreenPos));
            if (ds.mDisableTesting || ds.mVehicle.Price > ds.WorkingBudget || ds.mVehicle.CoreComponent == null || ds.mVehicle.CoreComponent.RequiredRank() > GameLogicHandler.GetRank(GimbalGameManager.ClientOptions.Money))
            {
                ds.mFlightTestButton.GreyOut = true;
            }
            else
            {
                ds.mFlightTestButton.GreyOut = false;
            }
            for (int i = 0; i < ds.mSoftwareList.Count; i++)
            {
                BaseSoftwareComponent baseSoftwareComponent = ds.mSoftwareList[i] as BaseSoftwareComponent;
                if (baseSoftwareComponent != null)
                {
                    baseSoftwareComponent.DesignerWorldPositionOverride = ds.TransformFromScreenToWorldCoords(ds.PositionForSoftware(i) + new Vector2(16f, 16f));
                }
            }
            foreach (IGUIControl current in ds.mControls)
            {
                current.Update(gameTime);
            }
            ds.mWorld.Camera.Update(gameTime);
            if (ds.mVehicle.CoreComponent != null)
            {
                ds.mVehicle.CoreComponent.ClearCacheRecursive();
            }
            if (ds.mSelectedGridComponent != null)
            {
                ds.mSelectedGridComponent.ClearCacheRecursive();
            }
        }


        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "PositionForSoftware", Skip = true)]
        public static Vector2 PositionForSoftware(DesignerScreen ds, int i)
        {
            Vector2 value = new Vector2(ds.mGridBounds.X + 0, ds.mGridBounds.Y + 2);
            int num = 17;
            float num2 = 29f;
            int num3 = i / num;
            int num4 = i % num;
            return value + new Vector2((float)num4 * num2, (float)num3 * num2);
        }

        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "ScreenInput", Skip = true)]
        public static void ScreenInput(DesignerScreen ds, GameTime gameTime, KeyboardState keyboardState, ColinPadState colinPadState, MouseState mouseState)
        {
            if (ds.mFlagForExit)
            {
                GimbalGameManager.ScreenManager.RelinquishControl(ds, false);
                ds.FireLeaveEvent();
                return;
            }
            keyboardState = ds.mInputReset.UpdateAndFilterState(keyboardState);
            mouseState = ds.mInputReset.UpdateAndFilterState(mouseState);
            ds.mMouseWorldCoords = ds.TransformFromScreenToWorldCoords(new Vector2((float)mouseState.X, (float)mouseState.Y));
            bool flag = ds.ScreenPosInOnGrid(new Vector2((float)mouseState.X, (float)mouseState.Y));
            if (ds.mState == DesignerState.PickingLink)
            {
                ds.mCurrentLinker.PotentialLinkWorldPos = ds.mMouseWorldCoords + ds.mMouseGrabWorldOffset;
                Component component = ds.CheckSoftwareForMouseOver(mouseState);
                if (component != null && component != ds.mSelectedGridComponent)
                {
                    ds.mProspectiveMountParent = component;
                    ds.mCurrentLinker.PotentialLink = component;
                }
                else if (ds.mVehicle.CoreComponent.CheckSpotForLink(ds.mSelectedGridComponent, ds.mMouseWorldCoords, ds.mMastermindMode, out ds.mInspectedGridComponent))
                {
                    if (ds.mInspectedGridComponent != null && ds.mInspectedGridComponent.IsMissile)
                    {
                        ds.mInspectedGridComponent = ds.mInspectedGridComponent.ParentAttachment;
                    }
                    ds.mProspectiveMountParent = ds.mInspectedGridComponent;
                    ds.mCurrentLinker.PotentialLink = ds.mInspectedGridComponent;
                }
                else
                {
                    ds.mProspectiveMountParent = null;
                    ds.mCurrentLinker.PotentialLink = null;
                }
                if (!flag)
                {
                    ds.mInspectedGridComponent = null;
                }
                ds.mProspectiveTouchParent = null;
            }
            if (ds.mState != DesignerState.DraggingPart)
            {
                ds.mControls.Sort(new GUIControlComparer());
                if (ds.mControls[0].WantsToHogInput)
                {
                    ds.mControls[0].DoInput(gameTime, keyboardState, colinPadState, mouseState);
                    return;
                }
                using (List<IGUIControl>.Enumerator enumerator = ds.mControls.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IGUIControl current = enumerator.Current;
                        if (current.DoInput(gameTime, keyboardState, colinPadState, mouseState))
                        {
                            return;
                        }
                    }
                    goto IL_1F4;
                }
            }
            ds.mRemoveButton.DoInput(gameTime, keyboardState, colinPadState, mouseState);
        IL_1F4:
            if (ds.mLastKeyboardState.IsKeyUp(Keys.PageUp) && keyboardState.IsKeyDown(Keys.PageUp))
            {
                ds.mPartSelector.StepLeft();
            }
            if (ds.mLastKeyboardState.IsKeyUp(Keys.PageDown) && keyboardState.IsKeyDown(Keys.PageDown))
            {
                ds.mPartSelector.StepRight();
            }
            if (ds.mSelectedGridComponent != null && ds.mSelectedGridComponent.ParentAttachment != null)
            {
                float num = 0.5f;
                if (ds.mLastKeyboardState.IsKeyUp(Keys.Up) && keyboardState.IsKeyDown(Keys.Up))
                {
                    ds.MoveSelectedPart(new Vector2(0f, -num));
                }
                if (ds.mLastKeyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyDown(Keys.Left))
                {
                    ds.MoveSelectedPart(new Vector2(-num, 0f));
                }
                if (ds.mLastKeyboardState.IsKeyUp(Keys.Down) && keyboardState.IsKeyDown(Keys.Down))
                {
                    ds.MoveSelectedPart(new Vector2(0f, num));
                }
                if (ds.mLastKeyboardState.IsKeyUp(Keys.Right) && keyboardState.IsKeyDown(Keys.Right))
                {
                    ds.MoveSelectedPart(new Vector2(num, 0f));
                }
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                if (ds.mSaveIsDirty)
                {
                    ConfirmationDialog confirmationDialog = new ConfirmationDialog("You have unsaved changes.\nAre you sure you want to leave?", "Yes", "No");
                    confirmationDialog.OnAccept += new BasicEventCallback(ds.AcceptLeaveCallback);
                    GimbalGameManager.ScreenManager.PushScreen(confirmationDialog);
                    ds.HaltInputCallback(ds, new EventArgs());
                    return;
                }
                GimbalGameManager.ScreenManager.RelinquishControl(ds, false);
                ds.FireLeaveEvent();
            }
            if (ds.mState == DesignerState.DraggingPart)
            {
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    ds.ReleaseAndMountPart(ds.mMouseWorldCoords);
                }
                if (ds.mProspectiveFirstMountPossible)
                {
                    ds.mProspectiveFirstMountPossible = false;
                }
            }
            if((ds.mState == DesignerState.InspectingVehiclePart) && mouseState.LeftButton == ButtonState.Pressed && (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.LeftControl)))
            {
                ds.RecordUndoEntry();
                ds.mState = DesignerState.DraggingPart;
                ds.mSelectedGridComponentBeforeBinGrab = ds.mSelectedGridComponent;
                ds.mUndoDragSelectedIndex = ds.mPartSelector.SelectedIndex;

                Player player = new Player(ds.mWorld.TeamA);
                player.AssignPrimaryVehicle(new Vehicle("Copied Part", player, ClippingType.ActiveVehicle));
                

                Component c = ds.mSelectedGridComponent.DuplicateMeFromBluePrints(player.PrimaryVehicle, true);
                player.PrimaryVehicle.AssignCoreComponent(c);
                c.FixReferencesDownTree(ds.mSelectedGridComponent);

                player.PrimaryVehicle.CoreComponent.AssignDefaultControls();
                ds.mWorld.IntroduceNewPlayer(player, PlayerType.Insignificant);
                player.PrimaryVehicle.CoreComponent.Update(new GameTime());
                ds.mSelectedGridComponent = player.PrimaryVehicle.CoreComponent;

                ds.mMouseGrabWorldOffset = new Vector2(-3f, -3f);
                ds.mUndoDragParent = null;
                ds.mDragIsAPartsBinDrag = false;

            }
            if (ds.mState == DesignerState.Chilling || ds.mState == DesignerState.InspectingPartsBinPart || ds.mState == DesignerState.InspectingVehiclePart)
            {
                if (!ds.mCaughtDownClickOnComponent)
                {
                    Component component2 = ds.CheckSoftwareForMouseOver(mouseState);
                    if (component2 == null)
                    {
                        if (ds.mVehicle.CoreComponent != null)
                        {
                            ds.mVehicle.CoreComponent.CheckSpotForHit(ds.mMouseWorldCoords, ds.mSelectedGridComponent, out ds.mInspectedGridComponent);
                            if (!flag)
                            {
                                ds.mInspectedGridComponent = null;
                            }
                        }
                    }
                    else
                    {
                        ds.mInspectedGridComponent = component2;
                    }
                    if (ds.mInspectedGridComponent != null && ds.mInspectedGridComponent.IsMissile)
                    {
                        ds.mInspectedGridComponent = ds.mInspectedGridComponent.ParentAttachment;
                    }
                    if (ds.mInspectedGridComponent != null)
                    {
                        ds.mState = DesignerState.InspectingVehiclePart;
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            ds.mSelectedGridComponent = ds.mInspectedGridComponent;
                            ds.mSelectedMirroredGridComponent = null;
                            for (int i = 0; i < ds.mPartSelector.Entries.Count; i++)
                            {
                                SelectionEntry selectionEntry = ds.mPartSelector.Entries[i];
                                if (selectionEntry.Payload == ds.mSelectedGridComponent)
                                {
                                    ds.mPartSelector.Select(i);
                                    break;
                                }
                            }
                            ds.mMouseGrabScreenCoords = new Vector2((float)mouseState.X, (float)mouseState.Y);
                            ds.mMouseGrabWorldOffset = ds.mSelectedGridComponent.AbsolutePosition - ds.mMouseWorldCoords;
                            if (PartsBin.IsSoftware(ds.mSelectedGridComponent))
                            {
                                Vector2 screenPos = ds.PositionForSoftware(ds.GetSoftwareIndex(ds.mSelectedGridComponent)) + new Vector2(16f, 16f);
                                Vector2 value = ds.TransformFromScreenToWorldCoords(screenPos);
                                ds.mMouseGrabWorldOffset = value - ds.mMouseWorldCoords;
                            }
                            ds.mCaughtDownClickOnComponent = true;
                        }
                    }
                    else
                    {
                        if (ds.mState != DesignerState.InspectingPartsBinPart)
                        {
                            ds.mState = DesignerState.Chilling;
                        }
                        if (mouseState.LeftButton == ButtonState.Pressed && Utility.RectangleTest(ds.mGridBounds, new Vector2((float)mouseState.X, (float)mouseState.Y)))
                        {
                            ds.mPartSelector.Select(ds.mVehicle.CoreComponent);
                            ds.mSelectedGridComponent = null;
                            ds.mSelectedMirroredGridComponent = null;
                        }
                    }
                }
                else if (ds.mCaughtDownClickOnComponent && (ds.mMouseGrabScreenCoords - new Vector2((float)mouseState.X, (float)mouseState.Y)).Length() > 1f)
                {
                    ds.RecordUndoEntry();
                    ds.mState = DesignerState.DraggingPart;
                    ds.mSelectedGridComponentBeforeBinGrab = ds.mSelectedGridComponent;
                    ds.mUndoDragParent = ds.mSelectedGridComponent.ParentAttachment;
                    ds.mUndoDragPos = ds.mSelectedGridComponent.Physics.Position;
                    ds.mUndoDragSelectedIndex = ds.mPartSelector.SelectedIndex;
                    if (ds.mSelectedGridComponent.IsCoreComponent)
                    {
                        ds.mVehicle.ClearCoreComponent();
                        ds.mInspectedGridComponent = null;
                    }
                    else
                    {
                        ds.mSelectedGridComponent.ReallyDetach();
                    }
                    ds.mCaughtDownClickOnComponent = false;
                    ds.mDragIsAPartsBinDrag = false;
                }
            }
            if (ds.mState != DesignerState.DraggingPart && !ds.mCaughtDownClickOnComponent && !ds.mCaughtDownClickOnGrid && mouseState.LeftButton == ButtonState.Pressed)
            {
                ds.mCameraGrabScreenCoords = new Vector2((float)mouseState.X, (float)mouseState.Y);
                ds.mCaughtDownClickOnGrid = true;
            }
            if (ds.mCaughtDownClickOnComponent && mouseState.LeftButton == ButtonState.Released)
            {
                ds.mCaughtDownClickOnComponent = false;
            }
            if (ds.mCaughtDownClickOnGrid && mouseState.LeftButton == ButtonState.Released)
            {
                ds.mCaughtDownClickOnGrid = false;
            }
            if (ds.mState == DesignerState.DraggingPart)
            {
                ds.mSelectedGridComponent.Physics.Position = ds.mMouseWorldCoords + ds.mMouseGrabWorldOffset - Utility.Rotate(ds.mSelectedGridComponent.Physics.CoMAdjustment, ds.mSelectedGridComponent.AbsoluteAngle);
                if (ds.mSnapCheckbox.State)
                {
                    float snapSize = 16f / (float)Math.Pow(2.0, (double)ds.mZoomLevel);
                    ds.mSelectedGridComponent.Physics.Position = Utility.Snap(ds.mSelectedGridComponent.Physics.Position, snapSize, Utility.Rotate(-ds.mSelectedGridComponent.Physics.CoMAdjustment, ds.mSelectedGridComponent.Physics.Angle));
                }
                if (ds.mSelectedMirroredGridComponent != null)
                {
                    ds.mSelectedMirroredGridComponent.Physics.Position = ds.mSelectedGridComponent.Physics.Position * new Vector2(-1f, 1f);
                    ds.mSelectedMirroredGridComponent.ClearCacheRecursive();
                }
                ds.mSelectedGridComponent.ClearCacheRecursive();
                Vector2 vector = ds.mMouseWorldCoords + ds.mMouseGrabWorldOffset;
                ds.mProspectiveMountParent = null;
                ds.mProspectiveTouchParent = null;
                ds.mProspectiveMountParentMirrored = null;
                if (ds.mVehicle.CoreComponent == null && ds.WorldPosIsOnGrid(vector))
                {
                    if (PartsBin.IsFuselage(ds.mSelectedGridComponent))
                    {
                        ds.mProspectiveFirstMountPossible = true;
                    }
                    else
                    {
                        ds.mStatusPumpedMessage = "You must first choose a platform";
                    }
                }
                else if (ds.mVehicle.CoreComponent != null)
                {
                    string value2 = "";
                    string text = "";
                    if (PartsBin.IsFuselage(ds.mSelectedGridComponent))
                    {
                        if (ds.WorldPosIsOnGrid(vector))
                        {
                            if (ds.mVehicle.CoreComponent.ChildAttachments.Count == 0)
                            {
                                ds.mProspectiveFirstMountPossible = true;
                            }
                            else
                            {
                                ds.mStatusPumpedMessage = "Remove all parts before replacing your platform";
                            }
                        }
                    }
                    else if (PartsBin.IsSoftware(ds.mSelectedGridComponent))
                    {
                        ds.mVehicle.CoreComponent.CheckSpotForMountable(ds.mSelectedGridComponent, ds.mSelectedGridComponent.AbsolutePosition, null, ds.mMastermindMode, out ds.mProspectiveMountParent, out ds.mProspectiveTouchParent, out value2);
                        if (ds.mProspectiveMountParent == null && ds.WorldPosIsOnGrid(vector))
                        {
                            ds.mProspectiveMountParent = ds.mVehicle.CoreComponent;
                        }
                    }
                    else
                    {
                        Component component3 = null;
                        ds.mVehicle.CoreComponent.CheckSpotForMountable(ds.mSelectedGridComponent, ds.mSelectedGridComponent.AbsolutePosition, null, ds.mMastermindMode, out ds.mProspectiveMountParent, out ds.mProspectiveTouchParent, out value2);
                        if (ds.mSelectedMirroredGridComponent != null)
                        {
                            ds.mVehicle.CoreComponent.CheckSpotForMountable(ds.mSelectedMirroredGridComponent, ds.mSelectedMirroredGridComponent.AbsolutePosition, null, ds.mMastermindMode, out ds.mProspectiveMountParentMirrored, out component3, out value2);
                        }
                    }
                    if (ds.mProspectiveMountParent != null && (!ds.mSelectedGridComponent.EditorOkayToMount(ds.mProspectiveMountParent, ds.mMastermindMode, out value2) || !ds.WorldPosIsOnGrid(ds.mSelectedGridComponent.Physics.Position)))
                    {
                        ds.mProspectiveMountParent = null;
                    }
                    if (ds.mProspectiveMountParentMirrored != null && !ds.mSelectedMirroredGridComponent.EditorOkayToMount(ds.mProspectiveMountParentMirrored, ds.mMastermindMode, out text))
                    {
                        ds.mProspectiveMountParentMirrored = null;
                    }
                    if (ds.mProspectiveTouchParent != null && (!ds.mSelectedGridComponent.EditorOkayToMount(ds.mProspectiveTouchParent, ds.mMastermindMode, out value2) || !ds.WorldPosIsOnGrid(ds.mSelectedGridComponent.Physics.Position)))
                    {
                        ds.mProspectiveTouchParent = null;
                    }
                    if (!string.IsNullOrEmpty(value2))
                    {
                        ds.mStatusPumpedMessage = value2;
                    }
                    if (ds.mProspectiveMountParent != null && ds.mProspectiveMountParentMirrored != null)
                    {
                        Component component4 = null;
                        if (ds.mSelectedGridComponent.CheckSpotForHit(ds.mSelectedMirroredGridComponent.AbsolutePosition, out component4) || ds.mSelectedMirroredGridComponent.CheckSpotForHit(ds.mSelectedGridComponent.AbsolutePosition, out component4))
                        {
                            ds.mProspectiveMountParentMirrored = null;
                        }
                    }
                }
            }
            if ((ds.mState == DesignerState.InspectingVehiclePart || ds.mState == DesignerState.Chilling) && keyboardState.IsKeyDown(Keys.A))
            {
                if (ds.mCustomSteerersLinker != null && !ds.mCustomSteerersLinker.Hide)
                {
                    ds.mCustomSteerersLinker.StartPick();
                }
                if (ds.mCustomSlavesLinker != null && !ds.mCustomSlavesLinker.Hide)
                {
                    ds.mCustomSlavesLinker.StartPick();
                }
                if (ds.mCustomGunsLinker != null && !ds.mCustomGunsLinker.Hide)
                {
                    ds.mCustomGunsLinker.StartPick();
                }
                ds.HaltInputCallback(ds, new EventArgs());
            }
            if ((ds.mState == DesignerState.InspectingVehiclePart || ds.mState == DesignerState.Chilling) && !ds.mRemoveButton.GreyOut && !ds.mCaughtDownClickOnComponent && !ds.mCaughtDownClickOnGrid && keyboardState.IsKeyDown(Keys.Delete))
            {
                if (ds.mState == DesignerState.InspectingVehiclePart)
                {
                    ds.mSelectedGridComponent = ds.mInspectedGridComponent;
                    ds.mSelectedMirroredGridComponent = null;
                    ds.mInspectedGridComponent = null;
                    ds.RemoveButtonCallback(ds, new EventArgs());
                    ds.mState = DesignerState.Chilling;
                }
                else
                {
                    ds.RemoveButtonCallback(ds, new EventArgs());
                }
                ds.HaltInputCallback(ds, new EventArgs());
            }
            if ((ds.mState == DesignerState.InspectingVehiclePart || ds.mState == DesignerState.InspectingPartsBinPart || ds.mState == DesignerState.Chilling) && !ds.mCaughtDownClickOnComponent && !ds.mCaughtDownClickOnGrid && keyboardState.IsKeyDown(Keys.S) && (keyboardState.IsKeyDown(Keys.RightControl) || keyboardState.IsKeyDown(Keys.LeftControl)))
            {
                if (!ds.mSaveButton.GreyOut)
                {
                    ds.SaveButtonCallback(ds, new EventArgs());
                }
                ds.HaltInputCallback(ds, new EventArgs());
            }
            else if (!ds.mLastKeyboardState.IsKeyDown(Keys.S) && keyboardState.IsKeyDown(Keys.S))
            {
                ds.mSnapCheckbox.State = !ds.mSnapCheckbox.State;
            }
            if (keyboardState.IsKeyDown(Keys.N) && (keyboardState.IsKeyDown(Keys.RightControl) || keyboardState.IsKeyDown(Keys.LeftControl)))
            {
                ds.NewButtonCallback(ds, new EventArgs());
                ds.HaltInputCallback(ds, new EventArgs());
            }
            if (keyboardState.IsKeyDown(Keys.Z) && (keyboardState.IsKeyDown(Keys.RightControl) || keyboardState.IsKeyDown(Keys.LeftControl)))
            {
                ds.UndoButtonCallback(ds, new EventArgs());
                ds.HaltInputCallback(ds, new EventArgs());
            }
            if (keyboardState.IsKeyDown(Keys.Y) && (keyboardState.IsKeyDown(Keys.RightControl) || keyboardState.IsKeyDown(Keys.LeftControl)))
            {
                ds.RedoButtonCallback(ds, new EventArgs());
                ds.HaltInputCallback(ds, new EventArgs());
            }
            if (!ds.mLastKeyboardState.IsKeyDown(Keys.M) && keyboardState.IsKeyDown(Keys.M))
            {
                ds.mMirrorCheckbox.State = !ds.mMirrorCheckbox.State;
            }
            if (!ds.mLastKeyboardState.IsKeyDown(Keys.OemComma) && keyboardState.IsKeyDown(Keys.OemComma))
            {
                ds.RotateSelectedPart(false);
            }
            if (!ds.mLastKeyboardState.IsKeyDown(Keys.OemPeriod) && keyboardState.IsKeyDown(Keys.OemPeriod))
            {
                ds.RotateSelectedPart(true);
            }
            ds.mFineControl = (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift));
            ds.mCoarseControl = (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl));
            if (ds.mMastermindMode && ds.ScreenPosInOnGrid(new Vector2((float)mouseState.X, (float)mouseState.Y)))
            {
                if (mouseState.ScrollWheelValue > ds.mLastMouseState.ScrollWheelValue)
                {
                    ds.ZoomInButtonCallback(new object(), new EventArgs());
                }
                else if (mouseState.ScrollWheelValue < ds.mLastMouseState.ScrollWheelValue)
                {
                    ds.ZoomOutButtonCallback(new object(), new EventArgs());
                }
            }
            ds.mLastKeyboardState = keyboardState;
            ds.mLastMouseState = mouseState;
        }


        // Colin.Gimbal.DesignerScreen
        [Ultra.AutoHook("Colin.Gimbal.DesignerScreen", "ScreenDraw", Skip = true)]
        public static void ScreenDraw(DesignerScreen ds, GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice graphicsDevice = GimbalGameManager.GraphicsDevice;
            graphicsDevice.SetRenderTarget(0, ds.mViewportRenderTarget);
            graphicsDevice.Clear(new Color(0, 0, 0, 0));
            Vector3 position = new Vector3(-1f, -1f, 0f);
            Matrix transformMatrix = ds.mWorld.Camera.GetCameraTransform(0f) * Matrix.CreateTranslation(position);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, transformMatrix);
            ds.DrawShip(gameTime, spriteBatch);
            ds.DrawSelected(gameTime, spriteBatch);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(0, null);
            graphicsDevice.Clear(new Color(8, 14, 17));
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            spriteBatch.Draw(ds.mBinBackdropTexture, ds.mPosition + new Vector2(14f, 66f), new Rectangle(0, 0, 468, 32), Color.White);
            spriteBatch.Draw(ds.mBinBackdropTexture, new Rectangle(14, 98, 469, PBRows * 84 + 20), new Rectangle(0, 33, 468, 179), Color.White);
            spriteBatch.Draw(ds.mBinBackdropTexture, ds.mPosition + new Vector2(14f, 98f + 20 + 84 * PBRows), new Rectangle(0, 211, 468, 1), Color.White);
            spriteBatch.Draw(ds.mDescBackdropTexture, ds.mPosition + new Vector2(14f, ds.mDimensions.Y - 478), Color.White);
            //spriteBatch.Draw(ds.mGridBackdropTexture, ds.mPosition + new Vector2(496f, 98f), Color.White);
            spriteBatch.Draw(White, new Rectangle(ds.mGridBounds.Left - 2, ds.mGridBounds.Top - 2, ds.mGridBounds.Width + 4, 2), Color.Gray);
            spriteBatch.Draw(White, new Rectangle(ds.mGridBounds.Left - 2, ds.mGridBounds.Bottom + 1, ds.mGridBounds.Width + 4, 2), Color.Gray);
            spriteBatch.Draw(White, new Rectangle(ds.mGridBounds.Left - 2, ds.mGridBounds.Top - 2, 2, ds.mGridBounds.Height + 4), Color.Gray);
            spriteBatch.Draw(White, new Rectangle(ds.mGridBounds.Right + 1, ds.mGridBounds.Top - 2, 2, ds.mGridBounds.Height + 4), Color.Gray);
            spriteBatch.Draw(White, ds.mGridBounds, new Color(24, 32, 42, 131));
            if (!GimbalGameManager.ClientOptions.DebugMode)
            {
                spriteBatch.Draw(ds.mHeadingTexture, ds.mPosition + new Vector2(20f, 17f), Color.White);
            }
            if (GimbalGameManager.ClientOptions.DebugMode)
            {
                spriteBatch.DrawString(Utility.ColinsFont, ds.mState.ToString(), new Vector2(4f, 0f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                if (ds.mSelectedGridComponent != null)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "mSelectedGridComponent=" + ds.mSelectedGridComponent.ToString(), new Vector2(4f, 15f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (ds.mInspectedGridComponent != null)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "mInspectedGridComponent=" + ds.mInspectedGridComponent.ToString(), new Vector2(4f, 30f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (ds.mProspectiveMountParent != null)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "mProspectiveMountParent=" + ds.mProspectiveMountParent.ToString(), new Vector2(4f, 45f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (ds.mProspectiveTouchParent != null)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "mProspectiveTouchParent=" + ds.mProspectiveTouchParent.ToString(), new Vector2(4f, 60f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (ds.mVehicle.CoreComponent != null)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "Vehicle Bounds=" + ds.mVehicle.CoreComponent.BoundingRectangle.ToString(), new Vector2(4f, 75f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(Utility.ColinsFont, "Mouse Exceeds Bounds=" + ds.RectangleExceedsBounds(ds.mVehicle.CoreComponent.BoundingRectangle.PushOut(ds.mMouseWorldCoords)), new Vector2(4f, 90f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
            ds.mScreenDescLabel.Hide = GimbalGameManager.ClientOptions.DebugMode;
            foreach (IGUIControl current in ds.mControls)
            {
                if (!(current is ComponentLinker))
                {
                    current.Draw(gameTime, spriteBatch, ds.mPosition);
                }
            }
            if (ds.mMastermindMode)
            {
                spriteBatch.Draw(ds.mMastermindLogo, ds.mPosition + new Vector2(517f, -2f), Color.White);
            }
            if (ds.mVehicle.CoreComponent != null)
            {
                Vector2 value = new Vector2(0f, 0f);
                Vector2 pos = Vector2.Transform(-ds.mVehicle.CoreComponent.Physics.CoMAdjustment, ds.GetWorldToScreenTransform()) + value;
                pos = Utility.Bound(ds.mGridBounds, pos);
                Vector2 origin = new Vector2(0f, 4f);
                float num = 252f;
                Color color = new Color(180, 255, 255, 210);
                Vector2 vector = Utility.Snap(new Vector2(pos.X, ds.mGridBounds.Top + 4));
                Vector2 vector2 = Utility.Snap(new Vector2(ds.mGridBounds.Left + 4, pos.Y));
                Vector2 vector3 = Utility.Snap(new Vector2(ds.mGridBounds.Right - 4, pos.Y));
                Utility.Snap(new Vector2(pos.X, ds.mGridCenter.Y + ds.mPosition.Y + num));
                spriteBatch.Draw(ds.mCGTexture, vector2, null, color, 0f, origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ds.mCGTexture, vector3, null, color, 3.14159274f, origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ds.mCGTexture, vector, null, color, 1.57079637f, origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ds.mCGTextTexture, vector2 + new Vector2(-1f, 5f), color);
                spriteBatch.Draw(ds.mCGTextTexture, vector3 + new Vector2(-12f, 4f), color);
                spriteBatch.Draw(ds.mCGTextTexture, vector + new Vector2(4f, -1f), color);
            }
            else
            {
                string text = "  To get started, drag your\nchosen platform into this area";
                spriteBatch.DrawString(ds.mFont, text, Utility.CenterText(ds.mFont, ds.mGridCenter + ds.mPosition + new Vector2(0f, -40f), text, 1f), Color.White);
            }
            Texture2D texture = ds.mViewportRenderTarget.GetTexture();
            spriteBatch.Draw(texture, ds.mGridCenter + ds.mPosition - new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2)), Color.White);
            ds.mPanUpButton.Draw(gameTime, spriteBatch, ds.mPosition);
            ds.mPanDownButton.Draw(gameTime, spriteBatch, ds.mPosition);
            ds.mPanLeftButton.Draw(gameTime, spriteBatch, ds.mPosition);
            ds.mPanRightButton.Draw(gameTime, spriteBatch, ds.mPosition);
            ds.mPanResetButton.Draw(gameTime, spriteBatch, ds.mPosition);
            ds.mZoomInButton.Draw(gameTime, spriteBatch, ds.mPosition);
            ds.mZoomOutButton.Draw(gameTime, spriteBatch, ds.mPosition);
            if (ds.mState == DesignerState.DraggingPart)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, ds.GetWorldToScreenTransform());
                ds.DrawSelected(gameTime, spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            }
            for (int i = 0; i < ds.mSoftwareList.Count; i++)
            {
                Component component = ds.mSoftwareList[i];
                if (component != ds.mSelectedGridComponent || ds.mState != DesignerState.DraggingPart)
                {
                    if ((component == ds.mSelectedGridComponent && (ds.mInspectedGridComponent == null || ds.mState == DesignerState.PickingLink)) || component == ds.mInspectedGridComponent)
                    {
                        spriteBatch.Draw(ds.mSoftwareStandInTexture, ds.PositionForSoftware(i), Color.White);
                        spriteBatch.Draw(ds.mSoftwareStandInSelectedTexture, ds.PositionForSoftware(i), Color.White);
                    }
                    else if (component == ds.mSelectedGridComponent)
                    {
                        spriteBatch.Draw(ds.mSoftwareStandInTexture, ds.PositionForSoftware(i), Color.White);
                        spriteBatch.Draw(ds.mSoftwareStandInSelectedTexture, ds.PositionForSoftware(i), new Color(255, 255, 255, 130));
                    }
                    else
                    {
                        spriteBatch.Draw(ds.mSoftwareStandInTexture, ds.PositionForSoftware(i), Color.White);
                    }
                    if (component.StaticData.SoftwareTextureName != null)
                    {
                        spriteBatch.Draw(Utility.LoadTexture(component.StaticData.SoftwareTextureName), new Rectangle((int)ds.PositionForSoftware(i).X + 8, (int)ds.PositionForSoftware(i).Y + 4, 17, 24), Color.White);
                    }
                }
            }
            if (ds.mState == DesignerState.DraggingPart && PartsBin.IsSoftware(ds.mSelectedGridComponent))
            {
                Vector2 value2 = Vector2.Transform(ds.mSelectedGridComponent.AbsolutePosition, ds.GetWorldToScreenTransform());
                if (ds.mProspectiveMountParent != null || ds.mPartIsOnTrashIconOrPartsBin)
                {
                    spriteBatch.Draw(ds.mSoftwareStandInTexture, value2 + new Vector2(-16f, -16f), new Color(190, 240, 255, 150));
                }
                else
                {
                    spriteBatch.Draw(ds.mSoftwareStandInTexture, value2 + new Vector2(-16f, -16f), new Color(255, 50, 0, 127));
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            if (ds.mState != DesignerState.DraggingPart && ds.mState != DesignerState.PickingLink)
            {
                for (int j = 0; j < ds.mSoftwareList.Count; j++)
                {
                    Component component2 = ds.mSoftwareList[j];
                    if (component2 == ds.mInspectedGridComponent || (component2 == ds.mSelectedGridComponent && ds.mInspectedGridComponent == null))
                    {
                        Vector2 vector4 = Vector2.Transform(component2.AbsolutePosition, ds.GetWorldToScreenTransform());
                        Vector2 vector5 = ds.PositionForSoftware(j) + new Vector2(27f, 6f);
                        Vector2 vector6 = ds.PositionForSoftware(j) + new Vector2(10f, 27f);
                        vector4 = Utility.Bound(ds.mGridBounds, vector4);
                        vector5 = Utility.Bound(ds.mGridBounds, vector5);
                        vector6 = Utility.Bound(ds.mGridBounds, vector6);
                        ds.mVirtualPositionLine.Draw(spriteBatch, vector4, vector5, 0.5f, new Color(255, 255, 255, 100));
                        ds.mVirtualPositionLine.Draw(spriteBatch, vector4, vector6, 0.5f, new Color(255, 255, 255, 100));
                        spriteBatch.Draw(ds.mSoftwareStandInTexture, vector4, null, Color.White, 0f, new Vector2(16f, 16f), 0.5f, SpriteEffects.None, 0f);
                    }
                }
            }
            Color color2 = new Color(3, 255, 110);
            Color color3 = new Color(3, 255, 110);
            if (ds.mState == DesignerState.DraggingPart)
            {
                if (ds.mProspectiveMountParent == null && !ds.mProspectiveFirstMountPossible && !ds.mPartIsOnTrashIconOrPartsBin)
                {
                    color2 = new Color(255, 60, 0, 110);
                }
                if (ds.mProspectiveMountParentMirrored == null)
                {
                    color3 = new Color(255, 60, 0, 110);
                }
            }
            if (ds.mSelectedGridComponent != null)
            {
                Vector2 vector7 = Vector2.Transform(ds.mSelectedGridComponent.AbsolutePosition, ds.GetWorldToScreenTransform());
                vector7 = Utility.Snap(vector7);
                if (ds.mState != DesignerState.DraggingPart)
                {
                    vector7 = Utility.Bound(ds.mGridBounds, vector7);
                }
                spriteBatch.Draw(ds.mSelectedOverlayPosTexture, vector7, null, color2, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0f);
                if (!(ds.mSelectedGridComponent is BaseSoftwareComponent))
                {
                    spriteBatch.Draw(ds.mSelectedOverlayDirTexture, vector7, null, color2, ds.mSelectedGridComponent.AbsoluteAngle, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0f);
                }
                if (ds.IsMirrorDragRightNow && ds.mState == DesignerState.DraggingPart)
                {
                    vector7 = Vector2.Transform(ds.mSelectedGridComponent.AbsolutePosition * new Vector2(-1f, 1f), ds.GetWorldToScreenTransform());
                    if (ds.mState != DesignerState.DraggingPart)
                    {
                        vector7 = Utility.Bound(ds.mGridBounds, vector7);
                    }
                    spriteBatch.Draw(ds.mSelectedOverlayPosTexture, vector7, null, color3, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0f);
                }
            }
            if (GimbalGameManager.ClientOptions.DebugMode)
            {
                if (ds.mState == DesignerState.InspectingVehiclePart)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "Component ID:" + ds.mInspectedGridComponent.ComponentID.ToString(), new Vector2(400f, 0f), Color.White);
                }
                else if (ds.mSelectedGridComponent != null)
                {
                    spriteBatch.DrawString(Utility.ColinsFont, "Component ID:" + ds.mSelectedGridComponent.ComponentID.ToString(), new Vector2(400f, 0f), Color.White);
                }
            }
            foreach (IGUIControl current2 in ds.mControls)
            {
                if (current2 is ComponentLinker)
                {
                    current2.Draw(gameTime, spriteBatch, ds.mPosition);
                }
            }
            spriteBatch.End();
        }

        // Colin.Gimbal.PartsBin
        [Ultra.AutoHook("Colin.Gimbal.PartsBin", ".ctor", Skip = true)]
        public static void PartsBinConstructor(PartsBin pb, Vector2 pos)
        {
            pb.mInputReset = new InputReset();
            pb.mIsFirstFrame = true;
            pb.mPosition = Utility.Snap(pos);

        }

        public static int PBRows = 0;

        public static void FinishPartsBin(PartsBin pb, int rows)
        {
            PBRows = rows;
            pb.mDimensions = new Vector2(469f, 39 + rows * 84f);
            pb.mScrollBar = new ScrollBarVert(new Vector2(452f, 104f), rows * 84);
            pb.mCategorySelector = new Selector(new Vector2(34f, 74f), 180);
            pb.mPlayerRank = GameLogicHandler.GetRank(GimbalGameManager.ClientOptions.Money);
            pb.mPartTabletTexture = Utility.LoadTexture("designer_bin_tablet");
            pb.mPartTabletSelectedTexture = Utility.LoadTexture("designer_bin_tablet_selected");
            pb.mStarsTexture = Utility.LoadTexture("stars_scoreboard");
            pb.mBetaWarningTexture = Utility.LoadTexture("beta_warning");
            pb.MastermindMode = false;
            foreach (PartsCategories partsCategories in Enum.GetValues(typeof(PartsCategories)))
            {
                pb.mCurrentCategory = partsCategories;
                pb.RefreshListing();
            }
            pb.RefreshCategories();
            pb.mCategorySelector.OnChange += new BasicEventCallback(pb.CategorySelectorCallback);
            pb.mCategorySelector.Select(0);
            pb.mTestBox = new StretchyBox("white_box");
            pb.mTestBox.LoadContent();
        }

        // Colin.Gimbal.PartsBin
        [Ultra.AutoHook("Colin.Gimbal.PartsBin", "Draw", Skip = true)]
        public static void Draw(PartsBin pb, GameTime gameTime, SpriteBatch spriteBatch, Vector2 upperLeftCorner)
        {
            pb.mOrigin = upperLeftCorner + pb.mPosition;
            if (pb.Hide)
            {
                return;
            }
            pb.mScrollBar.Draw(gameTime, spriteBatch, upperLeftCorner);
            pb.mCategorySelector.Draw(gameTime, spriteBatch, upperLeftCorner);
            for (int i = pb.WindowStart; i < pb.WindowEnd; i++)
            {
                Type arg_4F_0 = pb.mCurrentPartsList[i];
                bool flag = pb.mCurrentPartsStaticData[i].RequiredRank > pb.mPlayerRank;
                if (flag)
                {
                    Color color = new Color(128, 128, 128, 128);
                    spriteBatch.Draw(pb.mPartTabletTexture, pb.mPosition + pb.PositionForTablet(i), null, color);
                }
                else
                {
                    spriteBatch.Draw(pb.mPartTabletTexture, pb.mPosition + pb.PositionForTablet(i), null, Color.White);
                }
                Vector2 origin = -pb.mCurrentPartsTextureBounds[i].CenterPosition;
                float scale = PartsBin.CalcScaleForThumbnail(pb.mCurrentPartsTextureBounds[i]);
                if (flag)
                {
                    Color color2 = new Color(255, 255, 255, 150);
                    spriteBatch.Draw(pb.mCurrentPartsGreyTextures[i], pb.mPosition + pb.PositionForTablet(i) + new Vector2(36f, 36f), null, color2, 0f, origin, scale, SpriteEffects.None, 0f);
                    Vector2 value = new Vector2(1f, 2f);
                    Color color3 = new Color(255, 255, 255, 120);
                    spriteBatch.Draw(Player.GetRankStarTexture(pb.mCurrentPartsStaticData[i].RequiredRank), pb.mPosition + pb.PositionForTablet(i) + value, null, color3, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(pb.mCurrentPartsTextures[i], pb.mPosition + pb.PositionForTablet(i) + new Vector2(36f, 36f), null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
                    if (pb.mCurrentPartsStaticData[i].SoftwareTextureName != null)
                    {
                        spriteBatch.Draw(Utility.LoadTexture(pb.mCurrentPartsStaticData[i].SoftwareTextureName), pb.mPosition + pb.PositionForTablet(i) + new Vector2(36f, 36f), null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
                if (pb.mHaveSelection && i == pb.mSelectedIndex)
                {
                    spriteBatch.Draw(pb.mPartTabletSelectedTexture, pb.mPosition + pb.PositionForTablet(i) - new Vector2(9f, 9f), Color.White);
                }
                if (pb.mCurrentPartsStaticData[i].IsBetaOnly)
                {
                    spriteBatch.Draw(pb.mBetaWarningTexture, pb.mPosition + pb.mPosition + pb.PositionForTablet(i) + new Vector2(-5f, -34f), Color.White);
                }
            }
        }

        [Ultra.AutoHook("Colin.Gimbal.PartsBin", "get_WindowEnd", Skip = true)]
        public static int PBWindowEnd(PartsBin pb)
        {
            int num = 5 * (pb.mRowWindowStart + PBRows - 1) + 5;
            if (num > pb.mCurrentPartsList.Count)
            {
                num = pb.mCurrentPartsList.Count;
            }
            return num;
        }

        // Colin.Gimbal.PartsBin
        [Ultra.AutoHook("Colin.Gimbal.PartsBin", "Update", Skip = true)]
        public static void Update(PartsBin pb, GameTime gameTime)
        {
            pb.mScrollBar.Update(gameTime);
            pb.mCategorySelector.Update(gameTime);
            pb.mScrollBar.NumSteps = (pb.mCurrentPartsList.Count - 1) / 5 - (PBRows-2);
            pb.mScrollBar.StepsPerPage = PBRows;
            pb.mScrollBar.GreyOut = (pb.mScrollBar.NumSteps <= 1);
            pb.mRowWindowStart = (int)((pb.mScrollBar.Progress - 0.001f) * (float)pb.mScrollBar.NumSteps);
        }
    }
}
