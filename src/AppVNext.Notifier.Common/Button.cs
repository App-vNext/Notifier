using Newtonsoft.Json;

namespace AppVNext.Notifier.Common
{
	[JsonObject(Id = "button")]
	public class Button
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("text")]
		public string Text { get; set; }
		[JsonProperty("arguments")]
		public string Arguments { get; set; }
	}
}