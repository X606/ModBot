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
    /// Defines constants that represent the different layers defined in the unity editor
    /// </summary>
    public static class Layers
    {
        /// <summary>
        /// Builtin Unity layer, default layer if none is defined
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ✔ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int Default = 0;

        /// <summary>
        /// Builtin Unity layer, used for the Voxel Particle System when cutting body parts
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ✔ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int TransparentFX = 1;

        /// <summary>
        /// Builtin Unity layer, raycasts will ignore any objects with this layer
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ✔ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int IgnoreRaycast = 2;

        /// <summary>
        /// Unassigned
        /// </summary>
        /// <remarks>
        /// Collides with every layer
        /// </remarks>
        public const int Unassigned_3 = 3;

        /// <summary>
        /// Builtin Unity layer, unused in-game
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ✔ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int Water = 4;

        /// <summary>
        /// Builtin Unity layer, default layer for all UI
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ✔ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int UI = 5;

        /// <summary>
        /// Unassigned
        /// </summary>
        /// <remarks>
        /// Collides with every layer
        /// </remarks>
        public const int Unassigned_6 = 6;

        /// <summary>
        /// Unassigned
        /// </summary>
        /// <remarks>
        /// Collides with every layer
        /// </remarks>
        public const int Unassigned_7 = 7;

        /// <summary>
        /// Layer for all objects under the arena, upgrade room, garbage shute, start area, etc
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ✔ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ✔ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int UnderArea = 8;

        /// <summary>
        /// Layer for the game logo on the title screen
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CloneDroneLogo = 9;

        /// <summary>
        /// Layer for the space station in chapter 2
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CentauriStation = 10;

        /// <summary>
        /// An object with this layer will be ignored when finding the spawn location of a twitch spawn (Raycast starts above the arena, and goes down until it finds an object without this layer), mostly used for triggers
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ✔ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int IgnoreDuringTwitchSpawnRaycast = 11;

        /// <summary>
        /// Used for the level editor tool handles (position, rotation, scale)
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int ToolCamera = 12;

        /// <summary>
        /// Layer used for sawblades
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int EnvironmentalHazards = 13;

        /// <summary>
        /// Used for various cutscenes, like the ending of chapter 1, and the fleet overseer model on the screens in chapter 3
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CutsceneCameraSet = 14;

        /// <summary>
        /// Used on all cutscene objects in the chapter 3 ending
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int BluescreenNetwork = 15;

        /// <summary>
        /// Used for nearly every object characters and projectiles collide with
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ✔ <see cref="CameraCollider"/><br/>
        /// ✔ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ✔ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int Environment = 16;

        /// <summary>
        /// Layer used on the root object for all <see cref="Character"/>s
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ✔ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CharacterRoot = 17;

        /// <summary>
        /// Layer for all <see cref="BaseBodyPart"/> components
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ✔ <see cref="EnvironmentRigidBodies"/><br/>
        /// ✔ <see cref="CharacterObjectPusher"/><br/>
        /// ✔ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int BodyPart = 18;

        /// <summary>
        /// Layer for all projectiles
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ✔ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ✔ <see cref="BodyPartHitBox"/><br/>
        /// ✔ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int Projectile = 19;

        /// <summary>
        /// Layer used on all <see cref="Camera"/>s attached to a <see cref="Character"/>
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CameraCollider = 20;

        /// <summary>
        /// Same as <see cref="Environment"/>, but for <see cref="Rigidbody"/> objects
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ✔ <see cref="EnvironmentRigidBodies"/><br/>
        /// ✔ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int EnvironmentRigidBodies = 21;

        /// <summary>
        /// Used on an invisible collider on characters to push garbage and other <see cref="Rigidbody"/> objects
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ✔ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CharacterObjectPusher = 22;

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int BodyPartOnlyCollider = 23;

        /// <summary>
        /// Used for the Last Bot Standing final area pillar
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ✔ <see cref="Default"/><br/>
        /// ✔ <see cref="TransparentFX"/><br/>
        /// ✔ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ✔ <see cref="Water"/><br/>
        /// ✔ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ✔ <see cref="CloneDroneLogo"/><br/>
        /// ✔ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ✔ <see cref="ToolCamera"/><br/>
        /// ✔ <see cref="EnvironmentalHazards"/><br/>
        /// ✔ <see cref="CutsceneCameraSet"/><br/>
        /// ✔ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ✔ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ✔ <see cref="CameraCollider"/><br/>
        /// ✔ <see cref="EnvironmentRigidBodies"/><br/>
        /// ✔ <see cref="CharacterObjectPusher"/><br/>
        /// ✔ <see cref="BodyPartOnlyCollider"/><br/>
        /// ✔ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int FinalAreaObelisk = 24;

        /// <summary>
        /// Used for the InvisibleCharacterCollider level object
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ✔ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ❌ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int CharacterOnlyCollider = 25;

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ✔ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ✔ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ❌ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ❌ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int EnvironmentOnlyCollider = 26;

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ❌ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ❌ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int BodyPartHitBox = 27;

        /// <summary>
        /// Used for the <see cref="BattleCruiserShields"/> model
        /// </summary>
        /// <remarks>
        /// Collisions:<br/>
        /// ❌ <see cref="Default"/><br/>
        /// ❌ <see cref="TransparentFX"/><br/>
        /// ❌ <see cref="IgnoreRaycast"/><br/>
        /// ✔ <see cref="Unassigned_3"/><br/>
        /// ❌ <see cref="Water"/><br/>
        /// ❌ <see cref="UI"/><br/>
        /// ✔ <see cref="Unassigned_6"/><br/>
        /// ✔ <see cref="Unassigned_7"/><br/>
        /// ❌ <see cref="UnderArea"/><br/>
        /// ❌ <see cref="CloneDroneLogo"/><br/>
        /// ❌ <see cref="CentauriStation"/><br/>
        /// ✔ <see cref="IgnoreDuringTwitchSpawnRaycast"/><br/>
        /// ❌ <see cref="ToolCamera"/><br/>
        /// ❌ <see cref="EnvironmentalHazards"/><br/>
        /// ❌ <see cref="CutsceneCameraSet"/><br/>
        /// ❌ <see cref="BluescreenNetwork"/><br/>
        /// ❌ <see cref="Environment"/><br/>
        /// ❌ <see cref="CharacterRoot"/><br/>
        /// ❌ <see cref="BodyPart"/><br/>
        /// ✔ <see cref="Projectile"/><br/>
        /// ❌ <see cref="CameraCollider"/><br/>
        /// ❌ <see cref="EnvironmentRigidBodies"/><br/>
        /// ❌ <see cref="CharacterObjectPusher"/><br/>
        /// ❌ <see cref="BodyPartOnlyCollider"/><br/>
        /// ❌ <see cref="FinalAreaObelisk"/><br/>
        /// ❌ <see cref="CharacterOnlyCollider"/><br/>
        /// ❌ <see cref="EnvironmentOnlyCollider"/><br/>
        /// ❌ <see cref="BodyPartHitBox"/><br/>
        /// ❌ <see cref="BattlecruiserShield"/><br/>
        /// ✔ <see cref="PlanetEarth"/><br/>
        /// ✔ <see cref="Unassigned_30"/><br/>
        /// ✔ <see cref="Unassigned_31"/><br/>
        /// </remarks>
        public const int BattlecruiserShield = 28;

        /// <summary>
        /// Used for the Earth planet object in chapter 5
        /// </summary>
        /// <remarks>
        /// Collides with every layer
        /// </remarks>
        public const int PlanetEarth = 29;

        /// <summary>
        /// Unassigned
        /// </summary>
        /// <remarks>
        /// Collides with every layer
        /// </remarks>
        public const int Unassigned_30 = 30;

        /// <summary>
        /// Unassigned
        /// </summary>
        /// <remarks>
        /// Collides with every layer
        /// </remarks>
        public const int Unassigned_31 = 31;
    }
}
