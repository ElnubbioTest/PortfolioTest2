using System.Text.Json.Serialization;

namespace PortfolioWebAssem.Models
{
	public class Recording
	{
		public string title { get; set; }


		[JsonPropertyName("artist-credit")]
		public List<ArtistCredit> artistcredit { get; set; }

		public bool IsSelected { get; set; } = false;
	}
	public class ArtistCredit
	{
		public Artist artist { get; set; }
		public string name { get; set; }
	}
}
