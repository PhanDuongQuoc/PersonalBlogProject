<template>
  <q-page class="admin-settings-page">
    <!-- 1. Header Section -->
    <header class="settings-header-section">
      <!-- Navigation Filter Pills (Quick Jump) -->
      <div class="settings-nav-tabs">
        <button
          v-for="tab in tabs"
          :key="tab.id"
          type="button"
          class="tab-nav-btn"
          :class="{ active: activeTab === tab.id }"
          @click="activeTab = tab.id"
        >
          <q-icon :name="tab.icon" size="13px" />
          <span>{{ tab.label }}</span>
        </button>
      </div>

      <!-- Action Buttons Right -->
      <div class="header-actions">
        <button
          type="button"
          class="btn-action-refresh"
          title="Tải lại cấu hình từ máy chủ"
          :disabled="loading || saving"
          @click="fetchSettings"
        >
          <q-icon name="fa-solid fa-rotate-right" size="13px" :class="{ 'fa-spin': loading }" />
          <span>Làm mới</span>
        </button>

        <button
          type="button"
          class="btn-action-save"
          title="Lưu tất cả thay đổi cấu hình"
          :disabled="loading || saving || !settings"
          @click="saveSettings"
        >
          <q-spinner-tail v-if="saving" size="14px" color="white" />
          <q-icon v-else name="fa-solid fa-floppy-disk" size="13px" />
          <span>{{ saving ? 'Đang lưu...' : 'Lưu Thay Đổi' }}</span>
        </button>
      </div>
    </header>

    <!-- 2. Loading State -->
    <div v-if="loading && !settings" class="page-loading-box">
      <q-spinner-tail color="pink-7" size="36px" />
      <span class="loading-text">Đang tải cấu hình hệ thống...</span>
    </div>

    <!-- 3. Settings Modules Main Container -->
    <main v-else-if="settings" class="settings-main-container">
      <!-- Tab 1: ALL OR GENERAL -->
      <section v-show="activeTab === 'all' || activeTab === 'general'" class="settings-section">
        <GeneralSiteSettingsCard v-model="settings.general" />
      </section>

      <!-- Tab 2: ALL OR SEO -->
      <section v-show="activeTab === 'all' || activeTab === 'seo'" class="settings-section">
        <SeoMetaSettingsCard v-model="settings.seo" />
      </section>

      <!-- Tab 3: ALL OR READING -->
      <section v-show="activeTab === 'all' || activeTab === 'reading'" class="settings-section">
        <ReadingPreferencesCard v-model="settings.reading" />
      </section>

      <!-- Tab 4: ALL OR COMMENTS -->
      <section v-show="activeTab === 'all' || activeTab === 'comments'" class="settings-section">
        <CommentModerationSettingsCard v-model="settings.comments" />
      </section>

      <!-- Tab 5: ALL OR CONTACT -->
      <section v-show="activeTab === 'all' || activeTab === 'contact'" class="settings-section">
        <ContactChannelsSettingsCard v-model="settings.contact" />
      </section>

      <!-- Tab 6: ALL OR MAINTENANCE -->
      <section v-show="activeTab === 'all' || activeTab === 'maintenance'" class="settings-section">
        <SystemMaintenanceCard
          v-model="settings.maintenance"
          @export-config="exportConfigJson"
        />
      </section>
    </main>

    <!-- Floating Sticky Bottom Bar for Quick Save -->
    <div v-if="settings" class="floating-save-bar">
      <div class="bar-left-hint">
        <q-icon name="fa-solid fa-circle-info" size="13px" class="text-indigo-6 q-mr-xs" />
        <span>Các thay đổi sẽ có hiệu lực ngay lập tức trên toàn bộ hệ thống sau khi lưu.</span>
      </div>
      <button
        type="button"
        class="btn-floating-save"
        :disabled="saving"
        @click="saveSettings"
      >
        <q-spinner-tail v-if="saving" size="14px" color="white" />
        <q-icon v-else name="fa-solid fa-floppy-disk" size="13px" />
        <span>{{ saving ? 'Đang lưu...' : 'Lưu Thay Đổi' }}</span>
      </button>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import GeneralSiteSettingsCard from "@/components/admin/settings/GeneralSiteSettingsCard.vue";
import SeoMetaSettingsCard from "@/components/admin/settings/SeoMetaSettingsCard.vue";
import ReadingPreferencesCard from "@/components/admin/settings/ReadingPreferencesCard.vue";
import CommentModerationSettingsCard from "@/components/admin/settings/CommentModerationSettingsCard.vue";
import ContactChannelsSettingsCard from "@/components/admin/settings/ContactChannelsSettingsCard.vue";
import SystemMaintenanceCard from "@/components/admin/settings/SystemMaintenanceCard.vue";
import type { SiteSettings } from "@/types/admin-settings";
import { adminSettingsService } from "@/services/admin-settings.service";
import { swalToast, swalError } from "@/utils/swal";

const tabs = [
  { id: "all", label: "Tất cả thiết lập", icon: "fa-solid fa-border-all" },
  { id: "general", label: "Chung & Thương hiệu", icon: "fa-solid fa-sliders" },
  { id: "seo", label: "SEO & Mạng xã hội", icon: "fa-solid fa-magnifying-glass-chart" },
  { id: "reading", label: "Đọc & Hiển thị", icon: "fa-solid fa-book-open-reader" },
  { id: "comments", label: "Bình luận & Spam", icon: "fa-solid fa-comments" },
  { id: "contact", label: "Liên hệ & Social", icon: "fa-solid fa-share-nodes" },
  { id: "maintenance", label: "Bảo trì & Công cụ", icon: "fa-solid fa-screwdriver-wrench" }
];

const activeTab = ref("all");
const loading = ref(false);
const saving = ref(false);
const settings = ref<SiteSettings | null>(null);

async function fetchSettings() {
  try {
    loading.value = true;
    settings.value = await adminSettingsService.getSettings();
  } catch (err: any) {
    console.error("Failed to fetch settings:", err);
    swalError("Lỗi tải dữ liệu", err.response?.data?.message || "Không thể tải cấu hình hệ thống.");
  } finally {
    loading.value = false;
  }
}

async function saveSettings() {
  if (!settings.value) return;

  try {
    saving.value = true;
    const res = await adminSettingsService.updateSettings(settings.value);
    if (res.data) {
      settings.value = res.data;
    }
    swalToast(res.message || "Đã lưu thay đổi cấu hình thành công!", "success");
  } catch (err: any) {
    console.error("Failed to update settings:", err);
    swalError("Lưu thất bại", err.response?.data?.message || "Không thể lưu thay đổi.");
  } finally {
    saving.value = false;
  }
}

function exportConfigJson() {
  if (!settings.value) return;
  const jsonStr = JSON.stringify(settings.value, null, 2);
  const blob = new Blob([jsonStr], { type: "application/json" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `site-settings-${new Date().toISOString().slice(0, 10)}.json`;
  link.click();
  swalToast("Đã tải tệp sao lưu cấu hình JSON thành công!", "success");
}

onMounted(() => {
  fetchSettings();
});
</script>

<style scoped lang="scss">
.admin-settings-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 80px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Header Section */
.settings-header-section {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 22px;
  flex-wrap: wrap;

  .settings-nav-tabs {
    display: flex;
    align-items: center;
    gap: 8px;
    flex-wrap: wrap;

    .tab-nav-btn {
      padding: 7px 14px;
      border-radius: 9px;
      font-family: var(--font-headline, sans-serif);
      font-size: 12.5px;
      font-weight: 700;
      color: var(--text-secondary, #475569);
      background: var(--bg-surface-low, #ffffff);
      border: 1px solid var(--border-hairline, #e2e8f0);
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 7px;
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--border-subtle, #cbd5e1);
        background: var(--bg-surface-high, #f8fafc);
        color: var(--text-primary, #0b1326);
      }

      &.active {
        background: rgba(223, 38, 106, 0.12);
        border-color: rgba(223, 38, 106, 0.25);
        color: #df266a;
      }
    }
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-left: auto;

    .btn-action-refresh {
      height: 40px;
      padding: 0 16px;
      border-radius: 10px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      background: var(--bg-surface-low, #ffffff);
      border: 1px solid var(--border-hairline, #e2e8f0);
      color: var(--text-secondary, #475569);
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        border-color: #df266a;
        color: #df266a;
        background: rgba(223, 38, 106, 0.12);
      }
    }

    .btn-action-save {
      height: 40px;
      padding: 0 18px;
      border-radius: 10px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      background: #df266a;
      border: none;
      color: #ffffff;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.25);
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        background: #be185d;
        transform: translateY(-1px);
        box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
      }

      &:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }
    }
  }
}

/* 2. Loading State */
.page-loading-box {
  padding: 80px 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;

  .loading-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    color: var(--text-muted, #64748b);
  }
}

/* 3. Main Container */
.settings-main-container {
  display: flex;
  flex-direction: column;
  gap: 22px;
}

.settings-section {
  width: 100%;
}

/* 4. Floating Save Bar */
.floating-save-bar {
  position: fixed;
  bottom: 20px;
  right: 24px;
  left: 304px; // Account for admin drawer width
  background: rgba(11, 19, 38, 0.92);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(248, 250, 252, 0.12);
  border-radius: 14px;
  padding: 12px 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
  z-index: 100;
  transition: all 0.25s ease;

  @media (max-width: 1024px) {
    left: 20px;
    right: 20px;
  }

  .bar-left-hint {
    font-family: var(--font-body, sans-serif);
    font-size: 12.5px;
    color: #dae2fd;
    display: flex;
    align-items: center;

    @media (max-width: 768px) {
      display: none;
    }
  }

  .btn-floating-save {
    height: 38px;
    padding: 0 20px;
    background: #df266a;
    color: #ffffff;
    border: none;
    border-radius: 9px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 700;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    transition: all 0.2s ease;
    box-shadow: 0 4px 12px rgba(223, 38, 106, 0.3);

    &:hover:not(:disabled) {
      background: #be185d;
      transform: translateY(-1px);
      box-shadow: 0 6px 16px rgba(223, 38, 106, 0.4);
    }

    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
  }
}
</style>
