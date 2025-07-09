using MiniTwitter.Core.Domain.Entities;
using System.Text;
namespace MiniTwitter.Service
{
    public static class CursorUtils
    {
        public static (DateTime?, int?) DecodeCursorSafe(string? cursor)
        {
            if (string.IsNullOrWhiteSpace(cursor)) return (null, null);

            try
            {
                var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var parts = raw.Split('|');
                return (DateTime.Parse(parts[0]), int.Parse(parts[1]));
            }
            catch
            {
                return (null, null); // fallback if invalid cursor
            }
        }

        public static string? CreateCursor(Tweet? tweet)
        {
            if (tweet == null) return null;
            var raw = $"{tweet.CreatedAt:o}|{tweet.Id}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        }

        public static IQueryable<Tweet> ApplyCursorFilter(IQueryable<Tweet> query, DateTime? cursorDate, int? cursorId)
        {
            if (cursorDate.HasValue && cursorId.HasValue)
            {
                return query.Where(t =>
                    t.CreatedAt < cursorDate.Value ||
                    (t.CreatedAt == cursorDate.Value && t.Id < cursorId.Value));
            }
            return query;
        }
    }
}
