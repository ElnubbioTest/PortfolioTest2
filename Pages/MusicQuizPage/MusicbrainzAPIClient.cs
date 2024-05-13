using PortfolioWebAssem.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace PortfolioWebAssem.Pages.MusicQuizPage
{
	//API Client for fetching possible artists to choose from

	public static class MusicbrainzAPIClient
	{
		static HttpClient httpClient = new();
		static List<Artist> ArtistList = new();
		static List<Recording> RecordingList = new();
		public static async Task<List<Artist>> FetchNPossibleArtists(string artistName, int N)
		{
			//MUSICBRAINZ FETCH - gives a list of N possible artists that match the given artistName for the user to choose one
			try
			{
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://musicbrainz.org/ws/2/artist/?query={artistName}&fmt=json");
				var response = await httpClient.SendAsync(httpRequestMessage);
				var returnObject = await response.Content.ReadFromJsonAsync<ArtistResponse>();

				ArtistList = returnObject.Artists.DistinctBy((a) => a.Name).Take(N).ToList();
				return ArtistList;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Could not find artist: {artistName}");
				return new();
			}
		}
		public static async Task<List<Recording>> FetchNPossibleSongTitles(string songName, int N)
		{
			//MUSICBRAINZ FETCH - gives a list of N possible songs that match the given songName for the user to choose one
			try
			{
				Console.WriteLine("Search by possible song title");
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://musicbrainz.org/ws/2/recording/?query=\"{songName}\" AND NOT artist:\"{songName}\"&fmt=json");
				// https://musicbrainz.org/ws/2/recording/?query=\"in the end\" AND NOT artist:\"in the end\" AND NOT status:\"bootleg\" AND NOT status:\"live\"&fmt=json
				var response = await httpClient.SendAsync(httpRequestMessage);
				var returnObject = await response.Content.ReadFromJsonAsync<RecordingResponse>();

				RecordingList = returnObject.Recordings.DistinctBy((a) => a.artistcredit[0].name).Take(N).ToList();
				//Console.WriteLine(returnObject.Recordings.First().artistcredit.First().name);
				return RecordingList;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Could not find song: {songName} Exception: {e}");
				return new();
			}
		}
	}
}
