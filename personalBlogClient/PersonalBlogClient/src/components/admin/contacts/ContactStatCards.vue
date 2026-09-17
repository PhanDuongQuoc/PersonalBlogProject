<template>
  <div class="contact-stat-cards-grid">
    <!-- Card 1: Tổng tin nhắn -->
    <div class="stat-card card-blue">
      <div class="stat-icon-box icon-blue">
        <q-icon name="fa-solid fa-inbox" size="18px" />
      </div>
      <div class="stat-info">
        <span class="stat-label">Tổng Tin Nhắn</span>
        <div class="stat-value font-mono">{{ summary.totalMessages }}</div>
      </div>
    </div>

    <!-- Card 2: Chưa đọc (Unread) -->
    <div class="stat-card card-rose" :class="{ 'has-unread': summary.unreadMessages > 0 }">
      <div class="stat-icon-box icon-rose">
        <q-icon name="fa-solid fa-envelope" size="18px" />
      </div>
      <div class="stat-info">
        <div class="label-with-dot">
          <span class="stat-label">Chưa Xử Lý</span>
          <span v-if="summary.unreadMessages > 0" class="pulse-red-dot" title="Có tin nhắn mới"></span>
        </div>
        <div class="stat-value text-rose font-mono">{{ summary.unreadMessages }}</div>
      </div>
    </div>

    <!-- Card 3: Đã phản hồi (Replied) -->
    <div class="stat-card card-emerald">
      <div class="stat-icon-box icon-emerald">
        <q-icon name="fa-solid fa-reply-all" size="18px" />
      </div>
      <div class="stat-info">
        <span class="stat-label">Đã Phản Hồi</span>
        <div class="stat-value text-emerald font-mono">{{ summary.repliedMessages }}</div>
      </div>
    </div>

    <!-- Card 4: Nhận trong tháng này -->
    <div class="stat-card card-indigo">
      <div class="stat-icon-box icon-indigo">
        <q-icon name="fa-solid fa-calendar-check" size="18px" />
      </div>
      <div class="stat-info">
        <span class="stat-label">Nhận Trong Tháng</span>
        <div class="stat-value font-mono">{{ summary.thisMonthMessages }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ContactSummary } from "@/types/admin-contact";

defineProps<{
  summary: ContactSummary;
}>();
</script>

<style scoped lang="scss">
.contact-stat-cards-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;

  @media (max-width: 1024px) {
    grid-template-columns: repeat(2, 1fr);
  }

  @media (max-width: 600px) {
    grid-template-columns: 1fr;
  }
}

.stat-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 16px 18px;
  display: flex;
  align-items: center;
  gap: 14px;
  box-shadow: 0 4px 18px rgba(11, 19, 38, 0.03);
  transition: all 0.25s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(11, 19, 38, 0.06);
    border-color: #cbd5e1;
  }

  &.has-unread {
    border-color: #fecdd3;
    background: #fffafa;
  }

  .stat-icon-box {
    width: 44px;
    height: 44px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;

    &.icon-blue {
      background: #eff6ff;
      color: #2563eb;
      border: 1px solid #dbeafe;
    }

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

    &.icon-indigo {
      background: #eef2ff;
      color: #4f46e5;
      border: 1px solid #e0e7ff;
    }
  }

  .stat-info {
    flex: 1;

    .label-with-dot {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .stat-label {
      font-family: var(--font-headline, sans-serif);
      font-size: 12.5px;
      font-weight: 600;
      color: #64748b;
    }

    .pulse-red-dot {
      width: 7px;
      height: 7px;
      border-radius: 50%;
      background: #df266a;
      animation: pulse-red 1.5s infinite;
    }

    .stat-value {
      font-size: 22px;
      font-weight: 800;
      color: #0b1326;
      line-height: 1.2;
      margin-top: 2px;

      &.text-rose {
        color: #df266a;
      }

      &.text-emerald {
        color: #059669;
      }
    }
  }
}

@keyframes pulse-red {
  0% {
    box-shadow: 0 0 0 0 rgba(223, 38, 106, 0.7);
  }
  70% {
    box-shadow: 0 0 0 6px rgba(223, 38, 106, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(223, 38, 106, 0);
  }
}
</style>
