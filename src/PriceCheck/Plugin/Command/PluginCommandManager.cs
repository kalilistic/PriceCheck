using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using static Dalamud.Game.Command.CommandInfo;

namespace PriceCheck
{
	public class PluginCommandManager<THost> : IDisposable
	{
		private readonly THost _host;
		private readonly (string, CommandInfo)[] _pluginCommands;
		private readonly DalamudPluginInterface _pluginInterface;

		public PluginCommandManager(THost host, DalamudPluginInterface pluginInterface)
		{
			_pluginInterface = pluginInterface;
			_host = host;

			_pluginCommands = host.GetType()
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
				.Where(method => method.GetCustomAttribute<CommandAttribute>() != null)
				.SelectMany(GetCommandInfoTuple)
				.ToArray();

			AddCommandHandlers();
		}

		public void Dispose()
		{
			RemoveCommandHandlers();
		}

		private void AddCommandHandlers()
		{
			foreach (var t in _pluginCommands)
			{
				var (command, commandInfo) = t;
				_pluginInterface.CommandManager.AddHandler(command, commandInfo);
			}
		}

		private void RemoveCommandHandlers()
		{
			foreach (var t in _pluginCommands)
			{
				var (command, _) = t;
				_pluginInterface.CommandManager.RemoveHandler(command);
			}
		}

		private IEnumerable<(string, CommandInfo)> GetCommandInfoTuple(MethodInfo method)
		{
			var handlerDelegate = (HandlerDelegate) Delegate.CreateDelegate(typeof(HandlerDelegate), _host, method);

			var command = handlerDelegate.Method.GetCustomAttribute<CommandAttribute>();
			var aliases = handlerDelegate.Method.GetCustomAttribute<AliasesAttribute>();
			var helpMessage = handlerDelegate.Method.GetCustomAttribute<HelpMessageAttribute>();
			var doNotShowInHelp = handlerDelegate.Method.GetCustomAttribute<DoNotShowInHelpAttribute>();

			var commandInfo = new CommandInfo(handlerDelegate)
			{
				HelpMessage = helpMessage?.HelpMessage ?? string.Empty,
				ShowInHelp = doNotShowInHelp == null
			};

			var commandInfoTuples = new List<(string, CommandInfo)> {(command.Command, commandInfo)};
			if (aliases == null) return commandInfoTuples;
			for (var i = aliases.Aliases.Length - 1; i >= 0; i--)
				commandInfoTuples.Add((aliases.Aliases[i], commandInfo));

			return commandInfoTuples;
		}
	}
}