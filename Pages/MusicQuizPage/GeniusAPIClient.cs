using Newtonsoft.Json.Linq;
using PortfolioWebAssem.Models;
using PortfolioWebAssem.Models2;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using FuzzySharp;

namespace PortfolioWebAssem.Pages.MusicQuizPage
{
	public static class GeniusAPIClient
	{
		private static string _apiKey = "RpBaUk-iRsnuJJ8ozLN7ZCjSQyd-MU4Vnp2F2HxOllHYPRpVRgooijV63QEToIdT";
		public static string NewArtistName = "";
		
		public static async Task<string> GetArtistId(Object input)
		{
			//returns Genius API's Artist ID for a selected artist/recording

			HttpClient httpClient = new();
			double similarityScore;
			string _fetchURL;
			string artistID = "";

			//Search by Song Title
			if (input is Recording recording)
			{
				_fetchURL = $"https://api.genius.com/search?q={recording.title}&access_token={_apiKey}";
				try
				{
					var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURL);
					var response = await httpClient.SendAsync(httpRequestMessage);
					var returnObject = await response.Content.ReadFromJsonAsync<ArtistIDResponse>();

					foreach (Hit hit in returnObject.response.hits)
					{
						similarityScore = Fuzz.Ratio(hit.result.primary_Artist.name.ToLower(), recording.artistcredit.First().name.ToLower());
						//check if SELECTED artistName matches this song's artist name
						Console.WriteLine($"Checking {hit.result.primary_Artist.name} against {recording.artistcredit.First().name}, score: {similarityScore}");
						if (similarityScore > 90)
						{
							//found match - 90 in case of different spacing etc.
							artistID = hit.result.primary_Artist.id.ToString();
							NewArtistName = hit.result.primary_Artist.name.ToString();
							break;
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Could not find artistID for {recording.artistcredit[0].name} - {recording.title}. Exception: {e}");
				}
			}

			//Search by Artist Name
			if (input is string artistName)
			{
				_fetchURL = $"https://api.genius.com/search?q={artistName}&access_token={_apiKey}";
				try
				{
					//fetch
					var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURL);
					var response = await httpClient.SendAsync(httpRequestMessage);
					var returnObject = await response.Content.ReadFromJsonAsync<ArtistIDResponse>();

					//fuzzy check 
					foreach (Hit hit in returnObject.response.hits)
					{
						similarityScore = Fuzz.Ratio(hit.result.primary_Artist.name, artistName);
						//check if SELECTED artistName matches this artist name
						Console.WriteLine($"Checking {hit.result.primary_Artist.name} against {artistName}, score: {similarityScore}");
						if (similarityScore > 90)
						{
							//found match, this is my Artist ID - 90 in case of different spacing etc.
							artistID = hit.result.primary_Artist.id.ToString();
							NewArtistName = hit.result.primary_Artist.name.ToString();
							break;
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Could not find artistID for {artistName}. Exception: {e}");
				}
			}

			return artistID;
		}



		public static async Task<List<string>> GetSongTitles(string artistId, int numberOfSongs, CancellationToken cancToken)
		//returns Genius API's Song Titles using Genius API's Artist ID. Returns numberOfSongs song titles - can be increased
		{
			List<Song> songsFromFetch = new();
			List<string> songTitles = new();
			HttpClient httpClient = new();

			string _fetchURLForSongs = $"https://api.genius.com/artists/{artistId}/songs?sort=popularity&per_page={numberOfSongs}&access_token={_apiKey}";
			//sometimes the api returns an empty string for artistId since response.hits is empty - see "1StepKloser", so skip fetch
			if (artistId != "")
			{
				try
				{
					//fetch
					var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForSongs);
					var response = await httpClient.SendAsync(httpRequestMessage, cancToken);
					var returnObject = await response.Content.ReadFromJsonAsync<SongsResponse>();

					songsFromFetch = returnObject.response.songs;
					
					//put song titles in my list
					
						foreach (Song song in songsFromFetch)
						{
						if (song.primary_artist.id.ToString() == artistId)
							{
								songTitles.Add(song.title);
							}
						}
					
				}
				catch (Exception e)
				{
					Console.WriteLine($"Could not find song titles for {artistId}. Exception: ", e);
				}
			}
			//numberOfSongs song titles
			return songTitles;
		}
		
	}
}
