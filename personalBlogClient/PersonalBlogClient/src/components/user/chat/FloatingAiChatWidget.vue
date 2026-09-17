<template>
  <div class="pdq-ai-chat-widget">
    <!-- 1. Floating Chat Trigger Button -->
    <transition name="pop-scale">
      <button v-if="!isOpen" type="button" class="floating-chat-trigger" aria-label="Mở Trợ lý AI PDQ"
        @click="toggleChat">
        <div class="trigger-glow-ring"></div>
        <div class="trigger-icon-box">
          <q-icon name="fa-solid fa-wand-magic-sparkles" size="18px" class="sparkle-icon" />
        </div>
        <span class="trigger-label">Hỏi AI</span>
        <span class="status-pulse-dot" title="AI Trực tuyến"></span>
      </button>
    </transition>

    <!-- Backdrop overlay when expanded as a dialog -->
    <transition name="pop-scale">
      <div v-if="isOpen && isExpanded" class="chat-dialog-backdrop" @click="toggleExpand"></div>
    </transition>

    <!-- 2. Chat Window Panel -->
    <transition name="chat-panel-slide">
      <div v-if="isOpen" class="ai-chat-panel" :class="{ 'is-expanded': isExpanded }">
        <!-- Header -->
        <header class="panel-header">
          <div class="header-left">
            <div class="bot-avatar-box">
              <q-icon name="fa-solid fa-robot" size="16px" class="bot-icon" />
              <span class="online-indicator"></span>
            </div>
            <div class="bot-title-box">
              <h3 class="bot-name">PDQ Assistant</h3>
              <!-- <p class="bot-status">
                <span class="dot-sep">•</span>
                <span class="status-text">Sẵn sàng hỗ trợ</span>
              </p> -->
            </div>
          </div>

          <div class="header-actions">
            <!-- Maximize / Expand Dialog Toggle Button -->
            <button
              type="button"
              class="btn-header-action btn-expand"
              :title="isExpanded ? 'Thu nhỏ cửa sổ' : 'Phóng to thành hộp thoại lớn'"
              @click="toggleExpand"
            >
              <q-icon :name="isExpanded ? 'fa-solid fa-compress' : 'fa-solid fa-up-right-and-down-left-from-center'" size="11px" />
            </button>

            <!-- Clear history button -->
            <button type="button" class="btn-header-action" title="Xóa đoạn hội thoại" @click="clearHistory">
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>

            <!-- Close / Minimize button -->
            <button type="button" class="btn-header-action btn-close" title="Đóng cửa sổ chat" @click="toggleChat">
              <q-icon name="fa-solid fa-xmark" size="14px" />
            </button>
          </div>
        </header>

        <!-- Message Body -->
        <div ref="messagesContainer" class="panel-messages">
          <!-- Welcome Screen (When no conversation yet) -->
          <div v-if="messages.length === 0" class="welcome-card">
            <div class="welcome-icon-box">
              <q-icon name="fa-solid fa-sparkles" size="24px" class="text-rose" />
            </div>
            <h4 class="welcome-title">Xin chào! 👋</h4>
            <p class="welcome-desc">
              Tôi là <strong>PDQ Assistant</strong> — trợ lý AI của Blog & Portfolio cá nhân Phan Dương Quốc. Bạn có thể
              hỏi tôi về kinh nghiệm, kỹ năng của tác giả hoặc các bài viết trên blog!
            </p>

            <div class="quick-prompts-title">
              <q-icon name="fa-solid fa-lightbulb" size="11px" class="text-amber-8 q-mr-xs" />
              <span>Gợi ý câu hỏi nhanh:</span>
            </div>
            <div class="quick-prompts-list">
              <button v-for="(prompt, idx) in defaultPrompts" :key="idx" type="button" class="prompt-chip"
                @click="sendQuickPrompt(prompt)">
                <span>{{ prompt }}</span>
                <q-icon name="fa-solid fa-arrow-turn-down" size="10px" class="chip-arrow" />
              </button>
            </div>
          </div>

          <!-- Messages Stream -->
          <div v-for="(msg, index) in messages" :key="index" class="message-item"
            :class="msg.role === 'user' ? 'user-msg' : 'model-msg'">
            <!-- Avatar for Model -->
            <div v-if="msg.role !== 'user'" class="msg-avatar">
              <q-icon name="fa-solid fa-robot" size="12px" />
            </div>

            <!-- Message Bubble -->
            <div class="msg-bubble-container">
              <div class="msg-bubble" :class="{ 'msg-error': msg.isError }">
                <!-- User Plain Text -->
                <div v-if="msg.role === 'user'" class="user-text">
                  {{ msg.content }}
                </div>

                <!-- Model Formatted Markdown -->
                <div v-else class="model-markdown" v-html="renderMarkdown(msg.content)"></div>
              </div>
              <span class="msg-time">{{ formatTime(msg.timestamp) }}</span>
            </div>
          </div>

          <!-- Typing Indicator -->
          <div v-if="isTyping" class="message-item model-msg typing-msg">
            <div class="msg-avatar">
              <q-icon name="fa-solid fa-robot" size="12px" />
            </div>
            <div class="msg-bubble typing-bubble">
              <div class="typing-dot"></div>
              <div class="typing-dot"></div>
              <div class="typing-dot"></div>
            </div>
          </div>

          <!-- Follow-up suggestions after model response -->
          <div v-if="!isTyping && followUpSuggestions.length > 0 && messages.length > 0" class="followups-container">
            <span class="followup-label">Gợi ý tiếp theo:</span>
            <div class="followup-chips">
              <button v-for="(item, idx) in followUpSuggestions" :key="idx" type="button" class="followup-chip"
                @click="sendQuickPrompt(item)">
                {{ item }}
              </button>
            </div>
          </div>
        </div>

        <!-- Footer Input Area -->
        <footer class="panel-footer">
          <form class="input-form" @submit.prevent="handleSend">
            <input ref="inputRef" v-model="inputMessage" type="text" placeholder="Hỏi về tác giả, bài viết, kỹ năng..."
              class="chat-input" :disabled="isTyping" maxlength="500" @keydown.enter.prevent="handleSend" />

            <button type="submit" class="btn-send" :disabled="!inputMessage.trim() || isTyping"
              title="Gửi tin nhắn (Enter)">
              <q-spinner-tail v-if="isTyping" size="14px" color="white" />
              <q-icon v-else name="fa-solid fa-paper-plane" size="12px" />
            </button>
          </form>
          <div class="footer-hint">
            <span>Powered by Gemini AI • Nhấn Enter để gửi</span>
          </div>
        </footer>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, nextTick, watch } from "vue";
import type { ChatMessage } from "@/types/ai-chat";
import { aiChatService } from "@/services/ai-chat.service";

const isOpen = ref(false);
const isExpanded = ref(false);
const isTyping = ref(false);
const inputMessage = ref("");
const messagesContainer = ref<HTMLElement | null>(null);
const inputRef = ref<HTMLInputElement | null>(null);

const defaultPrompts = [
  "Giới thiệu về tác giả Phan Dương Quốc",
  "Kỹ năng lập trình & công nghệ chính?",
  "Gợi ý các bài viết hay trên Blog",
  "Làm thế nào để liên hệ với tác giả?"
];

const followUpSuggestions = ref<string[]>([]);

const messages = reactive<ChatMessage[]>([]);

function toggleChat() {
  isOpen.value = !isOpen.value;
  if (isOpen.value) {
    nextTick(() => {
      scrollToBottom();
      inputRef.value?.focus();
    });
  } else {
    isExpanded.value = false;
  }
}

function toggleExpand() {
  isExpanded.value = !isExpanded.value;
  nextTick(() => {
    scrollToBottom();
    inputRef.value?.focus();
  });
}

function clearHistory() {
  messages.splice(0, messages.length);
  followUpSuggestions.value = [];
}

function sendQuickPrompt(promptText: string) {
  inputMessage.value = promptText;
  handleSend();
}

async function handleSend() {
  const text = inputMessage.value.trim();
  if (!text || isTyping.value) return;

  // 1. Add User Message
  messages.push({
    role: "user",
    content: text,
    timestamp: new Date()
  });

  inputMessage.value = "";
  followUpSuggestions.value = [];
  scrollToBottom();

  try {
    isTyping.value = true;

    // 2. Prepare History
    const historyPayload = messages.slice(-10).map((m) => ({
      role: m.role === "user" ? "user" : "model",
      content: m.content
    }));

    // 3. Call AI Service
    const res = await aiChatService.sendMessage({
      message: text,
      history: historyPayload
    });

    if (res.success && res.reply) {
      messages.push({
        role: "model",
        content: res.reply,
        timestamp: new Date()
      });

      if (res.suggestedFollowUps && res.suggestedFollowUps.length > 0) {
        followUpSuggestions.value = res.suggestedFollowUps;
      }
    } else {
      messages.push({
        role: "model",
        content: res.error || "Rất tiếc, AI tạm thời không phản hồi. Vui lòng thử lại sau!",
        timestamp: new Date(),
        isError: true
      });
    }
  } catch (err: any) {
    console.error("AI Chat error:", err);
    messages.push({
      role: "model",
      content: "Đã xảy ra lỗi kết nối đến máy chủ AI. Vui lòng kiểm tra lại kết nối mạng!",
      timestamp: new Date(),
      isError: true
    });
  } finally {
    isTyping.value = false;
    scrollToBottom();
    nextTick(() => {
      inputRef.value?.focus();
    });
  }
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
}

function formatTime(timestamp?: Date | string): string {
  const date = timestamp ? new Date(timestamp) : new Date();
  return date.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" });
}

// Lightweight, safe Markdown formatting
function renderMarkdown(raw: string): string {
  if (!raw) return "";

  let html = raw
    // Escape HTML special characters
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");

  // Code blocks (```language ... ```)
  html = html.replace(/```([a-zA-Z0-9_-]*)\n([\s\S]*?)```/g, (_match, _lang, code) => {
    return `<pre class="chat-code-block"><code>${code.trim()}</code></pre>`;
  });

  // Inline code (`code`)
  html = html.replace(/`([^`]+)`/g, '<code class="chat-inline-code">$1</code>');

  // Bold (**text** or __text__)
  html = html.replace(/\*\*([^*]+)\*\*/g, "<strong>$1</strong>");
  html = html.replace(/__([^_]+)__/g, "<strong>$1</strong>");

  // Italic (*text* or _text_)
  html = html.replace(/\*([^*]+)\*/g, "<em>$1</em>");

  // Links [text](url)
  html = html.replace(
    /\[([^\]]+)\]\(([^)]+)\)/g,
    '<a href="$2" target="_blank" rel="noopener noreferrer" class="chat-link">$1</a>'
  );

  // Headers (### Header)
  html = html.replace(/^### (.*$)/gim, '<h5 class="chat-h5">$1</h5>');
  html = html.replace(/^## (.*$)/gim, '<h4 class="chat-h4">$1</h4>');

  // Unordered list items (- item or * item)
  html = html.replace(/^[-*] (.*$)/gim, '<li class="chat-li">$1</li>');

  // Line breaks
  html = html.replace(/\n/g, "<br />");

  return html;
}
</script>

<style scoped lang="scss">
.pdq-ai-chat-widget {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 9999;
  font-family: var(--font-body, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif);
}

/* 1. Floating Trigger Button */
.floating-chat-trigger {
  position: relative;
  height: 52px;
  padding: 0 20px 0 16px;
  border-radius: 999px;
  background: var(--bg-surface, #0b1326);
  border: 1px solid rgba(223, 38, 106, 0.4);
  color: #ffffff;
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  box-shadow: 0 10px 25px rgba(11, 19, 38, 0.35), 0 0 15px rgba(223, 38, 106, 0.25);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);

  &:hover {
    transform: translateY(-3px) scale(1.03);
    border-color: #df266a;
    box-shadow: 0 14px 30px rgba(11, 19, 38, 0.45), 0 0 22px rgba(223, 38, 106, 0.45);
  }

  .trigger-icon-box {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: linear-gradient(135deg, #df266a 0%, #7c3aed 100%);
    display: flex;
    align-items: center;
    justify-content: center;
    color: #ffffff;
    box-shadow: 0 2px 8px rgba(223, 38, 106, 0.4);
  }

  .trigger-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 13.5px;
    font-weight: 700;
    letter-spacing: 0.2px;
    color: #f8fafc;
  }

  .status-pulse-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: #10b981;
    box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
    animation: pulse-green 2s infinite;
  }
}

@keyframes pulse-green {
  0% {
    box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
  }

  70% {
    box-shadow: 0 0 0 8px rgba(16, 185, 129, 0);
  }

  100% {
    box-shadow: 0 0 0 0 rgba(16, 185, 129, 0);
  }
}

/* 2. Chat Panel */
.ai-chat-panel {
  width: 380px;
  height: 550px;
  max-width: calc(100vw - 32px);
  max-height: calc(100vh - 48px);
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.12));
  border-radius: 20px;
  box-shadow: 0 20px 45px rgba(0, 0, 0, 0.4), 0 0 1px rgba(248, 250, 252, 0.08);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  animation: panelIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  transition: width 0.3s ease, height 0.3s ease, transform 0.3s ease;

  &.is-expanded {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: min(900px, 94vw);
    height: min(780px, 88vh);
    max-width: 94vw;
    max-height: 88vh;
    border-radius: 22px;
    z-index: 10001;
    box-shadow: 0 30px 80px rgba(0, 0, 0, 0.75), 0 0 0 1px rgba(248, 250, 252, 0.12);

    .panel-messages {
      padding: 22px 28px;
    }

    .welcome-card {
      max-width: 650px;
      margin: 16px auto;
      padding: 24px 28px;
    }

    .user-msg .msg-bubble {
      max-width: 600px;
      font-size: 13.5px;
    }

    .model-msg .msg-bubble-container {
      max-width: calc(100% - 44px);
    }

    .model-msg .msg-bubble {
      font-size: 13.5px;
      padding: 14px 18px;
    }

    .panel-footer {
      padding: 14px 20px 12px;
    }
  }

  @media (max-width: 480px) {
    width: calc(100vw - 24px);
    height: calc(100vh - 40px);
    bottom: 12px;
    right: 12px;
  }
}

.chat-dialog-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(6, 14, 32, 0.72);
  backdrop-filter: blur(6px);
  -webkit-backdrop-filter: blur(6px);
  z-index: 10000;
}

/* Header */
.panel-header {
  padding: 14px 18px;
  background: var(--bg-surface-lowest, #060e20);
  color: #ffffff;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));

  .header-left {
    display: flex;
    align-items: center;
    gap: 10px;

    .bot-avatar-box {
      position: relative;
      width: 36px;
      height: 36px;
      border-radius: 10px;
      background: linear-gradient(135deg, #df266a 0%, #7c3aed 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      color: #ffffff;

      .online-indicator {
        position: absolute;
        bottom: -2px;
        right: -2px;
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: #10b981;
        border: 2px solid var(--bg-surface-lowest, #060e20);
      }
    }

    .bot-title-box {
      .bot-name {
        font-family: var(--font-headline, sans-serif);
        font-size: 14px;
        font-weight: 700;
        color: #ffffff;
        margin: 0 0 1px;
      }
    }
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 6px;

    .btn-header-action {
      width: 28px;
      height: 28px;
      border-radius: 7px;
      background: rgba(255, 255, 255, 0.08);
      border: none;
      color: #cbd5e1;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(255, 255, 255, 0.18);
        color: #ffffff;
      }

      &.btn-close:hover {
        background: #df266a;
        color: #ffffff;
      }
    }
  }
}

/* Messages Body */
.panel-messages {
  flex: 1;
  padding: 16px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 14px;
  background: var(--bg-surface-lowest, #060e20);

  &::-webkit-scrollbar {
    width: 5px;
  }

  &::-webkit-scrollbar-thumb {
    background: var(--border-subtle, rgba(248, 250, 252, 0.12));
    border-radius: 4px;
  }
}

/* Welcome Card */
.welcome-card {
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 14px;
  padding: 16px;
  text-align: center;
  box-shadow: var(--shadow-card, 0 4px 12px rgba(0, 0, 0, 0.2));

  .welcome-icon-box {
    width: 44px;
    height: 44px;
    margin: 0 auto 10px;
    border-radius: 12px;
    background: var(--accent-primary-container, rgba(223, 38, 106, 0.12));
    border: 1px solid rgba(223, 38, 106, 0.25);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .welcome-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 15px;
    font-weight: 700;
    color: var(--text-primary, #dae2fd);
    margin: 0 0 6px;
  }

  .welcome-desc {
    font-size: 12.5px;
    color: var(--text-secondary, #94a3b8);
    line-height: 1.5;
    margin: 0 0 14px;
  }

  .quick-prompts-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 11.5px;
    font-weight: 700;
    color: var(--text-primary, #dae2fd);
    text-align: left;
    margin-bottom: 8px;
    display: flex;
    align-items: center;
  }

  .quick-prompts-list {
    display: flex;
    flex-direction: column;
    gap: 6px;

    .prompt-chip {
      padding: 8px 12px;
      background: var(--bg-surface-container, #131b2e);
      border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.08));
      border-radius: 8px;
      font-size: 12px;
      color: var(--text-primary, #dae2fd);
      text-align: left;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: space-between;
      transition: all 0.2s ease;

      &:hover {
        background: var(--accent-primary-container, rgba(223, 38, 106, 0.15));
        border-color: var(--accent-primary, #df266a);
        color: var(--accent-primary, #df266a);

        .chip-arrow {
          transform: translateX(2px);
          color: var(--accent-primary, #df266a);
        }
      }

      .chip-arrow {
        color: var(--text-muted, #64748b);
        transition: transform 0.2s ease;
      }
    }
  }
}

/* Message Items */
.message-item {
  display: flex;
  gap: 8px;
  align-items: flex-start;

  &.user-msg {
    justify-content: flex-end;

    .msg-bubble-container {
      align-items: flex-end;
    }

    .msg-bubble {
      background: var(--accent-primary, #df266a);
      color: #ffffff;
      border-radius: 14px 14px 2px 14px;
      padding: 10px 14px;
      font-size: 13px;
      line-height: 1.45;
      max-width: 270px;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.3);
    }
  }

  &.model-msg {
    justify-content: flex-start;

    .msg-avatar {
      width: 26px;
      height: 26px;
      border-radius: 8px;
      background: var(--accent-primary-container, rgba(223, 38, 106, 0.12));
      color: var(--accent-primary, #df266a);
      border: 1px solid rgba(223, 38, 106, 0.25);
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      margin-top: 2px;
    }

    .msg-bubble-container {
      align-items: flex-start;
      max-width: calc(100% - 34px);
    }

    .msg-bubble {
      background: var(--bg-surface, #0b1326);
      border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
      border-radius: 14px 14px 14px 2px;
      padding: 12px 14px;
      font-size: 13px;
      line-height: 1.5;
      color: var(--text-primary, #dae2fd);
      box-shadow: var(--shadow-card, 0 2px 8px rgba(0, 0, 0, 0.15));

      &.msg-error {
        background: rgba(239, 68, 68, 0.15);
        border-color: rgba(239, 68, 68, 0.3);
        color: #fca5a5;
      }
    }
  }

  .msg-bubble-container {
    display: flex;
    flex-direction: column;
    gap: 3px;

    .msg-time {
      font-size: 10px;
      color: var(--text-muted, #64748b);
      padding: 0 4px;
    }
  }
}

/* Typing Bubble */
.typing-bubble {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 10px 14px !important;

  .typing-dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    background: var(--accent-primary, #df266a);
    animation: bounce 1.4s infinite ease-in-out both;

    &:nth-child(1) {
      animation-delay: -0.32s;
    }

    &:nth-child(2) {
      animation-delay: -0.16s;
    }
  }
}

@keyframes bounce {
  0%,
  80%,
  100% {
    transform: scale(0);
  }

  40% {
    transform: scale(1);
  }
}

/* Follow Up Chips */
.followups-container {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 6px 0 2px 34px;

  .followup-label {
    font-size: 11px;
    font-weight: 600;
    color: var(--text-muted, #64748b);
  }

  .followup-chips {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;

    .followup-chip {
      padding: 5px 10px;
      background: var(--bg-surface-container, #131b2e);
      border: 1px solid var(--accent-secondary-border, rgba(99, 102, 241, 0.22));
      border-radius: 12px;
      font-size: 11.5px;
      color: var(--accent-secondary-text, #a5b4fc);
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--accent-secondary-container, rgba(99, 102, 241, 0.15));
        border-color: var(--accent-primary, #df266a);
        color: var(--text-primary, #ffffff);
      }
    }
  }
}

/* Markdown Styling inside Model Message */
:deep(.model-markdown) {
  p {
    margin: 0 0 8px;

    &:last-child {
      margin-bottom: 0;
    }
  }

  h4.chat-h4 {
    font-size: 13.5px;
    font-weight: 700;
    margin: 8px 0 4px;
    color: var(--text-primary, #dae2fd);
  }

  h5.chat-h5 {
    font-size: 12.5px;
    font-weight: 700;
    margin: 6px 0 3px;
    color: var(--text-primary, #dae2fd);
  }

  strong {
    color: var(--text-primary, #ffffff);
    font-weight: 700;
  }

  li.chat-li {
    margin: 2px 0;
    padding-left: 4px;
  }

  .chat-link {
    color: var(--accent-primary, #df266a);
    text-decoration: underline;
    font-weight: 600;

    &:hover {
      color: var(--accent-primary-hover, #f43f7e);
    }
  }

  .chat-inline-code {
    font-family: var(--font-mono, monospace);
    font-size: 11.5px;
    background: var(--bg-surface-container, #131b2e);
    color: var(--accent-primary, #df266a);
    border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
    padding: 1px 5px;
    border-radius: 4px;
  }

  .chat-code-block {
    background: var(--bg-surface-lowest, #060e20);
    color: var(--text-primary, #dae2fd);
    border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.1));
    border-radius: 8px;
    padding: 8px 10px;
    margin: 6px 0;
    overflow-x: auto;
    font-family: var(--font-mono, monospace);
    font-size: 11.5px;
  }
}

/* Footer */
.panel-footer {
  padding: 10px 14px 8px;
  background: var(--bg-surface, #0b1326);
  border-top: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));

  .input-form {
    display: flex;
    align-items: center;
    gap: 8px;
    background: var(--bg-surface-container, #131b2e);
    border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.1));
    border-radius: 12px;
    padding: 4px 6px 4px 12px;
    transition: all 0.2s ease;

    &:focus-within {
      background: var(--bg-surface-high, #171f33);
      border-color: var(--accent-primary, #df266a);
      box-shadow: 0 0 0 3px var(--accent-primary-container, rgba(223, 38, 106, 0.12));
    }

    .chat-input {
      flex: 1;
      border: none;
      background: transparent;
      outline: none;
      font-size: 12.5px;
      color: var(--text-primary, #dae2fd);

      &::placeholder {
        color: var(--text-muted, #64748b);
        font-size: 12px;
      }
    }

    .btn-send {
      width: 32px;
      height: 32px;
      border-radius: 9px;
      background: var(--accent-primary, #df266a);
      border: none;
      color: #ffffff;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        background: var(--accent-primary-hover, #f43f7e);
        transform: scale(1.04);
      }

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
    }
  }

  .footer-hint {
    font-size: 10px;
    color: var(--text-muted, #64748b);
    text-align: center;
    margin-top: 5px;
  }
}

/* Animations */
.pop-scale-enter-active,
.pop-scale-leave-active {
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}

.pop-scale-enter-from,
.pop-scale-leave-to {
  opacity: 0;
  transform: scale(0.8);
}

.chat-panel-slide-enter-active,
.chat-panel-slide-leave-active {
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.chat-panel-slide-enter-from,
.chat-panel-slide-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.95);
}
</style>
