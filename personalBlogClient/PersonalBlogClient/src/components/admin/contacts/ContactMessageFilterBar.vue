<template>
  <div class="contact-filter-bar">
    <!-- 1. Search Box Left -->
    <div class="search-box-wrapper">
      <div class="search-input-box">
        <q-icon name="fa-solid fa-magnifying-glass" size="13px" class="search-icon" />
        <input
          v-model="searchModel"
          type="text"
          placeholder="Tìm theo tên, email, tiêu đề tin nhắn..."
          class="search-input"
          @keydown.enter="$emit('search')"
        />
        <button
          v-if="searchModel"
          type="button"
          class="btn-clear-search"
          title="Xóa tìm kiếm"
          @click="clearSearch"
        >
          <q-icon name="fa-solid fa-xmark" size="11px" />
        </button>
      </div>

      <button
        type="button"
        class="btn-search-trigger"
        :disabled="loading"
        @click="$emit('search')"
      >
        <span>Tìm</span>
      </button>
    </div>

    <!-- 2. Status Filter Tab Pills Center -->
    <div class="status-tabs-wrapper">
      <button
        v-for="tab in statusTabs"
        :key="tab.value"
        type="button"
        class="status-tab-btn"
        :class="{ active: statusModel === tab.value }"
        @click="selectTab(tab.value)"
      >
        <q-icon :name="tab.icon" size="12px" />
        <span>{{ tab.label }}</span>
        <span
          v-if="tab.badgeCount !== undefined && tab.badgeCount > 0"
          class="tab-count-badge"
          :class="tab.badgeClass"
        >
          {{ tab.badgeCount }}
        </span>
      </button>
    </div>

    <!-- 3. Refresh Action Right -->
    <div class="filter-actions-right">
      <button
        type="button"
        class="btn-action-refresh"
        title="Tải lại danh sách tin nhắn"
        :disabled="loading"
        @click="$emit('refresh')"
      >
        <q-icon name="fa-solid fa-rotate-right" size="13px" :class="{ 'fa-spin': loading }" />
        <span class="refresh-text">Làm mới</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

const statusModel = defineModel<string>("status", { default: "All" });
const searchModel = defineModel<string>("keyword", { default: "" });

const props = defineProps<{
  totalCount: number;
  unreadCount: number;
  repliedCount: number;
  loading: boolean;
}>();

const emit = defineEmits<{
  (e: "search"): void;
  (e: "refresh"): void;
}>();

const statusTabs = computed(() => [
  { value: "All", label: "Tất cả", icon: "fa-solid fa-layer-group", badgeCount: props.totalCount, badgeClass: "badge-slate" },
  { value: "Unread", label: "Chưa đọc", icon: "fa-solid fa-envelope", badgeCount: props.unreadCount, badgeClass: "badge-rose" },
  { value: "Read", label: "Đã đọc", icon: "fa-solid fa-envelope-open", badgeClass: "" },
  { value: "Replied", label: "Đã phản hồi", icon: "fa-solid fa-reply", badgeCount: props.repliedCount, badgeClass: "badge-emerald" },
  { value: "Archived", label: "Lưu trữ", icon: "fa-solid fa-box-archive", badgeClass: "" }
]);

function selectTab(val: string) {
  statusModel.value = val;
  emit("search");
}

function clearSearch() {
  searchModel.value = "";
  emit("search");
}
</script>

<style scoped lang="scss">
.contact-filter-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 10px 14px;
  box-shadow: 0 4px 18px rgba(11, 19, 38, 0.02);

  @media (max-width: 900px) {
    flex-direction: column;
    align-items: stretch;
  }
}

/* 1. Search Box */
.search-box-wrapper {
  display: flex;
  align-items: center;
  gap: 8px;
  flex: 1;
  max-width: 380px;

  @media (max-width: 900px) {
    max-width: 100%;
  }

  .search-input-box {
    position: relative;
    flex: 1;
    display: flex;
    align-items: center;

    .search-icon {
      position: absolute;
      left: 12px;
      color: #94a3b8;
      pointer-events: none;
    }

    .search-input {
      width: 100%;
      height: 38px;
      padding: 0 32px 0 34px;
      background: #f8fafc;
      border: 1px solid #e2e8f0;
      border-radius: 9px;
      font-size: 13px;
      color: #0b1326;
      outline: none;
      transition: all 0.2s ease;

      &::placeholder {
        color: #94a3b8;
        font-size: 12.5px;
      }

      &:focus {
        background: #ffffff;
        border-color: #df266a;
        box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
      }
    }

    .btn-clear-search {
      position: absolute;
      right: 8px;
      width: 20px;
      height: 20px;
      border-radius: 50%;
      background: #e2e8f0;
      border: none;
      color: #64748b;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;

      &:hover {
        background: #cbd5e1;
        color: #0b1326;
      }
    }
  }

  .btn-search-trigger {
    height: 38px;
    padding: 0 14px;
    border-radius: 9px;
    background: #0b1326;
    color: #ffffff;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    border: none;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover:not(:disabled) {
      background: #1e293b;
    }
  }
}

/* 2. Status Tabs */
.status-tabs-wrapper {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-wrap: wrap;

  .status-tab-btn {
    height: 34px;
    padding: 0 12px;
    border-radius: 8px;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    color: #475569;
    background: #f8fafc;
    border: 1px solid #e2e8f0;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 6px;
    transition: all 0.2s ease;

    &:hover {
      background: #f1f5f9;
      color: #0b1326;
      border-color: #cbd5e1;
    }

    &.active {
      background: #fdf2f6;
      border-color: #fce7f3;
      color: #df266a;
    }

    .tab-count-badge {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 700;
      padding: 1px 6px;
      border-radius: 999px;

      &.badge-slate {
        background: #e2e8f0;
        color: #475569;
      }

      &.badge-rose {
        background: #df266a;
        color: #ffffff;
      }

      &.badge-emerald {
        background: #059669;
        color: #ffffff;
      }
    }
  }
}

/* 3. Action Right */
.filter-actions-right {
  margin-left: auto;

  @media (max-width: 900px) {
    margin-left: 0;
  }

  .btn-action-refresh {
    height: 36px;
    padding: 0 14px;
    border-radius: 9px;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    color: #475569;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 6px;
    transition: all 0.2s ease;

    &:hover:not(:disabled) {
      border-color: #df266a;
      color: #df266a;
      background: #fdf2f6;
    }

    @media (max-width: 600px) {
      .refresh-text {
        display: none;
      }
    }
  }
}
</style>
