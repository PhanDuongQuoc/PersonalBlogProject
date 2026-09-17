<template>
  <div class="settings-card seo-settings-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-indigo">
          <q-icon name="fa-solid fa-magnifying-glass-chart" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Tối Ưu SEO & Thẻ Mạng Xã Hội</h3>
          <p class="card-subtitle">Cấu hình thẻ meta, OpenGraph preview và Google Analytics</p>
        </div>
      </div>
      <span class="module-badge">Search & Social</span>
    </div>

    <div class="card-body-form">
      <!-- Title Template & Canonical URL -->
      <div class="form-row-2col">
        <div class="form-group">
          <label class="form-label">
            <span>Cấu trúc Tiêu đề SEO (Title Template)</span>
          </label>
          <input
            v-model="model.metaTitleTemplate"
            type="text"
            placeholder="VD: %title% · PDQ Blog"
            class="form-input font-mono"
          />
          <span class="form-help-text">Dùng %title% làm biến đại diện cho tiêu đề từng bài viết</span>
        </div>

        <div class="form-group">
          <label class="form-label">
            <span>Tên miền gốc (Canonical Base URL)</span>
          </label>
          <input
            v-model="model.canonicalBaseUrl"
            type="url"
            placeholder="VD: https://phanduongquoc.dev"
            class="form-input font-mono"
          />
        </div>
      </div>

      <!-- Keywords -->
      <div class="form-group">
        <label class="form-label">
          <span>Từ khóa mặc định (Meta Keywords)</span>
        </label>
        <input
          v-model="model.defaultKeywords"
          type="text"
          placeholder="VD: Vue.js, .NET 9, Clean Architecture, PostgreSQL, Tech Blog"
          class="form-input"
        />
        <span class="form-help-text">Phân cách các từ khóa bằng dấu phẩy</span>
      </div>

      <!-- Social Share Image & Google Analytics -->
      <div class="form-row-2col">
        <div class="form-group">
          <label class="form-label">
            <span>Ảnh chia sẻ mặc định (OpenGraph Image URL)</span>
          </label>
          <input
            v-model="model.defaultOgImageUrl"
            type="url"
            placeholder="https://.../og-banner.jpg"
            class="form-input font-mono"
          />
        </div>

        <div class="form-group">
          <label class="form-label">
            <span>Google Analytics ID</span>
          </label>
          <input
            v-model="model.googleAnalyticsId"
            type="text"
            placeholder="VD: G-XXXXXXXXXX"
            class="form-input font-mono"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { SeoMetaSettings } from "@/types/admin-settings";

const model = defineModel<SeoMetaSettings>({ required: true });
</script>

<style scoped lang="scss">
.settings-card {
  background: var(--bg-surface-low, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(11, 19, 38, 0.03);
  transition: all 0.25s ease;

  &:hover {
    box-shadow: 0 8px 26px rgba(11, 19, 38, 0.06);
    border-color: var(--border-subtle, #cbd5e1);
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

      &.icon-indigo {
        background: rgba(79, 70, 229, 0.12);
        color: #6366f1;
        border: 1px solid rgba(79, 70, 229, 0.25);
      }
    }

    .card-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 16px;
      font-weight: 700;
      color: var(--text-primary, #0b1326);
      margin: 0 0 2px;
    }

    .card-subtitle {
      font-family: var(--font-body, sans-serif);
      font-size: 12.5px;
      color: var(--text-muted, #64748b);
      margin: 0;
    }
  }

  .module-badge {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 700;
    background: var(--bg-surface-lowest, #f8fafc);
    color: var(--text-secondary, #475569);
    border: 1px solid var(--border-hairline, #e2e8f0);
    padding: 3px 8px;
    border-radius: 6px;
  }
}

.card-body-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-row-2col {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;

  @media (max-width: 640px) {
    grid-template-columns: 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;

  .form-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 700;
    color: var(--text-secondary, #334155);
  }

  .form-input {
    width: 100%;
    background: var(--bg-surface-lowest, #f8fafc);
    border: 1px solid var(--border-hairline, #e2e8f0);
    border-radius: 9px;
    padding: 10px 14px;
    font-family: var(--font-body, sans-serif);
    font-size: 13.5px;
    color: var(--text-primary, #0b1326);
    outline: none;
    transition: all 0.2s ease;

    &::placeholder {
      color: var(--text-muted, #94a3b8);
      font-size: 13px;
    }

    &:focus {
      background: var(--bg-surface-low, #ffffff);
      border-color: #4f46e5;
      box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
    }

    &.font-mono {
      font-family: var(--font-mono, monospace);
    }
  }

  .form-help-text {
    font-size: 11.5px;
    color: var(--text-muted, #94a3b8);
  }
}
</style>
