<template>
  <q-page class="admin-posts-page">
    <!-- 1. Stats Quick Overview Cards -->
    <section class="posts-stats-grid">
      <!-- Card: Total Posts -->
      <div class="stat-card card-rose">
        <div class="stat-icon-box icon-rose">
          <q-icon name="fa-solid fa-newspaper" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalPosts }}</div>
          <div class="stat-label">TỔNG BÀI VIẾT</div>
        </div>
      </div>

      <!-- Card: Published Posts -->
      <div class="stat-card card-emerald">
        <div class="stat-icon-box icon-emerald">
          <q-icon name="fa-solid fa-circle-check" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.publishedPosts }}</div>
          <div class="stat-label">ĐÃ XUẤT BẢN</div>
        </div>
      </div>

      <!-- Card: Draft Posts -->
      <div class="stat-card card-amber">
        <div class="stat-icon-box icon-amber">
          <q-icon name="fa-solid fa-file-pen" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.draftPosts }}</div>
          <div class="stat-label">BẢN NHÁP</div>
        </div>
      </div>

      <!-- Card: Total Views -->
      <div class="stat-card card-indigo">
        <div class="stat-icon-box icon-indigo">
          <q-icon name="fa-solid fa-eye" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ formatViews(stats.totalViews) }}</div>
          <div class="stat-label">TỔNG LƯỢT XEM</div>
        </div>
      </div>
    </section>

    <!-- 2. Reusable Editorial Table Section -->
    <section class="posts-table-section">
      <AdminDataTable
        :items="posts"
        :columns="columns"
        :loading="loading"
        v-model:search="searchQuery"
        search-placeholder="Tìm kiếm bài viết theo tiêu đề, tóm tắt..."
        :pagination="pagination"
        empty-title="Không có bài viết nào"
        empty-message="Chưa tìm thấy bài viết nào phù hợp với điều kiện tìm kiếm hoặc bộ lọc hiện tại."
        @page-change="onPageChange"
      >
        <!-- Single Toolbar Filters + Action Button -->
        <template #filters>
          <!-- Status Dropdown Select -->
          <div class="filter-select-wrap">
            <select v-model="activeStatusTab" class="custom-filter-select" @change="onStatusChange">
              <option value="all">Tất cả trạng thái ({{ stats.totalPosts }})</option>
              <option value="Published">Đã xuất bản ({{ stats.publishedPosts }})</option>
              <option value="Draft">Bản nháp ({{ stats.draftPosts }})</option>
            </select>
          </div>

          <!-- Category Dropdown Select -->
          <div class="filter-select-wrap">
            <select v-model="selectedCategoryId" class="custom-filter-select" @change="onCategoryChange">
              <option :value="0">Tất cả danh mục</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>
          </div>

          <!-- Refresh Button -->
          <button type="button" class="btn-refresh" title="Tải lại dữ liệu" @click="fetchData">
            <q-icon name="fa-solid fa-rotate-right" size="12px" :class="{ 'fa-spin': loading }" />
          </button>
        </template>

        <!-- Action Button in Single Toolbar Row -->
        <template #actions>
          <button type="button" class="btn-primary-rose" @click="openCreateDialog">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Viết bài mới</span>
          </button>
        </template>

        <!-- Custom Cell: Thumbnail -->
        <template #body-cell-thumbnail="{ row }">
          <div class="post-thumb-wrap">
            <img
              v-if="row.thumbnailUrl"
              :src="row.thumbnailUrl"
              :alt="row.title"
              class="post-thumb-img"
              @error="onImageError"
            />
            <div v-else class="post-thumb-placeholder">
              <q-icon name="fa-solid fa-image" size="14px" />
            </div>
          </div>
        </template>

        <!-- Custom Cell: Title & Meta -->
        <template #body-cell-title="{ row }">
          <div class="post-info-cell">
            <div class="post-title-text" :title="row.title">{{ row.title }}</div>
            <div class="post-meta-sub">
              <span class="post-slug-label">/posts/{{ row.slug }}</span>
              <span class="meta-dot">·</span>
              <span class="post-author-name">
                <q-icon name="fa-solid fa-user-pen" size="10px" class="q-mr-xs" />
                {{ row.authorName }}
              </span>
            </div>
          </div>
        </template>

        <!-- Custom Cell: Category -->
        <template #body-cell-category="{ row }">
          <span class="category-badge">
            <q-icon name="fa-solid fa-folder" size="10px" class="q-mr-xs" />
            {{ row.categoryName || 'Chưa phân loại' }}
          </span>
        </template>

        <!-- Custom Cell: Tags -->
        <template #body-cell-tags="{ row }">
          <div v-if="row.tags && row.tags.length > 0" class="table-tags-wrap">
            <span v-for="(tag, idx) in row.tags.slice(0, 2)" :key="idx" class="table-tag-badge">
              #{{ tag }}
            </span>
            <span v-if="row.tags.length > 2" class="table-tag-more">
              +{{ row.tags.length - 2 }}
            </span>
          </div>
          <span v-else class="text-muted-empty">—</span>
        </template>

        <!-- Custom Cell: Status -->
        <template #body-cell-status="{ row }">
          <button
            type="button"
            class="status-toggle-badge"
            :class="row.status === 'Published' ? 'status-published' : 'status-draft'"
            :title="row.status === 'Published' ? 'Bấm để chuyển về Bản nháp' : 'Bấm để Xuất bản'"
            @click="toggleStatus(row)"
          >
            <span class="status-indicator-dot"></span>
            <span>{{ row.status === 'Published' ? 'Xuất bản' : 'Bản nháp' }}</span>
          </button>
        </template>

        <!-- Custom Cell: Views & Date -->
        <template #body-cell-stats="{ row }">
          <div class="stats-date-cell">
            <div class="views-row">
              <q-icon name="fa-solid fa-eye" size="11px" class="q-mr-xs text-muted" />
              <span>{{ row.viewCount }} lượt xem</span>
            </div>
            <div class="date-row">
              {{ formatDate(row.publishedAt || row.createdAt) }}
            </div>
          </div>
        </template>

        <!-- Custom Cell: Actions -->
        <template #body-cell-actions="{ row }">
          <div class="table-actions-cell">
            <!-- View Live Post -->
            <a
              :href="`/posts/${row.slug}`"
              target="_blank"
              class="table-action-btn btn-view"
              title="Xem bài viết trên trang công khai"
            >
              <q-icon name="fa-solid fa-arrow-up-right-from-square" size="12px" />
            </a>

            <!-- Edit Post -->
            <button
              type="button"
              class="table-action-btn btn-edit"
              title="Chỉnh sửa bài viết"
              @click="openEditDialog(row)"
            >
              <q-icon name="fa-solid fa-pen-to-square" size="12px" />
            </button>

            <!-- Delete Post -->
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa bài viết"
              @click="confirmDelete(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- 3. Fullscreen / Modal Post Editor Dialog -->
    <AdminPostEditorDialog
      v-model="editorDialogOpen"
      :post="selectedPost"
      :categories="categories"
      :available-tags="availableTags"
      @saved="onPostSaved"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from "vue";
import { useRoute } from "vue-router";
import type { ColumnDef, TablePagination } from "@/components/admin/AdminDataTable.vue";
import AdminDataTable from "@/components/admin/AdminDataTable.vue";
import AdminPostEditorDialog from "@/components/admin/AdminPostEditorDialog.vue";
import type {
  AdminPostSummary,
  AdminPostStats,
  AdminCategorySummary,
  AdminTagSummary
} from "@/types/admin-post";
import { adminPostService } from "@/services/admin-post.service";
import { swalConfirm, swalSuccess, swalError, swalToast } from "@/utils/swal";

const route = useRoute();

// Reactive States
const loading = ref(false);
const posts = ref<AdminPostSummary[]>([]);
const categories = ref<AdminCategorySummary[]>([]);
const availableTags = ref<AdminTagSummary[]>([]);

const stats = reactive<AdminPostStats>({
  totalPosts: 0,
  publishedPosts: 0,
  draftPosts: 0,
  totalViews: 0
});

// Filters & Pagination
const searchQuery = ref("");
const selectedCategoryId = ref<number>(
  route.query.categoryId ? Number(route.query.categoryId) : 0
);
const activeStatusTab = ref("all");
const currentPage = ref(1);
const pageSize = 10;
const totalCount = ref(0);
const totalPages = ref(1);

// Dialog State
const editorDialogOpen = ref(false);
const selectedPost = ref<AdminPostSummary | null>(null);

// Table Columns definition
const columns: ColumnDef[] = [
  { key: "thumbnail", label: "Ảnh", width: "70px", align: "center" },
  { key: "title", label: "Tiêu đề & Đường dẫn", width: "35%" },
  { key: "category", label: "Danh mục", width: "15%" },
  { key: "tags", label: "Thẻ Tags", width: "15%" },
  { key: "status", label: "Trạng thái", width: "12%", align: "center" },
  { key: "stats", label: "Lượt xem & Ngày", width: "13%" },
  { key: "actions", label: "Thao tác", width: "100px", align: "right" }
];

const pagination = computed<TablePagination>(() => ({
  page: currentPage.value,
  pageSize,
  totalPages: totalPages.value,
  totalCount: totalCount.value
}));

// Fetch Stats & Metadata
async function fetchMetadataAndStats() {
  try {
    const [metaRes, statsRes] = await Promise.all([
      adminPostService.getMetadata(),
      adminPostService.getStats()
    ]);
    categories.value = metaRes.categories;
    availableTags.value = metaRes.tags;

    stats.totalPosts = statsRes.totalPosts;
    stats.publishedPosts = statsRes.publishedPosts;
    stats.draftPosts = statsRes.draftPosts;
    stats.totalViews = statsRes.totalViews;
  } catch (err) {
    console.error("Lỗi khi tải metadata bài viết:", err);
  }
}

// Fetch Posts with debounce search
let searchTimer: number | null = null;
async function fetchData() {
  try {
    loading.value = true;
    const res = await adminPostService.getPosts({
      search: searchQuery.value,
      categoryId: selectedCategoryId.value > 0 ? selectedCategoryId.value : null,
      status: activeStatusTab.value,
      page: currentPage.value,
      pageSize
    });

    posts.value = res.items;
    totalCount.value = res.totalCount;
    totalPages.value = res.totalPages;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Không thể tải danh sách bài viết từ máy chủ.";
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

watch(activeStatusTab, () => {
  currentPage.value = 1;
  fetchData();
});

watch(
  () => route.query.categoryId,
  (newVal) => {
    selectedCategoryId.value = newVal ? Number(newVal) : 0;
    currentPage.value = 1;
    fetchData();
  }
);

function onStatusChange() {
  currentPage.value = 1;
  fetchData();
}

function onCategoryChange() {
  currentPage.value = 1;
  fetchData();
}

function onPageChange(page: number) {
  currentPage.value = page;
  fetchData();
}

// Dialog openers
function openCreateDialog() {
  selectedPost.value = null;
  editorDialogOpen.value = true;
}

function openEditDialog(post: any) {
  selectedPost.value = post as AdminPostSummary;
  editorDialogOpen.value = true;
}

function onPostSaved() {
  fetchData();
  fetchMetadataAndStats();
}

// Status Toggle Quick Action
async function toggleStatus(post: any) {
  const nextStatus = post.status === "Published" ? "Draft" : "Published";
  const actionName = nextStatus === "Published" ? "Xuất bản" : "Chuyển về bản nháp";

  try {
    await adminPostService.updateStatus(post.id, nextStatus);
    post.status = nextStatus;
    swalToast(`Đã ${actionName} bài viết "${post.title}"!`);
    fetchMetadataAndStats();
  } catch (err: any) {
    swalError("Không thể đổi trạng thái", err.response?.data?.message || "Lỗi hệ thống");
  }
}

// Delete confirmation with SweetAlert2
async function confirmDelete(post: any) {
  const confirmed = await swalConfirm({
    title: "Xóa bài viết này?",
    text: `Bạn có chắc chắn muốn xóa bài viết "${post.title}"? Dữ liệu và các bình luận liên quan sẽ bị xóa vĩnh viễn.`,
    confirmButtonText: "Xóa vĩnh viễn",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });

  if (!confirmed) return;

  try {
    await adminPostService.deletePost(post.id);
    swalSuccess("Đã xóa bài viết!", `Bài viết "${post.title}" đã được xóa thành công.`);
    fetchData();
    fetchMetadataAndStats();
  } catch (err: any) {
    swalError("Lỗi khi xóa bài viết", err.response?.data?.message || "Không thể xóa bài viết.");
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

function formatViews(views: number): string {
  if (views >= 1000) {
    return (views / 1000).toFixed(1) + "k";
  }
  return views.toString();
}

function onImageError(e: Event) {
  (e.target as HTMLImageElement).src =
    "https://images.unsplash.com/photo-1499750310107-5fef28a66643?w=150&auto=format&fit=crop&q=60";
}

onMounted(() => {
  fetchData();
  fetchMetadataAndStats();
});
</script>

<style scoped lang="scss">
.admin-posts-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 20px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Stats Grid */
.posts-stats-grid {
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

.posts-table-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

/* 2. Header & Action button */
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

/* Secondary Filters */
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

/* 3. Table Cell Contents */
.post-thumb-wrap {
  width: 54px;
  height: 38px;
  border-radius: 6px;
  overflow: hidden;
  background: var(--bg-surface-low, #f1f5f9);
  border: 1px solid var(--border-hairline, #e2e8f0);
  display: flex;
  align-items: center;
  justify-content: center;

  .post-thumb-img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .post-thumb-placeholder {
    color: var(--text-muted, #cbd5e1);
  }
}

.post-info-cell {
  display: flex;
  flex-direction: column;
  gap: 3px;

  .post-title-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    font-weight: 700;
    color: var(--text-primary, #0b1326);
    line-height: 1.4;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
  }

  .post-meta-sub {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 11.5px;
    color: var(--text-muted, #64748b);

    .post-slug-label {
      font-family: var(--font-mono, monospace);
      color: var(--text-muted, #94a3b8);
    }

    .meta-dot {
      color: var(--border-subtle, #cbd5e1);
    }

    .post-author-name {
      font-weight: 600;
      color: var(--text-secondary, #475569);
    }
  }
}

.category-badge {
  font-family: var(--font-headline, sans-serif);
  font-size: 11.5px;
  font-weight: 700;
  color: #818cf8;
  background: rgba(99, 102, 241, 0.15);
  border: 1px solid rgba(99, 102, 241, 0.3);
  padding: 4px 10px;
  border-radius: 6px;
  display: inline-flex;
  align-items: center;
}

.table-tags-wrap {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;

  .table-tag-badge {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 600;
    color: #34d399;
    background: rgba(16, 185, 129, 0.15);
    border: 1px solid rgba(16, 185, 129, 0.3);
    padding: 2px 6px;
    border-radius: 4px;
  }

  .table-tag-more {
    font-family: var(--font-mono, monospace);
    font-size: 10px;
    color: var(--text-muted, #64748b);
    background: var(--bg-surface-high, #f1f5f9);
    padding: 2px 4px;
    border-radius: 4px;
  }
}

.text-muted-empty {
  color: var(--text-muted, #cbd5e1);
}

/* Status Button Badge */
.status-toggle-badge {
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

  &.status-published {
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

  &.status-draft {
    background: var(--bg-surface-high, #f1f5f9);
    color: var(--text-muted, #64748b);
    border: 1px solid var(--border-hairline, #cbd5e1);

    .status-indicator-dot {
      background: #94a3b8;
    }

    &:hover {
      background: var(--bg-surface-highest, #e2e8f0);
      color: var(--text-primary, #0b1326);
    }
  }
}

.stats-date-cell {
  display: flex;
  flex-direction: column;
  gap: 2px;

  .views-row {
    font-family: var(--font-mono, monospace);
    font-size: 11.5px;
    font-weight: 600;
    color: var(--text-primary, #0b1326);
  }

  .date-row {
    font-size: 11px;
    color: var(--text-muted, #94a3b8);
  }
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

    &.btn-view:hover {
      border-color: #4f46e5;
      color: #4f46e5;
      background: rgba(99, 102, 241, 0.12);
    }

    &.btn-edit:hover {
      border-color: #df266a;
      color: #df266a;
      background: rgba(223, 38, 106, 0.12);
    }

    &.btn-delete:hover {
      border-color: #e11d48;
      color: #e11d48;
      background: rgba(225, 29, 72, 0.12);
    }
  }
}
</style>
