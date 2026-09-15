<template>
  <AnalyticsChartCard
    title="Cơ cấu Trạng thái Bài viết"
    icon="fa-solid fa-chart-pie"
    @export-excel="exportToCsv"
    @export-ppt="exportToPpt"
  >
    <AdminHighchart
      ref="chartRef"
      :options="chartOptions"
      height="330px"
    />
  </AnalyticsChartCard>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import type Highcharts from "highcharts";
import AnalyticsChartCard from "./AnalyticsChartCard.vue";
import AdminHighchart from "./AdminHighchart.vue";
import type { AdminAnalyticsSummary } from "@/types/admin-analytics";
import { swalToast } from "@/utils/swal";
import { exportPostStatusPptx } from "@/utils/pptx-export";

const props = defineProps<{
  summary: AdminAnalyticsSummary;
}>();

const chartRef = ref<InstanceType<typeof AdminHighchart> | null>(null);

const chartOptions = computed<Highcharts.Options>(() => {
  const published = props.summary.publishedPosts || 0;
  const draft = props.summary.draftPosts || 0;
  const archived = props.summary.archivedPosts || 0;

  return {
    chart: {
      type: "pie"
    },
    tooltip: {
      useHTML: true,
      pointFormat:
        '<div style="font-size:12.5px;">' +
        '<b>{point.name}</b>: {point.y} bài ({point.percentage:.1f}%)' +
        '</div>'
    },
    plotOptions: {
      pie: {
        innerSize: "62%",
        borderWidth: 2,
        borderColor: "#ffffff",
        allowPointSelect: true,
        cursor: "pointer",
        dataLabels: {
          enabled: true,
          format: "<b>{point.name}</b>: {point.percentage:.0f}%",
          style: {
            fontSize: "11px",
            color: "#475569"
          },
          distance: 14
        },
        showInLegend: true
      }
    },
    legend: {
      layout: "horizontal",
      align: "center",
      verticalAlign: "bottom"
    },
    series: [
      {
        type: "pie",
        name: "Bài viết",
        data: [
          { name: "Đã xuất bản", y: published, color: "#10b981" },
          { name: "Bản nháp", y: draft, color: "#f59e0b" },
          { name: "Lưu trữ", y: archived, color: "#94a3b8" }
        ]
      }
    ]
  };
});

function exportToCsv() {
  const headers = "Trạng thái,Số lượng\n";
  const rows = [
    `"Đã xuất bản",${props.summary.publishedPosts}`,
    `"Bản nháp",${props.summary.draftPosts}`,
    `"Lưu trữ",${props.summary.archivedPosts}`
  ].join("\n");
  const blob = new Blob([headers + rows], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `post-status-distribution.csv`;
  link.click();
  swalToast("Đã xuất dữ liệu trạng thái thành công!", "success");
}

async function exportToPpt() {
  try {
    await exportPostStatusPptx(props.summary);
    swalToast("Đã xuất slide PPTX trạng thái bài viết thành công!", "success");
  } catch (err) {
    console.error("Failed to export PPTX:", err);
    swalToast("Có lỗi khi xuất file PowerPoint", "error");
  }
}
</script>
