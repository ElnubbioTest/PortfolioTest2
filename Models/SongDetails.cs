namespace PortfolioWebAssem.Models
{
	public class SongDetails
	{
		public int ID { get; set; }
		public string SongName { get; set; }
		public List<string> Lyrics { get; set; }

        public SongDetails(int id, string songName, List<String> lyrics)
        {
			ID = id;
			SongName = songName;
			Lyrics = lyrics;
        }
    }
}
