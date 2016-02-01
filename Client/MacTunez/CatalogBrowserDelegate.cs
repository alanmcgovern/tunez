using System;
using System.Linq;
using AppKit;
using Foundation;
using Tunez;
using System.Collections.Generic;
using System.Collections;

namespace MacTunez
{
	public class NSCatalogItem : NSObject
	{
		Catalog Catalog {
			get; set;
		}

		public NSArtistItem[] Artists {
			get; private set;
		}

		public NSCatalogItem  (Catalog catalog)
		{
			Catalog = catalog;
			Artists = catalog.Artists.Select (artist => new NSArtistItem (artist)).ToArray ();
		}

		public override string Description {
			get {
				return "Catalog";
			}
		}
	}

	public class NSArtistItem : NSObject, IEnumerable<Track>
	{
		Artist Artist{
			get; set;
		}

		public NSAlbumItem [] Albums {
			get; private set;
		}

		public override string Description {
			get {
				return Artist.Name;
			}
		}

		public NSArtistItem (Artist artist)
		{
			Artist = artist;
			Albums = artist.Albums.Select (album => new NSAlbumItem (album)).ToArray ();
		}

		public IEnumerator<Track> GetEnumerator ()
		{
			foreach (var track in Artist)
				yield return track;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	public class NSAlbumItem : NSObject, IEnumerable<Track>
	{
		Album Album {
			get; set;
		}

		public override string Description {
			get {
				return Album.Name;
			}
		}

		public NSTrackItem[] Tracks {
			get; set;
		}

		public NSAlbumItem (Album album)
		{
			Album = album;
			Tracks = Album.Tracks.Select (t => new NSTrackItem (album, t)).ToArray ();
		}

		public IEnumerator<Track> GetEnumerator ()
		{
			foreach (var track in Album)
				yield return track;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	public class NSTrackItem : NSObject, IEnumerable<Track>
	{
		public override string Description {
			get {
				return string.Format ("{0}. {1}", Track.Number, Track.Name);
			}
		}

		Album Album {
			get; set;
		}

		Track Track {
			get; set;
		}

		public NSTrackItem (Album album, Track track)
		{
			Album = album;
			Track = track;
		}

		public IEnumerator<Track> GetEnumerator ()
		{
			foreach (var track in Album.Tracks.SkipWhile (t => t != Track))
				yield return track;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	public class CatalogBrowserDelegate : NSBrowserDelegate
	{
		NSCatalogItem catalogItem;
		public Catalog Catalog {
			get; set ;
		}

		public CatalogBrowserDelegate (Catalog catalog)
		{
			Catalog = catalog;
			catalogItem = new NSCatalogItem (catalog);
		}

		public override NSObject RootItemForBrowser (NSBrowser browser)
		{
			return catalogItem;
		}

		public override nint CountChildren (NSBrowser browser, Foundation.NSObject item)
		{
			if (item is NSCatalogItem)
				return ((NSCatalogItem)item).Artists.Length;
			if (item is NSArtistItem)
				return ((NSArtistItem) item).Albums.Length;
			if (item is NSAlbumItem)
				return ((NSAlbumItem) item).Tracks.Length;
			return 0;
		}

		public override NSObject GetChild (NSBrowser browser, nint index, NSObject item)
		{
			if (item is NSCatalogItem)
				return ((NSCatalogItem)item).Artists [(int)index];
			if (item is NSArtistItem)
				return ((NSArtistItem)item).Albums [(int)index];
			if (item is NSAlbumItem)
				return ((NSAlbumItem)item).Tracks [(int)index];
			return null;
		}

		public override bool IsLeafItem (NSBrowser browser, NSObject item)
		{
			return item is NSTrackItem;
		}

		public override NSObject ObjectValueForItem (NSBrowser browser, NSObject item)
		{
			return (NSString) item.Description;
		}
	}
}

