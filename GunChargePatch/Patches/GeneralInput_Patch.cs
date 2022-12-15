using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Photon.Pun;
using UnityEngine;
using HarmonyLib;

namespace GunChargePatch.Patches
{
    [HarmonyPatch(typeof(GeneralInput))]
    class GeneralInput_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(900)]
        [HarmonyPatch("Update")]
        private static void AimCorrection(GeneralInput __instance, CharacterData ___data)
        {
            if (!___data)
            {
                return;
            }

            if (!___data.view.IsMine)
            {
                return;
            }

            if (!___data.weaponHandler)
            {
                return;
            }

            if (!___data.weaponHandler.gun)
            {
                return;
            }

            if (!___data.weaponHandler.gun.useCharge)
            {
                return;
            }

            Gun gun = ___data.weaponHandler.gun;

            if (___data.playerActions.Device == null)
            {
                __instance.aimDirection = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition) - __instance.transform.position;
                __instance.aimDirection.z = 0f;
                __instance.aimDirection.Normalize();
                if (Optionshandler.lockMouse)
                {
                    try
                    {
                        __instance.aimDirection = (Vector3)typeof(GeneralInput).InvokeMethod("MakeEightDirections", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, __instance, new object[] { __instance.aimDirection });
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log("Error during lock mouse.");
                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
            else
            {
                __instance.aimDirection.x = __instance.aimDirection.x + ___data.playerActions.Aim.X;
                __instance.aimDirection.y = __instance.aimDirection.y + ___data.playerActions.Aim.Y;
                if (Optionshandler.lockStick)
                {
                    try
                    {
                        __instance.aimDirection = (Vector3)typeof(GeneralInput).InvokeMethod("MakeEightDirections", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, __instance, new object[] { __instance.aimDirection });
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log("Error during lock stick.");
                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
            try
            {
                if (__instance.aimDirection != Vector3.zero)
                {
                    __instance.aimDirection += Vector3.up * 0.13f / Mathf.Clamp(gun.projectileSpeed * gun.currentCharge * gun.chargeSpeedTo, 1f, 100f);
                }
                if (__instance.aimDirection != Vector3.zero)
                {
                    __instance.lastAimDirection = __instance.aimDirection;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Error elsewhere.");
                UnityEngine.Debug.LogException(e);
            }
        }

        [HarmonyPostfix]
        [HarmonyPriority(Priority.LowerThanNormal)]
        [HarmonyPatch("Update")]
        private static void AimCorrectionIn0G(GeneralInput __instance, CharacterData ___data)
        {
            if (!___data)
            {
                return;
            }

            if (!___data.view.IsMine)
            {
                return;
            }

            if (!___data.weaponHandler)
            {
                return;
            }

            if (!___data.weaponHandler.gun)
            {
                return;
            }

            if (!___data.weaponHandler.gun.useCharge)
            {
                return;
            }

            Gun gun = ___data.weaponHandler.gun;

            if (!gun.useCharge || gun.gravity > 0f)
            {
                return;
            }

            if (___data.playerActions.Device == null)
            {
                __instance.aimDirection = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition) - __instance.transform.position;
                __instance.aimDirection.z = 0f;
                __instance.aimDirection.Normalize();
                if (Optionshandler.lockMouse)
                {
                    __instance.aimDirection = (Vector3)typeof(GeneralInput).InvokeMethod("MakeEightDirections", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, __instance, new object[] { __instance.aimDirection });
                }
            }
            else
            {
                __instance.aimDirection.x = __instance.aimDirection.x + ___data.playerActions.Aim.X;
                __instance.aimDirection.y = __instance.aimDirection.y + ___data.playerActions.Aim.Y;
                if (Optionshandler.lockStick)
                {
                    __instance.aimDirection = (Vector3)typeof(GeneralInput).InvokeMethod("MakeEightDirections", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, __instance, new object[] { __instance.aimDirection });
                }
            }
            if (__instance.aimDirection != Vector3.zero)
            {
                __instance.lastAimDirection = __instance.aimDirection;
            }
        }

        //[HarmonyPatch("Update")]
        //      [HarmonyTranspiler]
        //      static IEnumerable<CodeInstruction> PassCharge(IEnumerable<CodeInstruction> instructions)
        //      {
        //          var codes = new List<CodeInstruction>(instructions);
        //          var multiplyByCharge = new List<CodeInstruction>();
        //          var passCharge = new List<CodeInstruction>();
        //          //var resetCharge = new List<CodeInstruction>();

        //          for (var i = 0; i < codes.Count; i++)
        //          {
        //              UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
        //          }

        //          var data = AccessTools.Field(typeof(GeneralInput), "data");
        //          var weaponHandler = AccessTools.Field(typeof(CharacterData), nameof(CharacterData.weaponHandler));
        //          var gun = AccessTools.Field(typeof(WeaponHandler), nameof(WeaponHandler.gun));
        //          var currentCharge = AccessTools.Field(typeof(Gun), nameof(Gun.currentCharge));
        //          var projectileSpeed = AccessTools.Field(typeof(Gun), nameof(Gun.projectileSpeed));
        //          var chargeSpeedTo = AccessTools.Field(typeof(Gun), nameof(Gun.chargeSpeedTo));
        //          var dup = new CodeInstruction(OpCodes.Dup);
        //          var convf = new CodeInstruction(OpCodes.Ldind_R4);
        //          var arg = new CodeInstruction(OpCodes.Ldarg_0);

        //          List<CodeInstruction> calculation = new List<CodeInstruction>();

        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, projectileSpeed));
        //          //calculation.Add(convf);

        //          //calculation.Add(arg);
        //          calculation.Add(arg);
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, data));
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, weaponHandler));
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, gun));
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, currentCharge));
        //          //calculation.Add(convf);
        //          calculation.Add(new CodeInstruction(OpCodes.Mul));

        //          calculation.Add(arg);
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, data));
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, weaponHandler));
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, gun));
        //          calculation.Add(new CodeInstruction(OpCodes.Ldfld, chargeSpeedTo));
        //          //calculation.Add(convf);
        //          calculation.Add(new CodeInstruction(OpCodes.Mul));

        //          //calculation.Add(dup);
        //          //calculation.Add(new CodeInstruction(OpCodes.Ldfld, currentCharge));
        //          //calculation.Add(convf);
        //          //calculation.Add(new CodeInstruction(OpCodes.Ldfld, chargeSpeedTo));
        //          //calculation.Add(convf);
        //          //calculation.Add(new CodeInstruction(OpCodes.Mul));
        //          //calculation.Add(new CodeInstruction(OpCodes.Mul));

        //          //calculation.Add(dup);
        //          //calculation.Add(dup);
        //          //calculation.Add(new CodeInstruction(OpCodes.Ldfld, projectileSpeed));
        //          //calculation.Add(new CodeInstruction(OpCodes.Ldfld, currentCharge));
        //          //calculation.Add(new CodeInstruction(OpCodes.Ldfld, chargeSpeedTo));
        //          //calculation.Add(new CodeInstruction(OpCodes.Mul));
        //          //calculation.Add(new CodeInstruction(OpCodes.Mul));

        //          List<int> indices= new List<int>();

        //          for (int i = 0; i < codes.Count; i++)
        //          {
        //              if (codes[i].opcode == OpCodes.Ldfld)
        //              {
        //                  if ((FieldInfo)codes[i].operand == projectileSpeed)
        //                  {
        //                      indices.Add(i);
        //                      UnityEngine.Debug.Log($"Code found on line {i}");
        //                  }
        //              }
        //          }

        //          for (int i = (indices.Count() - 1); i >= 0; i--)
        //          {
        //              codes.RemoveAt(indices[i]);
        //              codes.InsertRange(indices[i], calculation);
        //          }

        //          for (var i = 0; i < codes.Count; i++)
        //          {
        //              UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
        //          }

        //          //passCharge.Add(new CodeInstruction(OpCodes.Dup));
        //          //passCharge.Add(new CodeInstruction(OpCodes.Ldfld, currentCharge));
        //          //passCharge.Add(new CodeInstruction(OpCodes.Conv_R4));

        //          //resetCharge.Add(new CodeInstruction(OpCodes.Ldarg_0));
        //          //resetCharge.Add(new CodeInstruction(OpCodes.Ldfld, gun));
        //          //resetCharge.Add(new CodeInstruction(OpCodes.Ldc_R4, 0.0f));
        //          //resetCharge.Add(new CodeInstruction(OpCodes.Stfld, currentCharge));

        //          //codes.InsertRange(117, resetCharge);
        //          //codes.RemoveAt(111);
        //          //codes.InsertRange(111, passCharge);

        //          //UnityEngine.Debug.Log("New code:");

        //          //for (var i = 0; i < codes.Count; i++)
        //          //{
        //          //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
        //          //}

        //          return codes.AsEnumerable();
        //      }
    }
}