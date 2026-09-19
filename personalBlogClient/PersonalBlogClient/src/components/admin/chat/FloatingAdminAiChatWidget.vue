<template>
  <div class="pdq-admin-ai-chat-widget">
    <!-- Proactive Greeting Speech Bubble for Admin -->
    <transition name="bubble-fade">
      <div v-if="!isOpen && showGreeting" class="floating-greeting-bubble" @click="openChatFromGreeting">
        <button type="button" class="btn-close-greeting" title="Đóng lời chào" aria-label="Đóng lời chào"
          @click.stop="dismissGreeting">
          <q-icon name="fa-solid fa-xmark" size="11px" />
        </button>

        <div class="greeting-content">
          <div class="greeting-header">
            <span class="greeting-wave-icon">🚀</span>
            <span class="greeting-badge">ADMIN AI</span>
          </div>
          <p class="greeting-text">
            Chào Admin! Cần hỗ trợ lên dàn ý bài viết, tối ưu SEO hay phân tích số liệu không?
          </p>
          <div class="greeting-action">
            <span class="action-hint">Bấm để mở Copilot</span>
            <q-icon name="fa-solid fa-arrow-right" size="11px" class="action-arrow" />
          </div>
        </div>
        <div class="greeting-tail"></div>
      </div>
    </transition>

    <!-- 1. Floating Chat Trigger Button (Circular Robot FAB) -->
    <transition name="pop-scale">
      <button v-if="!isOpen" type="button" class="floating-chat-trigger" aria-label="Mở Admin AI"
        title="Trò chuyện cùng Trợ lý Admin AI" @click="toggleChat">
        <div class="trigger-glow-ring"></div>
        <div class="trigger-icon-box">
          <q-icon name="fa-solid fa-wand-magic-sparkles" size="22px" class="robot-fab-icon" />
        </div>
        <span class="status-pulse-dot" title="Admin Copilot Trực tuyến"></span>
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
              <div class="bot-title-row">
                <h3 class="bot-name">Admin AI</h3>
                <!-- <span class="copilot-badge">PRO</span> -->
              </div>
              <p class="bot-status">
                <span class="status-dot"></span>
                <span class="status-text">Cố vấn quản trị</span>
              </p>
            </div>
          </div>

          <div class="header-actions">
            <!-- Maximize / Expand Dialog Toggle Button -->
            <button type="button" class="btn-header-action btn-expand"
              :title="isExpanded ? 'Thu nhỏ cửa sổ' : 'Phóng to thành hộp thoại lớn'" @click="toggleExpand">
              <q-icon :name="isExpanded ? 'fa-solid fa-compress' : 'fa-solid fa-up-right-and-down-left-from-center'"
                size="11px" />
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
              <q-icon name="fa-solid fa-wand-magic-sparkles" size="24px" class="text-rose" />
            </div>
            <h4 class="welcome-title">Xin chào Admin! 🚀</h4>
            <p class="welcome-desc">
              Tôi là <strong>PDQ Admin AI</strong> — trợ lý AI đắc lực hỗ trợ bạn sáng tạo nội dung,
              lên dàn ý bài viết kỹ thuật, tối ưu hóa SEO, phân tích số liệu và hỗ trợ kỹ thuật .NET 9 & Vue 3.
            </p>

            <div class="quick-prompts-title">
              <q-icon name="fa-solid fa-lightbulb" size="12px" class="text-amber-8 q-mr-xs" />
              <span>Gợi ý tác vụ nhanh cho Admin:</span>
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

            <!-- Avatar for User/Admin -->
            <div v-else class="msg-avatar admin-avatar">
              <q-icon name="fa-solid fa-user-shield" size="12px" />
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

                <!-- Copy Message Button for Model Reply -->
                <button v-if="msg.role !== 'user' && !msg.isError" type="button" class="btn-copy-msg"
                  title="Sao chép nội dung" @click="copyToClipboard(msg.content)">
                  <q-icon name="fa-regular fa-copy" size="11px" />
                </button>
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
            <div class="followup-header">
              <q-icon name="fa-solid fa-wand-magic-sparkles" size="11px" class="text-rose q-mr-xs" />
              <span class="followup-label">Đề xuất tác vụ tiếp theo:</span>
            </div>
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
            <input ref="inputRef" v-model="inputMessage" type="text"
              placeholder="Hỏi Copilot về bài viết, SEO, code .NET/Vue, phân tích..." class="chat-input"
              :disabled="isTyping" maxlength="1000" @keydown.enter.prevent="handleSend" />

            <button type="submit" class="btn-send" :disabled="!inputMessage.trim() || isTyping"
              title="Gửi tin nhắn (Enter)">
              <q-spinner-tail v-if="isTyping" size="14px" color="white" />
              <q-icon v-else name="fa-solid fa-paper-plane" size="12px" />
            </button>
          </form>
          <div class="footer-hint">
            <span>Powered by PDQ AI • Nhấn Enter để gửi</span>
          </div>
        </footer>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, nextTick, onMounted, onBeforeUnmount } from "vue";
import { useQuasar } from "quasar";
import type { ChatMessage } from "@/types/ai-chat";
import { adminAiChatService } from "@/services/admin-ai-chat.service";

const $q = useQuasar();

const isOpen = ref(false);
const isExpanded = ref(false);
const isTyping = ref(false);
const showGreeting = ref(false);
let greetingTimer: any = null;

const inputMessage = ref("");
const messagesContainer = ref<HTMLElement | null>(null);
const inputRef = ref<HTMLInputElement | null>(null);

const defaultPrompts = [
  "Lập dàn ý bài viết mới về Clean Architecture trong .NET 9",
  "Gợi ý 5 chủ đề blog công nghệ đang thịnh hành",
  "Phân tích và tối ưu hóa SEO cho các bài viết",
  "Soạn email chuyên nghiệp phản hồi cơ hội việc làm"
];

const followUpSuggestions = ref<string[]>([]);
const messages = reactive<ChatMessage[]>([]);

onMounted(() => {
  // Show welcoming speech bubble after 1.5s if chat is not opened yet
  greetingTimer = setTimeout(() => {
    if (!isOpen.value) {
      showGreeting.value = true;
    }
  }, 1500);
});

onBeforeUnmount(() => {
  if (greetingTimer) clearTimeout(greetingTimer);
});

function openChatFromGreeting() {
  showGreeting.value = false;
  isOpen.value = true;
  nextTick(() => {
    scrollToBottom();
    inputRef.value?.focus();
  });
}

function dismissGreeting() {
  showGreeting.value = false;
}

function toggleChat() {
  isOpen.value = !isOpen.value;
  if (isOpen.value) {
    showGreeting.value = false;
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

    // 3. Call Admin AI Service
    const res = await adminAiChatService.sendMessage({
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
    console.error("Admin AI Chat error:", err);
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

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text).then(() => {
    $q.notify({
      message: "Đã sao chép nội dung vào bộ nhớ tạm",
      color: "positive",
      icon: "fa-solid fa-check",
      timeout: 1500,
      position: "top"
    });
  });
}

// Lightweight, safe Markdown formatting
function renderMarkdown(raw: string): string {
  if (!raw) return "";

  let html = raw
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");

  // Code blocks (```language ... ```)
  html = html.replace(/```([a-zA-Z0-9_-]*)\n([\s\S]*?)```/g, (_match, lang, code) => {
    const langLabel = lang ? `<span class="code-lang-tag">${lang}</span>` : "";
    return `<div class="chat-code-wrapper">${langLabel}<pre class="chat-code-block"><code>${code.trim()}</code></pre></div>`;
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

  // Headers
  html = html.replace(/^### (.*$)/gim, '<h5 class="chat-h5">$1</h5>');
  html = html.replace(/^## (.*$)/gim, '<h4 class="chat-h4">$1</h4>');
  html = html.replace(/^# (.*$)/gim, '<h3 class="chat-h3">$1</h3>');

  // Unordered list items (- item or * item)
  html = html.replace(/^[-*] (.*$)/gim, '<li class="chat-li">$1</li>');

  // Line breaks
  html = html.replace(/\n/g, "<br />");

  return html;
}
</script>

<style scoped lang="scss">
.pdq-admin-ai-chat-widget {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 9999;
  font-family: var(--font-body, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif);
}

/* 0. Proactive Greeting Speech Bubble */
.floating-greeting-bubble {
  position: absolute;
  bottom: 68px;
  right: 4px;
  width: 270px;
  background: var(--bg-surface, #0f172a);
  border: 1px solid rgba(223, 38, 106, 0.4);
  border-radius: 18px 18px 4px 18px;
  padding: 14px 16px 12px;
  box-shadow: 0 14px 35px rgba(0, 0, 0, 0.45), 0 0 16px rgba(223, 38, 106, 0.2);
  cursor: pointer;
  z-index: 10000;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  backdrop-filter: blur(12px);

  &:hover {
    transform: translateY(-3px) scale(1.02);
    border-color: #df266a;
    box-shadow: 0 18px 40px rgba(0, 0, 0, 0.55), 0 0 24px rgba(223, 38, 106, 0.35);

    .action-arrow {
      transform: translateX(4px);
      color: #df266a;
    }

    .action-hint {
      color: #df266a;
    }
  }

  .btn-close-greeting {
    position: absolute;
    top: 8px;
    right: 8px;
    width: 20px;
    height: 20px;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.08);
    border: none;
    color: var(--text-muted, #64748b);
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      background: rgba(223, 38, 106, 0.25);
      color: #df266a;
    }
  }

  .greeting-content {
    display: flex;
    flex-direction: column;
    gap: 6px;

    .greeting-header {
      display: flex;
      align-items: center;
      gap: 6px;

      .greeting-wave-icon {
        font-size: 14px;
        animation: wave-bot 2s infinite ease-in-out;
      }

      .greeting-badge {
        font-size: 11px;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        color: #df266a;
      }
    }

    .greeting-text {
      font-size: 12.5px;
      line-height: 1.45;
      color: var(--text-primary, #f8fafc);
      margin: 0;
      font-weight: 500;
    }

    .greeting-action {
      display: flex;
      align-items: center;
      gap: 5px;
      margin-top: 2px;

      .action-hint {
        font-size: 11px;
        font-weight: 600;
        color: #a5b4fc;
        transition: color 0.2s ease;
      }

      .action-arrow {
        color: #a5b4fc;
        transition: all 0.2s ease;
      }
    }
  }

  .greeting-tail {
    position: absolute;
    bottom: -8px;
    right: 20px;
    width: 0;
    height: 0;
    border-left: 8px solid transparent;
    border-right: 8px solid transparent;
    border-top: 8px solid var(--bg-surface, #0f172a);
    filter: drop-shadow(0 2px 2px rgba(0, 0, 0, 0.2));
  }
}

@keyframes wave-bot {

  0%,
  100% {
    transform: rotate(0deg);
  }

  25% {
    transform: rotate(-12deg);
  }

  75% {
    transform: rotate(12deg);
  }
}

.bubble-fade-enter-active,
.bubble-fade-leave-active {
  transition: all 0.35s cubic-bezier(0.16, 1, 0.3, 1);
}

.bubble-fade-enter-from,
.bubble-fade-leave-to {
  opacity: 0;
  transform: translateY(12px) scale(0.92);
}

/* 1. Floating Trigger Button */
.floating-chat-trigger {
  position: relative;
  width: 54px;
  height: 54px;
  border-radius: 50%;
  background: linear-gradient(135deg, #df266a 0%, #9333ea 50%, #6366f1 100%);
  border: 1px solid rgba(255, 255, 255, 0.25);
  color: #ffffff;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  cursor: pointer;
  box-shadow: 0 10px 25px rgba(223, 38, 106, 0.4), 0 0 20px rgba(147, 51, 234, 0.3);
  transition: all 0.35s cubic-bezier(0.16, 1, 0.3, 1);
  overflow: visible;

  &::before {
    content: "";
    position: absolute;
    inset: -3px;
    border-radius: 50%;
    background: linear-gradient(135deg, #df266a, #9333ea, #6366f1);
    opacity: 0.45;
    filter: blur(8px);
    z-index: -1;
    transition: opacity 0.3s ease, filter 0.3s ease;
    animation: ring-pulse 3s infinite ease-in-out;
  }

  &:hover {
    transform: translateY(-4px) scale(1.08);
    box-shadow: 0 16px 36px rgba(223, 38, 106, 0.55), 0 0 30px rgba(147, 51, 234, 0.5);

    &::before {
      opacity: 0.85;
      filter: blur(12px);
    }

    .robot-fab-icon {
      transform: scale(1.15) rotate(12deg);
    }
  }

  &:active {
    transform: translateY(-1px) scale(0.96);
  }

  .trigger-icon-box {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;

    .robot-fab-icon {
      color: #ffffff;
      filter: drop-shadow(0 2px 6px rgba(0, 0, 0, 0.25));
      transition: transform 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
    }
  }

  .status-pulse-dot {
    position: absolute;
    top: 0;
    right: 0;
    width: 13px;
    height: 13px;
    border-radius: 50%;
    background: #10b981;
    border: 2.5px solid var(--bg-surface, #0f172a);
    box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
    animation: pulse-green 2s infinite;
  }
}

@keyframes ring-pulse {

  0%,
  100% {
    transform: scale(1);
    opacity: 0.4;
  }

  50% {
    transform: scale(1.12);
    opacity: 0.75;
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
  width: 390px;
  height: 560px;
  max-width: calc(100vw - 32px);
  max-height: calc(100vh - 48px);
  background: var(--bg-surface, #0f172a);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 20px;
  box-shadow: 0 20px 45px rgba(0, 0, 0, 0.45), 0 0 1px rgba(255, 255, 255, 0.1);
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
    width: min(920px, 94vw);
    height: min(800px, 88vh);
    max-width: 94vw;
    max-height: 88vh;
    border-radius: 22px;
    z-index: 10001;
    box-shadow: 0 30px 80px rgba(0, 0, 0, 0.8), 0 0 0 1px rgba(255, 255, 255, 0.15);

    .panel-messages {
      padding: 22px 28px;
    }

    .welcome-card {
      max-width: 650px;
      margin: 16px auto;
      padding: 24px 28px;
    }

    .user-msg .msg-bubble {
      max-width: 650px;
      font-size: 13.5px;
    }

    .model-msg .msg-bubble-container {
      max-width: calc(100% - 44px);
    }

    .model-msg .msg-bubble {
      font-size: 13.5px;
      padding: 14px 18px;
    }
  }
}

.chat-dialog-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.65);
  backdrop-filter: blur(8px);
  z-index: 10000;
}

@keyframes panelIn {
  from {
    opacity: 0;
    transform: translateY(16px) scale(0.95);
  }

  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Panel Header */
.panel-header {
  padding: 14px 16px;
  background: linear-gradient(180deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0) 100%),
    var(--bg-surface, #0f172a);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  display: flex;
  align-items: center;
  justify-content: space-between;

  .header-left {
    display: flex;
    align-items: center;
    gap: 10px;

    .bot-avatar-box {
      position: relative;
      width: 36px;
      height: 36px;
      border-radius: 10px;
      background: linear-gradient(135deg, #df266a 0%, #9333ea 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      color: #ffffff;
      box-shadow: 0 4px 12px rgba(223, 38, 106, 0.3);

      .online-indicator {
        position: absolute;
        bottom: -2px;
        right: -2px;
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: #10b981;
        border: 2px solid var(--bg-surface, #0f172a);
      }
    }

    .bot-title-box {
      display: flex;
      flex-direction: column;

      .bot-title-row {
        display: flex;
        align-items: center;
        gap: 6px;

        .bot-name {
          font-size: 14px;
          font-weight: 700;
          color: var(--text-primary, #f8fafc);
          margin: 0;
          line-height: 1.2;
        }

        .copilot-badge {
          font-size: 9px;
          font-weight: 800;
          padding: 1px 5px;
          border-radius: 4px;
          background: linear-gradient(135deg, #df266a, #9333ea);
          color: #ffffff;
          letter-spacing: 0.5px;
        }
      }

      .bot-status {
        font-size: 11px;
        color: var(--text-muted, #94a3b8);
        margin: 2px 0 0;
        display: flex;
        align-items: center;
        gap: 4px;

        .status-dot {
          width: 5px;
          height: 5px;
          border-radius: 50%;
          background: #10b981;
        }
      }
    }
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 4px;

    .btn-header-action {
      width: 28px;
      height: 28px;
      border-radius: 8px;
      background: transparent;
      border: 1px solid transparent;
      color: var(--text-muted, #94a3b8);
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(255, 255, 255, 0.08);
        color: var(--text-primary, #f8fafc);
      }

      &.btn-close:hover {
        background: rgba(239, 68, 68, 0.2);
        color: #ef4444;
      }
    }
  }
}

/* Panel Messages */
.panel-messages {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 14px;
  scroll-behavior: smooth;

  &::-webkit-scrollbar {
    width: 5px;
  }

  &::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.15);
    border-radius: 10px;
  }
}

/* Welcome Card */
.welcome-card {
  background: rgba(255, 255, 255, 0.03);
  border: 1px dashed rgba(223, 38, 106, 0.3);
  border-radius: 14px;
  padding: 16px;
  text-align: center;
  margin-top: 4px;

  .welcome-icon-box {
    width: 44px;
    height: 44px;
    margin: 0 auto 10px;
    border-radius: 12px;
    background: rgba(223, 38, 106, 0.15);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .welcome-title {
    font-size: 15px;
    font-weight: 700;
    color: var(--text-primary, #f8fafc);
    margin: 0 0 6px;
  }

  .welcome-desc {
    font-size: 12px;
    line-height: 1.5;
    color: var(--text-muted, #94a3b8);
    margin: 0 0 14px;
  }

  .quick-prompts-title {
    font-size: 11.5px;
    font-weight: 600;
    color: var(--text-primary, #f8fafc);
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
      text-align: left;
      font-size: 11.5px;
      line-height: 1.4;
      padding: 8px 10px;
      border-radius: 8px;
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.08);
      color: var(--text-primary, #f8fafc);
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 6px;
      transition: all 0.2s ease;

      .chip-arrow {
        color: var(--text-muted, #64748b);
        transition: transform 0.2s ease;
      }

      &:hover {
        background: rgba(223, 38, 106, 0.15);
        border-color: rgba(223, 38, 106, 0.35);
        color: #f43f5e;

        .chip-arrow {
          transform: translateX(2px);
          color: #f43f5e;
        }
      }
    }
  }
}

/* Message Items */
.message-item {
  display: flex;
  gap: 8px;
  align-items: flex-start;

  .msg-avatar {
    width: 26px;
    height: 26px;
    border-radius: 8px;
    background: linear-gradient(135deg, #df266a, #9333ea);
    display: flex;
    align-items: center;
    justify-content: center;
    color: #ffffff;
    flex-shrink: 0;
    margin-top: 2px;

    &.admin-avatar {
      background: linear-gradient(135deg, #3b82f6, #6366f1);
    }
  }

  .msg-bubble-container {
    display: flex;
    flex-direction: column;
    max-width: calc(100% - 34px);

    .msg-time {
      font-size: 10px;
      color: var(--text-muted, #64748b);
      margin-top: 3px;
    }
  }

  .msg-bubble {
    padding: 10px 14px;
    border-radius: 14px;
    font-size: 12.5px;
    line-height: 1.5;
    word-break: break-word;
    position: relative;
  }

  &.user-msg {
    flex-direction: row-reverse;

    .msg-bubble-container {
      align-items: flex-end;
    }

    .msg-bubble {
      background: linear-gradient(135deg, #df266a 0%, #be185d 100%);
      color: #ffffff;
      border-bottom-right-radius: 4px;
    }
  }

  &.model-msg {
    .msg-bubble-container {
      align-items: flex-start;
    }

    .msg-bubble {
      background: rgba(255, 255, 255, 0.06);
      border: 1px solid rgba(255, 255, 255, 0.08);
      color: var(--text-primary, #f8fafc);
      border-bottom-left-radius: 4px;

      &.msg-error {
        background: rgba(239, 68, 68, 0.15);
        border-color: rgba(239, 68, 68, 0.3);
        color: #fca5a5;
      }
    }

    .btn-copy-msg {
      position: absolute;
      top: 6px;
      right: 6px;
      opacity: 0;
      width: 22px;
      height: 22px;
      border-radius: 4px;
      background: rgba(0, 0, 0, 0.4);
      border: 1px solid rgba(255, 255, 255, 0.1);
      color: var(--text-muted, #94a3b8);
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(223, 38, 106, 0.3);
        color: #ffffff;
      }
    }

    &:hover .btn-copy-msg {
      opacity: 1;
    }
  }
}

/* Typing Indicator */
.typing-msg {
  .typing-bubble {
    display: flex;
    align-items: center;
    gap: 4px;
    padding: 10px 14px;
    background: rgba(255, 255, 255, 0.06);
    border: 1px solid rgba(255, 255, 255, 0.08);

    .typing-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background: #df266a;
      animation: typingBounce 1.4s infinite ease-in-out both;

      &:nth-child(1) {
        animation-delay: -0.32s;
      }

      &:nth-child(2) {
        animation-delay: -0.16s;
      }
    }
  }
}

@keyframes typingBounce {

  0%,
  80%,
  100% {
    transform: scale(0);
    opacity: 0.4;
  }

  40% {
    transform: scale(1);
    opacity: 1;
  }
}

/* Follow-ups */
.followups-container {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-top: 4px;
  padding-left: 34px;

  .followup-header {
    display: flex;
    align-items: center;
    font-size: 11px;
    font-weight: 600;
    color: var(--text-muted, #94a3b8);
  }

  .followup-chips {
    display: flex;
    flex-direction: column;
    gap: 4px;

    .followup-chip {
      text-align: left;
      font-size: 11.5px;
      line-height: 1.35;
      padding: 6px 10px;
      border-radius: 8px;
      background: rgba(223, 38, 106, 0.08);
      border: 1px solid rgba(223, 38, 106, 0.25);
      color: #f43f5e;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(223, 38, 106, 0.2);
        border-color: #df266a;
        transform: translateX(3px);
      }
    }
  }
}

/* Footer Input Area */
.panel-footer {
  padding: 12px 16px 14px;
  background: linear-gradient(0deg, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 0) 100%),
    var(--bg-surface, #0f172a);
  border-top: 1px solid rgba(255, 255, 255, 0.08);

  .input-form {
    display: flex;
    align-items: center;
    gap: 8px;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 12px;
    padding: 4px 6px 4px 12px;
    transition: all 0.2s ease;

    &:focus-within {
      border-color: #df266a;
      box-shadow: 0 0 0 2px rgba(223, 38, 106, 0.2);
      background: rgba(255, 255, 255, 0.08);
    }

    .chat-input {
      flex: 1;
      background: transparent;
      border: none;
      outline: none;
      color: var(--text-primary, #f8fafc);
      font-size: 12.5px;

      &::placeholder {
        color: var(--text-muted, #64748b);
        font-size: 12px;
      }
    }

    .btn-send {
      width: 32px;
      height: 32px;
      border-radius: 8px;
      background: linear-gradient(135deg, #df266a, #9333ea);
      border: none;
      color: #ffffff;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        transform: scale(1.05);
        box-shadow: 0 4px 12px rgba(223, 38, 106, 0.4);
      }

      &:disabled {
        opacity: 0.4;
        cursor: not-allowed;
      }
    }
  }

  .footer-hint {
    margin-top: 6px;
    font-size: 10px;
    color: var(--text-muted, #64748b);
    text-align: center;
  }
}

/* Markdown Formatted Elements */
:deep(.model-markdown) {

  .chat-h3,
  .chat-h4,
  .chat-h5 {
    margin: 8px 0 4px;
    font-weight: 700;
    color: #f43f5e;
  }

  .chat-h3 {
    font-size: 14px;
  }

  .chat-h4 {
    font-size: 13px;
  }

  .chat-h5 {
    font-size: 12px;
  }

  .chat-li {
    margin-left: 14px;
    list-style-type: disc;
    margin-bottom: 3px;
  }

  .chat-link {
    color: #38bdf8;
    text-decoration: underline;

    &:hover {
      color: #7dd3fc;
    }
  }

  .chat-inline-code {
    background: rgba(0, 0, 0, 0.35);
    padding: 2px 5px;
    border-radius: 4px;
    font-family: monospace;
    font-size: 11.5px;
    color: #fb7185;
    border: 1px solid rgba(255, 255, 255, 0.08);
  }

  .chat-code-wrapper {
    margin: 8px 0;
    position: relative;

    .code-lang-tag {
      position: absolute;
      top: 6px;
      right: 8px;
      font-size: 9px;
      text-transform: uppercase;
      font-weight: 700;
      color: #94a3b8;
    }

    .chat-code-block {
      background: #020617;
      border: 1px solid rgba(255, 255, 255, 0.1);
      border-radius: 8px;
      padding: 10px 12px;
      overflow-x: auto;
      font-family: monospace;
      font-size: 11.5px;
      line-height: 1.45;
      color: #e2e8f0;
    }
  }
}
</style>
