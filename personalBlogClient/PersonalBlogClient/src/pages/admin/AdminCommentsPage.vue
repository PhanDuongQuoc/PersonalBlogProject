<template>
  <q-page class="admin-comments-page">
    <!-- 1. Stats Quick Overview Cards -->
    <section class="comments-stats-grid">
      <!-- Card: Total Comments -->
      <div class="stat-card card-rose">
        <div class="stat-icon-box icon-rose">
          <q-icon name="fa-solid fa-comments" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalComments }}</div>
          <div class="stat-label">TỔNG BÌNH LUẬN</div>
        </div>
      </div>

      <!-- Card: Approved Comments -->
      <div class="stat-card card-emerald">
        <div class="stat-icon-box icon-emerald">
          <q-icon name="fa-solid fa-circle-check" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.approvedComments }}</div>
          <div class="stat-label">ĐÃ DUYỆT</div>
        </div>
      </div>

      <!-- Card: Pending Comments -->
      <div class="stat-card card-amber">
        <div class="stat-icon-box icon-amber">
          <q-icon name="fa-solid fa-hourglass-half" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.pendingComments }}</div>
          <div class="stat-label">CHỜ DUYỆT</div>
        </div>
      </div>

      <!-- Card: Spam / Rejected -->
      <div class="stat-card card-red">
        <div class="stat-icon-box icon-red">
          <q-icon name="fa-solid fa-ban" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.spamComments }}</div>
          <div class="stat-label">SPAM / TỪ CHỐI</div>
        </div>
      </div>
    </section>

    <!-- 2. Reusable Editorial Table Section -->
    <section class="comments-table-section">
      <AdminDataTable
        :items="comments"
        :columns="columns"
        :loading="loading"
        v-model:search="searchQuery"
        search-placeholder="Tìm kiếm theo tên độc giả, email, nội dung, bài viết..."
        :pagination="pagination"
        empty-title="Không có bình luận nào"
        empty-message="Chưa tìm thấy bình luận nào phù hợp với điều kiện tìm kiếm hoặc bộ lọc hiện tại."
        @page-change="onPageChange"
      >
        <!-- Single Toolbar Filters -->
        <template #filters>
          <!-- Status Dropdown Select -->
          <div class="filter-select-wrap">
            <select v-model="activeStatus" class="custom-filter-select" @change="onStatusChange">
              <option value="all">Tất cả trạng thái ({{ stats.totalComments }})</option>
              <option value="Approved">Đã duyệt ({{ stats.approvedComments }})</option>
              <option value="Pending">Chờ duyệt ({{ stats.pendingComments }})</option>
              <option value="Spam">Spam / Vi phạm ({{ stats.spamComments }})</option>
            </select>
          </div>

          <!-- Refresh Button -->
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchData">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loading }" />
          </button>
        </template>

        <!-- Custom Cell: Reader / Author -->
        <template #body-cell-author="{ row }">
          <div class="author-table-cell">
            <div class="author-avatar-circle">
              <img
                v-if="row.authorAvatar"
                :src="row.authorAvatar"
                :alt="row.authorName"
                class="avatar-img"
              />
              <span v-else class="avatar-fallback">
                {{ row.authorName.charAt(0).toUpperCase() }}
              </span>
            </div>
            <div class="author-meta-box">
              <div class="author-name-text" :title="row.authorName">
                {{ row.authorName }}
              </div>
              <div class="author-email-text" :title="row.authorEmail || ''">
                {{ row.authorEmail || '—' }}
              </div>
            </div>
          </div>
        </template>

        <!-- Custom Cell: Content Preview -->
        <template #body-cell-content="{ row }">
          <div
            class="comment-content-cell"
            :title="row.content"
            @click="openDetailDialog(row)"
          >
            {{ row.content }}
          </div>
        </template>

        <!-- Custom Cell: Target Post -->
        <template #body-cell-post="{ row }">
          <div class="post-table-cell">
            <a
              :href="`/posts/${row.postSlug}`"
              target="_blank"
              class="post-link-text"
              :title="row.postTitle"
            >
              <q-icon name="fa-solid fa-newspaper" size="11px" class="q-mr-xs text-muted" />
              <span>{{ row.postTitle }}</span>
            </a>
          </div>
        </template>

        <!-- Custom Cell: Status Pill -->
        <template #body-cell-status="{ row }">
          <button
            type="button"
            class="status-toggle-pill"
            :class="`status-${row.status.toLowerCase()}`"
            :title="`Trạng thái: ${getStatusLabel(row.status)}. Bấm để đổi trạng thái`"
            @click="toggleStatus(row)"
          >
            <span class="status-indicator-dot"></span>
            <span>{{ getStatusLabel(row.status) }}</span>
          </button>
        </template>

        <!-- Custom Cell: Created At -->
        <template #body-cell-createdAt="{ row }">
          <div class="date-cell">
            {{ formatDateTime(row.createdAt) }}
          </div>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <!-- View / Moderation Dialog -->
            <button
              type="button"
              class="table-action-btn btn-view"
              title="Xem chi tiết bình luận"
              @click="openDetailDialog(row)"
            >
              <q-icon name="fa-solid fa-eye" size="12px" />
            </button>

            <!-- Quick Approve if Pending -->
            <button
              v-if="row.status !== 'Approved'"
              type="button"
              class="table-action-btn btn-approve"
              title="Duyệt bình luận ngay"
              @click="quickUpdateStatus(row, 'Approved')"
            >
              <q-icon name="fa-solid fa-check" size="12px" />
            </button>

            <!-- Delete Comment -->
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa bình luận"
              @click="confirmDelete(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- 3. Comment Detail Modal Dialog -->
    <AdminCommentDetailDialog
      v-model="detailDialogOpen"
      :comment="selectedComment"
      @status-changed="onDetailStatusChanged"
      @deleted="onCommentDeleted"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from "vue";
import type { ColumnDef, TablePagination } from "@/components/admin/AdminDataTable.vue";
import AdminDataTable from "@/components/admin/AdminDataTable.vue";
import AdminCommentDetailDialog from "@/components/admin/AdminCommentDetailDialog.vue";
import type {
  AdminCommentSummary,
  AdminCommentStats
} from "@/types/admin-comment";
import { adminCommentService } from "@/services/admin-comment.service";
import { swalConfirm, swalSuccess, swalError, swalToast } from "@/utils/swal";

// Reactive States
const loading = ref(false);
const comments = ref<AdminCommentSummary[]>([]);

const stats = reactive<AdminCommentStats>({
  totalComments: 0,
  approvedComments: 0,
  pendingComments: 0,
  spamComments: 0
});

// Filters & Pagination
const searchQuery = ref("");
const activeStatus = ref("all");
const currentPage = ref(1);
const pageSize = 10;
const totalCount = ref(0);
const totalPages = ref(1);

// Dialog State
const detailDialogOpen = ref(false);
const selectedComment = ref<AdminCommentSummary | null>(null);

// Table Columns definition
const columns: ColumnDef[] = [
  { key: "author", label: "Độc giả", width: "22%" },
  { key: "content", label: "Nội dung bình luận", width: "32%" },
  { key: "post", label: "Bài viết liên quan", width: "20%" },
  { key: "status", label: "Trạng thái", width: "12%", align: "center" },
  { key: "createdAt", label: "Thời gian", width: "14%" },
  { key: "actions", label: "Thao tác", width: "100px", align: "right" }
];

const pagination = computed<TablePagination>(() => ({
  page: currentPage.value,
  pageSize,
  totalPages: totalPages.value,
  totalCount: totalCount.value
}));

// Fetch Stats
async function fetchStats() {
  try {
    const res = await adminCommentService.getStats();
    stats.totalComments = res.totalComments;
    stats.approvedComments = res.approvedComments;
    stats.pendingComments = res.pendingComments;
    stats.spamComments = res.spamComments;
  } catch (err) {
    console.error("Lỗi khi tải thống kê bình luận:", err);
  }
}

// Fetch Comments with debounce search
let searchTimer: number | null = null;
async function fetchData() {
  try {
    loading.value = true;
    const res = await adminCommentService.getComments({
      search: searchQuery.value,
      status: activeStatus.value,
      page: currentPage.value,
      pageSize
    });

    comments.value = res.items;
    totalCount.value = res.totalCount;
    totalPages.value = res.totalPages;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Không thể tải danh sách bình luận.";
    swalError("Lỗi kết nối", msg);
  } finally {
    loading.value = false;
  }
}

// Watch filters
watch(searchQuery, () => {
  if (searchTimer) clearTimeout(searchTimer);
  searchTimer = window.setTimeout(() => {
    currentPage.value = 1;
    fetchData();
  }, 350);
});

watch(activeStatus, () => {
  currentPage.value = 1;
  fetchData();
});

function onStatusChange() {
  currentPage.value = 1;
  fetchData();
}

function onPageChange(page: number) {
  currentPage.value = page;
  fetchData();
}

// Dialog opener
function openDetailDialog(comment: AdminCommentSummary) {
  selectedComment.value = comment;
  detailDialogOpen.value = true;
}

function onDetailStatusChanged() {
  fetchData();
  fetchStats();
}

function onCommentDeleted() {
  fetchData();
  fetchStats();
}

// Status Toggle Quick Action
async function toggleStatus(comment: AdminCommentSummary) {
  const nextStatus = comment.status === "Approved" ? "Pending" : "Approved";
  await quickUpdateStatus(comment, nextStatus);
}

async function quickUpdateStatus(comment: AdminCommentSummary, nextStatus: string) {
  try {
    await adminCommentService.updateStatus(comment.id, nextStatus);
    comment.status = nextStatus as any;
    const label = nextStatus === "Approved" ? "Đã duyệt" : nextStatus === "Pending" ? "Chuyển chờ duyệt" : "Đánh dấu Spam";
    swalToast(`${label} bình luận của ${comment.authorName}!`);
    fetchStats();
  } catch (err: any) {
    swalError("Không thể đổi trạng thái", err.response?.data?.message || "Lỗi hệ thống.");
  }
}

// Delete confirmation with SweetAlert2
async function confirmDelete(comment: AdminCommentSummary) {
  const confirmed = await swalConfirm({
    title: "Xóa bình luận này?",
    text: `Bạn có chắc chắn muốn xóa vĩnh viễn bình luận của "${comment.authorName}"?`,
    confirmButtonText: "Xóa vĩnh viễn",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });

  if (!confirmed) return;

  try {
    await adminCommentService.deleteComment(comment.id);
    swalSuccess("Đã xóa bình luận!");
    fetchData();
    fetchStats();
  } catch (err: any) {
    swalError("Lỗi khi xóa bình luận", err.response?.data?.message || "Không thể xóa bình luận.");
  }
}

// Helpers
function getStatusLabel(status: string): string {
  switch (status) {
    case "Approved":
      return "Đã duyệt";
    case "Pending":
      return "Chờ duyệt";
    case "Spam":
      return "Spam";
    case "Rejected":
      return "Từ chối";
    default:
      return status;
  }
}

function formatDateTime(dateStr: string): string {
  if (!dateStr) return "—";
  try {
    const d = new Date(dateStr);
    return d.toLocaleDateString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric"
    });
  } catch {
    return dateStr;
  }
}

onMounted(() => {
  fetchData();
  fetchStats();
});
</script>

<style scoped lang="scss">
.admin-comments-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 20px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Stats Grid */
.comments-stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 20px;
  flex-shrink: 0;

  .stat-card {
    background: #ffffff;
    border: 1px solid #e2e8f0;
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
        background: #fdf2f6;
        color: #df266a;
        border: 1px solid #fce7f3;
      }
      &.icon-emerald {
        background: #ecfdf5;
        color: #059669;
        border: 1px solid #d1fae5;
      }
      &.icon-amber {
        background: #fffbeb;
        color: #d97706;
        border: 1px solid #fef3c7;
      }
      &.icon-red {
        background: #fff1f2;
        color: #e11d48;
        border: 1px solid #ffe4e6;
      }
    }

    .stat-info {
      display: flex;
      flex-direction: column;

      .stat-value {
        font-family: var(--font-headline, sans-serif);
        font-size: 22px;
        font-weight: 800;
        color: #0b1326;
        line-height: 1.2;
      }

      .stat-label {
        font-family: var(--font-mono, monospace);
        font-size: 10.5px;
        font-weight: 700;
        color: #64748b;
        letter-spacing: 0.06em;
        margin-top: 2px;
      }
    }
  }
}

.comments-table-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

/* 2. Secondary Filters */
.filter-select-wrap {
  .custom-filter-select {
    height: 40px;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 10px;
    padding: 0 14px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: #0b1326;
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
  background: #ffffff;
  border: 1px solid #e2e8f0;
  color: #64748b;
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

/* 3. Table Cell Contents */
.author-table-cell {
  display: flex;
  align-items: center;
  gap: 12px;

  .author-avatar-circle {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    overflow: hidden;
    background: #eef2ff;
    border: 1px solid #c7d2fe;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;

    .avatar-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .avatar-fallback {
      font-family: var(--font-headline, sans-serif);
      font-size: 14px;
      font-weight: 800;
      color: #4f46e5;
    }
  }

  .author-meta-box {
    display: flex;
    flex-direction: column;
    min-width: 0;

    .author-name-text {
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: #0b1326;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .author-email-text {
      font-size: 11px;
      color: #64748b;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
}

.comment-content-cell {
  font-family: var(--font-body, sans-serif);
  font-size: 13px;
  color: #334155;
  line-height: 1.45;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  cursor: pointer;
  padding: 4px 6px;
  border-radius: 6px;
  transition: all 0.15s ease;

  &:hover {
    background: #f1f5f9;
    color: #0b1326;
  }
}

.post-table-cell {
  .post-link-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: #475569;
    text-decoration: none;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    line-height: 1.4;
    transition: color 0.15s ease;

    &:hover {
      color: #df266a;
    }
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

  &.status-approved {
    background: #ecfdf5;
    color: #059669;
    border: 1px solid #a7f3d0;

    .status-indicator-dot {
      background: #10b981;
      box-shadow: 0 0 6px rgba(16, 185, 129, 0.6);
    }

    &:hover {
      background: #d1fae5;
    }
  }

  &.status-pending {
    background: #fffbeb;
    color: #d97706;
    border: 1px solid #fef3c7;

    .status-indicator-dot {
      background: #f59e0b;
    }

    &:hover {
      background: #fef3c7;
    }
  }

  &.status-spam,
  &.status-rejected {
    background: #fff1f2;
    color: #e11d48;
    border: 1px solid #ffe4e6;

    .status-indicator-dot {
      background: #ef4444;
    }

    &:hover {
      background: #fee2e2;
    }
  }
}

.date-cell {
  font-family: var(--font-mono, monospace);
  font-size: 11.5px;
  color: #64748b;
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
    background: #ffffff;
    border: 1px solid #e2e8f0;
    color: #64748b;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.15s ease;

    &.btn-view:hover {
      color: #4f46e5;
      border-color: #4f46e5;
      background: #eef2ff;
    }

    &.btn-approve:hover {
      color: #059669;
      border-color: #059669;
      background: #ecfdf5;
    }

    &.btn-delete:hover {
      color: #e11d48;
      border-color: #e11d48;
      background: #fff1f2;
    }
  }
}
</style>
