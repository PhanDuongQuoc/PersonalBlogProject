<template>
  <div class="analytics-stat-cards-grid">
    <!-- Card 1: Total Views -->
    <div class="stat-kpi-card card-rose">
      <div class="card-kpi-top">
        <div class="kpi-icon-box icon-rose">
          <q-icon name="fa-solid fa-eye" size="16px" />
        </div>
        <span class="growth-trend-badge badge-positive">
          <q-icon name="fa-solid fa-arrow-trend-up" size="10px" />
          <span>+{{ summary.viewsGrowthRate }}%</span>
        </span>
      </div>
      <div class="card-kpi-bottom">
        <div class="kpi-value">{{ formatNumber(summary.totalViews) }}</div>
        <div class="kpi-label">TỔNG LƯỢT XEM BLOG</div>
        <div class="kpi-subtext">Lưu lượng độc giả trên toàn hệ thống</div>
      </div>
    </div>

    <!-- Card 2: Total Posts -->
    <div class="stat-kpi-card card-indigo">
      <div class="card-kpi-top">
        <div class="kpi-icon-box icon-indigo">
          <q-icon name="fa-solid fa-newspaper" size="16px" />
        </div>
        <span class="status-pill-sub">
          {{ summary.publishedPosts }} Đã xuất bản
        </span>
      </div>
      <div class="card-kpi-bottom">
        <div class="kpi-value">{{ summary.totalPosts }}</div>
        <div class="kpi-label">TỔNG SỐ BÀI VIẾT</div>
        <div class="kpi-subtext">{{ summary.draftPosts }} bản nháp · {{ summary.archivedPosts }} lưu trữ</div>
      </div>
    </div>

    <!-- Card 3: Total Comments -->
    <div class="stat-kpi-card card-emerald">
      <div class="card-kpi-top">
        <div class="kpi-icon-box icon-emerald">
          <q-icon name="fa-solid fa-comments" size="16px" />
        </div>
        <span v-if="summary.pendingComments > 0" class="status-pill-pending">
          {{ summary.pendingComments }} Chờ duyệt
        </span>
        <span v-else class="status-pill-emerald">
          {{ summary.approvedComments }} Đã duyệt
        </span>
      </div>
      <div class="card-kpi-bottom">
        <div class="kpi-value">{{ summary.totalComments }}</div>
        <div class="kpi-label">BÌNH LUẬN & PHẢN HỒI</div>
        <div class="kpi-subtext">Tương tác từ cộng đồng độc giả</div>
      </div>
    </div>

    <!-- Card 4: Categories & Tags -->
    <div class="stat-kpi-card card-amber">
      <div class="card-kpi-top">
        <div class="kpi-icon-box icon-amber">
          <q-icon name="fa-solid fa-layer-group" size="16px" />
        </div>
        <span class="status-pill-amber">
          {{ summary.totalTags }} Thẻ gắn
        </span>
      </div>
      <div class="card-kpi-bottom">
        <div class="kpi-value">{{ summary.totalCategories }}</div>
        <div class="kpi-label">DANH MỤC CÔNG NGHỆ</div>
        <div class="kpi-subtext">Phân loại kiến trúc & nội dung</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { AdminAnalyticsSummary } from "@/types/admin-analytics";

defineProps<{
  summary: AdminAnalyticsSummary;
}>();

function formatNumber(num: number): string {
  if (num >= 1000000) return (num / 1000000).toFixed(1) + "M";
  if (num >= 1000) return (num / 1000).toFixed(1) + "k";
  return num.toLocaleString();
}
</script>

<style scoped lang="scss">
.analytics-stat-cards-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 22px;

  @media (max-width: 1150px) {
    grid-template-columns: repeat(2, 1fr);
  }

  @media (max-width: 600px) {
    grid-template-columns: 1fr;
  }
}

.stat-kpi-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 18px 20px;
  box-shadow: 0 4px 18px rgba(11, 19, 38, 0.03);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  transition: all 0.25s ease;
  position: relative;
  overflow: hidden;

  &::before {
    content: "";
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 3px;
  }

  &.card-rose::before { background: linear-gradient(90deg, #df266a, #f43f5e); }
  &.card-indigo::before { background: linear-gradient(90deg, #4f46e5, #6366f1); }
  &.card-emerald::before { background: linear-gradient(90deg, #10b981, #059669); }
  &.card-amber::before { background: linear-gradient(90deg, #f59e0b, #d97706); }

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(11, 19, 38, 0.07);
    border-color: #cbd5e1;
  }

  .card-kpi-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 14px;

    .kpi-icon-box {
      width: 38px;
      height: 38px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;

      &.icon-rose { background: #fdf2f6; color: #df266a; border: 1px solid #fce7f3; }
      &.icon-indigo { background: #eef2ff; color: #4f46e5; border: 1px solid #e0e7ff; }
      &.icon-emerald { background: #ecfdf5; color: #10b981; border: 1px solid #d1fae5; }
      &.icon-amber { background: #fffbeb; color: #f59e0b; border: 1px solid #fef3c7; }
    }

    .growth-trend-badge {
      font-family: var(--font-mono, monospace);
      font-size: 11.5px;
      font-weight: 700;
      padding: 3px 8px;
      border-radius: 6px;
      display: inline-flex;
      align-items: center;
      gap: 4px;

      &.badge-positive {
        background: #ecfdf5;
        color: #059669;
        border: 1px solid #a7f3d0;
      }
    }

    .status-pill-sub, .status-pill-emerald, .status-pill-amber, .status-pill-pending {
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      padding: 3px 8px;
      border-radius: 6px;
    }

    .status-pill-sub { background: #eef2ff; color: #4f46e5; }
    .status-pill-emerald { background: #ecfdf5; color: #059669; }
    .status-pill-amber { background: #fffbeb; color: #d97706; }
    .status-pill-pending { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
  }

  .card-kpi-bottom {
    .kpi-value {
      font-family: var(--font-headline, sans-serif);
      font-size: 26px;
      font-weight: 800;
      color: #0b1326;
      line-height: 1.1;
      margin-bottom: 4px;
      letter-spacing: -0.02em;
    }

    .kpi-label {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 800;
      color: #64748b;
      letter-spacing: 0.06em;
      margin-bottom: 2px;
    }

    .kpi-subtext {
      font-family: var(--font-body, sans-serif);
      font-size: 12px;
      color: #94a3b8;
    }
  }
}
</style>
