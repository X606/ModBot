using System.Collections.Generic;

namespace ModLibrary
{
    public class CommandManager
    {
        public static readonly CommandManager Instance = new CommandManager();
        
        private readonly Dictionary<string, Command> commands = new Dictionary<string, Command>();

        /// <summary>
        /// Check if a Command is registered.
        /// </summary>
        public bool HasCommand(string commandName)
        {
            return commands.ContainsKey(commandName);
        }
        
        /// <summary>
        /// Register a command to be used by the player
        /// </summary>
        public void RegisterCommand(Command command)
        {
            commands.Add(command.GetName().ToLower(), command);
        }

        /// <summary>
        /// Run a command. (This should not be used by mods)
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="args">Given arguments</param>
        public void RunCommand(string commandName, List<string> args)
        {
            if (!commands.ContainsKey(commandName))
            {
                debug.Log("Invalid Command!");
            }

            var command = commands[commandName];
            if (!command.Run(args))
            {
                debug.Log(command.GetUsage());
            }
        }
    }
}