<template>
  <AnalyticsChartCard
    title="Tương tác Bình luận Độc giả"
    icon="fa-solid fa-comments"
    :filter-options="filterOptions"
    v-model="currentPeriod"
    @update:model-value="fetchCommentsData"
    @export-excel="exportToCsv"
    @export-ppt="exportToPpt"
  >
    <AdminHighchart
      ref="chartRef"
      :options="chartOptions"
      :loading="loading"
      height="330px"
    />
  </AnalyticsChartCard>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import type Highcharts from "highcharts";
import AnalyticsChartCard, { type ChartFilterOption } from "./AnalyticsChartCard.vue";
import AdminHighchart from "./AdminHighchart.vue";
import type { MonthlyCommentsItem } from "@/types/admin-analytics";
import { adminAnalyticsService } from "@/services/admin-analytics.service";
import { swalToast } from "@/utils/swal";
import { exportCommentsTrendPptx } from "@/utils/pptx-export";

const DEFAULT_MONTHS = [
  "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6",
  "Thg 7", "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12"
];

const filterOptions: ChartFilterOption[] = [
  { label: "Theo tháng (Monthly)", value: "monthly" },
  { label: "30 ngày qua", value: "30d" },
  { label: "7 ngày qua", value: "7d" },
  { label: "Theo năm (Yearly)", value: "yearly" }
];

const currentPeriod = ref<"7d" | "30d" | "monthly" | "yearly">("monthly");
const loading = ref(false);
const commentsData = ref<MonthlyCommentsItem[]>([]);
const chartRef = ref<InstanceType<typeof AdminHighchart> | null>(null);

async function fetchCommentsData() {
  try {
    loading.value = true;
    const res = await adminAnalyticsService.getCommentsTrend(currentPeriod.value);
    commentsData.value = res || [];
  } catch (err) {
    console.error("Failed to load comments analytics:", err);
  } finally {
    loading.value = false;
  }
}

const chartOptions = computed<Highcharts.Options>(() => {
  const items = commentsData.value && commentsData.value.length > 0 ? commentsData.value : [];
  const categories = items.length > 0 ? items.map((i) => i.monthLabel) : DEFAULT_MONTHS;
  const totalComments = items.length > 0 ? items.map((i) => i.totalComments) : Array(12).fill(0);
  const approvedComments = items.length > 0 ? items.map((i) => i.approvedComments) : Array(12).fill(0);

  return {
    chart: {
      type: "spline"
    },
    xAxis: {
      categories,
      crosshair: {
        width: 1,
        color: "#cbd5e1",
        dashStyle: "Dash"
      }
    },
    yAxis: {
      title: { text: undefined },
      min: 0,
      minTickInterval: 1,
      allowDecimals: false
    },
    tooltip: {
      shared: true,
      useHTML: true,
      headerFormat: '<div style="font-weight:700;margin-bottom:4px;color:inherit">{point.key}</div>',
      pointFormat:
        '<div style="display:flex;align-items:center;gap:6px;font-size:12px;margin-top:2px;">' +
        '<span style="display:inline-block;width:8px;height:8px;border-radius:50%;background-color:{series.color}"></span>' +
        '<span>{series.name}:</span> <b>{point.y} bình luận</b>' +
        '</div>'
    },
    plotOptions: {
      spline: {
        lineWidth: 3,
        marker: {
          enabled: true,
          radius: 4.5,
          symbol: "circle",
          lineWidth: 2,
          lineColor: "#ffffff"
        }
      }
    },
    series: [
      {
        type: "spline",
        name: "Tổng bình luận gửi về",
        data: totalComments,
        color: "#059669" // Emerald Teal matching sample image
      },
      {
        type: "spline",
        name: "Bình luận đã phê duyệt",
        data: approvedComments,
        color: "#4f46e5",
        dashStyle: "ShortDot"
      }
    ]
  };
});

function exportToCsv() {
  if (!commentsData.value.length) return;
  const headers = "Thời gian,Tổng bình luận,Đã duyệt\n";
  const rows = commentsData.value
    .map((c) => `"${c.monthLabel}",${c.totalComments},${c.approvedComments}`)
    .join("\n");
  const blob = new Blob([headers + rows], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `comments-trend-${currentPeriod.value}.csv`;
  link.click();
  swalToast("Đã xuất dữ liệu bình luận thành công!", "success");
}

async function exportToPpt() {
  if (!commentsData.value.length) {
    swalToast("Chưa có dữ liệu để xuất slide", "info");
    return;
  }
  try {
    await exportCommentsTrendPptx(commentsData.value, currentPeriod.value);
    swalToast("Đã xuất slide PPTX bình luận thành công!", "success");
  } catch (err) {
    console.error("Failed to export PPTX:", err);
    swalToast("Có lỗi khi xuất file PowerPoint", "error");
  }
}

defineExpose({
  fetchCommentsData
});

onMounted(() => {
  fetchCommentsData();
});
</script>
