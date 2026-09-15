using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PersonalBlogServer.Utils;

public static partial class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalizedString = text.Trim().ToLowerInvariant();

        // 1. Thay thế các ký tự đặc biệt tiếng Việt như 'đ' / 'Đ'
        normalizedString = normalizedString.Replace("đ", "d").Replace("Đ", "d");

        // 2. Tách dấu (Decomposition)
        var formD = normalizedString.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var ch in formD)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }

        var withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

        // 3. Thay thế các ký tự không phải chữ/số thành gạch nối
        var cleaned = CleanNonAlphaRegex().Replace(withoutDiacritics, "-");

        // 4. Xóa gạch nối liên tiếp và ở đầu/cuối
        var trimmedSlug = MultipleDashesRegex().Replace(cleaned, "-").Trim('-');

        return string.IsNullOrWhiteSpace(trimmedSlug) ? "post-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() : trimmedSlug;
    }

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex CleanNonAlphaRegex();

    [GeneratedRegex(@"\s+|-+")]
    private static partial Regex MultipleDashesRegex();
}
