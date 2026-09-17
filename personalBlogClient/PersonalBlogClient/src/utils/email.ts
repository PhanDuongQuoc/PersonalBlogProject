/**
 * Utility for opening Email compose in Webmail (Gmail Web) and Desktop Mail apps.
 */
export interface EmailReplyOptions {
  email: string;
  name?: string | null | undefined;
  subject?: string | null | undefined;
  message?: string | null | undefined;
}

/**
 * Opens Gmail Web compose directly in a new tab with pre-filled fields.
 */
export function openGmailCompose(options: EmailReplyOptions) {
  const email = options.email.trim();
  const subject = `Re: ${options.subject?.trim() || "Liên hệ từ PDQ Portfolio"}`;
  const body = options.message?.trim()
    ? `\n\n---\nTin nhắn gốc từ ${options.name?.trim() || email} (${email}):\n"${options.message.trim()}"`
    : "";

  const url = `https://mail.google.com/mail/?view=cm&fs=1&to=${encodeURIComponent(email)}&su=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
  window.open(url, "_blank", "noopener,noreferrer");
}

/**
 * Triggers standard mailto: without opening an orphan about:blank tab.
 */
export function openDesktopMailClient(options: EmailReplyOptions) {
  const email = options.email.trim();
  const subject = encodeURIComponent(`Re: ${options.subject?.trim() || "Liên hệ từ PDQ Portfolio"}`);
  const body = encodeURIComponent(
    options.message?.trim()
      ? `\n\n---\nTin nhắn gốc từ ${options.name?.trim() || email} (${email}):\n"${options.message.trim()}"`
      : ""
  );

  const mailtoUrl = `mailto:${encodeURIComponent(email)}?subject=${subject}&body=${body}`;
  const link = document.createElement("a");
  link.href = mailtoUrl;
  link.style.display = "none";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
}

/**
 * Default Smart Reply action (Opens Gmail Web directly in a new tab).
 */
export function replyViaEmail(options: EmailReplyOptions) {
  openGmailCompose(options);
}
