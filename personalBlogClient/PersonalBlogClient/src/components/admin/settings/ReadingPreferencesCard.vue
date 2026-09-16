<template>
  <div class="settings-card reading-settings-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-emerald">
          <q-icon name="fa-solid fa-book-open-reader" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Tùy Chọn Đọc & Hiển Thị Bài Viết</h3>
          <p class="card-subtitle">Cấu hình trải nghiệm đọc bài và phân trang danh sách</p>
        </div>
      </div>
      <span class="module-badge">Reading Experience</span>
    </div>

    <div class="card-body-form">
      <!-- 1. Posts Per Page Selector -->
      <div class="form-group">
        <label class="form-label">
          <span>Số bài viết hiển thị mỗi trang (Posts Per Page)</span>
        </label>
        <div class="posts-per-page-options">
          <button
            v-for="num in [6, 9, 12, 15]"
            :key="num"
            type="button"
            class="num-choice-btn"
            :class="{ active: model.postsPerPage === num }"
            @click="model.postsPerPage = num"
          >
            {{ num }} bài / trang
          </button>
        </div>
      </div>

      <!-- 2. Interactive Feature Toggles Grid -->
      <div class="toggles-list">
        <!-- Toggle: Show Reading Time -->
        <div class="toggle-row" @click="model.showReadingTime = !model.showReadingTime">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-clock" size="12px" class="q-mr-xs text-grey-6" />
              <span>Thời gian đọc ước tính</span>
            </div>
            <div class="toggle-desc">Tự động tính toán số phút đọc dựa trên độ dài nội dung (VD: "5 phút đọc").</div>
          </div>
          <q-toggle
            v-model="model.showReadingTime"
            color="pink-7"
            dense
            @click.stop
          />
        </div>

        <!-- Toggle: Show Public View Count -->
        <div class="toggle-row" @click="model.showPublicViewCount = !model.showPublicViewCount">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-eye" size="12px" class="q-mr-xs text-grey-6" />
              <span>Lượt xem công khai</span>
            </div>
            <div class="toggle-desc">Hiển thị số lượt xem bài viết cho độc giả trên thẻ bài và trang chi tiết.</div>
          </div>
          <q-toggle
            v-model="model.showPublicViewCount"
            color="pink-7"
            dense
            @click.stop
          />
        </div>

        <!-- Toggle: Show Related Posts -->
        <div class="toggle-row" @click="model.showRelatedPosts = !model.showRelatedPosts">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-newspaper" size="12px" class="q-mr-xs text-grey-6" />
              <span>Khối Bài viết liên quan</span>
            </div>
            <div class="toggle-desc">Gợi ý 3 bài viết cùng chủ đề ở chân trang chi tiết bài viết.</div>
          </div>
          <q-toggle
            v-model="model.showRelatedPosts"
            color="pink-7"
            dense
            @click.stop
          />
        </div>

        <!-- Toggle: Show Author Bio In Posts -->
        <div class="toggle-row" @click="model.showAuthorBioInPosts = !model.showAuthorBioInPosts">
          <div class="toggle-info">
            <div class="toggle-title">
              <q-icon name="fa-solid fa-user-pen" size="12px" class="q-mr-xs text-grey-6" />
              <span>Thẻ giới thiệu tác giả ở chân bài</span>
            </div>
            <div class="toggle-desc">Hiển thị hộp tóm tắt tiểu sử và mạng xã hội của bạn ở cuối mỗi bài viết.</div>
          </div>
          <q-toggle
            v-model="model.showAuthorBioInPosts"
            color="pink-7"
            dense
            @click.stop
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ReadingSettings } from "@/types/admin-settings";

const model = defineModel<ReadingSettings>({ required: true });
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

      &.icon-emerald {
        background: #ecfdf5;
        color: #059669;
        border: 1px solid #d1fae5;
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

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;

  .form-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 700;
    color: #334155;
  }

  .posts-per-page-options {
    display: flex;
    gap: 8px;
    flex-wrap: wrap;

    .num-choice-btn {
      padding: 8px 16px;
      background: #f8fafc;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 12.5px;
      font-weight: 700;
      color: #475569;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        border-color: #cbd5e1;
        background: #f1f5f9;
      }

      &.active {
        background: #fdf2f6;
        border-color: #df266a;
        color: #df266a;
      }
    }
  }
}

.toggles-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
  border-top: 1px solid #f1f5f9;
  padding-top: 14px;

  .toggle-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 16px;
    padding: 12px 14px;
    background: #f8fafc;
    border: 1px solid #f1f5f9;
    border-radius: 10px;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      background: #ffffff;
      border-color: #cbd5e1;
    }

    .toggle-info {
      flex: 1;

      .toggle-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 13.5px;
        font-weight: 700;
        color: #0b1326;
        margin-bottom: 2px;
      }

      .toggle-desc {
        font-family: var(--font-body, sans-serif);
        font-size: 12px;
        color: #64748b;
      }
    }
  }
}
</style>
