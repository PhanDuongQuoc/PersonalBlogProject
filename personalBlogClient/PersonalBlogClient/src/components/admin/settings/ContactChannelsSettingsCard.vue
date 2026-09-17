<template>
  <div class="settings-card contact-settings-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-sky">
          <q-icon name="fa-solid fa-share-nodes" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Kênh Liên Hệ & Mạng Xã Hội</h3>
          <p class="card-subtitle">Email nhận tin nhắn và các liên kết mạng xã hội chính</p>
        </div>
      </div>
      <span class="module-badge">Contact & Social</span>
    </div>

    <div class="card-body-form">
      <!-- Contact Email & Phone -->
      <div class="form-row-2col">
        <div class="form-group">
          <label class="form-label">
            <span>Email Nhận Thông Báo Liên Hệ</span>
            <span class="required-star">*</span>
          </label>
          <div class="input-with-icon">
            <q-icon name="fa-solid fa-envelope" size="13px" class="field-icon" />
            <input
              v-model="model.contactEmail"
              type="email"
              placeholder="VD: phanduongquoc111@gmail.com"
              class="form-input"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">
            <span>Số Điện Thoại Liên Hệ</span>
          </label>
          <div class="input-with-icon">
            <q-icon name="fa-solid fa-phone" size="13px" class="field-icon" />
            <input
              v-model="model.phone"
              type="text"
              placeholder="VD: +84 987 654 321"
              class="form-input"
            />
          </div>
        </div>
      </div>

      <!-- Location & Personal Website -->
      <div class="form-row-2col">
        <div class="form-group">
          <label class="form-label">
            <span>Địa Điểm / Thành Phố</span>
          </label>
          <div class="input-with-icon">
            <q-icon name="fa-solid fa-location-dot" size="13px" class="field-icon" />
            <input
              v-model="model.location"
              type="text"
              placeholder="VD: Hồ Chí Minh, Việt Nam"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">
            <span>Website Cá Nhân / Portfolio</span>
          </label>
          <div class="input-with-icon">
            <q-icon name="fa-solid fa-globe" size="13px" class="field-icon" />
            <input
              v-model="model.websiteUrl"
              type="url"
              placeholder="https://phanduongquoc.dev"
              class="form-input font-mono"
            />
          </div>
        </div>
      </div>

      <!-- Social Media URLs Grid -->
      <div class="social-links-grid">
        <!-- GitHub -->
        <div class="form-group">
          <label class="form-label">
            <q-icon name="fa-brands fa-github" size="14px" class="text-grey-9 q-mr-xs" />
            <span>GitHub Profile</span>
          </label>
          <input
            v-model="model.githubUrl"
            type="url"
            placeholder="https://github.com/..."
            class="form-input font-mono"
          />
        </div>

        <!-- LinkedIn -->
        <div class="form-group">
          <label class="form-label">
            <q-icon name="fa-brands fa-linkedin" size="14px" class="text-blue-8 q-mr-xs" />
            <span>LinkedIn Profile</span>
          </label>
          <input
            v-model="model.linkedinUrl"
            type="url"
            placeholder="https://linkedin.com/in/..."
            class="form-input font-mono"
          />
        </div>

        <!-- Facebook -->
        <div class="form-group">
          <label class="form-label">
            <q-icon name="fa-brands fa-facebook" size="14px" class="text-blue-7 q-mr-xs" />
            <span>Facebook Page / Profile</span>
          </label>
          <input
            v-model="model.facebookUrl"
            type="url"
            placeholder="https://facebook.com/..."
            class="form-input font-mono"
          />
        </div>

        <!-- X (Twitter) -->
        <div class="form-group">
          <label class="form-label">
            <q-icon name="fa-brands fa-x-twitter" size="14px" class="text-grey-9 q-mr-xs" />
            <span>X (Twitter)</span>
          </label>
          <input
            v-model="model.twitterUrl"
            type="url"
            placeholder="https://twitter.com/..."
            class="form-input font-mono"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ContactSettings } from "@/types/admin-settings";

const model = defineModel<ContactSettings>({ required: true });
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

      &.icon-sky {
        background: rgba(2, 132, 199, 0.12);
        color: #0284c7;
        border: 1px solid rgba(2, 132, 199, 0.25);
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

.social-links-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  border-top: 1px solid var(--border-hairline, #f1f5f9);
  padding-top: 14px;

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
    display: flex;
    align-items: center;

    .required-star {
      color: #df266a;
      margin-left: 4px;
    }
  }

  .input-with-icon {
    position: relative;
    display: flex;
    align-items: center;

    .field-icon {
      position: absolute;
      left: 12px;
      color: var(--text-muted, #94a3b8);
      pointer-events: none;
    }

    .form-input {
      padding-left: 36px;
    }
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
      border-color: #0284c7;
      box-shadow: 0 0 0 3px rgba(2, 132, 199, 0.1);
    }

    &.font-mono {
      font-family: var(--font-mono, monospace);
    }
  }
}
</style>
