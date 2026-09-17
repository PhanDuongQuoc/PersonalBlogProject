<template>
  <q-page class="admin-contacts-page">
    <!-- 1. Stats Quick Overview Cards -->
    <section class="contacts-stats-grid">
      <!-- Total Messages -->
      <div class="stat-card card-rose">
        <div class="stat-icon-box icon-rose">
          <q-icon name="fa-solid fa-inbox" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ summary.totalMessages }}</div>
          <div class="stat-label">TỔNG TIN NHẮN</div>
        </div>
      </div>

      <!-- Unread Messages -->
      <div class="stat-card card-red">
        <div class="stat-icon-box icon-red">
          <q-icon name="fa-solid fa-envelope" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ summary.unreadMessages }}</div>
          <div class="stat-label">CHƯA ĐỌC</div>
        </div>
      </div>

      <!-- Replied Messages -->
      <div class="stat-card card-emerald">
        <div class="stat-icon-box icon-emerald">
          <q-icon name="fa-solid fa-reply-all" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ summary.repliedMessages }}</div>
          <div class="stat-label">ĐÃ PHẢN HỒI</div>
        </div>
      </div>

      <!-- This Month Messages -->
      <div class="stat-card card-indigo">
        <div class="stat-icon-box icon-indigo">
          <q-icon name="fa-solid fa-calendar-check" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ summary.thisMonthMessages }}</div>
          <div class="stat-label">TRONG THÁNG</div>
        </div>
      </div>
    </section>

    <!-- 2. Shared Editorial Data Table Section (Full Height) -->
    <section class="contacts-table-section">
      <AdminDataTable :items="messages" :columns="columns" :loading="loading" v-model:search="keywordSearch"
        search-placeholder="Tìm kiếm theo tên người gửi, email, tiêu đề, nội dung..." :pagination="pagination"
        empty-title="Hộp thư trống"
        empty-message="Chưa tìm thấy tin nhắn nào phù hợp với điều kiện tìm kiếm hoặc bộ lọc hiện tại."
        @page-change="handleChangePage" @update:search="handleSearchInput">
        <!-- Single Toolbar Filters -->
        <template #filters>
          <!-- Status Dropdown Select -->
          <div class="filter-select-wrap">
            <select v-model="statusFilter" class="custom-filter-select" @change="onStatusFilterChange">
              <option value="All">Tất cả trạng thái ({{ summary.totalMessages }})</option>
              <option value="Unread">Chưa đọc ({{ summary.unreadMessages }})</option>
              <option value="Read">Đã đọc</option>
              <option value="Replied">Đã phản hồi ({{ summary.repliedMessages }})</option>
              <option value="Archived">Đã lưu trữ</option>
            </select>
          </div>

          <!-- Refresh Button -->
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchData">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loading }" />
          </button>
        </template>

        <!-- Custom Cell: Sender (Avatar + Name + Email) -->
        <template #body-cell-sender="{ row }">
          <div class="sender-table-cell" @click="handleSelectMessage(row)">
            <div class="sender-avatar-circle" :style="{ background: getAvatarColor(row.name) }">
              <span class="avatar-fallback">{{ getInitials(row.name) }}</span>
            </div>
            <div class="sender-meta-box">
              <div class="sender-name-text" :title="row.name">
                <span>{{ row.name }}</span>
                <span v-if="row.status === 'Unread'" class="unread-pulse-dot" title="Tin nhắn mới"></span>
              </div>
              <div class="sender-email-text" :title="row.email">
                {{ row.email }}
              </div>
            </div>
          </div>
        </template>

        <!-- Custom Cell: Subject & Content Excerpt -->
        <template #body-cell-subject="{ row }">
          <div class="subject-content-cell" @click="handleSelectMessage(row)">
            <div class="subject-title-line" :class="{ 'is-unread': row.status === 'Unread' }">
              {{ row.subject || 'Liên hệ từ độc giả' }}
            </div>
            <div class="content-excerpt-line" :title="row.message">
              {{ row.message }}
            </div>
          </div>
        </template>

        <!-- Custom Cell: Status Pill -->
        <template #body-cell-status="{ row }">
          <button type="button" class="status-toggle-pill" :class="`status-${row.status.toLowerCase()}`"
            :title="`Trạng thái: ${getStatusLabel(row.status)}. Bấm để xem chi tiết`" @click="handleSelectMessage(row)">
            <span class="status-indicator-dot"></span>
            <span>{{ getStatusLabel(row.status) }}</span>
          </button>
        </template>

        <!-- Custom Cell: Created At -->
        <template #body-cell-createdAt="{ row }">
          <div class="date-cell" :title="formatFullDateTime(row.createdAt)">
            {{ formatDateTime(row.createdAt) }}
          </div>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <!-- View / Detail Dialog -->
            <button type="button" class="table-action-btn btn-view" title="Xem chi tiết tin nhắn"
              @click="handleSelectMessage(row)">
              <q-icon name="fa-solid fa-eye" size="12px" />
            </button>

            <!-- Quick Reply via Mailto -->
            <button type="button" class="table-action-btn btn-reply" title="Phản hồi qua Email"
              @click="handleQuickReply(row)">
              <q-icon name="fa-solid fa-reply" size="12px" />
            </button>

            <!-- Delete Contact -->
            <button type="button" class="table-action-btn btn-delete" title="Xóa tin nhắn"
              @click="handleConfirmDelete(row)">
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- 4. Detail & Reply Modal Dialog -->
    <ContactDetailModal v-model="showDetailModal" :contact="selectedMessage" @status-updated="handleStatusUpdated"
      @delete-contact="handleConfirmDelete" />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from "vue";
import { useQuasar } from "quasar";
import type { ColumnDef, TablePagination } from "@/components/admin/AdminDataTable.vue";
import AdminDataTable from "@/components/admin/AdminDataTable.vue";
import ContactDetailModal from "@/components/admin/contacts/ContactDetailModal.vue";
import type { ContactMessage, ContactSummary, ContactStatus } from "@/types/admin-contact";
import { adminContactService } from "@/services/admin-contact.service";
import { signalRService } from "@/services/signalr.service";
import { swalConfirm, swalToast, swalError } from "@/utils/swal";
import { formatDateTime, formatFullDateTime } from "@/utils/date";
import { replyViaEmail } from "@/utils/email";

const $q = useQuasar();

// Reactive States
const loading = ref(false);
const statusFilter = ref("All");
const keywordSearch = ref("");
const pageIndex = ref(1);
const pageSize = 10;
const totalCount = ref(0);
const totalPages = ref(1);

const messages = ref<ContactMessage[]>([]);

const summary = reactive<ContactSummary>({
  totalMessages: 0,
  unreadMessages: 0,
  repliedMessages: 0,
  thisMonthMessages: 0
});

// Dialog State
const showDetailModal = ref(false);
const selectedMessage = ref<ContactMessage | null>(null);

// Column Definitions for shared AdminDataTable
const columns: ColumnDef[] = [
  { key: "sender", label: "Người gửi", width: "23%" },
  { key: "subject", label: "Tiêu đề & Nội dung", width: "37%" },
  { key: "status", label: "Trạng thái", width: "13%", align: "center" },
  { key: "createdAt", label: "Thời gian", width: "15%" },
  { key: "actions", label: "Thao tác", width: "120px", align: "right" }
];

const pagination = computed<TablePagination>(() => ({
  page: pageIndex.value,
  pageSize,
  totalPages: totalPages.value,
  totalCount: totalCount.value
}));

let searchDebounceTimer: any = null;
function handleSearchInput(val: string) {
  keywordSearch.value = val;
  clearTimeout(searchDebounceTimer);
  searchDebounceTimer = setTimeout(() => {
    pageIndex.value = 1;
    fetchData();
  }, 350);
}

function onStatusFilterChange() {
  pageIndex.value = 1;
  fetchData();
}

function handleChangePage(page: number) {
  pageIndex.value = page;
  fetchData();
}

async function fetchData() {
  try {
    loading.value = true;
    const [listRes, summaryRes] = await Promise.all([
      adminContactService.getContacts({
        pageIndex: pageIndex.value,
        pageSize,
        status: statusFilter.value,
        keyword: keywordSearch.value.trim() || undefined
      }),
      adminContactService.getSummary()
    ]);

    messages.value = listRes.items || [];
    totalCount.value = listRes.totalCount;
    totalPages.value = listRes.totalPages;
    pageIndex.value = listRes.pageIndex;

    summary.totalMessages = summaryRes.totalMessages;
    summary.unreadMessages = summaryRes.unreadMessages;
    summary.repliedMessages = summaryRes.repliedMessages;
    summary.thisMonthMessages = summaryRes.thisMonthMessages;
  } catch (err: any) {
    console.error("Failed to fetch contact messages:", err);
    swalError("Lỗi tải dữ liệu", err.response?.data?.message || "Không thể tải danh sách tin nhắn.");
  } finally {
    loading.value = false;
  }
}

async function handleSelectMessage(item: ContactMessage) {
  selectedMessage.value = item;
  showDetailModal.value = true;

  // If unread, auto-mark as read on backend and decrement counter
  if (item.status === "Unread") {
    try {
      const updated = await adminContactService.getContactById(item.id, true);
      if (updated) {
        item.status = "Read";
        if (summary.unreadMessages > 0) summary.unreadMessages--;
      }
    } catch (e) {
      console.warn("Could not mark message as read:", e);
    }
  }
}

function handleQuickReply(item: ContactMessage) {
  replyViaEmail({
    email: item.email,
    name: item.name,
    subject: item.subject,
    message: item.message
  });
}

function handleStatusUpdated(updated: ContactMessage) {
  const idx = messages.value.findIndex((m) => m.id === updated.id);
  if (idx !== -1) {
    messages.value[idx] = updated;
  }
  adminContactService.getSummary().then((s) => {
    Object.assign(summary, s);
  });
}

async function handleConfirmDelete(item: ContactMessage) {
  const confirmed = await swalConfirm({
    title: "Xác nhận xóa tin nhắn?",
    text: `Bạn có chắc chắn muốn xóa tin nhắn từ "${item.name}" (${item.email}) không? Hành động này không thể hoàn tác!`,
    confirmButtonText: "Xóa ngay",
    cancelButtonText: "Hủy",
    isDanger: true
  });

  if (!confirmed) return;

  try {
    const res = await adminContactService.deleteContact(item.id);
    if (res.success) {
      swalToast(res.message, "success");
      showDetailModal.value = false;
      fetchData();
    }
  } catch (err: any) {
    swalError("Lỗi khi xóa", err.response?.data?.message || "Không thể xóa tin nhắn.");
  }
}

// -------------------------------------------------------------
// Avatar & Format Utilities
// -------------------------------------------------------------
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


// -------------------------------------------------------------
// SignalR Real-time Listeners
// -------------------------------------------------------------
function playNotificationSound() {
  try {
    const audioCtx = new (window.AudioContext || (window as any).webkitAudioContext)();
    const osc = audioCtx.createOscillator();
    const gain = audioCtx.createGain();
    osc.type = "sine";
    osc.frequency.setValueAtTime(587.33, audioCtx.currentTime); // D5
    osc.frequency.setValueAtTime(880, audioCtx.currentTime + 0.1); // A5
    gain.gain.setValueAtTime(0.2, audioCtx.currentTime);
    gain.gain.exponentialRampToValueAtTime(0.01, audioCtx.currentTime + 0.3);
    osc.connect(gain);
    gain.connect(audioCtx.destination);
    osc.start();
    osc.stop(audioCtx.currentTime + 0.35);
  } catch (e) {
    // Audio Context might be restricted before interaction
  }
}

function onRealtimeNewMessage(msg: ContactMessage) {
  // 1. Play chime sound
  playNotificationSound();

  // 2. Show Quasar floating toast
  $q.notify({
    type: "positive",
    message: `📬 Tin nhắn mới từ ${msg.name}: "${msg.subject || 'Liên hệ'}"`,
    caption: msg.email,
    position: "top-right",
    timeout: 6000,
    actions: [
      {
        label: "Xem",
        color: "white",
        handler: () => {
          handleSelectMessage(msg);
        }
      }
    ]
  });

  // 3. Update summary counters
  summary.totalMessages++;
  summary.unreadMessages++;
  summary.thisMonthMessages++;

  // 4. If current filter includes new messages, prepend row to table
  if (statusFilter.value === "All" || statusFilter.value === "Unread") {
    messages.value.unshift(msg);
    totalCount.value++;
  }
}

function onRealtimeStatusUpdate(data: { id: number; status: string }) {
  const item = messages.value.find((m) => m.id === data.id);
  if (item) {
    item.status = data.status as any;
  }
}

onMounted(() => {
  fetchData();
  signalRService.start();
  signalRService.onNewContactMessage(onRealtimeNewMessage);
  signalRService.onContactStatusUpdate(onRealtimeStatusUpdate);
});

onUnmounted(() => {
  clearTimeout(searchDebounceTimer);
  signalRService.offNewContactMessage(onRealtimeNewMessage);
  signalRService.offContactStatusUpdate(onRealtimeStatusUpdate);
});
</script>

<style scoped lang="scss">
.admin-contacts-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 20px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Stats Overview Grid */
.contacts-stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 20px;
  flex-shrink: 0;

  .stat-card {
    background: var(--bg-surface-low, #ffffff);
    border: 1px solid var(--border-hairline, #e2e8f0);
    border-radius: 14px;
    padding: 18px 20px;
    display: flex;
    align-items: center;
    gap: 16px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);
    transition: transform 0.2s ease;

    &:hover {
      transform: translateY(-2px);
    }

    .stat-icon-box {
      width: 44px;
      height: 44px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;

      &.icon-rose {
        background: rgba(223, 38, 106, 0.12);
        color: #df266a;
        border: 1px solid rgba(223, 38, 106, 0.25);
      }
      &.icon-indigo {
        background: rgba(99, 102, 241, 0.12);
        color: #818cf8;
        border: 1px solid rgba(99, 102, 241, 0.25);
      }
      &.icon-emerald {
        background: rgba(16, 185, 129, 0.12);
        color: #10b981;
        border: 1px solid rgba(16, 185, 129, 0.25);
      }
      &.icon-slate {
        background: var(--bg-surface-high, #f1f5f9);
        color: var(--text-muted, #64748b);
        border: 1px solid var(--border-hairline, #e2e8f0);
      }
    }

    .stat-info {
      display: flex;
      flex-direction: column;

      .stat-value {
        font-family: var(--font-headline, sans-serif);
        font-size: 22px;
        font-weight: 800;
        color: var(--text-primary, #0b1326);
        line-height: 1.2;
      }

      .stat-label {
        font-family: var(--font-mono, monospace);
        font-size: 10.5px;
        font-weight: 700;
        color: var(--text-muted, #64748b);
        letter-spacing: 0.06em;
        margin-top: 2px;
      }
    }
  }
}

/* 2. Full Height Table Section */
.contacts-table-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

/* 3. Filter Toolbar Select */
.filter-select-wrap {
  .custom-filter-select {
    height: 40px;
    background: var(--bg-surface-low, #ffffff);
    border: 1px solid var(--border-hairline, #e2e8f0);
    border-radius: 10px;
    padding: 0 14px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: var(--text-primary, #0b1326);
    outline: none;
    cursor: pointer;
    transition: all 0.2s ease;

    &:focus {
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }
  }
}

.btn-refresh {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background: var(--bg-surface-low, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  color: var(--text-secondary, #64748b);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    color: #df266a;
    border-color: #df266a;
  }
}

/* 4. Table Cell Contents */
.sender-table-cell {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  padding: 2px 0;

  .sender-avatar-circle {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.12);

    .avatar-fallback {
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 800;
      color: #ffffff;
      text-transform: uppercase;
    }
  }

  .sender-meta-box {
    display: flex;
    flex-direction: column;
    min-width: 0;

    .sender-name-text {
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: var(--text-primary, #0b1326);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      display: flex;
      align-items: center;
      gap: 6px;

      .unread-pulse-dot {
        width: 6px;
        height: 6px;
        border-radius: 50%;
        background: #df266a;
        box-shadow: 0 0 6px rgba(223, 38, 106, 0.8);
      }
    }

    .sender-email-text {
      font-size: 11px;
      color: var(--text-muted, #64748b);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
}

.subject-content-cell {
  cursor: pointer;
  padding: 4px 6px;
  border-radius: 6px;
  transition: background 0.15s ease;

  &:hover {
    background: rgba(255, 255, 255, 0.04);
  }

  .subject-title-line {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: var(--text-secondary, #1e293b);
    margin-bottom: 2px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

    &.is-unread {
      font-weight: 800;
      color: var(--text-primary, #0b1326);
    }
  }

  .content-excerpt-line {
    font-family: var(--font-body, sans-serif);
    font-size: 12px;
    color: var(--text-muted, #64748b);
    line-height: 1.4;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}

.status-toggle-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  border-radius: 9999px;
  font-family: var(--font-mono, monospace);
  font-size: 11px;
  font-weight: 700;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;

  .status-indicator-dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
  }

  &.status-unread {
    background: rgba(225, 29, 72, 0.15);
    color: #fb7185;
    border: 1px solid rgba(225, 29, 72, 0.3);

    .status-indicator-dot {
      background: #df266a;
      box-shadow: 0 0 6px rgba(223, 38, 106, 0.6);
    }

    &:hover {
      background: rgba(225, 29, 72, 0.25);
    }
  }

  &.status-read {
    background: rgba(99, 102, 241, 0.15);
    color: #818cf8;
    border: 1px solid rgba(99, 102, 241, 0.3);

    .status-indicator-dot {
      background: #6366f1;
    }

    &:hover {
      background: rgba(99, 102, 241, 0.25);
    }
  }

  &.status-replied {
    background: rgba(16, 185, 129, 0.15);
    color: #34d399;
    border: 1px solid rgba(16, 185, 129, 0.3);

    .status-indicator-dot {
      background: #10b981;
      box-shadow: 0 0 6px rgba(16, 185, 129, 0.6);
    }

    &:hover {
      background: rgba(16, 185, 129, 0.25);
    }
  }

  &.status-archived {
    background: var(--bg-surface-high, #f1f5f9);
    color: var(--text-muted, #64748b);
    border: 1px solid var(--border-hairline, #e2e8f0);

    .status-indicator-dot {
      background: #94a3b8;
    }

    &:hover {
      background: var(--bg-surface-highest, #e2e8f0);
    }
  }
}

.date-cell {
  font-family: var(--font-mono, monospace);
  font-size: 11.5px;
  color: var(--text-secondary, #64748b);
}

/* Table Actions */
.table-actions-cell {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 6px;

  .table-action-btn {
    width: 30px;
    height: 30px;
    border-radius: 8px;
    background: var(--bg-surface-low, #ffffff);
    border: 1px solid var(--border-hairline, #e2e8f0);
    color: var(--text-secondary, #64748b);
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.15s ease;

    &.btn-view:hover {
      color: #818cf8;
      border-color: #818cf8;
      background: rgba(99, 102, 241, 0.12);
    }

    &.btn-reply:hover {
      color: #34d399;
      border-color: #34d399;
      background: rgba(16, 185, 129, 0.12);
    }

    &.btn-delete:hover {
      color: #e11d48;
      border-color: #e11d48;
      background: #fff1f2;
    }
  }
}
</style>
