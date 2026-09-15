<template>
  <q-page class="admin-categories-page">
    <!-- 1. Stats Quick Overview Cards -->
    <section class="categories-stats-grid">
      <!-- Card: Total Categories -->
      <div class="stat-card card-rose">
        <div class="stat-icon-box icon-rose">
          <q-icon name="fa-solid fa-layer-group" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalCategories }}</div>
          <div class="stat-label">TỔNG CHỦ ĐỀ</div>
        </div>
      </div>

      <!-- Card: Total Assigned Posts -->
      <div class="stat-card card-emerald">
        <div class="stat-icon-box icon-emerald">
          <q-icon name="fa-solid fa-newspaper" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.totalAssignedPosts }}</div>
          <div class="stat-label">BÀI VIẾT ĐÃ GÁN</div>
        </div>
      </div>

      <!-- Card: Empty Categories -->
      <div class="stat-card card-amber">
        <div class="stat-icon-box icon-amber">
          <q-icon name="fa-solid fa-folder-open" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.categoriesWithNoPosts }}</div>
          <div class="stat-label">CHỦ ĐỀ TRỐNG</div>
        </div>
      </div>

      <!-- Card: Top Active Category -->
      <div class="stat-card card-indigo">
        <div class="stat-icon-box icon-indigo">
          <q-icon name="fa-solid fa-fire" size="18px" />
        </div>
        <div class="stat-info">
          <div class="stat-value top-cat-title" :title="stats.topCategoryName || 'Chưa có'">
            {{ stats.topCategoryName || 'Chưa có' }}
          </div>
          <div class="stat-label">
            SÔI ĐỘNG NHẤT ({{ stats.topCategoryPostCount }} BÀI)
          </div>
        </div>
      </div>
    </section>

    <!-- 2. Reusable Editorial Table Section -->
    <section class="categories-table-section">
      <AdminDataTable
        :items="categories"
        :columns="columns"
        :loading="loading"
        v-model:search="searchQuery"
        search-placeholder="Tìm kiếm chủ đề theo tên, đường dẫn, mô tả..."
        :pagination="pagination"
        empty-title="Không có chủ đề & danh mục nào"
        empty-message="Chưa tìm thấy chủ đề nào phù hợp với từ khóa tìm kiếm của bạn."
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
            <span>Thêm chủ đề mới</span>
          </button>
        </template>

        <!-- Custom Cell: Thumbnail -->
        <template #body-cell-thumbnail="{ row }">
          <div class="cat-thumb-wrap">
            <img
              v-if="row.thumbnailUrl"
              :src="row.thumbnailUrl"
              :alt="row.name"
              class="cat-thumb-img"
              @error="onImageError"
            />
            <div v-else class="cat-thumb-placeholder">
              <q-icon name="fa-solid fa-layer-group" size="14px" />
            </div>
          </div>
        </template>

        <!-- Custom Cell: Name & Slug -->
        <template #body-cell-name="{ row }">
          <div class="cat-info-cell">
            <div class="cat-name-text" :title="row.name">{{ row.name }}</div>
            <div class="cat-slug-sub">
              <span class="cat-slug-label">/topics/{{ row.slug }}</span>
            </div>
          </div>
        </template>

        <!-- Custom Cell: Description -->
        <template #body-cell-description="{ row }">
          <div class="cat-desc-cell" :title="row.description || 'Chưa có mô tả'">
            {{ row.description || '—' }}
          </div>
        </template>

        <!-- Custom Cell: Post Count -->
        <template #body-cell-postCount="{ row }">
          <button
            type="button"
            class="post-count-badge"
            :class="row.postCount > 0 ? 'badge-has-posts' : 'badge-empty-posts'"
            :title="row.postCount > 0 ? `Bấm để lọc ${row.postCount} bài viết thuộc chủ đề '${row.name}'` : 'Chủ đề chưa có bài viết nào'"
            :disabled="row.postCount === 0"
            @click="navigateToPosts(row.id)"
          >
            <q-icon name="fa-solid fa-file-lines" size="10px" class="q-mr-xs" />
            <span>{{ row.postCount }} bài viết</span>
            <q-icon v-if="row.postCount > 0" name="fa-solid fa-arrow-right" size="9px" class="q-ml-xs arrow-icon" />
          </button>
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
            <!-- View Live Topic Page -->
            <a
              :href="`/topics/${row.slug}`"
              target="_blank"
              class="table-action-btn btn-view"
              title="Xem trang chủ đề công khai"
            >
              <q-icon name="fa-solid fa-arrow-up-right-from-square" size="12px" />
            </a>

            <!-- Edit Category -->
            <button
              type="button"
              class="table-action-btn btn-edit"
              title="Chỉnh sửa chủ đề"
              @click="openEditDialog(row)"
            >
              <q-icon name="fa-solid fa-pen-to-square" size="12px" />
            </button>

            <!-- Delete Category -->
            <button
              type="button"
              class="table-action-btn btn-delete"
              title="Xóa chủ đề"
              @click="confirmDelete(row)"
            >
              <q-icon name="fa-solid fa-trash-can" size="12px" />
            </button>
          </div>
        </template>
      </AdminDataTable>
    </section>

    <!-- 3. Category Editor Modal Dialog -->
    <AdminCategoryEditorDialog
      v-model="editorDialogOpen"
      :category="selectedCategory"
      @saved="onCategorySaved"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import type { ColumnDef, TablePagination } from "@/components/admin/AdminDataTable.vue";
import AdminDataTable from "@/components/admin/AdminDataTable.vue";
import AdminCategoryEditorDialog from "@/components/admin/AdminCategoryEditorDialog.vue";
import type {
  AdminCategorySummary,
  AdminCategoryStats
} from "@/types/admin-category";
import { adminCategoryService } from "@/services/admin-category.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

const router = useRouter();

// Reactive States
const loading = ref(false);
const categories = ref<AdminCategorySummary[]>([]);

const stats = reactive<AdminCategoryStats>({
  totalCategories: 0,
  totalAssignedPosts: 0,
  categoriesWithNoPosts: 0,
  topCategoryName: null,
  topCategoryPostCount: 0
});

// Filters & Pagination
const searchQuery = ref("");
const currentPage = ref(1);
const pageSize = 10;
const totalCount = ref(0);
const totalPages = ref(1);

// Dialog State
const editorDialogOpen = ref(false);
const selectedCategory = ref<AdminCategorySummary | null>(null);

// Table Columns definition
const columns: ColumnDef[] = [
  { key: "thumbnail", label: "Ảnh", width: "70px", align: "center" },
  { key: "name", label: "Tên chủ đề & Đường dẫn", width: "28%" },
  { key: "description", label: "Mô tả ngắn", width: "32%" },
  { key: "postCount", label: "Số bài viết", width: "14%", align: "center" },
  { key: "createdAt", label: "Ngày tạo", width: "14%" },
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
    const res = await adminCategoryService.getStats();
    stats.totalCategories = res.totalCategories;
    stats.totalAssignedPosts = res.totalAssignedPosts;
    stats.categoriesWithNoPosts = res.categoriesWithNoPosts;
    stats.topCategoryName = res.topCategoryName;
    stats.topCategoryPostCount = res.topCategoryPostCount;
  } catch (err) {
    console.error("Lỗi khi tải thống kê chủ đề:", err);
  }
}

// Fetch Categories with debounce search
let searchTimer: number | null = null;
async function fetchData() {
  try {
    loading.value = true;
    const res = await adminCategoryService.getCategories({
      search: searchQuery.value,
      page: currentPage.value,
      pageSize
    });

    categories.value = res.items;
    totalCount.value = res.totalCount;
    totalPages.value = res.totalPages;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Không thể tải danh sách chủ đề & danh mục.";
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
  selectedCategory.value = null;
  editorDialogOpen.value = true;
}

function openEditDialog(cat: AdminCategorySummary) {
  selectedCategory.value = cat;
  editorDialogOpen.value = true;
}

function onCategorySaved() {
  fetchData();
  fetchStats();
}

function navigateToPosts(categoryId: number) {
  router.push({
    path: "/admin/posts",
    query: { categoryId }
  });
}

// Delete confirmation with SweetAlert2
async function confirmDelete(cat: AdminCategorySummary) {
  if (cat.postCount > 0) {
    const shouldNavigate = await swalConfirm({
      title: "Không thể xóa chủ đề",
      text: `Chủ đề "${cat.name}" hiện đang có ${cat.postCount} bài viết liên kết. Bạn có muốn chuyển sang trang Quản lý Bài viết để lọc và thay đổi danh mục cho các bài viết này không?`,
      confirmButtonText: "Xem & lọc bài viết ngay",
      cancelButtonText: "Đóng",
      icon: "info",
      isDanger: false
    });

    if (shouldNavigate) {
      navigateToPosts(cat.id);
    }
    return;
  }

  const confirmed = await swalConfirm({
    title: "Xóa chủ đề này?",
    text: `Bạn có chắc chắn muốn xóa chủ đề "${cat.name}"? Thao tác này không thể hoàn tác.`,
    confirmButtonText: "Xóa vĩnh viễn",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });

  if (!confirmed) return;

  try {
    await adminCategoryService.deleteCategory(cat.id);
    swalSuccess("Đã xóa chủ đề!", `Chủ đề "${cat.name}" đã được xóa thành công.`);
    fetchData();
    fetchStats();
  } catch (err: any) {
    swalError("Lỗi khi xóa chủ đề", err.response?.data?.message || "Không thể xóa chủ đề.");
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

function onImageError(e: Event) {
  (e.target as HTMLImageElement).src =
    "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=150&auto=format&fit=crop&q=60";
}

onMounted(() => {
  fetchData();
  fetchStats();
});
</script>

<style scoped lang="scss">
.admin-categories-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 20px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Stats Grid */
.categories-stats-grid {
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
      &.icon-indigo {
        background: #eef2ff;
        color: #4f46e5;
        border: 1px solid #e0e7ff;
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
        color: #0b1326;
        line-height: 1.2;

        &.top-cat-title {
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
        color: #64748b;
        letter-spacing: 0.06em;
        margin-top: 2px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
    }
  }
}

.categories-table-section {
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
.cat-thumb-wrap {
  width: 54px;
  height: 38px;
  border-radius: 6px;
  overflow: hidden;
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  justify-content: center;

  .cat-thumb-img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .cat-thumb-placeholder {
    color: #cbd5e1;
  }
}

.cat-info-cell {
  display: flex;
  flex-direction: column;
  gap: 3px;

  .cat-name-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    font-weight: 700;
    color: #0b1326;
    line-height: 1.4;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .cat-slug-sub {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 11.5px;
    color: #64748b;

    .cat-slug-label {
      font-family: var(--font-mono, monospace);
      color: #94a3b8;
    }
  }
}

.cat-desc-cell {
  font-family: var(--font-body, sans-serif);
  font-size: 12.5px;
  color: #475569;
  line-height: 1.4;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.post-count-badge {
  font-family: var(--font-mono, monospace);
  font-size: 11px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 9999px;
  display: inline-flex;
  align-items: center;
  border: 1px solid transparent;
  background: transparent;
  cursor: default;
  transition: all 0.2s ease;

  .arrow-icon {
    opacity: 0.6;
    transition: transform 0.2s ease;
  }

  &.badge-has-posts {
    background: #eef2ff;
    color: #4f46e5;
    border-color: #c7d2fe;
    cursor: pointer;

    &:hover {
      background: #4f46e5;
      color: #ffffff;
      border-color: #4f46e5;
      box-shadow: 0 2px 8px rgba(79, 70, 229, 0.25);
      transform: translateY(-1px);

      .arrow-icon {
        opacity: 1;
        transform: translateX(2px);
      }
    }
  }

  &.badge-empty-posts {
    background: #f1f5f9;
    color: #94a3b8;
    border-color: #e2e8f0;
    cursor: not-allowed;
    opacity: 0.8;
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
    text-decoration: none;
    transition: all 0.15s ease;

    &.btn-view:hover {
      color: #4f46e5;
      border-color: #4f46e5;
      background: #eef2ff;
    }

    &.btn-edit:hover {
      color: #df266a;
      border-color: #df266a;
      background: #fdf2f6;
    }

    &.btn-delete:hover {
      color: #e11d48;
      border-color: #e11d48;
      background: #fff1f2;
    }
  }
}
</style>
