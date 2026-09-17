<template>
  <q-dialog :model-value="modelValue" persistent @update:model-value="$emit('update:modelValue', $event)">
    <div v-if="contact" class="contact-detail-modal-card">
      <!-- 1. Modal Header -->
      <header class="modal-header">
        <div class="header-left">
          <div class="sender-avatar" :style="{ backgroundColor: getAvatarColor(contact.name) }">
            {{ getInitials(contact.name) }}
          </div>
          <div>
            <h3 class="modal-title">{{ contact.name }}</h3>
            <div class="sender-email font-mono">{{ contact.email }}</div>
          </div>
        </div>

        <div class="header-right">
          <span class="status-badge" :class="getStatusClass(contact.status)">
            <q-icon :name="getStatusIcon(contact.status)" size="11px" class="q-mr-xs" />
            <span>{{ getStatusLabel(contact.status) }}</span>
          </span>
          <button
            type="button"
            class="btn-modal-close"
            title="Đóng cửa sổ"
            @click="$emit('update:modelValue', false)"
          >
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
        </div>
      </header>

      <!-- 2. Modal Body -->
      <div class="modal-body">
        <!-- Message Subject -->
        <div class="meta-section">
          <div class="meta-item">
            <span class="meta-lbl">Chủ đề:</span>
            <span class="meta-val font-headline font-bold text-dark">{{ contact.subject || '(Không có chủ đề)' }}</span>
          </div>
          <div class="meta-item">
            <span class="meta-lbl">Thời gian gửi:</span>
            <span class="meta-val font-mono">{{ formatDate(contact.createdAt) }}</span>
          </div>
          <div v-if="contact.ipAddress" class="meta-item">
            <span class="meta-lbl">Địa chỉ IP:</span>
            <span class="meta-val font-mono">{{ contact.ipAddress }}</span>
          </div>
        </div>

        <!-- Full Message Content Box -->
        <div class="message-content-box">
          <div class="content-header">
            <span class="box-title">Nội dung tin nhắn:</span>
            <button
              type="button"
              class="btn-copy-msg"
              title="Sao chép nội dung"
              @click="copyMessage(contact.message)"
            >
              <q-icon name="fa-solid fa-copy" size="11px" class="q-mr-xs" />
              <span>Sao chép</span>
            </button>
          </div>
          <div class="content-text">{{ contact.message }}</div>
        </div>

        <!-- Status Changer & Admin Reply Note -->
        <div class="admin-control-section">
          <div class="status-control-row">
            <label class="control-label">Đổi trạng thái tin nhắn:</label>
            <div class="status-buttons-group">
              <button
                v-for="st in availableStatuses"
                :key="st.value"
                type="button"
                class="status-choice-btn"
                :class="{ active: currentStatus === st.value }"
                @click="currentStatus = st.value"
              >
                {{ st.label }}
              </button>
            </div>
          </div>

          <!-- Internal Admin Reply Note -->
          <div class="note-control-group">
            <label class="control-label">Ghi chú xử lý / Nội dung đã phản hồi:</label>
            <textarea
              v-model="replyNote"
              rows="2"
              placeholder="Nhập ghi chú hoặc tóm tắt nội dung bạn đã phản hồi..."
              class="note-textarea"
            ></textarea>
          </div>
        </div>
      </div>

      <!-- 3. Modal Footer Actions -->
      <footer class="modal-footer">
        <div class="footer-left">
          <button
            type="button"
            class="btn-action-delete"
            title="Xóa tin nhắn này"
            @click="$emit('delete-contact', contact)"
          >
            <q-icon name="fa-solid fa-trash-can" size="12px" class="q-mr-xs" />
            <span>Xóa</span>
          </button>
        </div>

        <div class="footer-right">
          <!-- Mailto Direct Reply Button -->
          <button
            type="button"
            class="btn-action-email-reply"
            title="Mở ứng dụng thư để trả lời"
            @click="openMailClient"
          >
            <q-icon name="fa-solid fa-paper-plane" size="12px" />
            <span>Gửi Email Phản Hồi</span>
          </button>

          <!-- Save Status & Note Button -->
          <button
            type="button"
            class="btn-action-save"
            :disabled="saving"
            @click="handleSaveStatus"
          >
            <q-spinner-tail v-if="saving" size="13px" color="white" class="q-mr-xs" />
            <q-icon v-else name="fa-solid fa-check" size="12px" class="q-mr-xs" />
            <span>{{ saving ? 'Đang lưu...' : 'Lưu Thay Đổi' }}</span>
          </button>
        </div>
      </footer>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import type { ContactMessage, ContactStatus } from "@/types/admin-contact";
import { adminContactService } from "@/services/admin-contact.service";
import { swalToast, swalError } from "@/utils/swal";
import { formatDateTime } from "@/utils/date";

const props = defineProps<{
  modelValue: boolean;
  contact: ContactMessage | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "status-updated", contact: ContactMessage): void;
  (e: "delete-contact", contact: ContactMessage): void;
}>();

const currentStatus = ref<ContactStatus>("Read");
const replyNote = ref("");
const saving = ref(false);

const availableStatuses: Array<{ value: ContactStatus; label: string }> = [
  { value: "Unread", label: "Chưa đọc" },
  { value: "Read", label: "Đã đọc" },
  { value: "Replied", label: "Đã phản hồi" },
  { value: "Archived", label: "Lưu trữ" }
];

watch(
  () => props.contact,
  (newVal) => {
    if (newVal) {
      currentStatus.value = newVal.status;
      replyNote.value = newVal.replyNote || "";
    }
  },
  { immediate: true }
);

async function handleSaveStatus() {
  if (!props.contact) return;
  try {
    saving.value = true;
    const res = await adminContactService.updateStatus(props.contact.id, {
      status: currentStatus.value,
      replyNote: replyNote.value.trim()
    });

    if (res.success && res.data) {
      swalToast("Cập nhật trạng thái thành công!", "success");
      emit("status-updated", res.data);
      emit("update:modelValue", false);
    }
  } catch (err: any) {
    swalError("Lỗi cập nhật", err.response?.data?.message || "Không thể cập nhật trạng thái.");
  } finally {
    saving.value = false;
  }
}

function openMailClient() {
  if (!props.contact) return;
  const email = encodeURIComponent(props.contact.email);
  const subject = encodeURIComponent(`Re: ${props.contact.subject || 'Liên hệ từ PDQ Portfolio'}`);
  const body = encodeURIComponent(
    `\n\n---\nTin nhắn gốc từ ${props.contact.name} (${props.contact.email}):\n"${props.contact.message}"`
  );
  window.open(`mailto:${email}?subject=${subject}&body=${body}`, "_blank");
}

async function copyMessage(text: string) {
  try {
    await navigator.clipboard.writeText(text);
    swalToast("Đã sao chép nội dung tin nhắn!", "success");
  } catch (e) {
    console.error("Failed to copy:", e);
  }
}

function getInitials(name: string): string {
  if (!name) return "U";
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return "U";
  const first = parts[0] || "U";
  if (parts.length === 1) return first.charAt(0).toUpperCase();
  const last = parts[parts.length - 1] || "";
  return (first.charAt(0) + last.charAt(0)).toUpperCase();
}

function getAvatarColor(name: string): string {
  const colors = ["#df266a", "#4f46e5", "#059669", "#d97706", "#0284c7", "#7c3aed"];
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  const index = Math.abs(hash) % colors.length;
  return colors[index] || "#4f46e5";
}

function getStatusLabel(status: ContactStatus): string {
  switch (status) {
    case "Unread": return "Chưa đọc";
    case "Read": return "Đã đọc";
    case "Replied": return "Đã phản hồi";
    case "Archived": return "Đã lưu trữ";
    default: return status;
  }
}

function getStatusClass(status: ContactStatus): string {
  switch (status) {
    case "Unread": return "badge-unread";
    case "Read": return "badge-read";
    case "Replied": return "badge-replied";
    case "Archived": return "badge-archived";
    default: return "";
  }
}

function getStatusIcon(status: ContactStatus): string {
  switch (status) {
    case "Unread": return "fa-solid fa-envelope";
    case "Read": return "fa-solid fa-envelope-open";
    case "Replied": return "fa-solid fa-reply";
    case "Archived": return "fa-solid fa-box-archive";
    default: return "fa-solid fa-circle";
  }
}

function formatDate(dateStr?: string | null): string {
  return formatDateTime(dateStr);
}
</script>

<style scoped lang="scss">
.contact-detail-modal-card {
  width: 600px;
  max-width: calc(100vw - 32px);
  background: #ffffff;
  border-radius: 18px;
  box-shadow: 0 20px 50px rgba(11, 19, 38, 0.25);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 1. Header */
.modal-header {
  padding: 18px 22px;
  background: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;

  .header-left {
    display: flex;
    align-items: center;
    gap: 12px;

    .sender-avatar {
      width: 42px;
      height: 42px;
      border-radius: 12px;
      color: #ffffff;
      font-family: var(--font-headline, sans-serif);
      font-size: 15px;
      font-weight: 800;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .modal-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 16px;
      font-weight: 700;
      color: #0b1326;
      margin: 0 0 2px;
    }

    .sender-email {
      font-size: 12px;
      color: #64748b;
    }
  }

  .header-right {
    display: flex;
    align-items: center;
    gap: 10px;

    .btn-modal-close {
      width: 32px;
      height: 32px;
      border-radius: 8px;
      border: 1px solid #e2e8f0;
      background: #ffffff;
      color: #64748b;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: #fee2e2;
        color: #e11d48;
        border-color: #fca5a5;
      }
    }
  }
}

/* 2. Body */
.modal-body {
  padding: 20px 22px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  max-height: calc(85vh - 140px);
  overflow-y: auto;
}

.meta-section {
  display: flex;
  flex-direction: column;
  gap: 6px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 12px 14px;

  .meta-item {
    display: flex;
    align-items: baseline;
    gap: 8px;
    font-size: 13px;

    .meta-lbl {
      color: #64748b;
      min-width: 90px;
    }

    .meta-val {
      color: #1e293b;
    }
  }
}

.message-content-box {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 14px 16px;

  .content-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 8px;

    .box-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 12.5px;
      font-weight: 700;
      color: #334155;
    }

    .btn-copy-msg {
      background: #f1f5f9;
      border: 1px solid #e2e8f0;
      border-radius: 6px;
      padding: 3px 8px;
      font-size: 11px;
      font-weight: 600;
      color: #475569;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      transition: all 0.2s ease;

      &:hover {
        background: #e2e8f0;
        color: #0b1326;
      }
    }
  }

  .content-text {
    font-size: 13.5px;
    color: #0b1326;
    line-height: 1.6;
    white-space: pre-wrap;
    background: #f8fafc;
    border-radius: 8px;
    padding: 12px;
    border: 1px dashed #cbd5e1;
  }
}

.admin-control-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  border-top: 1px solid #f1f5f9;
  padding-top: 14px;

  .control-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    color: #334155;
    margin-bottom: 4px;
  }

  .status-buttons-group {
    display: flex;
    gap: 8px;
    flex-wrap: wrap;

    .status-choice-btn {
      padding: 6px 14px;
      border-radius: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 12px;
      font-weight: 700;
      border: 1px solid #e2e8f0;
      background: #f8fafc;
      color: #475569;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: #f1f5f9;
        border-color: #cbd5e1;
      }

      &.active {
        background: #fdf2f6;
        border-color: #df266a;
        color: #df266a;
      }
    }
  }

  .note-textarea {
    width: 100%;
    background: #f8fafc;
    border: 1px solid #e2e8f0;
    border-radius: 8px;
    padding: 8px 12px;
    font-size: 12.5px;
    color: #0b1326;
    outline: none;
    resize: vertical;
    transition: all 0.2s ease;

    &:focus {
      background: #ffffff;
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }
  }
}

/* 3. Footer */
.modal-footer {
  padding: 14px 22px;
  background: #f8fafc;
  border-top: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;

  .btn-action-delete {
    height: 36px;
    padding: 0 14px;
    background: #ffffff;
    border: 1px solid #fecdd3;
    border-radius: 8px;
    color: #e11d48;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    transition: all 0.2s ease;

    &:hover {
      background: #fff1f2;
    }
  }

  .footer-right {
    display: flex;
    align-items: center;
    gap: 10px;

    .btn-action-email-reply {
      height: 36px;
      padding: 0 16px;
      background: #0b1326;
      border: none;
      border-radius: 8px;
      color: #ffffff;
      font-family: var(--font-headline, sans-serif);
      font-size: 12.5px;
      font-weight: 700;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 6px;
      transition: all 0.2s ease;

      &:hover {
        background: #1e293b;
        transform: translateY(-1px);
      }
    }

    .btn-action-save {
      height: 36px;
      padding: 0 16px;
      background: #df266a;
      border: none;
      border-radius: 8px;
      color: #ffffff;
      font-family: var(--font-headline, sans-serif);
      font-size: 12.5px;
      font-weight: 700;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 6px;
      box-shadow: 0 4px 12px rgba(223, 38, 106, 0.25);
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        background: #be185d;
        transform: translateY(-1px);
      }

      &:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }
    }
  }
}

/* Status Badges */
.status-badge {
  font-family: var(--font-headline, sans-serif);
  font-size: 11px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 6px;
  display: inline-flex;
  align-items: center;

  &.badge-unread {
    background: #fdf2f6;
    color: #df266a;
    border: 1px solid #fce7f3;
  }

  &.badge-read {
    background: #f1f5f9;
    color: #475569;
    border: 1px solid #e2e8f0;
  }

  &.badge-replied {
    background: #ecfdf5;
    color: #059669;
    border: 1px solid #d1fae5;
  }

  &.badge-archived {
    background: #f8fafc;
    color: #94a3b8;
    border: 1px solid #e2e8f0;
  }
}
</style>
