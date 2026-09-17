<template>
  <AnalyticsChartCard
    title="Thống kê Hộp thư & Email Liên hệ gửi về"
    icon="fa-solid fa-envelope-open-text"
    :filter-options="filterOptions"
    v-model="currentPeriod"
    @update:model-value="fetchContactsData"
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
import type { MonthlyContactsItem } from "@/types/admin-analytics";
import { adminAnalyticsService } from "@/services/admin-analytics.service";
import { swalToast } from "@/utils/swal";
import { exportContactsTrendPptx } from "@/utils/pptx-export";

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
const contactsData = ref<MonthlyContactsItem[]>([]);
const chartRef = ref<InstanceType<typeof AdminHighchart> | null>(null);

async function fetchContactsData() {
  try {
    loading.value = true;
    const res = await adminAnalyticsService.getContactsTrend(currentPeriod.value);
    contactsData.value = res || [];
  } catch (err) {
    console.error("Failed to load contacts analytics:", err);
  } finally {
    loading.value = false;
  }
}

const chartOptions = computed<Highcharts.Options>(() => {
  const items = contactsData.value && contactsData.value.length > 0 ? contactsData.value : [];
  const categories = items.length > 0 ? items.map((i) => i.monthLabel) : DEFAULT_MONTHS;
  const totalMessages = items.length > 0 ? items.map((i) => i.totalMessages) : Array(12).fill(0);
  const repliedMessages = items.length > 0 ? items.map((i) => i.repliedMessages) : Array(12).fill(0);
  const unreadMessages = items.length > 0 ? items.map((i) => i.unreadMessages) : Array(12).fill(0);

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
        '<span>{series.name}:</span> <b>{point.y} email</b>' +
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
        name: "Tổng email / tin nhắn nhận",
        data: totalMessages,
        color: "#df266a" // Rose Brand Color
      },
      {
        type: "spline",
        name: "Đã phản hồi qua email",
        data: repliedMessages,
        color: "#059669", // Emerald Teal
        dashStyle: "ShortDot"
      },
      {
        type: "spline",
        name: "Chưa đọc / Chưa xử lý",
        data: unreadMessages,
        color: "#f59e0b", // Amber
        dashStyle: "Dash"
      }
    ]
  };
});

function exportToCsv() {
  if (!contactsData.value.length) return;
  const headers = "Thời gian,Tổng email nhận,Đã phản hồi,Chưa đọc\n";
  const rows = contactsData.value
    .map((c) => `"${c.monthLabel}",${c.totalMessages},${c.repliedMessages},${c.unreadMessages}`)
    .join("\n");
  const blob = new Blob([headers + rows], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `contacts-trend-${currentPeriod.value}.csv`;
  link.click();
  swalToast("Đã xuất dữ liệu email & hộp thư thành công!", "success");
}

async function exportToPpt() {
  if (!contactsData.value.length) {
    swalToast("Chưa có dữ liệu để xuất slide", "info");
    return;
  }
  try {
    await exportContactsTrendPptx(contactsData.value, currentPeriod.value);
    swalToast("Đã xuất slide PPTX hộp thư liên hệ thành công!", "success");
  } catch (err) {
    console.error("Failed to export PPTX:", err);
    swalToast("Có lỗi khi xuất file PowerPoint", "error");
  }
}

defineExpose({
  fetchContactsData
});

onMounted(() => {
  fetchContactsData();
});
</script>
