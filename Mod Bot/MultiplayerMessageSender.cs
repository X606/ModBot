using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModLibrary;
using Bolt;
using UnityEngine;

namespace ModLibrary
{
    public static class MultiplayerMessageSender
    {
        public static void SendToClient(FirstPersonMover owner, string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(owner.entity.controller);
            SendEvent(genericStringForModdingEvent, message);
        }
        public static void SendToClient(BoltConnection owner, string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(owner);
            SendEvent(genericStringForModdingEvent, message);
        }


        public static void SendToAllClients(string message)
        {
            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(GlobalTargets.AllClients);
            SendEvent(genericStringForModdingEvent, message);
        }

        private static void SendEvent(GenericStringForModdingEvent myEvent, string message)
        {
            if (message.Length > 140)
            {
                throw new Exception("Attempted to send a message that is too long, all messages must be under 140 characters");
            }
            myEvent.EventData = message;
            myEvent.Send();
        }
        
    }
}
