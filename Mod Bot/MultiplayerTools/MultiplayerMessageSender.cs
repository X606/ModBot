using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModLibrary;
using Bolt;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Used to send generic string messages
    /// </summary>
    public static class MultiplayerMessageSender
    {
        /// <summary>
        /// Sends a multiplayer message the owner of a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="owner">The player to send message to</param>
        /// <param name="message">The message to send</param>
        public static void SendToClient(FirstPersonMover owner, string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(owner.entity.controller);
            SendEvent(genericStringForModdingEvent, message);
        }

        /// <summary>
        /// Sends a multiplayer message to a <see cref="BoltConnection"/>
        /// </summary>
        /// <param name="owner">The player to send the message to</param>
        /// <param name="message">The message to send</param>
        public static void SendToClient(BoltConnection owner, string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(owner);
            SendEvent(genericStringForModdingEvent, message);
        }

        /// <summary>
        /// Sends the given <paramref name="message"/> to all clients (including ourselves) connected to the same server we are connected to
        /// </summary>
        /// <param name="message">The message to send</param>
        public static void SendToAllClients(string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(GlobalTargets.AllClients);
            SendEvent(genericStringForModdingEvent, message);
        }

        /// <summary>
        /// Sends the given <paramref name="message"/> to the given <see cref="GlobalTargets"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targets"></param>
        public static void SendToClients(string message, GlobalTargets targets)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(targets);
            SendEvent(genericStringForModdingEvent, message);
        }

        private static void SendEvent(GenericStringForModdingEvent myEvent, string message)
        {
            if (message.Length > 140)
            {
                throw new Exception("Error sending modded string event: Attempted to send a message that is too long, messages cannot exceed 140 characters in length");
            }

            myEvent.EventData = message;
            myEvent.Send();
        }
    }
}
