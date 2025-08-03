using Microsoft.Extensions.Logging;
using MiniTwitter.Core.Domain.Entities;
using System.Text;
using System.Diagnostics;
namespace MiniTwitter.Service
{
    public static class CursorUtils
    {
        private static ILogger? _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        public static (DateTime?, int?) DecodeCursorSafe(string? cursor)
        {
            if (string.IsNullOrWhiteSpace(cursor))
            {
                _logger?.LogDebug("No cursor provided, using default values");
                return (null, null);
            }

            try
            {
                var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var parts = raw.Split('|');
                if (parts.Length != 2)
                {
                    _logger?.LogWarning("Invalid cursor format: {Cursor}", cursor);
                    return (null, null);
                }

                var result = (DateTime.Parse(parts[0]), int.Parse(parts[1]));
                _logger?.LogDebug("Decoded cursor: {Cursor} -> Date: {Date}, Id: {Id}", 
                    cursor, result.Item1, result.Item2);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error decoding cursor: {Cursor}", cursor);
                return (null, null);
            }
        }

        public static string? CreateCursor(Tweet? tweet)
        {
            if (tweet == null) 
            {
                _logger?.LogDebug("Cannot create cursor: tweet is null");
                return null;
            }
            
            var raw = $"{tweet.CreatedAt:o}|{tweet.Id}";
            var cursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
            _logger?.LogDebug("Created cursor for tweet {TweetId}: {Cursor}", tweet.Id, cursor);
            return cursor;
        }

        public static IQueryable<Tweet> ApplyCursorFilter(IQueryable<Tweet> query, DateTime? cursorDate, int? cursorId)
        {
            _logger?.LogDebug("Applying cursor filter - Date: {CursorDate}, Id: {CursorId}", cursorDate, cursorId);
            
            if (cursorDate.HasValue && cursorId.HasValue)
            {
                var filtered = query.Where(t =>
                    t.CreatedAt < cursorDate.Value ||
                    (t.CreatedAt == cursorDate.Value && t.Id < cursorId.Value));
                
                _logger?.LogDebug("Filtered query from {InitialCount} to {FilteredCount} items", 
                    query.Count(), filtered.Count());
                
                return filtered;
            }
            
            _logger?.LogDebug("No cursor filter applied, returning original query with {Count} items", query.Count());
            return query;
        }
    }
}
