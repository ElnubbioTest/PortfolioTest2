namespace PortfolioWebAssem.Models
{
	public class ArtistIDResponse
	{
		public Response response { get; set; }
	}

	public class Response
	{
		public List<Hit> hits { get; set; }
	}

	public class Hit
	{
		public Result result { get; set; }
	}

	public class Result
	{
		public PrimaryArtist primary_Artist { get; set; }
		public string title { get; set; }
	}

	public class PrimaryArtist
	{
		public int id { get; set; }

		//actually gonna use this name from now on to make sure matches line up \/
		public string name { get; set; }
	}
}
