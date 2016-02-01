# README #

### How do I get set up? ###

Create a file called `lastfm.cs` beside your checkout, or any any parent directory above it:
```
namespace Tunez
{
        public static class LastFMAppCredentials
        {
                public const string ApplicationName = "APPNAME";
                public const string APIKey = "APIKEY";
                public const string SharedSecret = "SECRET";
                public const string RegisteredTo = "NAME";
        }
}

```
If you register a last.fm api key this will be where you need to place the details.

