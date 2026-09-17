/**
 * Date and Time utilities for formatting timestamps in UTC / Local time (Vietnam GMT+7)
 */

/**
 * Parses a date string safely, ensuring UTC timestamps without explicit offset are parsed correctly as UTC.
 */
export function parseUtcDate(dateStr?: string | Date | null): Date | null {
  if (!dateStr) return null;
  if (dateStr instanceof Date) return isNaN(dateStr.getTime()) ? null : dateStr;

  let str = String(dateStr).trim();
  if (!str) return null;

  // If string doesn't end with Z and doesn't contain explicit timezone offset (+HH:mm or -HH:mm)
  // E.g. "2026-09-17T06:57:00" or "2026-09-17 06:57:00"
  if (!str.endsWith("Z") && !/[+-]\d{2}(:\d{2})?$/.test(str)) {
    // If it has a space instead of T, replace first space with T
    if (str.includes(" ") && !str.includes("T")) {
      str = str.replace(" ", "T");
    }
    str += "Z";
  }

  const d = new Date(str);
  if (isNaN(d.getTime())) {
    // Fallback to default Date parsing
    const fallback = new Date(dateStr);
    return isNaN(fallback.getTime()) ? null : fallback;
  }
  return d;
}

/**
 * Returns human-readable relative time (e.g. "Vừa xong", "5 phút trước", "2 giờ trước", "Hôm qua")
 */
export function formatTimeAgo(dateStr?: string | Date | null): string {
  if (!dateStr) return "";
  try {
    const d = parseUtcDate(dateStr);
    if (!d) return "";

    const now = new Date();
    const diffSec = Math.floor((now.getTime() - d.getTime()) / 1000);

    // Negative diff (due to slight server-client clock skew) or less than 60s
    if (diffSec < 60) return "Vừa xong";
    if (diffSec < 3600) return `${Math.floor(diffSec / 60)} phút trước`;
    if (diffSec < 86400) return `${Math.floor(diffSec / 3600)} giờ trước`;
    if (diffSec < 172800) return "Hôm qua";
    if (diffSec < 604800) return `${Math.floor(diffSec / 86400)} ngày trước`;

    return d.toLocaleDateString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric"
    });
  } catch {
    return String(dateStr);
  }
}

/**
 * Formats date into HH:mm DD/MM/YYYY
 */
export function formatDateTime(dateStr?: string | Date | null): string {
  if (!dateStr) return "—";
  try {
    const d = parseUtcDate(dateStr);
    if (!d) return "—";

    const time = d.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" });
    const date = d.toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
    return `${time} ${date}`;
  } catch {
    return String(dateStr);
  }
}

/**
 * Formats date into HH:mm:ss DD/MM/YYYY
 */
export function formatFullDateTime(dateStr?: string | Date | null): string {
  if (!dateStr) return "";
  try {
    const d = parseUtcDate(dateStr);
    if (!d) return "";
    return d.toLocaleString("vi-VN", {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      day: "2-digit",
      month: "2-digit",
      year: "numeric"
    });
  } catch {
    return String(dateStr);
  }
}
