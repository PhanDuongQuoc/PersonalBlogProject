<template>
  <div class="settings-card comment-settings-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-amber">
          <q-icon name="fa-solid fa-comments" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Quản Lý & Kiểm Duyệt Bình Luận</h3>
          <p class="card-subtitle">Chính sách tương tác độc giả và bộ lọc ngăn chặn spam</p>
        </div>
      </div>
      <span class="module-badge">Community & Moderation</span>
    </div>

    <div class="card-body-form">
      <!-- Toggles List -->
      <div class="toggles-list">
        <!-- Toggle 1: Enable Comments -->
        <div class="toggle-row" @click="model.enableComments = !model.enableComments">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-comment-dots" size="12px" class="q-mr-xs text-grey-6" />
              <span>Cho phép gửi bình luận toàn cục</span>
            </div>
            <div class="toggle-desc">Bật/Tắt khung gửi bình luận ở cuối tất cả bài viết trên toàn bộ blog.</div>
          </div>
          <q-toggle
            v-model="model.enableComments"
            color="pink-7"
            dense
            @click.stop
          />
        </div>

        <!-- Toggle 2: Require Moderation -->
        <div class="toggle-row" @click="model.requireModeration = !model.requireModeration">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-shield-halved" size="12px" class="q-mr-xs text-grey-6" />
              <span>Yêu cầu kiểm duyệt trước khi hiển thị</span>
            </div>
            <div class="toggle-desc">Bình luận mới sẽ vào trạng thái "Chờ duyệt" để quản trị viên kiểm tra trước khi công khai.</div>
          </div>
          <q-toggle
            v-model="model.requireModeration"
            color="pink-7"
            dense
            @click.stop
          />
        </div>

        <!-- Toggle 3: Allow Guest Comments -->
        <div class="toggle-row" @click="model.allowGuestComments = !model.allowGuestComments">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-user-clock" size="12px" class="q-mr-xs text-grey-6" />
              <span>Cho phép độc giả vãng lai bình luận</span>
            </div>
            <div class="toggle-desc">Độc giả không cần đăng nhập tài khoản, chỉ cần nhập Tên và Email để gửi bình luận.</div>
          </div>
          <q-toggle
            v-model="model.allowGuestComments"
            color="pink-7"
            dense
            @click.stop
          />
        </div>
      </div>

      <!-- Forbidden Keywords Filter -->
      <div class="form-group forbidden-group">
        <label class="form-label">
          <span>Bộ lọc từ khóa cấm & Chặn Spam</span>
        </label>
        <textarea
          v-model="model.forbiddenKeywords"
          rows="3"
          placeholder="spam, scam, quảng cáo, cờ bạc, lừa đảo..."
          class="form-textarea"
        ></textarea>
        <span class="form-help-text">Bình luận chứa các từ khóa này sẽ tự động bị chặn hoặc đánh dấu nghi ngờ spam (phân cách bằng dấu phẩy).</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { CommentSettings } from "@/types/admin-settings";

const model = defineModel<CommentSettings>({ required: true });
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

      &.icon-amber {
        background: rgba(217, 119, 6, 0.12);
        color: #d97706;
        border: 1px solid rgba(217, 119, 6, 0.25);
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
  gap: 18px;
}

.toggles-list {
  display: flex;
  flex-direction: column;
  gap: 10px;

  .toggle-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 16px;
    padding: 12px 14px;
    background: var(--bg-surface-lowest, #f8fafc);
    border: 1px solid var(--border-hairline, #f1f5f9);
    border-radius: 10px;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      background: var(--bg-surface-low, #ffffff);
      border-color: var(--border-subtle, #cbd5e1);
    }

    .toggle-info {
      flex: 1;

      .toggle-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 13.5px;
        font-weight: 700;
        color: var(--text-primary, #0b1326);
        margin-bottom: 2px;
      }

      .toggle-desc {
        font-family: var(--font-body, sans-serif);
        font-size: 12px;
        color: var(--text-muted, #64748b);
      }
    }
  }
}

.forbidden-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
  border-top: 1px solid var(--border-hairline, #f1f5f9);
  padding-top: 14px;

  .form-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 700;
    color: var(--text-secondary, #334155);
  }

  .form-textarea {
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
    resize: vertical;
    min-height: 72px;

    &::placeholder {
      color: var(--text-muted, #94a3b8);
      font-size: 13px;
    }

    &:focus {
      background: var(--bg-surface-low, #ffffff);
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }
  }

  .form-help-text {
    font-size: 11.5px;
    color: var(--text-muted, #94a3b8);
  }
}
</style>
