<template>
  <div class="admin-highchart-wrapper">
    <div v-if="loading" class="chart-loading-overlay">
      <q-spinner-tail color="pink-7" size="32px" />
      <span class="loading-text">Đang tải biểu đồ...</span>
    </div>
    <div ref="chartContainer" class="chart-container" :style="{ height: height || '340px' }"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch, nextTick } from "vue";
import Highcharts from "highcharts";

const props = withDefaults(
  defineProps<{
    options: Highcharts.Options;
    height?: string;
    loading?: boolean;
  }>(),
  {
    height: "340px",
    loading: false
  }
);

const chartContainer = ref<HTMLElement | null>(null);
let chartInstance: Highcharts.Chart | null = null;

// Global theme options for Editorial styling
const baseThemeOptions: Highcharts.Options = {
  chart: {
    backgroundColor: "transparent",
    style: {
      fontFamily: "var(--font-body, 'Plus Jakarta Sans', sans-serif)"
    },
    spacing: [15, 10, 15, 10]
  },
  title: {
    text: ""
  },
  credits: {
    enabled: false
  },
  tooltip: {
    backgroundColor: "#ffffff",
    borderColor: "#e2e8f0",
    borderRadius: 10,
    shadow: {
      color: "rgba(11, 19, 38, 0.08)",
      offsetX: 0,
      offsetY: 4,
      opacity: 0.12,
      width: 12
    },
    style: {
      color: "#0b1326",
      fontSize: "12.5px",
      fontFamily: "var(--font-body, 'Plus Jakarta Sans', sans-serif)"
    }
  },
  xAxis: {
    lineColor: "#e2e8f0",
    tickColor: "#e2e8f0",
    labels: {
      style: {
        color: "#64748b",
        fontSize: "11.5px",
        fontFamily: "var(--font-mono, monospace)"
      }
    }
  },
  yAxis: {
    gridLineColor: "#f1f5f9",
    gridLineDashStyle: "Solid",
    title: {
      text: undefined
    },
    labels: {
      style: {
        color: "#64748b",
        fontSize: "11.5px",
        fontFamily: "var(--font-mono, monospace)"
      }
    }
  },
  legend: {
    itemStyle: {
      color: "#475569",
      fontWeight: "600",
      fontSize: "12px"
    },
    itemHoverStyle: {
      color: "#0b1326"
    }
  }
};

function initChart() {
  if (!chartContainer.value) return;

  const mergedOptions = Highcharts.merge(baseThemeOptions, props.options);
  chartInstance = Highcharts.chart(chartContainer.value, mergedOptions);
}

watch(
  () => props.options,
  (newOptions) => {
    if (chartInstance && newOptions) {
      chartInstance.update(newOptions, true, false);
    } else if (!chartInstance && chartContainer.value) {
      initChart();
    }
  },
  { deep: true }
);

onMounted(() => {
  nextTick(() => {
    initChart();
  });
});

onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.destroy();
    chartInstance = null;
  }
});

// Expose methods for parent components
function getChartInstance(): Highcharts.Chart | null {
  return chartInstance;
}

function reflow() {
  if (chartInstance) {
    chartInstance.reflow();
  }
}

function exportAsImage(type: "image/png" | "image/jpeg" | "application/pdf" | "image/svg+xml" = "image/png") {
  if (chartInstance) {
    // If exporting module is available or fallback print
    if (typeof (chartInstance as any).exportChartLocal === "function") {
      (chartInstance as any).exportChartLocal({ type });
    } else if (typeof (chartInstance as any).print === "function") {
      (chartInstance as any).print();
    }
  }
}

defineExpose({
  getChartInstance,
  reflow,
  exportAsImage
});
</script>

<style scoped lang="scss">
.admin-highchart-wrapper {
  position: relative;
  width: 100%;
  min-height: 200px;
}

.chart-container {
  width: 100%;
}

.chart-loading-overlay {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.78);
  backdrop-filter: blur(2px);
  z-index: 5;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 10px;

  .loading-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: #64748b;
  }
}
</style>
