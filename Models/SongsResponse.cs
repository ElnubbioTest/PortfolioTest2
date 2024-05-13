using PortfolioWebAssem.Models;

namespace PortfolioWebAssem.Models2
{
	public class SongsResponse
	{
		public Response response { get; set; }
	}

	public class Response
	{
		public List<Song> songs {get; set;}
	}

	public class Song
	{
		public string title { get; set; }
		public PrimaryArtist primary_artist { get; set; }

	}
}
