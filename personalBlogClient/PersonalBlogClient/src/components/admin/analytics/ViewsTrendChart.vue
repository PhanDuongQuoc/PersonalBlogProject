<template>
  <AnalyticsChartCard
    title="Xu hướng Lượt xem & Độc giả"
    icon="fa-solid fa-chart-line"
    :filter-options="filterOptions"
    v-model="currentPeriod"
    @update:model-value="onPeriodChange"
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
import { ref, computed, watch, onMounted } from "vue";
import type Highcharts from "highcharts";
import AnalyticsChartCard, { type ChartFilterOption } from "./AnalyticsChartCard.vue";
import AdminHighchart from "./AdminHighchart.vue";
import type { ViewsTrendData } from "@/types/admin-analytics";
import { adminAnalyticsService } from "@/services/admin-analytics.service";
import { swalToast } from "@/utils/swal";
import { exportViewsTrendPptx } from "@/utils/pptx-export";

const filterOptions: ChartFilterOption[] = [
  { label: "Theo tháng (Monthly)", value: "monthly" },
  { label: "30 ngày qua", value: "30d" },
  { label: "7 ngày qua", value: "7d" },
  { label: "Theo năm (Yearly)", value: "yearly" }
];

const currentPeriod = ref<"7d" | "30d" | "monthly" | "yearly">("monthly");
const loading = ref(false);
const trendData = ref<ViewsTrendData | null>(null);
const chartRef = ref<InstanceType<typeof AdminHighchart> | null>(null);

async function fetchTrendData() {
  try {
    loading.value = true;
    trendData.value = await adminAnalyticsService.getViewsTrend(currentPeriod.value);
  } catch (err) {
    console.error("Failed to load views trend:", err);
  } finally {
    loading.value = false;
  }
}

function onPeriodChange(val: string) {
  currentPeriod.value = val as any;
  fetchTrendData();
}

const chartOptions = computed<Highcharts.Options>(() => {
  const points = trendData.value?.dataPoints || [];
  const categories = points.map((p) => p.dateLabel);
  const viewsSeries = points.map((p) => p.views);
  const readersSeries = points.map((p) => p.uniqueReaders);

  return {
    chart: {
      type: "areaspline"
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
      headerFormat: '<div style="font-weight:700;margin-bottom:4px;color:#0b1326">{point.key}</div>',
      pointFormat:
        '<div style="display:flex;align-items:center;gap:6px;font-size:12px;margin-top:2px;">' +
        '<span style="display:inline-block;width:8px;height:8px;border-radius:50%;background-color:{series.color}"></span>' +
        '<span>{series.name}:</span> <b>{point.y:,.0f}</b>' +
        '</div>'
    },
    plotOptions: {
      areaspline: {
        fillOpacity: 0.12,
        lineWidth: 3,
        marker: {
          enabled: false,
          radius: 4,
          symbol: "circle",
          states: {
            hover: {
              enabled: true,
              lineWidth: 2,
              lineColor: "#ffffff"
            }
          }
        }
      }
    },
    series: [
      {
        type: "areaspline",
        name: "Lượt xem (Views)",
        data: viewsSeries,
        color: "#df266a"
      },
      {
        type: "areaspline",
        name: "Độc giả (Unique Readers)",
        data: readersSeries,
        color: "#4f46e5"
      }
    ]
  };
});

function exportToCsv() {
  if (!trendData.value?.dataPoints) return;
  const headers = "Thời gian,Lượt xem,Độc giả\n";
  const rows = trendData.value.dataPoints
    .map((p) => `"${p.dateLabel}",${p.views},${p.uniqueReaders}`)
    .join("\n");
  const blob = new Blob([headers + rows], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `views-trend-${currentPeriod.value}.csv`;
  link.click();
  swalToast("Đã xuất dữ liệu CSV thành công!", "success");
}

async function exportToPpt() {
  if (!trendData.value?.dataPoints || !trendData.value.dataPoints.length) {
    swalToast("Chưa có dữ liệu để xuất slide", "info");
    return;
  }
  try {
    await exportViewsTrendPptx(trendData.value.dataPoints, currentPeriod.value);
    swalToast("Đã xuất slide PPTX xu hướng xem thành công!", "success");
  } catch (err) {
    console.error("Failed to export PPTX:", err);
    swalToast("Có lỗi khi xuất file PowerPoint", "error");
  }
}

onMounted(() => {
  fetchTrendData();
});
</script>
