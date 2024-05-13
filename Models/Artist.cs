namespace PortfolioWebAssem.Models
{
	public class Artist
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public int Score { get; set; }
		public bool IsSelected { get; set; } = false;
	}
}
