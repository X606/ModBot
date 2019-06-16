using System;
using System.Collections.Generic;

namespace ModLibrary
{
    public interface Command
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        /// <returns>Command Name.</returns>
        string GetName();
        /// <summary>
        /// The usage of the command.
        /// </summary>
        /// <returns>Command Usage.</returns>
        string GetUsage();
        /// <summary>
        /// Run when the command is ran by the player.
        /// </summary>
        /// <param name="args">Given arguments.</param>
        /// <returns>Whether the arguments given where satisfactory, if not then it will log the command usage.</returns>
        bool Run(List<string> args);
    }
}