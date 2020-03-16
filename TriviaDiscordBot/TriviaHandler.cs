using Discord.WebSocket;
using System;
using System.Web;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicDiscordBot
{
    internal class TriviaHandler
    {
        private HttpClient _httpClient;

        public TriviaHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task HandleMessage(SocketMessage message)
        {
            var messageText = message.ToString();

            if (messageText.ToCharArray()[0] == '!' && !message.Author.IsBot)
            {
                if (messageText.StartsWith("!question"))
                {
                    await SendQuestion(message);
                }
            }
        }

        private async Task<bool> SendQuestion(SocketMessage message)
        {
            var result = await _httpClient.GetAsync("https://opentdb.com/api.php?amount=1");
            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await result.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<TriviaResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive=true });

            var returnText = FormatReturnText(response);

            await message.Channel.SendMessageAsync(text: returnText);

            return true;
        }

        private string FormatReturnText(TriviaResponse response)
        {
            var question = response.Results[0];

            var sb = new StringBuilder();
            sb.Append("\nTrivia Question!\n\n");
            sb.Append($"Category: **{question.Category}**\n");
            sb.Append($"Type: **{question.Type}**\n");
            sb.Append($"Difficulty: **{question.Difficulty}**\n\n");
            sb.Append($"Question: {HttpUtility.HtmlDecode(question.Question)}\n");

            var possibleAnswers = new string[question.IncorrectAnswers.Length + 1];
            question.IncorrectAnswers.CopyTo(possibleAnswers, 0);
            possibleAnswers[question.IncorrectAnswers.Length] = question.CorrectAnswer;
            RandomiseArray(possibleAnswers);

            sb.Append($"Possible Answers: ");
            for (int i = 0; i < possibleAnswers.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append($"{HttpUtility.HtmlDecode(possibleAnswers[i])}");
                    continue;
                }
                sb.Append($", {HttpUtility.HtmlDecode(possibleAnswers[i])}"); 
            }
            sb.Append("\n\n");
            sb.Append($"Correct Answer:\n");
            sb.Append($"|| {HttpUtility.HtmlDecode(question.CorrectAnswer)} ||\n");

            return sb.ToString();
        }

        private void RandomiseArray<T>(T[] items)
        {
            Random rand = new Random();

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
    }
}
