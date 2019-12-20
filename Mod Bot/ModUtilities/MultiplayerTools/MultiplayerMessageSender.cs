using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModLibrary;
using Bolt;
using UnityEngine;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Used to send generic string messages
    /// </summary>
    public static class MultiplayerMessageSender
    {
        /// <summary>
        /// Sends the given <paramref name="message"/> to all clients (including ourselves) connected to the same server we are connected to
        /// </summary>
        /// <param name="message">The message to send</param>
        public static void SendToAllClients(string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(GlobalTargets.AllClients);
            sendEvent(genericStringForModdingEvent, message);
        }

        /// <summary>
        /// Sends the given <paramref name="message"/> to the given <see cref="GlobalTargets"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targets"></param>
        public static void SendToAllClients(string message, GlobalTargets targets)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(targets);
            sendEvent(genericStringForModdingEvent, message);
        }

        static void sendEvent(GenericStringForModdingEvent myEvent, string message)
        {
            if (message.Length > 1024)
                throw new Exception("Error sending modded string event: Attempted to send a message that is too long, messages cannot exceed 1024 characters in length");

            myEvent.EventData = message;
            myEvent.Send();
        }

    }
}
