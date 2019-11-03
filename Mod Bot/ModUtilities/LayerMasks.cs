using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable IDE1006 // Naming Styles

namespace ModLibrary
{
    /// <summary>
    /// Defines constants that represent the different layer masks defined in the unity editor
    /// </summary>
    public static class LayerMasks
    {
        /// <summary>
        /// Builtin Unity layer, default layer if none is defined
        /// </summary>
        public const int Default = 0;
        /// <summary>
        /// Builtin Unity layer, makes effects such as lens flares shine through the object its applied to
        /// </summary>
        public const int TransparentFX = 1;
        /// <summary>
        /// Builtin Unity layer, raycasts will ignore any objects wuth this layer
        /// </summary>
        public const int IgnoreRaycast = 2;
        /// <summary>
        /// Builtin Unity layer, unused in-game
        /// </summary>
        public const int Water = 4;
        /// <summary>
        /// Builtin Unity layer, default layer for all UI
        /// </summary>
        public const int UI = 5;
        /// <summary>
        /// Layer for the starting area and upgrade room
        /// </summary>
        public const int UnderArea = 8;
        /// <summary>
        /// Layer for the logo on the main menu
        /// </summary>
        public const int CloneDroneLogo = 9;
        /// <summary>
        /// Layer for the Centauri Station visible in chapter 2 of story mode
        /// </summary>
        public const int CentauriStation = 10;
        /// <summary>
        /// Used to ignore the raycast for twitch spawns
        /// </summary>
        public const int IgnoreDuringTwitchSpawnRaycast = 11;
        /// <summary>
        /// Unknown
        /// </summary>
        public const int ToolCamera = 12;
        /// <summary>
        /// Layer for all environmentel hazards, i.e. sawblades, spike traps, fire jets, lava, etc.
        /// </summary>
        public const int EnvironmentalHazards = 13;
        /// <summary>
        /// Unknown
        /// </summary>
        public const int CutsceneCameraSet = 14;
        /// <summary>
        /// Layer for the network like structure in the ending cutscene of chapter 3 of story mode
        /// </summary>
        public const int BluescreenNetwork = 15;
        /// <summary>
        /// Layer for all walkable surfaces, including level objects
        /// </summary>
        public const int Environment = 16;
        /// <summary>
        /// Layer for all characters
        /// </summary>
        public const int CharacterRoot = 17;
        /// <summary>
        /// Layer for all severed body parts
        /// </summary>
        public const int BodyPart = 18;
        /// <summary>
        /// Layer for all projectiles, i.e. arrows, grenade shrapnel and flame breath projectiles
        /// </summary>
        public const int Projectile = 19;
        /// <summary>
        /// Layer for all objects the camera should collide with
        /// </summary>
        public const int CameraCollider = 20;
        /// <summary>
        /// Layer for all environment rigid bodies
        /// </summary>
        public const int EnvironmentRigidBodies = 21;
        /// <summary>
        /// Layer for characters to push objects
        /// </summary>
        public const int CharacterObjectPusher = 22;
        /// <summary>
        /// Layer for objects that will only collide with body parts
        /// </summary>
        public const int BodyPartOnlyCollider = 23;
        /// <summary>
        /// Layer for the final area marker in Last Bot Standing
        /// </summary>
        public const int FinalAreaObelisk = 24;
        /// <summary>
        /// Layer for objects that will only collide with characters
        /// </summary>
        public const int CharacterOnlyCollider = 25;
        /// <summary>
        /// Layer for objects that will only collide with the environment
        /// </summary>
        public const int EnvironmentOnlyCollider = 26;

        /// <summary>
        /// Combines all the given layers into one layer mask
        /// </summary>
        /// <param name="layers">The layers to apply to the final mask</param>
        /// <returns>The layermask that will have all the collision layers of <paramref name="layers"/></returns>
        public static int Combine(params int[] layers)
        {
            int finalLayer = 0;

            foreach (int layer in layers)
            {
                finalLayer |= 1 << layer;
            }

            return finalLayer;
        }
    }
}
