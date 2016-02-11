﻿using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace Tunez
{
	public static class TrackLoader
	{
		static bool Filter (string filePath)
		{
			return filePath.EndsWith (".aac", StringComparison.OrdinalIgnoreCase)
				|| filePath.EndsWith (".ogg", StringComparison.OrdinalIgnoreCase)
				|| filePath.EndsWith (".mp3", StringComparison.OrdinalIgnoreCase)
				|| filePath.EndsWith (".wma", StringComparison.OrdinalIgnoreCase);
		}

		public static List<Track> Load (string directory, IProgress<float> monitor, CancellationToken token)
		{
			return Load (new [] { directory }, monitor, token);
		}

		public static List<Track> Load (IEnumerable<string> directories, IProgress<float> monitor, CancellationToken token)
		{
			var allFiles = Enumerable.Empty <string> ();
			foreach (var directory in directories) {
				if (Directory.Exists (directory))
					allFiles = allFiles.Concat (Directory.GetFiles (directory, "*.*", SearchOption.AllDirectories));
			}

			var filteredFiles = allFiles
				.Where (Filter)
				.ToArray ();

			return LoadFiles (filteredFiles, monitor, token);
		}

		static List<Track> LoadFiles (string[] filePaths, IProgress<float> monitor, CancellationToken token)
		{
			DateTime lastReported = DateTime.UtcNow;
			int uuid = 0;
			var results = new List<Track> ();
			foreach (var filePath in filePaths) {
				LoggingService.LogInfo ("Loading from: {0}", filePath);
				token.ThrowIfCancellationRequested ();
				try {
					using (var file = TagLib.File.Create (filePath)) {
						results.Add (new Track {
							Album = file.Tag.Album,
							AlbumArtist = file.Tag.FirstAlbumArtist,
							Disc = (int) file.Tag.Disc,
							Duration = (int)file.Properties.Duration.TotalSeconds,
							FilePath = filePath,
							FileSize = new FileInfo (filePath).Length,
							MusicBrainzId = !string.IsNullOrEmpty (file.Tag.MusicBrainzTrackId) ? file.Tag.MusicBrainzTrackId : null,
							Name = file.Tag.Title,
							Number = (int)file.Tag.Track,
							TrackArtist = file.Tag.FirstPerformer,
							UUID = uuid++
						});
					}

					if (DateTime.UtcNow - lastReported > TimeSpan.FromSeconds (1)) {
						lastReported = DateTime.UtcNow;
						monitor.Report (results.Count / (float) filePaths.Length);
					}
				} catch (Exception ex) {
					LoggingService.LogInfo ("Skipping '{0}' as it could not be parsed...{1}{2}", filePath, Environment.NewLine, ex);
				}
			}
			monitor.Report (1);
			return results;
		}
	}
}
