<template>
  <div class="editorial-data-table-wrapper">
    <!-- 1. Single Unified Toolbar Row (Search + Filters + Action Button) -->
    <div class="table-unified-toolbar">
      <!-- Left Controls Group (Prepend content like Tabs + Search Box) -->
      <div class="toolbar-left-group">
        <slot name="prepend-search"></slot>

        <!-- Search Input Box -->
        <div class="toolbar-search-box">
          <q-icon name="fa-solid fa-magnifying-glass" size="13px" class="search-icon" />
          <input :value="search" type="text" :placeholder="searchPlaceholder" class="search-input"
            @input="$emit('update:search', ($event.target as HTMLInputElement).value)" />
          <button v-if="search" type="button" class="search-clear-btn" title="Xóa tìm kiếm"
            @click="$emit('update:search', '')">
            <q-icon name="fa-solid fa-xmark" size="11px" />
          </button>
        </div>
      </div>

      <!-- Controls & Actions Group (Dropdowns + Action Buttons) -->
      <div class="toolbar-controls-group">
        <slot name="filters"></slot>
        <slot name="actions"></slot>
      </div>
    </div>

    <!-- 2. The Main Editorial Table Card -->
    <div class="table-card">
      <!-- Loading Skeleton Overlay -->
      <transition name="fade-loading">
        <div v-if="loading" class="table-loading-container">
          <div class="loading-spinner-box">
            <q-spinner-tail color="pink-7" size="32px" />
            <span class="loading-label">Đang tải dữ liệu...</span>
          </div>
        </div>
      </transition>

      <div class="table-scroll-container">
        <table class="editorial-table">
          <thead>
            <tr>
              <th v-for="col in columns" :key="col.key" :style="{ textAlign: col.align || 'left', width: col.width }"
                class="editorial-th">
                <slot :name="`header-${col.key}`" :column="col">
                  {{ col.label }}
                </slot>
              </th>
            </tr>
          </thead>

          <tbody v-if="items && items.length > 0">
            <tr v-for="(row, rowIndex) in items" :key="getRowKey(row, rowIndex)" class="editorial-tr">
              <td v-for="col in columns" :key="col.key" :style="{ textAlign: col.align || 'left' }"
                class="editorial-td">
                <slot :name="`body-cell-${col.key}`" :row="row" :index="rowIndex" :value="row[col.key]">
                  {{ row[col.key] }}
                </slot>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- 3. Empty State -->
      <div v-if="!loading && (!items || items.length === 0)" class="table-empty-state">
        <slot name="empty">
          <div class="empty-icon-wrap">
            <q-icon name="fa-regular fa-folder-open" size="32px" />
          </div>
          <div class="empty-title">{{ emptyTitle }}</div>
          <div class="empty-desc">{{ emptyMessage }}</div>
        </slot>
      </div>

      <!-- 4. Table Footer: Records Info & Numbered Box Pagination -->
      <div v-if="pagination && pagination.totalCount > 0" class="table-footer">
        <div class="footer-info">
          Hiển thị
          <span class="text-highlight">{{ pageStartRecord }}</span> -
          <span class="text-highlight">{{ pageEndRecord }}</span>
          trong tổng số
          <span class="text-highlight">{{ pagination.totalCount }}</span> mục
        </div>

        <!-- Box-style Numbered Pagination -->
        <div class="footer-pagination-boxes">
          <!-- Prev Button -->
          <button type="button" class="page-box-btn nav-btn" :disabled="pagination.page <= 1" title="Trang trước"
            @click="$emit('page-change', pagination.page - 1)">
            <q-icon name="fa-solid fa-chevron-left" size="11px" />
          </button>

          <!-- Number Buttons -->
          <template v-for="(p, pIdx) in paginationPages" :key="pIdx">
            <button v-if="typeof p === 'number'" type="button" class="page-box-btn num-btn"
              :class="{ active: pagination.page === p }" @click="$emit('page-change', p)">
              {{ p }}
            </button>
            <span v-else class="page-ellipsis-box">...</span>
          </template>

          <!-- Next Button -->
          <button type="button" class="page-box-btn nav-btn" :disabled="pagination.page >= pagination.totalPages"
            title="Trang sau" @click="$emit('page-change', pagination.page + 1)">
            <q-icon name="fa-solid fa-chevron-right" size="11px" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

export interface ColumnDef {
  key: string;
  label: string;
  align?: "left" | "center" | "right";
  width?: string;
}

export interface TablePagination {
  page: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}

const props = withDefaults(
  defineProps<{
    items: any[];
    columns: ColumnDef[];
    loading?: boolean;
    rowKey?: string;
    search?: string;
    searchPlaceholder?: string;
    pagination?: TablePagination | null;
    emptyTitle?: string;
    emptyMessage?: string;
  }>(),
  {
    loading: false,
    rowKey: "id",
    search: "",
    searchPlaceholder: "Tìm kiếm dữ liệu...",
    pagination: null,
    emptyTitle: "Không tìm thấy dữ liệu",
    emptyMessage: "Hiện chưa có bản ghi nào phù hợp với điều kiện tìm kiếm hoặc bộ lọc hiện tại."
  }
);

defineEmits<{
  (e: "update:search", value: string): void;
  (e: "page-change", page: number): void;
}>();

const getRowKey = (row: Record<string, any>, index: number) => {
  if (props.rowKey && row[props.rowKey] !== undefined) {
    return row[props.rowKey];
  }
  return index;
};

const pageStartRecord = computed(() => {
  if (!props.pagination || props.pagination.totalCount === 0) return 0;
  return (props.pagination.page - 1) * props.pagination.pageSize + 1;
});

const pageEndRecord = computed(() => {
  if (!props.pagination) return 0;
  return Math.min(props.pagination.page * props.pagination.pageSize, props.pagination.totalCount);
});

// Generate numbered page boxes
const paginationPages = computed<(number | string)[]>(() => {
  if (!props.pagination || props.pagination.totalPages <= 1) return [1];
  const total = props.pagination.totalPages;
  const current = props.pagination.page;
  const pages: (number | string)[] = [];

  if (total <= 6) {
    for (let i = 1; i <= total; i++) pages.push(i);
  } else {
    pages.push(1);
    if (current > 3) pages.push("...");

    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);
    for (let i = start; i <= end; i++) {
      if (!pages.includes(i)) pages.push(i);
    }

    if (current < total - 2) pages.push("...");
    if (!pages.includes(total)) pages.push(total);
  }

  return pages;
});
</script>

<style scoped lang="scss">
.editorial-data-table-wrapper {
  width: 100%;
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
}

/* 1. Single Unified Toolbar Row */
.table-unified-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  margin-bottom: 16px;
  flex-wrap: wrap;
  flex-shrink: 0;

  .toolbar-left-group {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
    flex: 1;
    min-width: 0;
  }

  .toolbar-search-box {
    position: relative;
    display: flex;
    align-items: center;
    min-width: 280px;
    max-width: 380px;
    flex: 1;

    .search-icon {
      position: absolute;
      left: 14px;
      color: var(--text-muted, #94a3b8);
      pointer-events: none;
    }

    .search-input {
      width: 100%;
      height: 40px;
      padding: 0 36px 0 38px;
      font-family: var(--font-body, sans-serif);
      font-size: 13.5px;
      color: var(--text-primary, #0b1326);
      background: var(--bg-surface-low, #ffffff);
      border: 1px solid var(--border-hairline, #e2e8f0);
      border-radius: 10px;
      outline: none;
      transition: all 0.2s ease;

      &:focus {
        border-color: #df266a;
        box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
      }

      &::placeholder {
        color: var(--text-muted, #94a3b8);
      }
    }

    .search-clear-btn {
      position: absolute;
      right: 10px;
      width: 20px;
      height: 20px;
      border-radius: 50%;
      background: var(--bg-surface-high, #e2e8f0);
      border: none;
      color: var(--text-secondary, #64748b);
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;

      &:hover {
        background: var(--bg-surface-highest, #cbd5e1);
        color: var(--text-primary, #0b1326);
      }
    }
  }

  .toolbar-controls-group {
    display: flex;
    align-items: center;
    gap: 10px;
    flex-wrap: wrap;
  }
}

/* 2. Table Card (Full height flex) */
.table-card {
  background: var(--bg-surface-lowest, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 14px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.02);
  overflow: hidden;
  position: relative;
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 480px;

  .table-loading-container {
    position: absolute;
    inset: 0;
    background: rgba(255, 255, 255, 0.65);
    backdrop-filter: blur(2px);
    z-index: 10;
    display: flex;
    align-items: center;
    justify-content: center;

    .loading-spinner-box {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 10px;

      .loading-label {
        font-family: var(--font-headline, sans-serif);
        font-size: 13px;
        font-weight: 600;
        color: var(--text-secondary, #475569);
      }
    }
  }

  .fade-loading-enter-active,
  .fade-loading-leave-active {
    transition: opacity 0.2s ease;
  }

  .fade-loading-enter-from,
  .fade-loading-leave-to {
    opacity: 0;
  }

  .table-scroll-container {
    overflow-x: auto;
    overflow-y: auto;
    flex: 1;
  }

  .editorial-table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0;
    font-size: 13.5px;

    .editorial-th {
      position: sticky;
      top: 0;
      z-index: 2;
      background: #df266a;
      border-bottom: none;
      border-right: 1px solid rgba(255, 255, 255, 0.22);
      padding: 14px 18px;
      font-family: var(--font-mono, monospace);
      font-size: 11.5px;
      font-weight: 800;
      color: #ffffff;
      text-transform: uppercase;
      letter-spacing: 0.06em;
      white-space: nowrap;

      &:last-child {
        border-right: none;
      }
    }

    .editorial-tr {
      transition: background 0.15s ease;

      &:hover {
        background: rgba(223, 38, 106, 0.08);
      }
    }

    .editorial-td {
      padding: 14px 18px;
      color: var(--text-primary, #1e293b);
      vertical-align: middle;
      border-right: 1px solid var(--border-hairline, #eaeff3);
      border-bottom: 1px solid var(--border-hairline, #e2e8f0);

      &:last-child {
        border-right: none;
      }
    }
  }
}

/* 3. Empty State */
.table-empty-state {
  padding: 56px 20px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  .empty-icon-wrap {
    width: 64px;
    height: 64px;
    border-radius: 16px;
    background: var(--bg-surface-low, #f8fafc);
    border: 1px dashed var(--border-subtle, #cbd5e1);
    color: var(--text-muted, #94a3b8);
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 14px;
  }

  .empty-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 16px;
    font-weight: 700;
    color: var(--text-primary, #0b1326);
    margin-bottom: 6px;
  }

  .empty-desc {
    font-family: var(--font-body, sans-serif);
    font-size: 13.5px;
    color: var(--text-muted, #64748b);
    max-width: 420px;
    line-height: 1.5;
  }
}

/* 4. Footer Pagination with Beautiful Boxes */
.table-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 20px;
  background: var(--bg-surface-lowest, #f8fafc);
  border-top: 1px solid var(--border-hairline, #e2e8f0);
  flex-wrap: wrap;
  gap: 12px;

  .footer-info {
    font-family: var(--font-body, sans-serif);
    font-size: 13px;
    color: var(--text-muted, #64748b);

    .text-highlight {
      font-weight: 700;
      color: var(--text-primary, #0b1326);
    }
  }

  .footer-pagination-boxes {
    display: flex;
    align-items: center;
    gap: 6px;

    .page-box-btn {
      min-width: 34px;
      height: 34px;
      padding: 0 8px;
      border-radius: 8px;
      background: var(--bg-surface-low, #ffffff);
      border: 1px solid var(--border-hairline, #e2e8f0);
      color: var(--text-secondary, #475569);
      font-family: var(--font-mono, monospace);
      font-size: 12.5px;
      font-weight: 700;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.03);

      &:hover:not(:disabled):not(.active) {
        border-color: #df266a;
        color: #df266a;
        background: rgba(223, 38, 106, 0.12);
        transform: translateY(-1px);
      }

      &.active {
        background: #df266a;
        color: #ffffff;
        border-color: #df266a;
        box-shadow: 0 4px 10px rgba(223, 38, 106, 0.28);
      }

      &:disabled {
        opacity: 0.35;
        cursor: not-allowed;
        box-shadow: none;
      }
    }

    .page-ellipsis-box {
      font-family: var(--font-mono, monospace);
      font-size: 13px;
      font-weight: 700;
      color: var(--text-muted, #94a3b8);
      padding: 0 4px;
    }
  }
}

@media (max-width: 768px) {
  .table-unified-toolbar {
    flex-direction: column;
    align-items: stretch;

    .toolbar-search-box {
      min-width: 100%;
      max-width: 100%;
    }

    .toolbar-controls-group {
      justify-content: space-between;
    }
  }

  .table-footer {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;

    .footer-pagination-boxes {
      justify-content: center;
    }
  }
}
</style>
