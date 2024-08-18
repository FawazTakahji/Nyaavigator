using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ErrorOr;
using Nyaavigator.Models;

namespace Nyaavigator.Utilities;

public static class Feeds
{
    public static ErrorOr<List<Feed>> Load()
    {
        string path = Path.Combine(App.BaseDirectory, "Feeds.json");

        if (!File.Exists(path))
            return Error.Failure("FileNotFound", "The feeds file doesn't exist.");

        try
        {
            string json = File.ReadAllText(path);
            List<Feed>? feeds = JsonSerializer.Deserialize<List<Feed>>(json);
            return feeds is null
                ? Error.Failure(description: "Failed to deserialize the feeds file.") : feeds;
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while loading the feeds file.", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }

    public static ErrorOr<Success> Save(List<Feed> feeds)
    {
        string path = Path.Combine(App.BaseDirectory, "Feeds.json");
        try
        {
            string json = JsonSerializer.Serialize(feeds, Singletons.SerializerOptions);
            File.WriteAllText(path, json);

            return new Success();
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while saving the feeds file.", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }
}