using System.Text.RegularExpressions;

namespace PortfolioWebAssem.Pages.MusicQuizPage
{
	//API Client for fetching lyrics
	public static class LyricsAPIClient
	{
		//TODO - replace this api with custom web scraping some random lyric website, this api has too many BIG songs without lyrics
		public static int NumberOfFetchesComplete;
		public static async Task<List<string>> FetchLyricsAsync(string artistName, string songName, Dictionary<string, bool> settings, CancellationToken cancToken) {
			//LYRICS API FETCH - gives the lyrics for a given artist and song
			//Have to use this API since Genius won't let you get lyrics without manually scraping
			//possible problem with mismatch between geniusAPI song name and this lyric's api song name
			try
			{
				//Add check to replace apostrophe with backtick for songName because API did that for some reason
				if (songName.Contains('’'))
				{
					songName = songName.Replace("’", "%60");
				}
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.lyrics.ovh/v1/{artistName}/{songName}");
				var httpClient = new HttpClient();
				var response = await httpClient.SendAsync(httpRequestMessage, cancToken);
				NumberOfFetchesComplete++;
		 		return getLyricSnippet(await response.Content.ReadAsStringAsync(), settings);
			} 
			catch (OperationCanceledException)
			{
				Console.WriteLine($"Canceled lyric search for {artistName}");
				return new List<string>();
			} 
			catch (Exception e)
			{
				Console.WriteLine($"Can not find lyrics for {artistName} - {songName}");
				return new List<string>();
			}
		 }

		private static List<string> filterLyricsToList(string response)
		{
			//remove start info + all [SINGER NAME] occurences
			string pattern = "\"lyrics\".*?\\n|\\[[^\\]]*\\]";
			Regex regex = new Regex(pattern);
			string result = regex.Replace(response, "");

			//remove start - could redo regex filter to remove this part
			int returnIndex = result.IndexOf("\\r") + 2;
			string stringWithoutStart = result.Substring(returnIndex);
			//remove end - remove "}
			string stringWithoutEnd = stringWithoutStart.Substring(0, stringWithoutStart.Length - 2);

			//remove Chorus: (Chorus) Singername: etc.
			string pattern2 = @"\{[^}]*\}|[A-Z]\w*:|\(.+?\+.+?\)|\((?:[Cc]horus|[Vv]erse|[Rr]efrain|[Ii]ntermediate|[Bb]ridge|[Rr]epeat)(?:,?\s+\w+)?\)";
			Regex regex2 = new Regex(pattern2);
			string result2 = regex2.Replace(stringWithoutEnd, "");

			//Tidy up empty parentheses
			string pattern3 = @"\(\)";
			Regex regex3 = new Regex(pattern3);
			string result3 = regex3.Replace(result2, "");

			//split strings to list
			List<string> lyricLinesWithNewLines = result3.Split("\\n").ToList();
			List<string> songLyricsSentences = lyricLinesWithNewLines.Where((s) => s.Length > 1).ToList();

			return songLyricsSentences;
		}

		private static List<string> getLyricSnippet(string response, Dictionary<string, bool> settings)
		{
			//Give me wordCount amount of words from response. Has to start at the beginning of a sentence.

			int wordCount = 10;
			List<string> blackListOfGenericStarts = ["i", "you", "we", "and", "but", "i'm", "or", "the", "oh", "ooh", "yeah", "so", "to", "i've", "it", "a", "in", "why"];
			List<string> songLyricsSentences = filterLyricsToList(response);

			//print all lines
			//foreach (string sentence in songLyricsSentences)
			//{
			//	Console.WriteLine(sentence);
			//}

			//Console.WriteLine(response);

			int maxStartingIndex = getMaxStartingIndex(songLyricsSentences, wordCount);
			Random rand = new Random();
			int startingIndex = rand.Next(0, maxStartingIndex);

			if (settings["startOnly"])
			{
				//always start from the first line
				startingIndex = 0;
			}
			if (settings["repeatsOnly"])
			{
				//basically that JS set interview question but in c#:
				//if not in set, add it and increment number of times it appears; if in set: increment number of times it appears
				HashSet<string> uniqueSentences = new HashSet<string>();
				Dictionary<string, int> uniqueSentencesWithCount = new();
				foreach(string sentence in songLyricsSentences)
				{
					if (uniqueSentences.Contains(sentence))
					{
						uniqueSentencesWithCount[sentence]++;
					} else
					{
						uniqueSentences.Add(sentence);
						uniqueSentencesWithCount[sentence] = 1;
					}

				}

				//sort by most occurrences - now have a list of the most repeated lines and how many times they're repeated
				var sortedUniqueSentencesWithCount = uniqueSentencesWithCount.OrderBy(pair => pair.Value).Reverse().ToDictionary(pair => pair.Key, pair => pair.Value);

				//print each unique line with how many times it appears
				//foreach (KeyValuePair<string,int> keyValuePair in sortedUniqueSentencesWithCount)
				//{
				//	Console.WriteLine(keyValuePair.Key + " " + sortedUniqueSentencesWithCount[keyValuePair.Key]);
				//}

				int firstIndexOfMostOccurringLine = songLyricsSentences.IndexOf(sortedUniqueSentencesWithCount.Keys.First());
				startingIndex = firstIndexOfMostOccurringLine;
				Console.WriteLine("Most Occurring Line: " + songLyricsSentences[firstIndexOfMostOccurringLine]);

				//find all occurrence's indexes in original lyric list - not necessary right now - will use this if I implement combination of noGenericStarts + mostRepeats
				//var result = Enumerable.Range(0, songLyricsSentences.Count).Where(i => songLyricsSentences[i] == sortedUniqueSentencesWithCount.Keys.First()).ToList();
				//foreach (int res in result)
				//{
				//	Console.WriteLine(res);
				//}
			}

			if (settings["noGenericStarts"])
			{
				for(int i = startingIndex; i<songLyricsSentences.Count; i++)
				//foreach(string sentence in songLyricsSentences)
				{
					List<string> splitSentence = songLyricsSentences[i].Split(" ").ToList();
					Console.WriteLine(splitSentence[0] + " " + blackListOfGenericStarts.Contains(splitSentence[0].ToLower()));
					if (!blackListOfGenericStarts.Contains(splitSentence[0].ToLower()))
					{
						startingIndex = i;
						break;
					}
				}
			}

			List<string> finalLyricLineList = new();
			int currentWordCount = 0;
			for (int i = startingIndex; i < songLyricsSentences.Count; i++)
			{
				string sentence = songLyricsSentences[i];
				List<string> sentenceWords = sentence.Split(" ").ToList();
				foreach (string word in sentenceWords)
				{
					finalLyricLineList.Add(word);
					currentWordCount++;
					if (currentWordCount >= wordCount)
					{
						if (settings["???"])
						{
							return finalLyricLineList.OrderBy(_ => Guid.NewGuid()).ToList();
						}
						return finalLyricLineList;
					}
				}
			}
			return ["oops"];
		}

		private static int getMaxStartingIndex(List<string> songLyrics, int maxWordCount)
		{
			//What is the last possible sentence I can choose to make sure that I have at least maxWordCount amount of words to display

			//reverse list
			songLyrics.Reverse();

			//count words
			int currentWordCount = 0;
			int loopCounter = 0;

			//add words one sentence at a time until maxWordCount
			foreach (string sentence in songLyrics)
			{
				int numOfWords = sentence.Split(" ").Count();
				currentWordCount += numOfWords;
				if (currentWordCount > maxWordCount)
				{
					songLyrics.Reverse();
					return songLyrics.Count - loopCounter - 1;
				}
				loopCounter++;
			}
			return -1;
		}
	}
}



