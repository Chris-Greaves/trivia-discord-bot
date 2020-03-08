using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BasicDiscordBot
{
	class Program
    {
		private IConfiguration _configuration;
		private DiscordSocketClient _client;
		private TriviaHandler _handler;

		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			_configuration = builder.Build();

			_client = new DiscordSocketClient();
			_client.Log += Log;
			await _client.LoginAsync(TokenType.Bot,
				_configuration.GetConnectionString("DiscordToken"));
			await _client.StartAsync();

			_handler = new TriviaHandler(new HttpClient());

			_client.MessageReceived += _client_MessageReceived;

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task _client_MessageReceived(SocketMessage message)
		{
			Console.WriteLine($"Message Received: {message}");
			return _handler.HandleMessage(message);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
