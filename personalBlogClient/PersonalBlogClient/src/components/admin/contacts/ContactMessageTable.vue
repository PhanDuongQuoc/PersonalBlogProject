<template>
  <div class="contact-table-card">
    <div class="table-container">
      <table class="custom-admin-table">
        <thead>
          <tr>
            <th class="th-sender">Người Gửi</th>
            <th class="th-subject">Tiêu Đề & Nội Dung</th>
            <th class="th-status">Trạng Thái</th>
            <th class="th-date">Thời Gian</th>
            <th class="th-actions text-right">Thao Tác</th>
          </tr>
        </thead>

        <tbody>
          <!-- Loading State -->
          <tr v-if="loading">
            <td colspan="5" class="empty-state-cell">
              <q-spinner-tail color="pink-7" size="36px" />
              <div class="empty-text">Đang tải danh sách tin nhắn liên hệ...</div>
            </td>
          </tr>

          <!-- Empty State -->
          <tr v-else-if="items.length === 0">
            <td colspan="5" class="empty-state-cell">
              <div class="empty-icon-box">
                <q-icon name="fa-solid fa-inbox" size="28px" class="text-grey-5" />
              </div>
              <div class="empty-title">Không tìm thấy tin nhắn liên hệ nào</div>
              <div class="empty-desc">Hộp thư sẽ hiển thị các yêu cầu liên hệ từ độc giả hoặc nhà tuyển dụng.</div>
            </td>
          </tr>

          <!-- Data Rows -->
          <tr
            v-for="item in items"
            v-else
            :key="item.id"
            class="table-data-row"
            :class="{
              'row-unread': item.status === 'Unread',
              'row-highlight-new': item.id === highlightedId
            }"
            @click="$emit('select-message', item)"
          >
            <!-- 1. Sender (Avatar Initials + Name + Email) -->
            <td class="td-sender">
              <div class="sender-cell-content">
                <div class="sender-avatar-initials" :style="{ backgroundColor: getAvatarColor(item.name) }">
                  {{ getInitials(item.name) }}
                </div>
                <div class="sender-meta">
                  <div class="sender-name">
                    <span>{{ item.name }}</span>
                    <span v-if="item.status === 'Unread'" class="unread-pulse-dot" title="Tin nhắn mới chưa đọc"></span>
                  </div>
                  <div class="sender-email font-mono">{{ item.email }}</div>
                </div>
              </div>
            </td>

            <!-- 2. Subject & Snippet -->
            <td class="td-subject">
              <div class="subject-cell-content">
                <div class="subject-title">
                  {{ item.subject || '(Không có tiêu đề)' }}
                </div>
                <div class="message-snippet">
                  {{ truncate(item.message, 90) }}
                </div>
              </div>
            </td>

            <!-- 3. Status Badge -->
            <td class="td-status">
              <span class="status-badge" :class="getStatusClass(item.status)">
                <q-icon :name="getStatusIcon(item.status)" size="11px" class="q-mr-xs" />
                <span>{{ getStatusLabel(item.status) }}</span>
              </span>
            </td>

            <!-- 4. Date & Time -->
            <td class="td-date">
              <div class="date-cell-content font-mono">
                <div class="date-main">{{ formatDate(item.createdAt) }}</div>
                <div class="date-relative">{{ formatTimeAgo(item.createdAt) }}</div>
              </div>
            </td>

            <!-- 5. Actions -->
            <td class="td-actions text-right" @click.stop>
              <div class="row-actions-group">
                <!-- View Detail -->
                <button
                  type="button"
                  class="btn-row-action btn-view"
                  title="Xem chi tiết tin nhắn"
                  @click="$emit('select-message', item)"
                >
                  <q-icon name="fa-solid fa-eye" size="12px" />
                </button>

                <!-- Mailto Reply -->
                <button
                  type="button"
                  class="btn-row-action btn-reply"
                  title="Gửi email phản hồi trực tiếp"
                  @click="$emit('quick-reply', item)"
                >
                  <q-icon name="fa-solid fa-envelope" size="12px" />
                </button>

                <!-- Delete -->
                <button
                  type="button"
                  class="btn-row-action btn-delete"
                  title="Xóa tin nhắn"
                  @click="$emit('delete-message', item)"
                >
                  <q-icon name="fa-solid fa-trash-can" size="12px" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination Footer -->
    <div v-if="totalPages > 1 || totalCount > 0" class="table-pagination-footer">
      <div class="pagination-info">
        Hiển thị <strong>{{ items.length }}</strong> trên tổng số <strong>{{ totalCount }}</strong> tin nhắn
      </div>

      <div class="pagination-controls">
        <q-pagination
          :model-value="pageIndex"
          :max="totalPages"
          :max-pages="5"
          color="pink-7"
          size="13px"
          direction-links
          boundary-links
          @update:model-value="$emit('change-page', $event)"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ContactMessage, ContactStatus } from "@/types/admin-contact";

defineProps<{
  items: ContactMessage[];
  loading: boolean;
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  highlightedId?: number | null;
}>();

defineEmits<{
  (e: "select-message", item: ContactMessage): void;
  (e: "quick-reply", item: ContactMessage): void;
  (e: "delete-message", item: ContactMessage): void;
  (e: "change-page", page: number): void;
}>();

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

function truncate(str?: string | null, len = 90): string {
  if (!str) return "";
  return str.length > len ? str.slice(0, len) + "..." : str;
}

function formatDate(dateStr?: string | null): string {
  if (!dateStr) return "—";
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return "—";
  return d.toLocaleDateString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  });
}

function formatTimeAgo(dateStr?: string | null): string {
  if (!dateStr) return "";
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return "";
  const now = new Date();
  const diffSec = Math.floor((now.getTime() - d.getTime()) / 1000);

  if (diffSec < 60) return "Vừa xong";
  if (diffSec < 3600) return `${Math.floor(diffSec / 60)} phút trước`;
  if (diffSec < 86400) return `${Math.floor(diffSec / 3600)} giờ trước`;
  if (diffSec < 604800) return `${Math.floor(diffSec / 86400)} ngày trước`;
  return "";
}
</script>

<style scoped lang="scss">
.contact-table-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 4px 20px rgba(11, 19, 38, 0.03);
  overflow: hidden;
}

.table-container {
  overflow-x: auto;
}

.custom-admin-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;

  thead tr {
    background: #f8fafc;
    border-bottom: 1px solid #e2e8f0;

    th {
      padding: 12px 16px;
      font-family: var(--font-headline, sans-serif);
      font-size: 12px;
      font-weight: 700;
      color: #475569;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      white-space: nowrap;
    }
  }

  tbody tr.table-data-row {
    border-bottom: 1px solid #f1f5f9;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      background: #fdf2f6;
    }

    &.row-unread {
      background: #fffdfd;
      .sender-name span {
        font-weight: 800;
        color: #0b1326;
      }
      .subject-title {
        font-weight: 800;
        color: #0b1326;
      }
    }

    &.row-highlight-new {
      animation: highlightGlow 3s ease-out;
    }

    td {
      padding: 14px 16px;
      vertical-align: middle;
      font-size: 13px;
    }
  }
}

@keyframes highlightGlow {
  0% {
    background: #fce7f3;
  }
  100% {
    background: #ffffff;
  }
}

/* 1. Sender Cell */
.sender-cell-content {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 180px;

  .sender-avatar-initials {
    width: 36px;
    height: 36px;
    border-radius: 10px;
    color: #ffffff;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 800;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
  }

  .sender-meta {
    .sender-name {
      display: flex;
      align-items: center;
      gap: 6px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: #1e293b;

      .unread-pulse-dot {
        width: 7px;
        height: 7px;
        border-radius: 50%;
        background: #df266a;
      }
    }

    .sender-email {
      font-size: 11.5px;
      color: #64748b;
      margin-top: 1px;
    }
  }
}

/* 2. Subject Cell */
.subject-cell-content {
  min-width: 260px;
  max-width: 420px;

  .subject-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 700;
    color: #334155;
    margin-bottom: 2px;
  }

  .message-snippet {
    font-size: 12px;
    color: #64748b;
    line-height: 1.4;
  }
}

/* 3. Status Badge */
.status-badge {
  font-family: var(--font-headline, sans-serif);
  font-size: 11px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 6px;
  display: inline-flex;
  align-items: center;
  white-space: nowrap;

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

/* 4. Date Cell */
.date-cell-content {
  font-size: 12px;
  color: #475569;
  white-space: nowrap;

  .date-relative {
    font-size: 11px;
    color: #94a3b8;
  }
}

/* 5. Row Action Buttons */
.row-actions-group {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 6px;

  .btn-row-action {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    border: 1px solid #e2e8f0;
    background: #ffffff;
    color: #64748b;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      transform: translateY(-1px);
    }

    &.btn-view:hover {
      background: #eff6ff;
      border-color: #dbeafe;
      color: #2563eb;
    }

    &.btn-reply:hover {
      background: #fdf2f6;
      border-color: #fce7f3;
      color: #df266a;
    }

    &.btn-delete:hover {
      background: #fff1f2;
      border-color: #fecdd3;
      color: #e11d48;
    }
  }
}

/* Empty State */
.empty-state-cell {
  text-align: center;
  padding: 60px 20px !important;

  .empty-icon-box {
    width: 54px;
    height: 54px;
    margin: 0 auto 12px;
    border-radius: 14px;
    background: #f8fafc;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .empty-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 15px;
    font-weight: 700;
    color: #334155;
    margin-bottom: 4px;
  }

  .empty-desc {
    font-size: 12.5px;
    color: #94a3b8;
  }

  .empty-text {
    margin-top: 10px;
    font-size: 13px;
    color: #64748b;
  }
}

/* Pagination Footer */
.table-pagination-footer {
  padding: 12px 16px;
  background: #f8fafc;
  border-top: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 12px;

  .pagination-info {
    font-size: 12.5px;
    color: #64748b;
  }
}
</style>
