<template>
  <q-page class="admin-tags-page">
    <!-- 1. Stats Quick Overview Cards -->
    <section class="tags-stats-grid">
      <!-- Card: Total Tags -->
      <div class="stat-card card-rose">
        <div class="stat-icon-box icon-rose">
          <q-icon name="fa-solid fa-tags" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalTags }}</div>
          <div class="stat-label">TỔNG THẺ TAGS</div>
        </div>
      </div>

      <!-- Card: Total Assigned Posts -->
      <div class="stat-card card-emerald">
        <div class="stat-icon-box icon-emerald">
          <q-icon name="fa-solid fa-newspaper" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalAssignedPosts }}</div>
          <div class="stat-label">LƯỢT GÁN BÀI VIẾT</div>
        </div>
      </div>

      <!-- Card: Empty Tags -->
      <div class="stat-card card-amber">
        <div class="stat-icon-box icon-amber">
          <q-icon name="fa-solid fa-tag" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.tagsWithNoPosts }}</div>
          <div class="stat-label">THẺ CHƯA DÙNG</div>
        </div>
      </div>

      <!-- Card: Top Active Tag -->
      <div class="stat-card card-indigo">
        <div class="stat-icon-box icon-indigo">
          <q-icon name="fa-solid fa-fire" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value top-tag-title" :title="stats.topTagName ? `#${stats.topTagName}` : 'Chưa có'">
            {{ stats.topTagName ? `#${stats.topTagName}` : 'Chưa có' }}
          </div>
          <div class="stat-label">
            PHỔ BIẾN NHẤT ({{ stats.topTagPostCount }} BÀI)
          </div>
        </div>
      </div>
    </section>

    <!-- 2. Reusable Editorial Table Section -->
    <section class="tags-table-section">
      <AdminDataTable
        :items="tags"
        :columns="columns"
        :loading="loading"
        v-model:search="searchQuery"
        search-placeholder="Tìm kiếm thẻ theo tên, đường dẫn..."
        :pagination="pagination"
        empty-title="Không có thẻ bài viết nào"
        empty-message="Chưa tìm thấy thẻ nào phù hợp với từ khóa tìm kiếm của bạn."
        @page-change="onPageChange"
      >
        <!-- Single Toolbar Filters + Action Button -->
        <template #filters>
          <!-- Refresh Button -->
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchData">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loading }" />
          </button>
        </template>

        <!-- Action Button in Single Toolbar Row -->
        <template #actions>
          <button type="button" class="btn-primary-rose" @click="openCreateDialog">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Thêm thẻ mới</span>
          </button>
        </template>

        <!-- Custom Cell: Tag Name Badge -->
        <template #body-cell-name="{ row }">
          <div class="tag-name-cell">
            <span class="tag-item-badge">
              #{{ row.name }}
            </span>
          </div>
        </template>

        <!-- Custom Cell: Slug -->
        <template #body-cell-slug="{ row }">
          <span class="tag-slug-text">
            /tags/{{ row.slug }}
          </span>
        </template>

        <!-- Custom Cell: Post Count -->
        <template #body-cell-postCount="{ row }">
          <span
            class="post-count-badge"
            :class="row.postCount > 0 ? 'badge-has-posts' : 'badge-empty-posts'"
          >
            <q-icon name="fa-solid fa-file-lines" size="10px" class="q-mr-xs" />
            <span>{{ row.postCount }} bài viết</span>
          </span>
        </template>

        <!-- Custom Cell: Created At -->
        <template #body-cell-createdAt="{ row }">
          <div class="date-cell">
            {{ formatDate(row.createdAt) }}
          </div>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <!-- Edit Tag -->
            <button
              type="button"
              class="table-action-btn btn-edit"
              title="Chỉnh sửa thẻ"
              @click="openEditDialog(row)"
            >
              <q-icon name="fa-solid fa-pen-to-square" size="12px" />
            </button>

            <!-- Delete Tag -->
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa thẻ bài viết"
              @click="confirmDelete(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- 3. Tag Editor Modal Dialog -->
    <AdminTagEditorDialog
      v-model="editorDialogOpen"
      :tag="selectedTag"
      @saved="onTagSaved"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from "vue";
import type { ColumnDef, TablePagination } from "@/components/admin/AdminDataTable.vue";
import AdminDataTable from "@/components/admin/AdminDataTable.vue";
import AdminTagEditorDialog from "@/components/admin/AdminTagEditorDialog.vue";
import type {
  AdminTagSummary,
  AdminTagStats
} from "@/types/admin-tag";
import { adminTagService } from "@/services/admin-tag.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

// Reactive States
const loading = ref(false);
const tags = ref<AdminTagSummary[]>([]);

const stats = reactive<AdminTagStats>({
  totalTags: 0,
  totalAssignedPosts: 0,
  tagsWithNoPosts: 0,
  topTagName: null,
  topTagPostCount: 0
});

// Filters & Pagination
const searchQuery = ref("");
const currentPage = ref(1);
const pageSize = 10;
const totalCount = ref(0);
const totalPages = ref(1);

// Dialog State
const editorDialogOpen = ref(false);
const selectedTag = ref<AdminTagSummary | null>(null);

// Table Columns definition
const columns: ColumnDef[] = [
  { key: "name", label: "Thẻ bài viết (Hashtag)", width: "30%" },
  { key: "slug", label: "Đường dẫn", width: "25%" },
  { key: "postCount", label: "Số bài viết gắn thẻ", width: "20%", align: "center" },
  { key: "createdAt", label: "Ngày tạo", width: "15%" },
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
    const res = await adminTagService.getStats();
    stats.totalTags = res.totalTags;
    stats.totalAssignedPosts = res.totalAssignedPosts;
    stats.tagsWithNoPosts = res.tagsWithNoPosts;
    stats.topTagName = res.topTagName;
    stats.topTagPostCount = res.topTagPostCount;
  } catch (err) {
    console.error("Lỗi khi tải thống kê thẻ:", err);
  }
}

// Fetch Tags with debounce search
let searchTimer: number | null = null;
async function fetchData() {
  try {
    loading.value = true;
    const res = await adminTagService.getTags({
      search: searchQuery.value,
      page: currentPage.value,
      pageSize
    });

    tags.value = res.items;
    totalCount.value = res.totalCount;
    totalPages.value = res.totalPages;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Không thể tải danh sách thẻ bài viết.";
    swalError("Lỗi kết nối", msg);
  } finally {
    loading.value = false;
  }
}

// Watch search query
watch(searchQuery, () => {
  if (searchTimer) clearTimeout(searchTimer);
  searchTimer = window.setTimeout(() => {
    currentPage.value = 1;
    fetchData();
  }, 350);
});

function onPageChange(page: number) {
  currentPage.value = page;
  fetchData();
}

// Dialog openers
function openCreateDialog() {
  selectedTag.value = null;
  editorDialogOpen.value = true;
}

function openEditDialog(tag: AdminTagSummary) {
  selectedTag.value = tag;
  editorDialogOpen.value = true;
}

function onTagSaved() {
  fetchData();
  fetchStats();
}

// Delete confirmation with SweetAlert2
async function confirmDelete(tag: AdminTagSummary) {
  const confirmed = await swalConfirm({
    title: "Xóa thẻ này?",
    text: `Bạn có chắc chắn muốn xóa thẻ "#${tag.name}"? ${
      tag.postCount > 0
        ? `Thẻ sẽ được gỡ khỏi ${tag.postCount} bài viết liên quan (bài viết vẫn giữ nguyên).`
        : 'Thao tác này không thể hoàn tác.'
    }`,
    confirmButtonText: "Xóa thẻ",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });

  if (!confirmed) return;

  try {
    await adminTagService.deleteTag(tag.id);
    swalSuccess("Đã xóa thẻ!", `Thẻ "#${tag.name}" đã được xóa thành công.`);
    fetchData();
    fetchStats();
  } catch (err: any) {
    swalError("Lỗi khi xóa thẻ", err.response?.data?.message || "Không thể xóa thẻ.");
  }
}

// Helpers
function formatDate(dateStr: string | null): string {
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
.admin-tags-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 20px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Stats Grid */
.tags-stats-grid {
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
      &.icon-emerald {
        background: rgba(16, 185, 129, 0.12);
        color: #10b981;
        border: 1px solid rgba(16, 185, 129, 0.25);
      }
      &.icon-amber {
        background: rgba(245, 158, 11, 0.12);
        color: #f59e0b;
        border: 1px solid rgba(245, 158, 11, 0.25);
      }
      &.icon-indigo {
        background: rgba(99, 102, 241, 0.12);
        color: #818cf8;
        border: 1px solid rgba(99, 102, 241, 0.25);
      }
    }

    .stat-info {
      display: flex;
      flex-direction: column;
      min-width: 0;

      .stat-value {
        font-family: var(--font-headline, sans-serif);
        font-size: 22px;
        font-weight: 800;
        color: var(--text-primary, #0b1326);
        line-height: 1.2;

        &.top-tag-title {
          font-size: 16px;
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
        }
      }

      .stat-label {
        font-family: var(--font-mono, monospace);
        font-size: 10.5px;
        font-weight: 700;
        color: var(--text-muted, #64748b);
        letter-spacing: 0.06em;
        margin-top: 2px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
    }
  }
}

.tags-table-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

/* 2. Action button */
.btn-primary-rose {
  background: #df266a;
  color: #ffffff;
  font-family: var(--font-headline, sans-serif);
  font-size: 13.5px;
  font-weight: 700;
  padding: 9px 18px;
  border-radius: 9999px;
  border: none;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  box-shadow: 0 4px 14px rgba(223, 38, 106, 0.28);
  transition: all 0.2s ease;

  &:hover {
    background: #be185d;
    transform: translateY(-1px);
    box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
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

/* 3. Table Cell Contents */
.tag-name-cell {
  display: flex;
  align-items: center;

  .tag-item-badge {
    font-family: var(--font-mono, monospace);
    font-size: 12.5px;
    font-weight: 700;
    color: #34d399;
    background: rgba(16, 185, 129, 0.15);
    border: 1px solid rgba(16, 185, 129, 0.3);
    padding: 4px 10px;
    border-radius: 6px;
    display: inline-flex;
    align-items: center;
  }
}

.tag-slug-text {
  font-family: var(--font-mono, monospace);
  font-size: 12px;
  color: var(--text-secondary, #64748b);
}

.post-count-badge {
  font-family: var(--font-mono, monospace);
  font-size: 11px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 9999px;
  display: inline-flex;
  align-items: center;

  &.badge-has-posts {
    background: rgba(99, 102, 241, 0.15);
    color: #818cf8;
    border: 1px solid rgba(99, 102, 241, 0.3);
  }

  &.badge-empty-posts {
    background: var(--bg-surface-high, #f1f5f9);
    color: var(--text-muted, #94a3b8);
    border: 1px solid var(--border-hairline, #e2e8f0);
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
    text-decoration: none;
    transition: all 0.15s ease;

    &.btn-edit:hover {
      color: #df266a;
      border-color: #df266a;
      background: rgba(223, 38, 106, 0.12);
    }

    &.btn-delete:hover {
      color: #e11d48;
      border-color: #e11d48;
      background: rgba(225, 29, 72, 0.12);
    }
  }
}
</style>
