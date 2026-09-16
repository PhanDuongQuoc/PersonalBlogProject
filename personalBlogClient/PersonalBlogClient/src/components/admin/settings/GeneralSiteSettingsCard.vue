<template>
  <div class="settings-card general-settings-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-rose">
          <q-icon name="fa-solid fa-sliders" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Cài Đặt Chung Website</h3>
          <p class="card-subtitle">Tên thương hiệu, khẩu hiệu, logo và thông tin bản quyền</p>
        </div>
      </div>
      <span class="module-badge">General Info</span>
    </div>

    <div class="card-body-form">
      <!-- Row 1: Site Title & Logo Text -->
      <div class="form-row-2col">
        <div class="form-group">
          <label class="form-label">
            <span>Tên Blog / Website</span>
            <span class="required-star">*</span>
          </label>
          <input
            v-model="model.siteTitle"
            type="text"
            placeholder="VD: PDQ Portfolio & Tech Blog"
            class="form-input"
            required
          />
        </div>

        <div class="form-group">
          <label class="form-label">
            <span>Logo Viết Tắt (Brand Mark)</span>
          </label>
          <input
            v-model="model.logoText"
            type="text"
            placeholder="VD: P."
            class="form-input font-mono"
            maxlength="10"
          />
        </div>
      </div>

      <!-- Row 2: Tagline / Slogan -->
      <div class="form-group">
        <label class="form-label">
          <span>Khẩu hiệu / Slogan</span>
        </label>
        <input
          v-model="model.tagline"
          type="text"
          placeholder="VD: Chia sẻ kiến trúc phần mềm, Vue.js, .NET & Cloud"
          class="form-input"
        />
      </div>

      <!-- Row 3: Site Description -->
      <div class="form-group">
        <label class="form-label">
          <span>Mô tả ngắn trang web (Footer & Giới thiệu)</span>
        </label>
        <textarea
          v-model="model.siteDescription"
          rows="3"
          placeholder="Mô tả tóm tắt về mục đích và nội dung của blog..."
          class="form-textarea"
        ></textarea>
      </div>

      <!-- Row 4: Footer Copyright -->
      <div class="form-group">
        <label class="form-label">
          <span>Dòng chữ bản quyền Footer</span>
        </label>
        <input
          v-model="model.footerCopyright"
          type="text"
          placeholder="VD: © 2026 Phan Duong Quoc. All rights reserved."
          class="form-input"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { SiteGeneralSettings } from "@/types/admin-settings";

const model = defineModel<SiteGeneralSettings>({ required: true });
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

      &.icon-rose {
        background: #fdf2f6;
        color: #df266a;
        border: 1px solid #fce7f3;
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
  gap: 16px;
}

.form-row-2col {
  display: grid;
  grid-template-columns: 2fr 1fr;
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
    color: #334155;
    display: flex;
    align-items: center;
    gap: 4px;

    .required-star {
      color: #df266a;
    }
  }

  .form-input, .form-textarea {
    width: 100%;
    background: #f8fafc;
    border: 1px solid #e2e8f0;
    border-radius: 9px;
    padding: 10px 14px;
    font-family: var(--font-body, sans-serif);
    font-size: 13.5px;
    color: #0b1326;
    outline: none;
    transition: all 0.2s ease;

    &::placeholder {
      color: #94a3b8;
      font-size: 13px;
    }

    &:focus {
      background: #ffffff;
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }

    &.font-mono {
      font-family: var(--font-mono, monospace);
    }
  }

  .form-textarea {
    resize: vertical;
    min-height: 76px;
    line-height: 1.45;
  }
}
</style>
