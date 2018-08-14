using Newtonsoft.Json;

namespace AppVNext.Notifier.Common
{
	[JsonObject(Id = "input")]
	public class Input
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("title")]
		public string Title { get; set; }
		[JsonProperty("placeholdertext")]
		public string PlaceHolderText { get; set; }
	}
}