﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EpilepsyPatch.patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EpilepsyPatch.tools;
using static EpilepsyPatch.patches.RadarBoosterPatch;

namespace EpilepsyPatch
{

    [BepInPlugin(modGUID, modName, modVersion)]

    public class EpilepsyPatchBase : BaseUnityPlugin
    {
        private const string modGUID = "LongParsnip.EpilepsyPatch";
        private const string modName = "EpilepsyPatch";
        private const string modVersion = "1.0.22.0";
        public const bool LogDebugMessages = false;                     //This is for helping with developing the transpiler code, to find the correct IL to modify.

        private readonly Harmony harmony = new Harmony(modGUID);
        private static EpilepsyPatchBase Instance;

        private ManualLogSource mls;

        //Config setting keys.
        public static string StunGrenadeExplosionDisabledKey = "Disable stun grenade explosion";
        public static string StunGrenadeFilterDisabledKey = "Disable stun grenade flashed effect";
        public static string ScanBlueFlashDisabledKey = "Disable scan blue flash effect";
        public static string GettingFiredLightDisabledKey = "Getting fired red flashing light disabled";
        public static string DisableGlobalNotificationsKey = "Disable global notifications";
        public static string DisableRadiationWarningKey = "Disable radiation warning";
        public static string ReplaceWarningWithHintKey = "Replace warning with hint";
        public static string ForceShipLightsOnKey = "Force ship lights on";
        public static string HideLightningStrikesKey = "Hide lightning strikes";
        public static string HideLightningExplosionsKey = "Hide lightning explosions";
        public static string DisableFearScreenFilterKey = "Disable fear screen filter";
        public static string DisableBeeZapsKey = "Disable bee zaps";
        public static string DisableBeamUpParticlesKey = "Disable beam up particles";
        public static string PreventFlashlightSpamKey = "Prevent flashlight spam";
        public static string FlashlightSpamCooldownKey = "Flashlight spam cooldown";
        public static string DisablePlayerMonitorBlinkKey = "Disable player monitor blink";
        public static string DisableStartRoomFanKey = "Disable the fan in the start room";
        public static string HideTerminalCaretKey = "Hide terminal caret";
        public static string DisableChargerAnimationKey = "Disable charger animation";
        public static string TryToHideTurretBulletsKey = "Try to hide turret bullets";
        public static string DisableRadarBoosterAnimationKey = "Disable radar booster animation";
        public static string DisableRadarBoosterFlashKey = "Disable radar booster flash";
        public static string DisableLandminesKey = "Disable landmines";
        public static string DisableFogKey = "Disable fog";
        public static string DisableVolumetricFogKey = "Disable volumetric fog";
        public static string DisableFogMovementKey = "Disable Fog Movement";
        public static string DisableFoggyModifierKey = "Disable foggy modifier";
        public static string DisableFPVHelmetKey = "Disable FPV helmet";
        public static string DisableFPVHelmetGlassKey = "Disable helmet glass";
        public static string DisableCriticalHealthMessageKey = "Disable critical health message";
        public static string DisableCustomShaderKey = "Disable custom pass shader";
        public static string DisableMiscReflectionsKey = "Disable misc reflections";
        public static string TryToStopTooltipFlashKey = "Try to stop tooltips flashing";
        public static string DisableTZPinhalentFilterKey = "Disble TZP filter";


        //Config Entries.
        public static ConfigEntry<bool> StunGrenadeExplosionDisabled;
        public static ConfigEntry<bool> StunGrenadeFilterDisabled;
        public static ConfigEntry<bool> ScanBlueFlashDisabled;
        public static ConfigEntry<bool> GettingFiredLightDisabled;
        public static ConfigEntry<bool> DisableGlobalNotifications;
        public static ConfigEntry<bool> DisableRadiationWarning;
        public static ConfigEntry<bool> ReplaceWarningWithHint;
        public static ConfigEntry<bool> ForceShipLightsOn;
        public static ConfigEntry<bool> HideLightningStrikes;
        public static ConfigEntry<bool> HideLightningExplosions;
        public static ConfigEntry<bool> DisableFearScreenFilter;
        public static ConfigEntry<bool> DisableBeeZaps;
        public static ConfigEntry<bool> DisableBeamUpParticles;
        public static ConfigEntry<bool> PreventFlashlightSpam;
        public static ConfigEntry<float> FlashlightSpamCooldown;
        public static ConfigEntry<bool> DisablePlayerMonitorBlink;
        public static ConfigEntry<bool> DisableStartRoomFan;
        public static ConfigEntry<bool> HideTerminalCaret;
        public static ConfigEntry<bool> DisableChargerAnimation;
        public static ConfigEntry<bool> TryToHideTurretBullets;
        public static ConfigEntry<bool> DisableRadarBoosterAnimation;
        public static ConfigEntry<bool> DisableRadarBoosterFlash;
        public static ConfigEntry<bool> DisableLandmines;
        public static ConfigEntry<bool> DisableFog;
        public static ConfigEntry<bool> DisableFPVHelmet;
        public static ConfigEntry<bool> DisableFPVHelmetGlass;
        public static ConfigEntry<bool> DisableVolumetricFog;
        public static ConfigEntry<bool> DisableCriticalHealthMessage;
        public static ConfigEntry<bool> DisableFogMovement;
        public static ConfigEntry<bool> DisableCustomShader;
        public static ConfigEntry<bool> DisableMiscReflections;
        public static ConfigEntry<bool> TryToStopTooltipFlash;
        public static ConfigEntry<bool> DisableFoggyModifier;
        public static ConfigEntry<bool> DisableTZPinhalentFilter;


        void Awake()
        {

            //Config bindings.
            StunGrenadeExplosionDisabled = (Config.Bind<bool>("General", StunGrenadeExplosionDisabledKey, true, new ConfigDescription("Should the stun grenade explosion animation be disabled.")));
            StunGrenadeFilterDisabled = (Config.Bind<bool>("General", StunGrenadeFilterDisabledKey, true, new ConfigDescription("Should the stun grenade stunned filter be disabled.")));
            ScanBlueFlashDisabled = (Config.Bind<bool>("General", ScanBlueFlashDisabledKey, true, new ConfigDescription("Should the blue flash when scanning be disabled.")));
            GettingFiredLightDisabled = (Config.Bind<bool>("General", GettingFiredLightDisabledKey, true, new ConfigDescription("Should the red light when getting fired be disabled")));
            DisableGlobalNotifications = (Config.Bind<bool>("General", DisableGlobalNotificationsKey, false, new ConfigDescription("Should global notifications be disabled")));
            DisableRadiationWarning = (Config.Bind<bool>("General", DisableRadiationWarningKey, true, new ConfigDescription("Should the radiation warning be disabled")));
            ReplaceWarningWithHint = (Config.Bind<bool>("General", ReplaceWarningWithHintKey, true, new ConfigDescription("Should warnings be replaced with hints")));
            ForceShipLightsOn = (Config.Bind<bool>("General", ForceShipLightsOnKey, true, new ConfigDescription("Should ship lights always be forced to be on")));
            HideLightningStrikes = (Config.Bind<bool>("General", HideLightningStrikesKey, true, new ConfigDescription("Should lightning strikes be hidden")));
            HideLightningExplosions = (Config.Bind<bool>("General", HideLightningExplosionsKey, true, new ConfigDescription("Should explosions from lightning strikes be hidden")));
            DisableFearScreenFilter = (Config.Bind<bool>("General", DisableFearScreenFilterKey, true, new ConfigDescription("Should the fear effect screen filter be hidden")));
            DisableBeeZaps = (Config.Bind<bool>("General", DisableBeeZapsKey, true, new ConfigDescription("Should the bee zap effect be hidden")));
            DisableBeamUpParticles = (Config.Bind<bool>("General", DisableBeamUpParticlesKey, true, new ConfigDescription("Should the particle effect when being beamed up be hidden")));
            PreventFlashlightSpam = (Config.Bind<bool>("General", PreventFlashlightSpamKey, true, new ConfigDescription("Prevent the flashlight from being spammed on and off")));
            FlashlightSpamCooldown = Config.Bind("General", FlashlightSpamCooldownKey, 2.0f, new ConfigDescription("Time in seconds for network players flashlight to be on cooldown for, this is to prevent spamming. Note: this may cause the state to not be synchronised.", new AcceptableValueRange<float>(0.1f, 5.0f)));
            DisablePlayerMonitorBlink = (Config.Bind<bool>("General", DisablePlayerMonitorBlinkKey, true, new ConfigDescription("Should the screen blink when switching players on the monitor animation be removed")));
            DisableStartRoomFan = (Config.Bind<bool>("General", DisableStartRoomFanKey, true, new ConfigDescription("Should the fan in the enterance of the facility be stopped from spinning")));
            HideTerminalCaret = (Config.Bind<bool>("General", HideTerminalCaretKey, true, new ConfigDescription("Should the blinking caret on the terminal be hidden")));
            DisableChargerAnimation = (Config.Bind<bool>("General", DisableChargerAnimationKey, true, new ConfigDescription("Should the Charger sparks animation be hidden")));
            TryToHideTurretBullets = (Config.Bind<bool>("General", TryToHideTurretBulletsKey, true, new ConfigDescription("Attemps to hide the bullets from the turret, hit detection may be affected")));
            DisableRadarBoosterAnimation = (Config.Bind<bool>("General", DisableRadarBoosterAnimationKey, true, new ConfigDescription("Should the spinning animation on the radar booster be hidden")));
            DisableRadarBoosterFlash = (Config.Bind<bool>("General", DisableRadarBoosterFlashKey, true, new ConfigDescription("Prevents the radar booster from flashing, unfortunately this means it wont work... sorry.")));
            DisableLandmines = (Config.Bind<bool>("General", DisableLandminesKey, false, new ConfigDescription("Stops landmines exploding... also means they wont kill you.")));
            DisableFog = (Config.Bind<bool>("General", DisableFogKey, true, new ConfigDescription("Disables all fog in the rendering gameobject.")));
            DisableVolumetricFog = (Config.Bind<bool>("General", DisableVolumetricFogKey, true, new ConfigDescription("Disables volumetric fog (3D fog) only")));
            DisableFogMovement = (Config.Bind<bool>("General", DisableFogMovementKey, true, new ConfigDescription("Stops the movement of the outdoors volumetric fog which can cause strobing near light sources. Note for this to work the other fog settings need to be set to false")));
            DisableFPVHelmet = (Config.Bind<bool>("General", DisableFPVHelmetKey, true, new ConfigDescription("Disables the first person helmet 3D model, in the rendering gamebobject, because light can reflect off the glass and flash.")));
            DisableFPVHelmetGlass = (Config.Bind<bool>("General", DisableFPVHelmetGlassKey, true, new ConfigDescription("Disables only the glass on the FPV helmet")));
            DisableCriticalHealthMessage = (Config.Bind<bool>("General", DisableCriticalHealthMessageKey, true, new ConfigDescription("Disables the warning message when health is critical")));
            DisableCustomShader = (Config.Bind<bool>("Rendering", DisableCustomShaderKey, true, new ConfigDescription("Disables the custom shader that does the outlining and produces the harsh colour gradients")));
            DisableMiscReflections = (Config.Bind<bool>("Rendering", DisableMiscReflectionsKey, true, new ConfigDescription("Disables misc reflections that can cause flickering")));
            TryToStopTooltipFlash = (Config.Bind<bool>("Rendering", TryToStopTooltipFlashKey, true, new ConfigDescription("Attempts to prevent the animator from making the tooltip messages from flashing")));
            DisableFoggyModifier = (Config.Bind<bool>("Rendering", DisableFoggyModifierKey, true, new ConfigDescription("Disables the foggy weather modifier, incase the fog is too intense for some players")));
            DisableTZPinhalentFilter = (Config.Bind<bool>("Rendering", DisableTZPinhalentFilterKey, true, new ConfigDescription("Disables the drunk effect when taking TZP inhalent")));


            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("EpilepsyPatch is awake and needs coffee.");

            harmony.PatchAll(typeof(StunGrenadePatch));
            harmony.PatchAll(typeof(StunGrenadeExplosionPatch));
            harmony.PatchAll(typeof(PingScan_performedPatch));
            harmony.PatchAll(typeof(DisplayGlobalNotification_Patch));
            harmony.PatchAll(typeof(RadiationWarningHUD_Patch));
            harmony.PatchAll(typeof(DisplayTip_Patch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            //harmony.PatchAll(typeof(ShipTeleporterPatch));        //Example of how to use a transpiler on a coroutine.
            harmony.PatchAll(typeof(LightningPatch));
            //harmony.PatchAll(typeof(ExplosionPatch));             //didn't work for lightning, might be useful for landmines though, needs more testing.
            harmony.PatchAll(typeof(insanityFilterPatch));
            harmony.PatchAll(typeof(BeeZapPatch));
            //harmony.PatchAll(typeof(FlashlightSpamPatch));        //Failed testing for preventing flashlight spam.
            //harmony.PatchAll(typeof(SwitchFlashlightPatch));      //Failed testing for preventing flashlight spam.
            //harmony.PatchAll(typeof(FlashlightSpamPatch));        //Failed testing for preventing flashlight spam.
            harmony.PatchAll(typeof(SwitchFlashlightSpamPatch));
            harmony.PatchAll(typeof(ManualCameraRendererPatch));
            harmony.PatchAll(typeof(Tools_ListAllGameObjects));
            harmony.PatchAll(typeof(StopEntryRoomFan));
            //harmony.PatchAll(typeof(HideTheSunDontPraiseIt));       //Yeah i cant get this one to work.
            harmony.PatchAll(typeof(TerminalPatch));
            harmony.PatchAll(typeof(ItemChargerPatch));
            harmony.PatchAll(typeof(StopTurretAnimator));
            harmony.PatchAll(typeof(StopLandmine));               //Not working 100%, just stops it from exploding.
            harmony.PatchAll(typeof(RadarBoosterPatch));
            harmony.PatchAll(typeof(RadarBoosterPatch2));           //This stops the flash from working.... not ideal.
            harmony.PatchAll(typeof(ToolTipFlashPatch));

        }


        public static bool IsRandomRangeCall(CodeInstruction instruction)
        {
            // Check if the instruction is a call to UnityEngine.Random.Range
            return instruction.opcode == OpCodes.Call &&
                   instruction.operand is MethodInfo methodInfo &&
                   methodInfo.DeclaringType == typeof(UnityEngine.Random) &&
                   methodInfo.Name == "Range";
        }


        public static void LogDebuggingMessages(CodeInstruction instruction, int i)
        {

            // Log each opcode
            UnityEngine.Debug.Log($"Opcode at index {i}: {instruction.opcode}");


            // Log field info.
            if (instruction.operand is FieldInfo fieldInfo2)
            {
                UnityEngine.Debug.Log($"Field Name: {fieldInfo2.Name}, Declaring Type: {fieldInfo2.DeclaringType}");
            }

            // Log random range call.
            if (IsRandomRangeCall(instruction))
            {
                UnityEngine.Debug.Log($"Found the call to UnityEngine.Random.Range at index {i}");
            }

        }

    }
}
