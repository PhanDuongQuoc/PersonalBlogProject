<template>
  <AnalyticsChartCard
    title="Phân bổ Bài viết & Lượt xem theo Chủ đề"
    icon="fa-solid fa-chart-column"
    :filter-options="viewModeOptions"
    v-model="viewMetric"
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
import type { CategoryAnalyticsItem } from "@/types/admin-analytics";
import { adminAnalyticsService } from "@/services/admin-analytics.service";
import { swalToast } from "@/utils/swal";
import { exportCategoryDistributionPptx } from "@/utils/pptx-export";

const viewModeOptions: ChartFilterOption[] = [
  { label: "Theo Lượt xem (Views)", value: "views" },
  { label: "Theo Số bài viết (Posts)", value: "posts" }
];

const viewMetric = ref<"views" | "posts">("views");
const loading = ref(false);
const categoriesData = ref<CategoryAnalyticsItem[]>([]);
const chartRef = ref<InstanceType<typeof AdminHighchart> | null>(null);

async function fetchCategoriesData() {
  try {
    loading.value = true;
    categoriesData.value = await adminAnalyticsService.getCategoryDistribution();
  } catch (err) {
    console.error("Failed to load category analytics:", err);
  } finally {
    loading.value = false;
  }
}

const chartOptions = computed<Highcharts.Options>(() => {
  const cats = categoriesData.value || [];
  const categories = cats.map((c) => c.categoryName);
  const data = cats.map((c) => (viewMetric.value === "views" ? c.totalViews : c.postCount));
  const seriesName = viewMetric.value === "views" ? "Tổng lượt xem" : "Số bài viết";

  return {
    chart: {
      type: "column"
    },
    xAxis: {
      categories,
      crosshair: true,
      labels: {
        rotation: -25,
        style: {
          fontSize: "11px",
          color: "#475569"
        }
      }
    },
    yAxis: {
      title: { text: undefined },
      labels: {
        formatter: function () {
          const val = Number(this.value);
          if (val >= 1000) return (val / 1000).toFixed(0) + "k";
          return val.toString();
        }
      }
    },
    tooltip: {
      shared: true,
      useHTML: true,
      headerFormat: '<div style="font-weight:700;margin-bottom:4px;color:inherit">{point.key}</div>',
      pointFormat:
        '<div style="font-size:12px;">' +
        '<span style="display:inline-block;width:8px;height:8px;border-radius:2px;background-color:{point.color}"></span> ' +
        '{series.name}: <b>{point.y:,.0f}</b>' +
        '</div>'
    },
    plotOptions: {
      column: {
        borderRadius: 6,
        colorByPoint: true,
        colors: [
          "#f97316", // Amber Orange (like screenshot)
          "#df266a", // Rose
          "#4f46e5", // Indigo
          "#10b981", // Emerald
          "#06b6d4", // Cyan
          "#8b5cf6", // Purple
          "#ec4899"  // Pink
        ],
        borderWidth: 0,
        dataLabels: {
          enabled: false
        }
      }
    },
    series: [
      {
        type: "column",
        name: seriesName,
        data: data
      }
    ]
  };
});

function exportToCsv() {
  if (!categoriesData.value.length) return;
  const headers = "Danh mục,Số bài viết,Tổng lượt xem,Tỷ lệ %\n";
  const rows = categoriesData.value
    .map((c) => `"${c.categoryName}",${c.postCount},${c.totalViews},${c.percentage}%`)
    .join("\n");
  const blob = new Blob([headers + rows], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `category-analytics.csv`;
  link.click();
  swalToast("Đã xuất dữ liệu danh mục thành công!", "success");
}

async function exportToPpt() {
  if (!categoriesData.value.length) {
    swalToast("Chưa có dữ liệu để xuất slide", "info");
    return;
  }
  try {
    await exportCategoryDistributionPptx(categoriesData.value, viewMetric.value);
    swalToast("Đã xuất slide PPTX danh mục thành công!", "success");
  } catch (err) {
    console.error("Failed to export PPTX:", err);
    swalToast("Có lỗi khi xuất file PowerPoint", "error");
  }
}

onMounted(() => {
  fetchCategoriesData();
});
</script>
