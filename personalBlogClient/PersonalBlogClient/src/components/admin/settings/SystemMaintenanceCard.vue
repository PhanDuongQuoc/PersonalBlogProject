<template>
  <div class="settings-card maintenance-settings-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-purple">
          <q-icon name="fa-solid fa-screwdriver-wrench" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Bảo Trì & Công Cụ Hệ Thống</h3>
          <p class="card-subtitle">Chế độ bảo trì, làm mới bộ nhớ đệm và sao lưu cấu hình</p>
        </div>
      </div>
      <span class="module-badge">System Utilities</span>
    </div>

    <div class="card-body-form">
      <!-- 1. Maintenance Mode Toggle -->
      <div class="maintenance-toggle-box" :class="{ 'in-maintenance': model.maintenanceMode }">
        <div class="toggle-row" @click="model.maintenanceMode = !model.maintenanceMode">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon
                :name="model.maintenanceMode ? 'fa-solid fa-triangle-exclamation' : 'fa-solid fa-power-off'"
                size="13px"
                :class="model.maintenanceMode ? 'text-amber-8' : 'text-grey-6'"
                class="q-mr-xs"
              />
              <span>Chế độ bảo trì hệ thống (Maintenance Mode)</span>
            </div>
            <div class="toggle-desc">
              Khi bật, độc giả truy cập website sẽ thấy trang thông báo bảo trì tạm thời. Quản trị viên vẫn truy cập bình thường.
            </div>
          </div>
          <q-toggle
            v-model="model.maintenanceMode"
            color="amber-9"
            dense
            @click.stop
          />
        </div>

        <div v-if="model.maintenanceMode" class="maintenance-notice-field">
          <label class="form-label">
            <span>Thông điệp bảo trì hiển thị cho độc giả:</span>
          </label>
          <input
            v-model="model.maintenanceNotice"
            type="text"
            placeholder="Hệ thống đang được bảo trì..."
            class="form-input"
          />
        </div>
      </div>

      <!-- 2. System Utilities Action Row -->
      <div class="utilities-actions-grid">
        <!-- Clear Cache Tool -->
        <div class="utility-card">
          <div class="utility-icon icon-rose">
            <q-icon name="fa-solid fa-broom" size="16px" />
          </div>
          <div class="utility-info">
            <div class="utility-title">Làm Mới Bộ Nhớ Đệm (Cache)</div>
            <div class="utility-desc">Xóa cache RAM để đồng bộ dữ liệu bài viết và chủ đề mới nhất.</div>
          </div>
          <button
            type="button"
            class="btn-utility-action btn-clear-cache"
            :disabled="clearingCache"
            @click="handleClearCache"
          >
            <q-spinner-tail v-if="clearingCache" size="14px" color="pink-7" />
            <q-icon v-else name="fa-solid fa-rotate" size="12px" />
            <span>{{ clearingCache ? 'Đang xóa...' : 'Xóa Cache' }}</span>
          </button>
        </div>

        <!-- Export / Backup Config Tool -->
        <div class="utility-card">
          <div class="utility-icon icon-indigo">
            <q-icon name="fa-solid fa-file-export" size="16px" />
          </div>
          <div class="utility-info">
            <div class="utility-title">Sao Lưu Cấu Hình (JSON)</div>
            <div class="utility-desc">Xuất toàn bộ thiết lập hệ thống ra tệp JSON để lưu trữ dự phòng.</div>
          </div>
          <button
            type="button"
            class="btn-utility-action btn-export-config"
            @click="$emit('export-config')"
          >
            <q-icon name="fa-solid fa-download" size="12px" />
            <span>Tải JSON</span>
          </button>
        </div>
      </div>

      <!-- System Timestamps Footer -->
      <div class="system-timestamps-row">
        <div class="timestamp-item">
          <span class="ts-lbl">Cập nhật cấu hình:</span>
          <span class="ts-val font-mono">{{ formatDate(model.lastSettingsUpdatedAt) }}</span>
        </div>
        <div class="timestamp-item">
          <span class="ts-lbl">Xóa cache gần nhất:</span>
          <span class="ts-val font-mono">{{ formatDate(model.lastCacheClearedAt) || 'Chưa xóa' }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import type { SystemMaintenanceSettings } from "@/types/admin-settings";
import { adminSettingsService } from "@/services/admin-settings.service";
import { swalToast, swalError } from "@/utils/swal";

const model = defineModel<SystemMaintenanceSettings>({ required: true });

defineEmits<{
  (e: "export-config"): void;
}>();

const clearingCache = ref(false);

async function handleClearCache() {
  try {
    clearingCache.value = true;
    const res = await adminSettingsService.clearCache();
    if (res.success) {
      model.value.lastCacheClearedAt = new Date().toISOString();
      swalToast(res.message, "success");
    }
  } catch (err: any) {
    swalError("Lỗi xóa cache", err.response?.data?.message || "Không thể làm mới cache.");
  } finally {
    clearingCache.value = false;
  }
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
</script>

<style scoped lang="scss">
.settings-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(11, 19, 38, 0.03);
  transition: all 0.25s ease;

  &:hover {
    box-shadow: 0 8px 26px rgba(11, 19, 38, 0.06);
    border-color: #cbd5e1;
  }
}

.card-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 22px;

  .header-left {
    display: flex;
    align-items: center;
    gap: 12px;

    .header-icon-box {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;

      &.icon-purple {
        background: #f5f3ff;
        color: #7c3aed;
        border: 1px solid #ede9fe;
      }
    }

    .card-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 16px;
      font-weight: 700;
      color: #0b1326;
      margin: 0 0 2px;
    }

    .card-subtitle {
      font-family: var(--font-body, sans-serif);
      font-size: 12.5px;
      color: #64748b;
      margin: 0;
    }
  }

  .module-badge {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 700;
    background: #f8fafc;
    color: #475569;
    border: 1px solid #e2e8f0;
    padding: 3px 8px;
    border-radius: 6px;
  }
}

.card-body-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.maintenance-toggle-box {
  background: #f8fafc;
  border: 1px solid #f1f5f9;
  border-radius: 12px;
  padding: 14px 16px;
  transition: all 0.2s ease;

  &.in-maintenance {
    background: #fffbeb;
    border-color: #fde68a;
  }

  .toggle-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 16px;
    cursor: pointer;

    .toggle-info {
      flex: 1;

      .toggle-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 13.5px;
        font-weight: 700;
        color: #0b1326;
        margin-bottom: 2px;
        display: flex;
        align-items: center;
      }

      .toggle-desc {
        font-family: var(--font-body, sans-serif);
        font-size: 12px;
        color: #64748b;
      }
    }
  }

  .maintenance-notice-field {
    margin-top: 12px;
    padding-top: 12px;
    border-top: 1px dashed #fed7aa;
    display: flex;
    flex-direction: column;
    gap: 6px;

    .form-label {
      font-size: 12.5px;
      font-weight: 700;
      color: #92400e;
    }

    .form-input {
      width: 100%;
      background: #ffffff;
      border: 1px solid #fcd34d;
      border-radius: 8px;
      padding: 8px 12px;
      font-size: 13px;
      color: #0b1326;
      outline: none;

      &:focus {
        border-color: #d97706;
      }
    }
  }
}

.utilities-actions-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;

  @media (max-width: 640px) {
    grid-template-columns: 1fr;
  }

  .utility-card {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 14px 16px;
    background: #f8fafc;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    transition: all 0.2s ease;

    &:hover {
      background: #ffffff;
      border-color: #cbd5e1;
    }

    .utility-icon {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;

      &.icon-rose {
        background: #fdf2f6;
        color: #df266a;
      }

      &.icon-indigo {
        background: #eef2ff;
        color: #4f46e5;
      }
    }

    .utility-info {
      flex: 1;
      min-width: 0;

      .utility-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 13px;
        font-weight: 700;
        color: #0b1326;
        margin-bottom: 2px;
      }

      .utility-desc {
        font-family: var(--font-body, sans-serif);
        font-size: 11.5px;
        color: #64748b;
        line-height: 1.35;
      }
    }

    .btn-utility-action {
      padding: 7px 14px;
      border-radius: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 12px;
      font-weight: 700;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 6px;
      transition: all 0.2s ease;
      flex-shrink: 0;

      &.btn-clear-cache {
        background: #fdf2f6;
        color: #df266a;
        border: 1px solid #fce7f3;

        &:hover:not(:disabled) {
          background: #df266a;
          color: #ffffff;
        }
      }

      &.btn-export-config {
        background: #eef2ff;
        color: #4f46e5;
        border: 1px solid #e0e7ff;

        &:hover {
          background: #4f46e5;
          color: #ffffff;
        }
      }

      &:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }
    }
  }
}

.system-timestamps-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding-top: 12px;
  border-top: 1px solid #f1f5f9;
  font-size: 12px;
  flex-wrap: wrap;

  .timestamp-item {
    display: flex;
    align-items: center;
    gap: 6px;

    .ts-lbl {
      color: #64748b;
    }

    .ts-val {
      color: #0b1326;
      font-weight: 600;

      &.font-mono {
        font-family: var(--font-mono, monospace);
      }
    }
  }
}
</style>
