<template>
  <q-page class="admin-analytics-page">
    <!-- 1. Header Banner -->
    <header class="analytics-header-section">
      <!-- <div class="header-text-block">
        <div class="kicker-tag">
          <q-icon name="fa-solid fa-chart-pie" size="11px" class="q-mr-xs text-rose" />
          <span>BÁO CÁO & PHÂN TÍCH HIỆU SUẤT · TỔNG QUAN</span>
        </div>
        <h1 class="page-title">Thống kê Hoạt động Blog</h1>
        <p class="page-subtitle">
          Theo dõi lưu lượng độc giả, độ phổ biến của từng chủ đề và xu hướng tương tác bình luận với biểu đồ Highcharts.
        </p>
      </div> -->

      <!-- Header Actions: Refresh + Print Report -->
      <div class="header-actions">
        <button type="button" class="btn-action-refresh" title="Tải lại toàn bộ dữ liệu" :disabled="loading"
          @click="refreshAllData">
          <q-icon name="fa-solid fa-rotate-right" size="13px" :class="{ 'fa-spin': loading }" />
          <span>Làm mới</span>
        </button>

        <button type="button" class="btn-action-print" title="Xuất slide báo cáo định dạng PPT"
          @click="exportPptReport">
          <q-icon name="fa-solid fa-file-powerpoint" size="13px" />
          <span>Xuất PPT</span>
        </button>
      </div>
    </header>

    <!-- 2. KPI Summary Stat Cards -->
    <section class="analytics-section">
      <AnalyticsStatCards :summary="summary" />
    </section>

    <!-- 3. Grid Row 1: Views Trend (Left) & Categories Distribution (Right) -->
    <section class="analytics-charts-grid-2col">
      <div class="grid-col">
        <ViewsTrendChart ref="viewsTrendRef" />
      </div>
      <div class="grid-col">
        <CategoryDistributionChart ref="categoryDistRef" />
      </div>
    </section>

    <!-- 4. Grid Row 2: Full Width Monthly Reader Engagement Chart -->
    <section class="analytics-section">
      <CommentsEngagementChart ref="commentsRef" />
    </section>

    <!-- 5. Grid Row 3: Post Status Donut (Left) & Top Performing Posts (Right) -->
    <section class="analytics-charts-grid-2col">
      <div class="grid-col">
        <PostStatusDonutChart :summary="summary" />
      </div>
      <div class="grid-col">
        <TopPostsTableCard />
      </div>
    </section>
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import AnalyticsStatCards from "@/components/admin/analytics/AnalyticsStatCards.vue";
import ViewsTrendChart from "@/components/admin/analytics/ViewsTrendChart.vue";
import CategoryDistributionChart from "@/components/admin/analytics/CategoryDistributionChart.vue";
import CommentsEngagementChart from "@/components/admin/analytics/CommentsEngagementChart.vue";
import PostStatusDonutChart from "@/components/admin/analytics/PostStatusDonutChart.vue";
import TopPostsTableCard from "@/components/admin/analytics/TopPostsTableCard.vue";
import type { AdminAnalyticsSummary } from "@/types/admin-analytics";
import { adminAnalyticsService } from "@/services/admin-analytics.service";
import { swalToast, swalError } from "@/utils/swal";
import { exportFullAnalyticsReportPptx } from "@/utils/pptx-export";

const loading = ref(false);
const summary = reactive<AdminAnalyticsSummary>({
  totalViews: 0,
  viewsGrowthRate: 15.8,
  totalPosts: 0,
  publishedPosts: 0,
  draftPosts: 0,
  archivedPosts: 0,
  totalComments: 0,
  approvedComments: 0,
  pendingComments: 0,
  totalCategories: 0,
  totalTags: 0
});

const viewsTrendRef = ref<any>(null);
const categoryDistRef = ref<any>(null);
const commentsRef = ref<any>(null);

async function fetchSummary() {
  try {
    loading.value = true;
    const res = await adminAnalyticsService.getSummary();
    Object.assign(summary, res);
  } catch (err: any) {
    swalError("Lỗi tải dữ liệu", err.response?.data?.message || "Không thể tải số liệu thống kê.");
  } finally {
    loading.value = false;
  }
}

function refreshAllData() {
  fetchSummary();
  if (viewsTrendRef.value?.fetchTrendData) viewsTrendRef.value.fetchTrendData();
  if (categoryDistRef.value?.fetchCategoriesData) categoryDistRef.value.fetchCategoriesData();
  if (commentsRef.value?.fetchCommentsData) commentsRef.value.fetchCommentsData();
  swalToast("Đã làm mới dữ liệu thống kê!", "success");
}

async function exportPptReport() {
  try {
    await exportFullAnalyticsReportPptx(summary);
    swalToast("Đã xuất slide báo cáo PPTX thành công!", "success");
  } catch (err) {
    console.error("Failed to export PPTX:", err);
    swalToast("Có lỗi khi xuất file PowerPoint", "error");
  }
}

onMounted(() => {
  fetchSummary();
});
</script>

<style scoped lang="scss">
.admin-analytics-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 40px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Header Section */
.analytics-header-section {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 16px;
  margin-bottom: 20px;
  flex-wrap: wrap;

  .header-text-block {
    max-width: 680px;

    .kicker-tag {
      display: inline-flex;
      align-items: center;
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      color: #64748b;
      letter-spacing: 0.08em;
      margin-bottom: 4px;
    }

    .page-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 24px;
      font-weight: 800;
      color: #0b1326;
      margin: 0 0 4px;
      letter-spacing: -0.02em;
    }

    .page-subtitle {
      font-family: var(--font-body, sans-serif);
      font-size: 13.5px;
      color: #64748b;
      margin: 0;
      line-height: 1.45;
    }
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-left: auto;

    .btn-action-refresh,
    .btn-action-print {
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
      transition: all 0.2s ease;
    }

    .btn-action-refresh {
      background: #ffffff;
      border: 1px solid #e2e8f0;
      color: #475569;

      &:hover:not(:disabled) {
        border-color: #df266a;
        color: #df266a;
        background: #fdf2f6;
      }
    }

    .btn-action-print {
      background: #df266a;
      border: none;
      color: #ffffff;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.25);

      &:hover {
        background: #be185d;
        transform: translateY(-1px);
        box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
      }
    }
  }
}

/* 2. Layout Sections & Grids */
.analytics-section {
  margin-bottom: 22px;
}

.analytics-charts-grid-2col {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
  margin-bottom: 22px;
  align-items: stretch;

  @media (max-width: 1024px) {
    grid-template-columns: 1fr;
  }
}

.grid-col {
  min-width: 0;
  display: flex;
  flex-direction: column;
  height: 100%;

  >* {
    flex: 1;
    height: 100%;
  }
}
</style>
