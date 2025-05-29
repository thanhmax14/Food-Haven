using System.Globalization;

namespace Food_Haven.Web.Services
{
/*    public static class OrderStatusHelper
    {
        public static List<OrderStatusTimeline> ParseStatusTimeline(string description)
        {
            var result = new List<OrderStatusTimeline>();
            if (string.IsNullOrWhiteSpace(description))
                return result;

            var entries = description.Split('#', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in entries)
            {
                var parts = entry.Split('-', 2); // chỉ tách phần đầu tiên để giữ nguyên status có dấu "-"
                if (parts.Length == 2 &&
                    DateTime.TryParse(parts[1], out var parsedDate))
                {
                    result.Add(new OrderStatusTimeline
                    {
                        Status = parts[0].Trim(),
                        Date = parsedDate
                    });
                }
            }

            return result
                .OrderBy(x => x.Date) // đảm bảo đúng thứ tự thời gian
                .ToList();
        }
    }*/
    public static class DateFormatter
    {
        public static string FormatOrderDate(DateTime date)
        {
            return date.ToString("dd MMM yyyy - hh:mmtt", CultureInfo.InvariantCulture);
        }
    }

}
